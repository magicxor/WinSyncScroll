namespace WinSyncScroll.Extensions;

public static class MouseHookDataExtensions
{
    public static string ToLogString(this Vanara.PInvoke.User32.MSLLHOOKSTRUCT data)
    {
        return $"X={data.pt.x.ToStringInvariant()}, Y={data.pt.y.ToStringInvariant()}, MouseData={data.mouseData.ToStringInvariant()}, Flags={data.flags.ToStringInvariant()}, Time={data.time.ToStringInvariant()}, ExtraInfo={data.dwExtraInfo.ToStringInvariant()}";
    }
}
