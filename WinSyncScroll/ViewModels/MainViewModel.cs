using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Windows.Data;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using CommunityToolkit.Mvvm.Input;
using EasyHook;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PropertyChanged.SourceGenerator;
using WinSyncScroll.Hook;
using WinSyncScroll.Hook.EventArguments;
using WinSyncScroll.Models;
using WinSyncScroll.Services;

namespace WinSyncScroll.ViewModels;

public partial class MainViewModel
{
    private readonly ILogger<MainViewModel> _logger;
    private readonly WinApiManager _winApiManager;

    public ObservableCollection<WindowInfo> Windows { get; } = [];
    public ICollectionView WindowsOrdered { get; }

    public RelayCommand RefreshWindowsCommand { get; }
    public RelayCommand StartCommand { get; }

    [Notify]
    private WindowInfo? _source { get; set; }

    [Notify]
    private WindowInfo? _target { get; set; }

    private object _eventProcessingLockObject = new();

    public MainViewModel(
        ILogger<MainViewModel> logger,
        WinApiManager winApiManager)
    {
        _logger = logger;
        _winApiManager = winApiManager;
        RefreshWindowsCommand = new RelayCommand(RefreshWindows);
        StartCommand = new RelayCommand(Start);

        WindowsOrdered = CollectionViewSource.GetDefaultView(Windows);
        WindowsOrdered.SortDescriptions.Add(new SortDescription(nameof(WindowInfo.WindowName), ListSortDirection.Ascending));
        WindowsOrdered.SortDescriptions.Add(new SortDescription(nameof(WindowInfo.ProcessName), ListSortDirection.Ascending));
        WindowsOrdered.SortDescriptions.Add(new SortDescription(nameof(WindowInfo.ProcessId), ListSortDirection.Ascending));
        WindowsOrdered.SortDescriptions.Add(new SortDescription(nameof(WindowInfo.ClassName), ListSortDirection.Ascending));
        WindowsOrdered.SortDescriptions.Add(new SortDescription(nameof(WindowInfo.WindowHandleLong), ListSortDirection.Ascending));
    }

    private void RefreshWindows()
    {
        var newWindows = _winApiManager.ListWindows();

        var toBeRemoved = Windows
            .Where(w => newWindows.All(nw => nw.WindowHandle != w.WindowHandle))
            .ToList();

        foreach (var window in toBeRemoved)
        {
            Windows.Remove(window);
        }

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

        var toBeAdded = newWindows
            .Where(nw => Windows.All(w => nw.WindowHandle != w.WindowHandle))
            .ToList();

        foreach (var window in toBeAdded)
        {
            Windows.Add(window);
        }
    }

    public static nuint CreateWParam(int hiWord, int loWord)
    {
        // Ensure the words fit within their respective 16-bit spaces
        if (hiWord < 0 || hiWord > 0xFFFF)
            throw new ArgumentOutOfRangeException(nameof(hiWord), "HIWORD must be between 0 and 65535.");
        if (loWord < 0 || loWord > 0xFFFF)
            throw new ArgumentOutOfRangeException(nameof(loWord), "LOWORD must be between 0 and 65535.");

        // Combine HIWORD and LOWORD into a single nuint value
        nuint wParam = (nuint)((hiWord << 16) | (loWord & 0xFFFF));
        return wParam;
    }

    public static nint CreateLParam(int hiWord, int loWord)
    {
        // Ensure the words fit within their respective 16-bit spaces
        if (hiWord < 0 || hiWord > 0xFFFF)
            throw new ArgumentOutOfRangeException(nameof(hiWord), "HIWORD must be between 0 and 65535.");
        if (loWord < 0 || loWord > 0xFFFF)
            throw new ArgumentOutOfRangeException(nameof(loWord), "LOWORD must be between 0 and 65535.");

        // Combine HIWORD and LOWORD into a single nint value
        nint lParam = (hiWord << 16) | (loWord & 0xFFFF);
        return lParam;
    }

    static int GetWheelDelta(nuint wParam)
    {
        return (short)(wParam >> 16);
    }

    static bool IsKeyDown(nuint wParam, int key)
    {
        return (wParam & (nuint)key) != 0;
    }

