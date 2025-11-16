namespace WinSyncScroll.VisualTestUtil.Models;

public sealed class ScrollEventViewModel
{
    public required DateTime Timestamp { get; set; }
    public required string Name { get; set; }
    public required int AbsoluteX { get; set; }
    public required int AbsoluteY { get; set; }
    public required int WpfX { get; set; }
    public required int WpfY { get; set; }
    public required int WheelDelta { get; set; }
    public required int VirtualKeys { get; set; }
}
