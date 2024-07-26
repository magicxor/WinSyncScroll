using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using System.Windows.Data;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PropertyChanged.SourceGenerator;
using WinSyncScroll.Enums;
using WinSyncScroll.Models;
using WinSyncScroll.Services;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using HWND = Windows.Win32.Foundation.HWND;

namespace WinSyncScroll.ViewModels;

public sealed partial class MainViewModel : IDisposable
{
    private readonly ILogger<MainViewModel> _logger;
    private readonly WinApiService _winApiService;
    private readonly MouseHook _mouseHook;

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    // ReSharper disable once MemberCanBePrivate.Global
    public ObservableCollection<WindowInfo> Windows { get; } = [];

    // ReSharper disable once MemberCanBePrivate.Global
    public ICollectionView WindowsOrdered { get; }

    public RelayCommand RefreshWindowsCommand { get; }
    public RelayCommand StartCommand { get; }
    public RelayCommand StopCommand { get; }

    [Notify]
    private WindowInfo? _source;

    [Notify]
    private WindowInfo? _target;

    [Notify]
    private AppState _appState = AppState.NotRunning;

    public bool IsRefreshButtonEnabled => AppState == AppState.NotRunning;

    public bool IsStartButtonEnabled => AppState == AppState.NotRunning
                                        && Source != null
                                        && Target != null
                                        && Source.WindowHandle != Target.WindowHandle;

    public bool IsStopButtonEnabled => AppState == AppState.Running;

    public bool IsSourceTargetComboBoxEnabled => AppState == AppState.NotRunning;

    public Brush RefreshButtonSvgColor => IsRefreshButtonEnabled
        ? Brushes.Black
        : Brushes.DimGray;

    public Brush StartButtonSvgColor => IsStartButtonEnabled
        ? Brushes.Black
        : Brushes.DimGray;

    public Brush StopButtonSvgColor => IsStopButtonEnabled
        ? Brushes.Black
        : Brushes.DimGray;

    private Task? _mouseEventProcessingLoopTask;
    private Task? _updateMouseHookRectsLoopTask;

    private int _smCxScreen;
    private int _smCyScreen;

    public MainViewModel(
        ILogger<MainViewModel> logger,
        WinApiService winApiService,
        MouseHook mouseHook)
    {
        _logger = logger;
        _winApiService = winApiService;
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
        _updateMouseHookRectsLoopTask = Task.Run(async () =>
            await RunUpdateMouseHookRectsLoopAsync(token),
            token);
    }

    private static INPUT CreateScrollInput(nuint mouseMessageId, int absoluteX, int absoluteY, short delta)
    {
        var inputScroll = new INPUT
        {
            type = INPUT_TYPE.INPUT_MOUSE,
        };
        var dwFlags = mouseMessageId switch
        {
            WinApiConstants.WM_MOUSEWHEEL => MOUSE_EVENT_FLAGS.MOUSEEVENTF_WHEEL | MOUSE_EVENT_FLAGS.MOUSEEVENTF_ABSOLUTE | MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE_NOCOALESCE,
            WinApiConstants.WM_MOUSEHWHEEL => MOUSE_EVENT_FLAGS.MOUSEEVENTF_HWHEEL | MOUSE_EVENT_FLAGS.MOUSEEVENTF_ABSOLUTE | MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE_NOCOALESCE,
            _ => MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE,
        };
        inputScroll.Anonymous.mi.dwFlags = dwFlags;
        inputScroll.Anonymous.mi.time = 0;
        inputScroll.Anonymous.mi.mouseData = (uint)delta;
        inputScroll.Anonymous.mi.dx = absoluteX;
        inputScroll.Anonymous.mi.dy = absoluteY;
        inputScroll.Anonymous.mi.dwExtraInfo = MouseHook.InjectedEventMagicNumber;

        return inputScroll;
    }

