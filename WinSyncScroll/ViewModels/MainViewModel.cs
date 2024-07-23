using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using System.Windows.Data;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PropertyChanged.SourceGenerator;
using WinSyncScroll.Enums;
using WinSyncScroll.Models;
using WinSyncScroll.Services;
using HWND = Windows.Win32.Foundation.HWND;

namespace WinSyncScroll.ViewModels;

public sealed partial class MainViewModel : IDisposable
{
    private readonly ILogger<MainViewModel> _logger;
    private readonly WinApiManager _winApiManager;
    private readonly MouseHook _mouseHook;

    // ReSharper disable once MemberCanBePrivate.Global
    public ObservableCollection<WindowInfo> Windows { get; } = [];

    // ReSharper disable once MemberCanBePrivate.Global
    public ICollectionView WindowsOrdered { get; }

    public RelayCommand RefreshWindowsCommand { get; }
    public RelayCommand StartCommand { get; }
    public RelayCommand StopCommand { get; }

    [Notify]
    // ReSharper disable once InconsistentNaming
    private WindowInfo? _source { get; set; }

    [Notify]
    // ReSharper disable once InconsistentNaming
    private WindowInfo? _target { get; set; }

    private AppState _appState = AppState.NotRunning;
    private Task? _mouseEventProcessingLoopTask;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public MainViewModel(
        ILogger<MainViewModel> logger,
        WinApiManager winApiManager,
        MouseHook mouseHook)
    {
        _logger = logger;
        _winApiManager = winApiManager;
        _mouseHook = mouseHook;

        RefreshWindowsCommand = new RelayCommand(RefreshWindows);
        StartCommand = new RelayCommand(Start);
        StopCommand = new RelayCommand(Stop);

        WindowsOrdered = CollectionViewSource.GetDefaultView(Windows);
        WindowsOrdered.SortDescriptions.Add(new SortDescription(nameof(WindowInfo.WindowName), ListSortDirection.Ascending));
        WindowsOrdered.SortDescriptions.Add(new SortDescription(nameof(WindowInfo.ProcessName), ListSortDirection.Ascending));
        WindowsOrdered.SortDescriptions.Add(new SortDescription(nameof(WindowInfo.ProcessId), ListSortDirection.Ascending));
        WindowsOrdered.SortDescriptions.Add(new SortDescription(nameof(WindowInfo.ClassName), ListSortDirection.Ascending));
        WindowsOrdered.SortDescriptions.Add(new SortDescription(nameof(WindowInfo.WindowHandleLong), ListSortDirection.Ascending));
    }

    private void InstallMouseHook()
    {
        _mouseHook.Install();

        var token = _cancellationTokenSource.Token;
        _mouseEventProcessingLoopTask = Task.Run(async () =>
            await RunMouseEventProcessingLoopAsync(_mouseHook.HookEvents, token),
            token);
    }

