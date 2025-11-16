namespace WinSyncScroll.Tests.Unit.Utils;

public static class TestUtils
{
    public static IntPtr CreateLParam(short hiWord, short loWord)
    {
        return (hiWord << 16) | (loWord & 0xFFFF);
    }
}
