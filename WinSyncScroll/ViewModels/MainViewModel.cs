﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using System.Windows.Data;
using System.Windows.Threading;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PropertyChanged.SourceGenerator;
using WinSyncScroll.Enums;
using WinSyncScroll.Extensions;
using WinSyncScroll.Models;
using WinSyncScroll.Services;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using HWND = Windows.Win32.Foundation.HWND;
using Point = System.Drawing.Point;

namespace WinSyncScroll.ViewModels;

public sealed partial class MainViewModel : IDisposable
{
    private readonly ILogger<MainViewModel> _logger;
    private readonly IOptions<GeneralOptions> _options;
    private readonly WinApiService _winApiService;
    private readonly MouseHook _mouseHook;

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Dispatcher? _dispatcher;

    // ReSharper disable once MemberCanBePrivate.Global
    public ObservableCollection<WindowInfo> Windows { get; } = [];

    // ReSharper disable once MemberCanBePrivate.Global
    public ICollectionView WindowsOrdered { get; }

    public AsyncRelayCommand RefreshWindowsCommand { get; }
    public RelayCommand StartCommand { get; }
    public RelayCommand StopCommand { get; }

    [Notify]
    private WindowInfo? _source;

    [Notify]
    private WindowInfo? _target;

    [Notify]
    private AppState _appState = AppState.NotRunning;

    [Notify]
    private bool _isRefreshing;

    public bool IsRefreshButtonEnabled => AppState == AppState.NotRunning && !IsRefreshing;

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

    public static string AppTitle => $"WinSyncScroll {System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion}";

    private Task? _mouseEventProcessingLoopTask;
    private Task? _updateMouseHookRectsLoopTask;

    private int _smCxScreen;
    private int _smCyScreen;

    private static readonly int SizeOfInput = Marshal.SizeOf(typeof(INPUT));

    public MainViewModel(
        ILogger<MainViewModel> logger,
        IOptions<GeneralOptions> options,
        WinApiService winApiService,
        MouseHook mouseHook)
    {
        _logger = logger;
        _options = options;
        _winApiService = winApiService;
        _mouseHook = mouseHook;

        RefreshWindowsCommand = new AsyncRelayCommand(RefreshWindowsAsync);
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
        inputScroll.Anonymous.mi.dwExtraInfo = (nuint)(nint)PInvoke.GetMessageExtraInfo();

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
        inputMove.Anonymous.mi.dwExtraInfo = (nuint)(nint)PInvoke.GetMessageExtraInfo();

        return inputMove;
    }

    private (int X, int Y) CalculateAbsoluteCoordinates(int x, int y)
    {
        return (
            X: PInvoke.MulDiv(x, 65536, _smCxScreen),
            Y: PInvoke.MulDiv(y, 65536, _smCyScreen)
        );
    }

    private static Point CalculateCenterOfWindow(WindowRect rect)
    {
        return new Point(
            rect.Left + ((rect.Right - rect.Left) / 2),
            rect.Top + ((rect.Bottom - rect.Top) / 2));
    }

    private void LogSendInput(INPUT[] inputs)
    {
        _logger.LogTrace("Sending input to target window: {NewLine}{Inputs}",
            Environment.NewLine,
            string.Join(
                Environment.NewLine,
                inputs.Select((item, index) =>
                    $"{index.ToStringInvariant()}: {item.ToLogString()}"))
        );
    }

    private static List<HWND> EnumChildWindows(HWND parentWindowHandle)
    {
        var windows = new List<HWND>();

        PInvoke.EnumChildWindows(
            parentWindowHandle,
            (hwnd, _) =>
            {
                var isVisible = PInvoke.IsWindowVisible(hwnd);

                if (isVisible)
                {
                    windows.Add(hwnd);
                }

                return true;
            },
            0);

        return windows;
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
                        _logger.LogTrace("Mouse event is not in the source window, skipping");
                        continue;
                    }

