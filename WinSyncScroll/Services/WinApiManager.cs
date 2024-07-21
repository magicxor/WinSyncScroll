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
        var windowHandles = new List<HWND>();

        PInvoke.EnumWindows((handle, _) =>
        {
            windowHandles.Add(handle);
            return true;
        }, IntPtr.Zero);

        _logger.LogDebug("Found {WindowCount} windows", windowHandles.Count);

        var windows = new List<WindowInfo>();
        var currentProcessId = Process.GetCurrentProcess().Id;

        foreach (var windowHandle in windowHandles)
        {
            try
            {
                var isVisible = PInvoke.IsWindowVisible(windowHandle);
                if (!isVisible)
                {
                    continue;
                }

                var (threadId, processId) = PInvoke.GetWindowThreadProcessId(windowHandle);
                if (processId == 0)
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
                var isRectAvailable = PInvoke.GetWindowRect(windowHandle, out var rect);
                var windowRect = new WindowRect
                {
                    Left = rect.left,
                    Top = rect.top,
                    Right = rect.right,
                    Bottom = rect.bottom
                };

                if (string.IsNullOrWhiteSpace(windowName))
                {
                    continue;
                }

                if (!isRectAvailable)
                {
                    continue;
                }

                windows.Add(new WindowInfo(
                    WindowName: windowName,
                    ClassName: className,
                    WindowHandle: windowHandle,
                    ProcessId: (int)processId,
                    ProcessName: processName,
                    WindowRect: windowRect));
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
