using System;
using WinSyncScroll.Common.Models;
using WinSyncScroll.Common.Shim;

namespace WinSyncScroll.Common.Utils;

public static class WinApiUtils
{
    public static (short Low, short High) GetHiLoWords(IntPtr value)
    {
        uint xy = unchecked(IntPtr.Size == 8 ? (uint)value.ToInt64() : (uint)value.ToInt32());
        short low = unchecked((short)xy);
        short high = unchecked((short)(xy >> 16));
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
