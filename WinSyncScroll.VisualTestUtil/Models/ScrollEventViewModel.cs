namespace WinSyncScroll.VisualTestUtil.Models;

public sealed class ScrollEventViewModel
{
    public required DateTime Timestamp { get; set; }
    public required string Name { get; set; }
    public required int X { get; set; }
    public required int Y { get; set; }
    public required int WheelDelta { get; set; }
    public required int VirtualKeys { get; set; }
}
