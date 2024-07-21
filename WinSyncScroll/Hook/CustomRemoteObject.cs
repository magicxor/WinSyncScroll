using WinSyncScroll.Hook.EventArguments;

namespace WinSyncScroll.Hook;

public class CustomRemoteObject : MarshalByRefObject
{
    public event EventHandler<InjectionEventArgs> InjectionEvent = delegate { };
    public event EventHandler<EventArgs> EntryPointEvent = delegate { };
    public event EventHandler<WindowProcEventArgs> WindowProcEvent = delegate { };
    public event EventHandler<LogMessageEventArgs> LogMessageEvent = delegate { };
    public event EventHandler<EventArgs> PingEvent = delegate { };
    public event EventHandler<EventArgs> ExitEvent = delegate { };
    public event EventHandler<ExceptionEventArgs> ExceptionEvent = delegate { };

    public void TriggerInjectionEvent(int clientProcessId)
    {
        InjectionEvent(null, new InjectionEventArgs { ClientProcessId = clientProcessId });
    }

    public void TriggerEntryPointEvent()
    {
        EntryPointEvent(null, EventArgs.Empty);
    }

    public void TriggerWindowProcEvent(string? serializedArgs)
    {
        WindowProcEvent(null, new WindowProcEventArgs
        {
            SerializedArgs = serializedArgs,
        });
    }

    public void TriggerLogMessageEvent(string message)
    {
        LogMessageEvent(null, new LogMessageEventArgs { Message = message });
    }

    public void TriggerPingEvent()
    {
        PingEvent(null, EventArgs.Empty);
    }

    public void TriggerExitEvent()
    {
        ExitEvent(null, EventArgs.Empty);
    }

    public void TriggerExceptionEvent(Exception exception)
    {
        ExceptionEvent(null, new ExceptionEventArgs
        {
            EventId = exception.HResult,
            Message = exception.Message,
            SerializedException = exception.ToString(),
        });
    }
}
