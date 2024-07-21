namespace WinSyncScroll.Hook.EventArguments;

public class ExceptionEventArgs : System.EventArgs
{
    public int EventId { get; set; }
    public string? Message { get; set; }
    public string? SerializedException { get; set; }
}
