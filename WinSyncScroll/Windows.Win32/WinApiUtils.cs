// ReSharper disable CheckNamespace

using WinSyncScroll.Models;
using WinSyncScroll.Shim;

namespace Windows.Win32;

public static class WinApiUtils
{
    public static (short Low, short High) GetHiLoWords(uint value)
    {
        var low = BitConverter.ToInt16(BitConverter.GetBytes(value), 0);
        var high = BitConverter.ToInt16(BitConverter.GetBytes(value), 2);
        return (Low: low, High: high);
    }

    public static bool PointInRect(WindowRect windowRect, int x, int y)
    {
        ArgumentNullExceptionShim.ThrowIfNull(windowRect);

        return x >= windowRect.Left
               && x <= windowRect.Right
               && y >= windowRect.Top
               && y <= windowRect.Bottom;
    }
}
