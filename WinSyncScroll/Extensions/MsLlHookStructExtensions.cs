using Vanara.PInvoke;

namespace WinSyncScroll.Extensions;

public static class MsLlHookStructExtensions
{
    // Tests if the LLMHF_INJECTED flag is set
    public static bool IsInjected(this User32.MSLLHOOKSTRUCT msLlHookStruct)
    {
        return (msLlHookStruct.flags & 0x00000001) != 0;
    }

    // Tests if the LLMHF_LOWER_IL_INJECTED flag is set.
    public static bool IsLowerIntegrityInjected(this User32.MSLLHOOKSTRUCT msLlHookStruct)
    {
        return (msLlHookStruct.flags & 0x00000002) != 0;
    }
}