    private async Task RunMouseEventProcessingLoopAsync(
        Channel<MouseEventArgs> channel,
        CancellationToken cancellationToken = default)
    {
        while (await channel.Reader.WaitToReadAsync(cancellationToken))
        {
            while (channel.Reader.TryRead(out var buffer))
            {
                try
                {
                    if (_appState != AppState.Running)
                    {
                        _logger.LogTrace("App is not running, skipping mouse event processing");
                        continue;
                    }
                    
                    if (Source is null)
                    {
                        _logger.LogTrace("Source window is not selected, skipping mouse event processing");
                        continue;
                    }

                    if (Target is null)
                    {
                        _logger.LogTrace("Target window is not selected, skipping mouse event processing");
                        continue;
                    }

                    if (buffer.MouseMessageId is not
                        (NativeConstants.WM_MOUSEWHEEL
                        or NativeConstants.WM_MOUSEHWHEEL))
                    {
                        _logger.LogTrace("Skipping non-scroll mouse event");
                        continue;
                    }

                    var scrollEventX = buffer.MouseMessageData.pt.X;
                    var scrollEventY = buffer.MouseMessageData.pt.Y;

                    PInvoke.GetWindowRect((HWND)Source.WindowHandle, out var sourceRectStruct);
                    var sourceRect = new WindowRect
                    {
                        Left = sourceRectStruct.left,
                        Top = sourceRectStruct.top,
                        Right = sourceRectStruct.right,
                        Bottom = sourceRectStruct.bottom,
                    };

                    PInvoke.GetWindowRect((HWND)Target.WindowHandle, out var targetRectStruct);
                    var targetRect = new WindowRect
                    {
                        Left = targetRectStruct.left,
                        Top = targetRectStruct.top,
                        Right = targetRectStruct.right,
                        Bottom = targetRectStruct.bottom,
                    };

                    if (!NativeNumberUtils.PointInRect(sourceRect, scrollEventX, scrollEventY))
                    {
                        _logger.LogTrace("Mouse event is not in the source window, skipping");
                        continue;
                    }

                    var relativeX = scrollEventX - sourceRect.Left;
                    var relativeY = scrollEventY - sourceRect.Top;

                    var targetX = targetRect.Left + relativeX;
                    var targetY = targetRect.Top + relativeY;

                    if (!NativeNumberUtils.PointInRect(targetRect, targetX, targetY))
                    {
                        _logger.LogTrace("Resulting mouse event is not in the target window, falling back to center of the target window");
                        targetX = targetRect.Left + targetRect.Right / 2;
                        targetY = targetRect.Top + targetRect.Bottom / 2;
                    }

                    // If the message is WM_MOUSEWHEEL, the high-order word of this member is the wheel delta. The low-order word is reserved.
                    var (_, high) = NativeNumberUtils.GetHiLoWords(buffer.MouseMessageData.mouseData);
                    var delta = high;

                    var input = new INPUT
                    {
                        type = INPUT_TYPE.INPUT_MOUSE,
                    };
                    var dwFlags = buffer.MouseMessageId switch
                    {
                        NativeConstants.WM_MOUSEWHEEL => MOUSE_EVENT_FLAGS.MOUSEEVENTF_WHEEL,
                        NativeConstants.WM_MOUSEHWHEEL => MOUSE_EVENT_FLAGS.MOUSEEVENTF_HWHEEL,
                        _ => MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE,
                    };
                    input.Anonymous.mi.dwFlags = dwFlags;
                    input.Anonymous.mi.time = 0;
                    input.Anonymous.mi.mouseData = (uint)delta;
                    input.Anonymous.mi.dx = targetX;
                    input.Anonymous.mi.dy = targetY;

                    PInvoke.SetCursorPos(targetX, targetY);
                    input.Anonymous.mi.dwExtraInfo = (nuint)(nint)PInvoke.GetMessageExtraInfo();

                    var inputs = new[] { input };
                    var sizeOfInput = Marshal.SizeOf(typeof(INPUT));

                    PInvoke.SendInput(inputs.AsSpan(), sizeOfInput);
                    PInvoke.SetCursorPos(scrollEventX, scrollEventY);

                    _logger.LogTrace("Successfully processed mouse event");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error processing mouse event");
                }
            }
        }
    }

    private void RefreshWindows()
    {
        var newWindows = _winApiManager.ListWindows();

        // remember the old windows to replace them with the new ones
        var oldSource = Source;
        var oldTarget = Target;

        // remove windows that are not in the new list
        var toBeRemoved = Windows
            .Where(w => newWindows.All(nw => nw.WindowHandle != w.WindowHandle))
            .ToList();

        foreach (var window in toBeRemoved)
        {
            Windows.Remove(window);
        }

        // update windows that are in the new list
        var toBeUpdated = Windows
            .Join(newWindows,
                w => w.WindowHandle,
                nw => nw.WindowHandle,
                (w, nw) => new
                {
                    OldWindow = w,
                    NewWindow = nw,
                })
            .ToList();

        foreach (var windowToBeUpdated in toBeUpdated)
        {
            var i = Windows.IndexOf(windowToBeUpdated.OldWindow);
            if (i != -1)
            {
                Windows[i] = windowToBeUpdated.NewWindow;
            }
        }

        // add windows that are not in the old list
        var toBeAdded = newWindows
            .Where(nw => Windows.All(w => nw.WindowHandle != w.WindowHandle))
            .ToList();

        foreach (var window in toBeAdded)
        {
            Windows.Add(window);
        }

        // restore the old source and target windows
        if (oldSource != null)
        {
            Source = Windows.FirstOrDefault(w => w.WindowHandle == oldSource.WindowHandle);
        }

        if (oldTarget != null)
        {
            Target = Windows.FirstOrDefault(w => w.WindowHandle == oldTarget.WindowHandle);
        }
    }

    public void Initialize()
    {
        InstallMouseHook();
        RefreshWindows();
    }

    private void Start()
    {
        if (Source == null || Target == null)
        {
            _logger.LogWarning("Source or target window is not selected: Source={Source}, Target={Target}", Source, Target);
        }
        else
        {
            _logger.LogInformation("Starting scroll sync between \"{SourceWindow}\" and \"{TargetWindow}\"", Source.DisplayName, Target.DisplayName);
            _appState = AppState.Running;
        }
    }

    private void Stop()
    {
        _appState = AppState.NotRunning;
    }

    public void HandleWindowClosing()
    {
        _mouseHook.Uninstall();
        _appState = AppState.NotRunning;
        try
        {
            _cancellationTokenSource.Cancel();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error cancelling the cancellation token source");
        }
    }

    public void Dispose()
    {
        _appState = AppState.NotRunning;
        try
        {
            _cancellationTokenSource.Cancel();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error cancelling the cancellation token source");
        }

        _mouseEventProcessingLoopTask?.Dispose();
        _mouseHook.Dispose();
        _cancellationTokenSource.Dispose();
    }
}
