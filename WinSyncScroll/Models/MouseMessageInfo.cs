using Vanara.PInvoke;

namespace WinSyncScroll.Models;

public sealed record MouseMessageInfo(
    UIntPtr MouseMessageId,
    User32.MSLLHOOKSTRUCT MouseMessageData
);
