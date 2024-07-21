using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using EasyHook;

namespace WinSyncScroll.Hook;

public class EntryPoint : IEntryPoint
{
    private readonly CustomRemoteObject _remoteObject;

    public EntryPoint(RemoteHooking.IContext context, string channelName, EntryPointParameters parameter)
    {
        // connect to host
        _remoteObject = RemoteHooking.IpcConnectClient<CustomRemoteObject>(channelName);
        _remoteObject.TriggerEntryPointEvent();
    }

    public void Run(RemoteHooking.IContext context, string channelName, EntryPointParameters parameter)
    {
        try
        {
            using var hook = LocalHook.Create(
                LocalHook.GetProcAddress("USER32.dll", "CallWindowProcW"),
                new CallWindowProcWFnPtr(CallWindowProcHooked),
                this);

            // Don't forget that all hooks will start deactivated.
            // The following ensures that all threads are intercepted:
            hook.ThreadACL.SetExclusiveACL(new int[1]);

            _remoteObject.TriggerInjectionEvent(RemoteHooking.GetCurrentProcessId());

            while (true)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1000));
                _remoteObject.TriggerPingEvent();
            }
        }
        catch (Exception e)
        {
            // We should notice our host process about this error
            _remoteObject.TriggerExceptionEvent(e);
        }
        finally
        {
            _remoteObject.TriggerExitEvent();
        }
    }

    private void SendLogMessageEvent(string logMessage)
    {
        _remoteObject.TriggerLogMessageEvent(logMessage);
    }

    private void SendWindowProcEventInfo(string serializedArgs)
    {
        _remoteObject.TriggerWindowProcEvent(serializedArgs);
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
    private delegate LRESULT CallWindowProcWFnPtr(WNDPROC lpPrevWndFunc, HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam);

    private static LRESULT CallWindowProcHooked(WNDPROC lpPrevWndFunc, HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
        var self = (EntryPoint)HookRuntimeInfo.Callback;
        var msgStr = msg.ToString();
        var wParamStr = ((ulong)wParam.Value).ToString();
        var lParamStr = ((long)lParam.Value).ToString();

        // self.SendLogMessageEvent($"CallWindowProc hook called; Msg: {msgStr}, WParam: {wParamStr}, LParam: {lParamStr}");

        if (msg is NativeConstants.WM_MOUSEWHEEL
            or NativeConstants.WM_MOUSEHWHEEL)
        {
            self.SendWindowProcEventInfo($"{{ Msg: {msgStr}, WParam: {wParamStr}, LParam: {lParamStr} }}");
        }

        // call original API
        return PInvoke.CallWindowProc(lpPrevWndFunc, hWnd, msg, wParam, lParam);
    }
}