    private void Start()
    {
        if (Source == null || Target == null)
        {
            _logger.LogWarning("Source or target window is not selected: Source={Source}, Target={Target}", Source, Target);
            return;
        }
        else
        {
            _logger.LogInformation("Starting sync between \"{SourceWindow}\" and \"{TargetWindow}\"", Source.DisplayName, Target.DisplayName);
        }

        DateTime? latestPing = null;
        bool exited = false;

        var remoteObject = new CustomRemoteObject();
        remoteObject.InjectionEvent += (sender, eventArgs) => { _logger.LogInformation("Injected into {EventArgsClientProcessId}", eventArgs.ClientProcessId); };
        remoteObject.EntryPointEvent += (sender, eventArgs) => { _logger.LogInformation("Entry point event"); };
        remoteObject.WindowProcEvent += RemoteObjectOnWindowProcEvent;
        remoteObject.LogMessageEvent += (sender, eventArgs) => { _logger.LogInformation("Log message event: {Message}", eventArgs.Message); };
        remoteObject.PingEvent += (sender, eventArgs) =>
        {
            _logger.LogTrace("Ping event");
            latestPing = DateTime.UtcNow;
        };
        remoteObject.ExitEvent += (sender, eventArgs) =>
        {
            _logger.LogInformation("Exit event");
            exited = true;
        };
        remoteObject.ExceptionEvent += (sender, eventArgs) => { _logger.LogError("Exception event ({EventId}): {Message}. Exception={SerializedException}", eventArgs.EventId, eventArgs.Message, eventArgs.SerializedException); };

        string? channelName = null;
        var ipcServerChannel = RemoteHooking.IpcCreateServer<CustomRemoteObject>(ref channelName, WellKnownObjectMode.Singleton, remoteObject);

        var parameter = new EntryPointParameters
        {
            Message = "hello world",
            HostProcessId = RemoteHooking.GetCurrentProcessId(),
        };

        var processId = Source.ProcessId;
        RemoteHooking.Inject(processId,
            InjectionOptions.Default | InjectionOptions.DoNotRequireStrongName,
            typeof(EntryPointParameters).Assembly.Location,
            typeof(EntryPointParameters).Assembly.Location,
            channelName,
            parameter);

        while (!exited)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            if (exited)
            {
                Console.WriteLine("Exit event received");
                break;
            }
        }
    }

    private bool PointInRect(WindowRect windowRect, int x, int y)
    {
        return x >= windowRect.Left
            && x <= windowRect.Right
            && y >= windowRect.Top
            && y <= windowRect.Bottom;
    }

    private void RemoteObjectOnWindowProcEvent(object sender, WindowProcEventArgs eventArgs)
    {
        lock (_eventProcessingLockObject)
        {
            var serializedArgs = eventArgs.SerializedArgs;
            if (!string.IsNullOrWhiteSpace(serializedArgs)
                && serializedArgs != null
                && serializedArgs.Contains('{')
                && serializedArgs.Contains('}'))
            {
                var parsedEventArgs = JsonConvert.DeserializeObject<ParsedWindowProcEventArgs>(serializedArgs);

                if (parsedEventArgs != null)
                {
                    uint msg = uint.TryParse(parsedEventArgs.Msg, out var msgValue) ? msgValue : 0;
                    nuint wParam = ulong.TryParse(parsedEventArgs.WParam, out var wParamValue) ? (nuint)wParamValue : 0;
                    nint lParam = long.TryParse(parsedEventArgs.LParam, out var lParamValue) ? (nint)lParamValue : 0;

                    if (msg is NativeConstants.WM_MOUSEWHEEL
                        or NativeConstants.WM_MOUSEHWHEEL)
                    {
                        int scrollEventX = unchecked((short)(long)lParam);
                        int scrollEventY = unchecked((short)((long)lParam >> 16));

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

                        if (!PointInRect(sourceRect, scrollEventX, scrollEventY))
                        {
                            return;
                        }

                        var relativeX = scrollEventX - sourceRect.Left;
                        var relativeY = scrollEventY - sourceRect.Top;

                        var targetX = targetRect.Left + relativeX;
                        var targetY = targetRect.Top + relativeY;

                        var delta = GetWheelDelta(wParam);

                        var input = new INPUT
                        {
                            type = INPUT_TYPE.INPUT_MOUSE,
                        };
                        var dwFlags = msg switch
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
                        input.Anonymous.mi.dwExtraInfo = (nuint)(nint)PInvoke.GetMessageExtraInfo();

                        var inputs = new[] { input };
                        var sizeOfInput = Marshal.SizeOf(typeof(INPUT));

                        PInvoke.SetCursorPos(targetX, targetY);
                        PInvoke.SendInput(inputs.AsSpan(), sizeOfInput);
                        PInvoke.SetCursorPos(scrollEventX, scrollEventY);
                    }
                }
            }
        }
    }
}
