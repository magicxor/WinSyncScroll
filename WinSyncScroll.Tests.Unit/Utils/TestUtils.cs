namespace WinSyncScroll.Tests.Unit.Utils;

public static class TestUtils
{
    public static nuint CreateWParam(int hiWord, int loWord)
    {
        // Ensure the words fit within their respective 16-bit spaces
        if (hiWord < 0 || hiWord > 0xFFFF)
        {
            throw new ArgumentOutOfRangeException(nameof(hiWord), "HIWORD must be between 0 and 65535.");
        }

        if (loWord < 0 || loWord > 0xFFFF)
        {
            throw new ArgumentOutOfRangeException(nameof(loWord), "LOWORD must be between 0 and 65535.");
        }

        // Combine HIWORD and LOWORD into a single nuint value
        nuint wParam = (nuint)((hiWord << 16) | (loWord & 0xFFFF));
        return wParam;
    }

    public static nint CreateLParam(int hiWord, int loWord)
    {
        // Ensure the words fit within their respective 16-bit spaces
        if (hiWord < 0 || hiWord > 0xFFFF)
        {
            throw new ArgumentOutOfRangeException(nameof(hiWord), "HIWORD must be between 0 and 65535.");
        }

        if (loWord < 0 || loWord > 0xFFFF)
        {
            throw new ArgumentOutOfRangeException(nameof(loWord), "LOWORD must be between 0 and 65535.");
        }

        // Combine HIWORD and LOWORD into a single nint value
        nint lParam = (hiWord << 16) | (loWord & 0xFFFF);
        return lParam;
    }
}
