using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
            .Where(w => newWindows.All(nw => nw.WindowHandle != w.WindowHandle));

        var toBeUpdated = Windows
            .Join(newWindows,
                w => w.WindowHandle,
                nw => nw.WindowHandle,
                (w, nw) => new
                {
                    OldWindow = w,
                    NewWindow = nw,
                });

        var toBeAdded = newWindows
            .Where(nw => Windows.All(w => nw.WindowHandle != w.WindowHandle));

        foreach (var window in toBeRemoved)
        {
            Windows.Remove(window);
        }

        foreach (var windowToBeUpdated in toBeUpdated)
        {
            var i = Windows.IndexOf(windowToBeUpdated.OldWindow);
            if (i != -1)
            {
                Windows[i] = windowToBeUpdated.NewWindow;
            }
        }

        foreach (var window in toBeAdded)
        {
            Windows.Add(window);
        }
    }

    static IntPtr MakeLParam(int x, int y)
    {
        return (IntPtr)((y << 16) | (x & 0xFFFF));
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
        remoteObject.WindowProcEvent += (sender, eventArgs) =>
        {
            // _logger.LogTrace("WindowProc event: {SerializedArgs}", eventArgs.SerializedArgs);

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

                    var newLparam = MakeLParam(Target.WindowRect.Left + 10, Target.WindowRect.Top + 10);
                    var delta = GetWheelDelta(wParam);

                    _logger.LogTrace("Sending message: hWnd={WindowHandle}, msg={Msg}, wParam={WParam}, lParam={LParam}", Target.WindowHandle, msg, wParam, lParam);

                    //PInvoke.SetForegroundWindow((HWND)Target.WindowHandle);
                    //PInvoke.SetFocus((HWND)Target.WindowHandle);

                    PInvoke.SendMessage((HWND)Target.WindowHandle, msg, 120, newLparam);
                    PInvoke.PostMessage((HWND)Target.WindowHandle, msg, 120, newLparam);

                    //PInvoke.SetForegroundWindow((HWND)Source.WindowHandle);
                    //PInvoke.SetFocus((HWND)Source.WindowHandle);

                    //PInvoke.mouse_event(MOUSE_EVENT_FLAGS.MOUSEEVENTF_WHEEL, Target.WindowRect.Left + 10, Target.WindowRect.Top + 10, delta, 0);

                }
            }
        };
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
}
