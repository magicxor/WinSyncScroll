using WinSyncScroll.Models;

namespace WinSyncScroll.Extensions;

public static class WindowRectExtensions
{
    public static string ToLogString(this WindowRect? windowRect)
    {
        return windowRect == null
            ? "null"
            : $"Left={windowRect.Left.ToStringInvariant()}, Top={windowRect.Top.ToStringInvariant()}, Right={windowRect.Right.ToStringInvariant()}, Bottom={windowRect.Bottom.ToStringInvariant()}";
    }
}
