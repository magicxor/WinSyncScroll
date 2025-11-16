namespace WinSyncScroll.Tests.Unit.Utils;

public static class TestUtils
{
    public static IntPtr CreateLParam(ushort hiWord, ushort loWord)
    {
        return (IntPtr)((hiWord << 16) | (loWord & 0xFFFF));
    }
}
