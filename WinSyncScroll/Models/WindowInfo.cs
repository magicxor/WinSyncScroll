namespace WinSyncScroll.Models;

public record WindowInfo(
    string WindowName,
    string ClassName,
    IntPtr WindowHandle,
    int ProcessId,
    string ProcessName
)
{
    public string DisplayName { get; } = $"{WindowName} [{ProcessName}, class: {ClassName}], pid: {ProcessId}, hwnd: {WindowHandle}";
    public long WindowHandleLong { get; } = (long)WindowHandle;
};