                    if (_options.Value.IsStrictProcessIdCheckEnabled)
                    {
                        var actualSourceWindowHwnd = PInvoke.WindowFromPoint(new Point(sourceEventX, sourceEventY));
                        var (_, actualSourceProcessId, _) = PInvoke.GetWindowThreadProcessId(actualSourceWindowHwnd);

                        if (actualSourceProcessId != Source.ProcessId)
                        {
                            _logger.LogTrace("Window in source rect (pid={ActualSourceProcessId}) does not belong to the selected source process (pid={SourceProcessId}), skipping", actualSourceProcessId, Source.ProcessId);
                            continue;
                        }
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

                    if (_options.Value.IsStrictProcessIdCheckEnabled)
                    {
                        var actualTargetWindowHwnd = PInvoke.WindowFromPoint(new Point(targetX, targetY));
                        var (_, actualTargetWindowProcessId, _) = PInvoke.GetWindowThreadProcessId(actualTargetWindowHwnd);

                        if (actualTargetWindowProcessId != Target.ProcessId)
                        {
                            _logger.LogTrace("Window in target rect (pid={ActualTargetWindowProcessId}) does not belong to the selected target process (pid={TargetProcessId}), skipping", actualTargetWindowProcessId, Target.ProcessId);
                            continue;
                        }
                    }

                    // If the message is WM_MOUSEWHEEL, the high-order word of this member is the wheel delta. The low-order word is reserved.
                    var (_, delta) = WinApiUtils.GetHiLoWords(buffer.MouseMessageData.mouseData);

                    var (sourceAbsoluteX, sourceAbsoluteY) = CalculateAbsoluteCoordinates(sourceEventX, sourceEventY);
                    var (targetAbsoluteX, targetAbsoluteY) = CalculateAbsoluteCoordinates(targetX, targetY);

                    _logger.LogTrace("Converted coordinates: Source=({SourceEventX},{SourceEventY}) -> ({SourceAbsoluteX},{SourceAbsoluteY}), Target=({TargetX},{TargetY}) -> ({TargetAbsoluteX},{TargetAbsoluteY}). _smCxScreen={SmCxScreen}, _smCyScreen={SmCyScreen}",
                        sourceEventX,
                        sourceEventY,
                        sourceAbsoluteX,
                        sourceAbsoluteY,
                        targetX,
                        targetY,
                        targetAbsoluteX,
                        targetAbsoluteY,
                        _smCxScreen,
                        _smCyScreen);

                    var inputMoveToTarget = CreateMoveInput(targetAbsoluteX, targetAbsoluteY);
                    var inputScrollTarget = CreateScrollInput(buffer.MouseMessageId, targetAbsoluteX, targetAbsoluteY, delta);
                    var inputMoveToSource = CreateMoveInput(sourceAbsoluteX, sourceAbsoluteY);

                    var inputs = new[] { inputMoveToTarget, inputScrollTarget, inputMoveToSource };

                    _mouseHook.SetPreventRealScrollEvents();

                    if (!_options.Value.IsLegacyModeEnabled)
                    {
                        LogSendInput(inputs);
                        PInvoke.SendInput(inputs.AsSpan(), SizeOfInput);
                    }
                    else
                    {
                        var childWindows = EnumChildWindows((HWND)Target.WindowHandle);
                        var lParam = PInvoke.MAKELPARAM((ushort)targetX, (ushort)targetY);

                        foreach (var windowHandle in childWindows)
                        {
                            _logger.LogTrace("Sending message to window: hwnd={WindowHandle}, msg={MouseMessageId}, wParam={MouseData}, lParam={LParam}",
                                windowHandle,
                                buffer.MouseMessageId,
                                buffer.MouseMessageData.mouseData,
                                lParam);
                            PInvoke.SendMessage(windowHandle, (uint)buffer.MouseMessageId, (nuint)buffer.MouseMessageData.mouseData, lParam);
                        }
                    }
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

                    _mouseHook.SetSourceRect(sourceRect);
                    _mouseHook.SetTargetRect(targetRect);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error updating mouse hook rects");

                    _mouseHook.SetSourceRect(null);
                    _mouseHook.SetTargetRect(null);

                    if (_dispatcher is not null)
                    {
                        await _dispatcher.InvokeAsync(async () =>
                        {
                            if (StopCommand.CanExecute(null))
                            {
                                StopCommand.Execute(null);
                            }

                            if (RefreshWindowsCommand.CanExecute(null))
                            {
                                await RefreshWindowsCommand.ExecuteAsync(null);
                            }
                        });
                    }
                }
            }
        }
    }

    private async Task RefreshWindowsAsync()
    {
        _logger.LogInformation("Refreshing windows");
        IsRefreshing = true;

        try
        {
            var newWindows = await Task.Run(() => _winApiService.ListWindows());

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
        finally
        {
            IsRefreshing = false;
        }
    }

    public void Initialize(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;

        InstallMouseHook();
        _ = RefreshWindowsAsync();
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
