using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using Microsoft.Extensions.Logging;
using Vanara.PInvoke;
using WinSyncScroll.Exceptions;
using WinSyncScroll.Models;

namespace WinSyncScroll.Services;

public sealed class MouseHook : IDisposable
{
    public const int InjectedEventMagicNumber = 520165553;

    private readonly ILogger<MouseHook> _logger;

    private UnhookWindowsHookExSafeHandle? _unhookSafeHandle;
    private Process? _process;
    private ProcessModule? _module;
    private FreeLibrarySafeHandle? _moduleHandle;
    private HOOKPROC? _hookProc;

    private volatile WindowRect? _sourceRect;
    private volatile WindowRect? _targetRect;
    private volatile DateTimeObj _preventRealScrollEvents = new(DateTime.MinValue);

    public Channel<MouseMessageInfo> HookEvents { get; } = Channel.CreateUnbounded<MouseMessageInfo>();

    public MouseHook(ILogger<MouseHook> logger)
    {
        _logger = logger;
    }

    private UnhookWindowsHookExSafeHandle SetHook(HOOKPROC proc)
    {
        _process = Process.GetCurrentProcess();
        _module = _process.MainModule ?? throw new ServiceException("Failed to get main module");
        _moduleHandle = PInvoke.GetModuleHandle(_module.ModuleName);
        _hookProc = proc;
        return PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_MOUSE_LL, _hookProc, _moduleHandle, 0);
    }

    private bool IsPreventRealScrollEventsActive()
    {
        return _preventRealScrollEvents.DateTime.AddMilliseconds(200) >= DateTime.UtcNow;
    }

    /// <summary>
    /// <para>An application-defined or library-defined callback function used with the SetWindowsHookExA/SetWindowsHookExW function. The system calls this function every time a new mouse input event is about to be posted into a thread input queue.</para>
    /// <para>The HOOKPROC type defines a pointer to this callback function. LowLevelMouseProc is a placeholder for the application-defined or library-defined function name.</para>
    /// <para>LowLevelMouseProc is a placeholder for the application-defined or library-defined function name.</para>
    /// </summary>
    /// <param name="code">
    /// <para>A code the hook procedure uses to determine how to process the message.</para>
    /// <para>If nCode is less than zero, the hook procedure must pass the message to the CallNextHookEx function without further processing and should return the value returned by CallNextHookEx.</para>
    /// <para>This parameter can be one of the following values: HC_ACTION (0)</para>
    /// </param>
    /// <param name="wParam">
    /// <para>The identifier of the mouse message.</para>
    /// <para>This parameter can be one of the following messages: WM_LBUTTONDOWN, WM_LBUTTONUP, WM_MOUSEMOVE, WM_MOUSEWHEEL, WM_RBUTTONDOWN or WM_RBUTTONUP.</para>
    /// </param>
    /// <param name="lParam">
    /// A pointer to an MSLLHOOKSTRUCT structure.
    /// </param>
    /// <returns>
    /// <para>If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx.</para>
    /// <para>If nCode is greater than or equal to zero, and the hook procedure did not process the message, it is highly recommended that you call CallNextHookEx and return the value it returns; otherwise, other applications that have installed WH_MOUSE_LL hooks will not receive hook notifications and may behave incorrectly as a result.</para>
    /// <para>If the hook procedure processed the message, it may return a nonzero value to prevent the system from passing the message to the rest of the hook chain or the target window procedure.</para>
    /// </returns>
    /// <remarks>
    /// <para>An application installs the hook procedure by specifying the WH_MOUSE_LL hook type and a pointer to the hook procedure in a call to the SetWindowsHookExA/SetWindowsHookExW function.</para>
    /// <para>This hook is called in the context of the thread that installed it. The call is made by sending a message to the thread that installed the hook. Therefore, the thread that installed the hook must have a message loop.</para>
    /// <para>The mouse input can come from the local mouse driver or from calls to the mouse_event function. If the input comes from a call to mouse_event, the input was "injected". However, the WH_MOUSE_LL hook is not injected into another process. Instead, the context switches back to the process that installed the hook and it is called in its original context. Then the context switches back to the application that generated the event.</para>
    /// <para>The hook procedure should process a message in less time than the data entry specified in the LowLevelHooksTimeout value in the following registry key:</para>
    /// <para>HKEY_CURRENT_USER\Control Panel\Desktop</para>
    /// <para>The value is in milliseconds. If the hook procedure times out, the system passes the message to the next hook. However, on Windows 7 and later, the hook is silently removed without being called. There is no way for the application to know whether the hook is removed.</para>
    /// <para>Windows 10 version 1709 and later The maximum timeout value the system allows is 1000 milliseconds (1 second). The system will default to using a 1000 millisecond timeout if the LowLevelHooksTimeout value is set to a value larger than 1000.</para>
    /// </remarks>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/winmsg/lowlevelmouseproc" />
    private LRESULT HookFunc(int code, WPARAM wParam, LPARAM lParam)
    {
        if (code >= 0
            && (wParam == WinApiConstants.WM_MOUSEWHEEL
                || wParam == WinApiConstants.WM_MOUSEHWHEEL
                || wParam == WinApiConstants.WM_MOUSEMOVE))
        {
            var mouseLowLevelDataObj = Marshal.PtrToStructure(lParam, typeof(User32.MSLLHOOKSTRUCT));
            if (mouseLowLevelDataObj is null)
            {
                _logger.LogWarning("Failed to marshal {NameOfLParam} to {NameOfStruct}", nameof(lParam), nameof(User32.MSLLHOOKSTRUCT));
            }
            else
            {
                var mouseLowLevelData = (User32.MSLLHOOKSTRUCT)mouseLowLevelDataObj;

                if (mouseLowLevelData.dwExtraInfo == InjectedEventMagicNumber)
                {
                    // we have nothing to do with injected events
                }
                else if (_sourceRect is not null
                         && WinApiUtils.PointInRect(_sourceRect, mouseLowLevelData.pt.X, mouseLowLevelData.pt.Y)
                         && (wParam == WinApiConstants.WM_MOUSEWHEEL
                             || wParam == WinApiConstants.WM_MOUSEHWHEEL))
                {
                    // we should process scroll events in this area
                    HookEvents.Writer.TryWrite(new MouseMessageInfo
                    {
                        MouseMessageId = wParam,
                        MouseMessageData = mouseLowLevelData,
                    });
                }
                else if (IsPreventRealScrollEventsActive()
                         && _targetRect is not null
                         && WinApiUtils.PointInRect(_targetRect, mouseLowLevelData.pt.X, mouseLowLevelData.pt.Y))
                {
                    // prevent the system from passing the message to the rest of the hook chain or the target window procedure
                    return (LRESULT)1;
                }
            }
        }

        return PInvoke.CallNextHookEx(_unhookSafeHandle, code, wParam, lParam);
    }

    public void SetSourceRect(WindowRect? rect)
    {
        _sourceRect = rect;
    }

    public void SetTargetRect(WindowRect? rect)
    {
        _targetRect = rect;
    }

    public void SetPreventRealScrollEvents()
    {
        _preventRealScrollEvents = new DateTimeObj(DateTime.UtcNow);
    }

    public void Install()
    {
        _logger.LogInformation("Installing mouse hook");

        if (_unhookSafeHandle is not null)
        {
            throw new InvalidOperationException("Mouse hook is already installed");
        }

        _unhookSafeHandle = SetHook(HookFunc);
    }

    public void Uninstall()
    {
        if (_unhookSafeHandle is { IsInvalid: false, IsClosed: false })
        {
            _logger.LogInformation("Uninstalling mouse hook");
            try
            {
                PInvoke.UnhookWindowsHookEx((HHOOK)_unhookSafeHandle.DangerousGetHandle());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to uninstall mouse hook");
            }

            _logger.LogDebug("Disposing {NameOfHandle}", nameof(_unhookSafeHandle));
            try
            {
                _unhookSafeHandle.Close();
                _unhookSafeHandle.SetHandleAsInvalid();
                _unhookSafeHandle.Dispose();
                _unhookSafeHandle = null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to dispose {NameOfHandle}", nameof(_unhookSafeHandle));
            }

            _module?.Dispose();
            _module = null;

            _process?.Dispose();
            _process = null;

            try
            {
                _moduleHandle?.Close();
                _moduleHandle?.SetHandleAsInvalid();
                _moduleHandle?.Dispose();
                _moduleHandle = null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to dispose {NameOfHandle}", nameof(_moduleHandle));
            }

            _hookProc = null;

            HookEvents.Writer.TryComplete();
        }
        else
        {
            _logger.LogDebug("Mouse hook is not installed, nothing to uninstall");
        }
    }

    private void ReleaseUnmanagedResources()
    {
        Uninstall();
    }

    private void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
        if (disposing)
        {
            _unhookSafeHandle?.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~MouseHook()
    {
        Dispose(false);
    }
}
