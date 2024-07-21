namespace WinSyncScroll.Hook.EventArguments;

public class TypedWindowProcEventArgs
{
    public uint Msg { get; set; }
    public ulong WParam { get; set; }
    public long LParam { get; set; }
}
