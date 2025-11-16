using System;
using WinSyncScroll.Common.Models;
using WinSyncScroll.Common.Shim;

namespace WinSyncScroll.Common.Utils;

public static class WinApiUtils
{
    public static (int Low, int High) GetHiLoWords(IntPtr value)
    {
        int low = unchecked((short)(long)value);
        int high = unchecked((short)((long)value >> 16));
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
