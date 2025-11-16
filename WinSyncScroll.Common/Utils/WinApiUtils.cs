using WinSyncScroll.Common.Models;
using WinSyncScroll.Common.Shim;

namespace WinSyncScroll.Common.Utils;

public static class WinApiUtils
{
    public static (ushort Low, ushort High) GetHiLoWords(uint value)
    {
        var low = (ushort)(value & 0xFFFF);
        var high = (ushort)(value >> 16);
        return (Low: low, High: high);
    }

    public static bool IsPointInRect(WindowRect windowRect, int x, int y)
    {
        ArgumentNullExceptionShim.ThrowIfNull(windowRect);

        return x >= windowRect.Left
               && x <= windowRect.Right
               && y >= windowRect.Top
               && y <= windowRect.Bottom;
    }
}
