using System.Diagnostics;
using Windows.Win32;
using Windows.Win32.Foundation;
using Microsoft.Extensions.Logging;
using WinSyncScroll.Models;

namespace WinSyncScroll.Services;

public class WinApiManager
{
    private readonly ILogger<WinApiManager> _logger;

    public WinApiManager(ILogger<WinApiManager> logger)
    {
        _logger = logger;
    }

    public List<WindowInfo> ListWindows()
    {
        _logger.LogDebug("Listing windows");

        var windowHandles = new List<HWND>();

        PInvoke.EnumWindows((handle, _) =>
        {
            windowHandles.Add(handle);
            return true;
        }, IntPtr.Zero);

        _logger.LogDebug("Found {WindowCount} windows", windowHandles.Count);

        var windows = new List<WindowInfo>();
        using var currentProcess = Process.GetCurrentProcess();
        var currentProcessId = currentProcess.Id;

        foreach (var windowHandle in windowHandles)
        {
            try
            {
                var isVisible = PInvoke.IsWindowVisible(windowHandle);
                if (!isVisible)
                {
                    continue;
                }

                var (_, processId, errorCode) = PInvoke.GetWindowThreadProcessId(windowHandle);
                if (processId == 0 || errorCode != 0)
                {
                    _logger.LogWarning("Failed to get process ID for window with handle {WindowHandle}", windowHandle);
                    continue;
                }
                if (processId == currentProcessId)
                {
                    continue;
                }

                var className = PInvoke.GetClassName(windowHandle);
                var windowName = PInvoke.GetWindowText(windowHandle);
                var process = Process.GetProcessById((int)processId);
                var processName = process.ProcessName;

                if (string.IsNullOrWhiteSpace(windowName))
                {
                    continue;
                }

                windows.Add(new WindowInfo(
                    WindowName: windowName,
                    ClassName: className,
                    WindowHandle: windowHandle,
                    ProcessId: (int)processId,
                    ProcessName: processName));
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to get information for window with handle {WindowHandle}", windowHandle);
            }
        }

        _logger.LogDebug("Successfully retrieved information for {WindowCount} windows", windows.Count);

        return windows;
    }
}
