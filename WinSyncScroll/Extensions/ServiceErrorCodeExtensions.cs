using Microsoft.Extensions.Logging;
using WinSyncScroll.Enums;

namespace WinSyncScroll.Extensions;

public static class ServiceErrorCodeExtensions
{
    private static int ToInt(this ServiceErrorCode serviceErrorCode)
    {
        return (int)serviceErrorCode;
    }

    public static EventId ToEventId(this ServiceErrorCode serviceErrorCode)
    {
        return new EventId(serviceErrorCode.ToInt(), serviceErrorCode.ToString());
    }
}
