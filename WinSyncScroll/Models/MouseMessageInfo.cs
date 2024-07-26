using Vanara.PInvoke;

namespace WinSyncScroll.Models;

public class MouseMessageInfo
{
    public UIntPtr MouseMessageId { get; set; }
    public User32.MSLLHOOKSTRUCT MouseMessageData { get; set; }
}
