namespace WinSyncScroll.Hook.EventArguments;

public class ParsedWindowProcEventArgs
{
    public string? Msg { get; set; }
    public string? WParam { get; set; }
    public string? LParam { get; set; }
}