    private static INPUT CreateMoveInput(int absoluteX, int absoluteY)
    {
        var inputMove = new INPUT
        {
            type = INPUT_TYPE.INPUT_MOUSE,
        };
        inputMove.Anonymous.mi.dwFlags = MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE | MOUSE_EVENT_FLAGS.MOUSEEVENTF_ABSOLUTE | MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE_NOCOALESCE;
        inputMove.Anonymous.mi.time = 0;
        inputMove.Anonymous.mi.mouseData = 0;
        inputMove.Anonymous.mi.dx = absoluteX;
        inputMove.Anonymous.mi.dy = absoluteY;
        inputMove.Anonymous.mi.dwExtraInfo = MouseHook.InjectedEventMagicNumber;

        return inputMove;
    }

    private (int X, int Y) CalculateAbsoluteCoordinates(int x, int y)
    {
        return (
            X: (x * 65536) / _smCxScreen,
            Y: (y * 65536) / _smCyScreen
        );
    }

    private Point CalculateCenterOfWindow(WindowRect rect)
    {
        return new Point(
            rect.Left + ((rect.Right - rect.Left) / 2),
            rect.Top + ((rect.Bottom - rect.Top) / 2));
    }

    private async Task RunMouseEventProcessingLoopAsync(
        Channel<MouseMessageInfo> channel,
        CancellationToken cancellationToken = default)
    {
        while (await channel.Reader.WaitToReadAsync(cancellationToken))
        {
            while (channel.Reader.TryRead(out var buffer))
            {
                try
                {
                    if (AppState != AppState.Running)
                    {
                        // _logger.LogTrace("App is not running, skipping mouse event processing");
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

                    if (Source.WindowHandle == Target.WindowHandle)
                    {
                        _logger.LogTrace("Source and target windows are the same, skipping mouse event processing");
                        continue;
                    }

                    if ((ulong)buffer.MouseMessageId is not
                        (WinApiConstants.WM_MOUSEWHEEL
                        or WinApiConstants.WM_MOUSEHWHEEL))
                    {
                        _logger.LogTrace("Skipping non-scroll mouse event");
                        continue;
                    }

                    var sourceEventX = buffer.MouseMessageData.pt.X;
                    var sourceEventY = buffer.MouseMessageData.pt.Y;

                    var sourceRect = PInvoke.GetWindowRect((HWND)Source.WindowHandle);
                    var targetRect = PInvoke.GetWindowRect((HWND)Target.WindowHandle);

                    if (!WinApiUtils.PointInRect(sourceRect, sourceEventX, sourceEventY))
                    {
                        // _logger.LogTrace("Mouse event is not in the source window, skipping");
                        continue;
                    }

                    var actualSourceWindowHwnd = PInvoke.WindowFromPoint(new Point(sourceEventX, sourceEventY));
                    var (_, actualSourceProcessId, _) = PInvoke.GetWindowThreadProcessId(actualSourceWindowHwnd);

                    if (actualSourceProcessId != Source.ProcessId)
                    {
                        _logger.LogTrace("Actual source window does not belong to the selected source process, skipping");
                        continue;
                    }

                    var relativeX = sourceEventX - sourceRect.Left;
                    var relativeY = sourceEventY - sourceRect.Top;

                    var targetX = targetRect.Left + relativeX;
                    var targetY = targetRect.Top + relativeY;

                    if (!WinApiUtils.PointInRect(targetRect, targetX, targetY))
                    {
                        _logger.LogTrace("Resulting mouse event is not in the target window, falling back to center of the target window");
                        var centerOfTarget = CalculateCenterOfWindow(targetRect);
                        targetX = centerOfTarget.X;
                        targetY = centerOfTarget.Y;
                    }

                    var actualTargetWindowHwnd = PInvoke.WindowFromPoint(new Point(targetX, targetY));
                    var (_, actualTargetWindowProcessId, _) = PInvoke.GetWindowThreadProcessId(actualTargetWindowHwnd);

                    if (actualTargetWindowProcessId != Target.ProcessId)
                    {
                        _logger.LogTrace("Actual target window does not belong to the selected target process, skipping");
                        continue;
                    }

                    // If the message is WM_MOUSEWHEEL, the high-order word of this member is the wheel delta. The low-order word is reserved.
                    var (_, delta) = WinApiUtils.GetHiLoWords(buffer.MouseMessageData.mouseData);

                    var (sourceAbsoluteX, sourceAbsoluteY) = CalculateAbsoluteCoordinates(sourceEventX, sourceEventY);
                    var (targetAbsoluteX, targetAbsoluteY) = CalculateAbsoluteCoordinates(targetX, targetY);

                    var inputMoveToTarget = CreateMoveInput(targetAbsoluteX, targetAbsoluteY);
                    var inputScrollTarget = CreateScrollInput(buffer.MouseMessageId, targetAbsoluteX, targetAbsoluteY, delta);
                    var inputMoveToSource = CreateMoveInput(sourceAbsoluteX, sourceAbsoluteY);

                    var inputs = new[] { inputMoveToTarget, inputScrollTarget, inputMoveToSource };
                    var sizeOfInput = Marshal.SizeOf(typeof(INPUT));

                    _mouseHook.SetPreventRealScrollEvents();
                    PInvoke.SendInput(inputs.AsSpan(), sizeOfInput);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error processing mouse event");
                }
            }
        }
    }

    private async Task RunUpdateMouseHookRectsLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500), token);

            if (AppState == AppState.NotRunning || Source is null || Target is null)
            {
                _mouseHook.SetSourceRect(null);
                _mouseHook.SetTargetRect(null);
            }
            else
            {
                try
                {
                    var sourceRect = PInvoke.GetWindowRect((HWND)Source.WindowHandle);
                    var targetRect = PInvoke.GetWindowRect((HWND)Target.WindowHandle);

                    var centerOfSource = CalculateCenterOfWindow(sourceRect);
                    var centerOfTarget = CalculateCenterOfWindow(targetRect);

                    var actualSourceWindowHwnd = PInvoke.WindowFromPoint(centerOfSource);
                    var (_, actualSourceProcessId, _) = PInvoke.GetWindowThreadProcessId(actualSourceWindowHwnd);

                    var actualTargetWindowHwnd = PInvoke.WindowFromPoint(centerOfTarget);
                    var (_, actualTargetWindowProcessId, _) = PInvoke.GetWindowThreadProcessId(actualTargetWindowHwnd);

                    if (actualSourceProcessId != Source.ProcessId
                        || actualTargetWindowProcessId != Target.ProcessId)
                    {
                        // _logger.LogTrace("Source or target window is not in the foreground, setting rects to null");
                        _mouseHook.SetSourceRect(null);
                        _mouseHook.SetTargetRect(null);
                    }
                    else
                    {
                        _mouseHook.SetSourceRect(sourceRect);
                        _mouseHook.SetTargetRect(targetRect);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error updating mouse hook rects");

                    _mouseHook.SetSourceRect(null);
                    _mouseHook.SetTargetRect(null);
                }
            }
        }
    }

    private void RefreshWindows()
    {
        _logger.LogInformation("Refreshing windows");

        var newWindows = _winApiService.ListWindows();

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

            _smCxScreen = PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXSCREEN);
            _smCyScreen = PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYSCREEN);

            AppState = AppState.Running;
        }
    }

    private void Stop()
    {
        _logger.LogInformation("Stopping scroll sync");
        AppState = AppState.NotRunning;
    }

    public void HandleWindowClosing()
    {
        _mouseHook.Uninstall();
        AppState = AppState.NotRunning;
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
        AppState = AppState.NotRunning;
        try
        {
            _cancellationTokenSource.Cancel();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error cancelling the cancellation token source");
        }

        _updateMouseHookRectsLoopTask?.Dispose();
        _mouseEventProcessingLoopTask?.Dispose();
        _mouseHook.Dispose();
        _cancellationTokenSource.Dispose();
    }
}
