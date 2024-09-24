using System.Globalization;

namespace WinSyncScroll.Extensions;

public static class Invariant
{
    public static string Format(string format, object? arg0)
    {
        return string.Format(CultureInfo.InvariantCulture, format, arg0);
    }

    public static string Format(string format, object? arg0, object? arg1)
    {
        return string.Format(CultureInfo.InvariantCulture, format, arg0, arg1);
    }

    public static string Format(string format, object? arg0, object? arg1, object? arg2)
    {
        return string.Format(CultureInfo.InvariantCulture, format, arg0, arg1, arg2);
    }

    public static string Format(string format, params object?[] args)
    {
        return string.Format(CultureInfo.InvariantCulture, format, args);
    }

    public static string ToStringInvariant(this sbyte value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public static string ToStringInvariant(this byte value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public static string ToStringInvariant(this short value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public static string ToStringInvariant(this ushort value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public static string ToStringInvariant(this int value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public static string ToStringInvariant(this nint value)
    {
        return $"{value}";
    }

    public static string ToStringInvariant(this uint value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public static string ToStringInvariant(this nuint value)
    {
        return $"{value}";
    }

    public static string ToStringInvariant(this long value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public static string ToStringInvariant(this ulong value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public static string ToStringInvariant(this float value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public static string ToStringInvariant(this double value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public static string ToStringInvariant(this decimal value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public static string? ToStringInvariant(this sbyte? value)
    {
        return value?.ToString(CultureInfo.InvariantCulture);
    }

    public static string? ToStringInvariant(this byte? value)
    {
        return value?.ToString(CultureInfo.InvariantCulture);
    }

    public static string? ToStringInvariant(this short? value)
    {
        return value?.ToString(CultureInfo.InvariantCulture);
    }

    public static string? ToStringInvariant(this ushort? value)
    {
        return value?.ToString(CultureInfo.InvariantCulture);
    }

    public static string? ToStringInvariant(this int? value)
    {
        return value?.ToString(CultureInfo.InvariantCulture);
    }

    public static string? ToStringInvariant(this uint? value)
    {
        return value?.ToString(CultureInfo.InvariantCulture);
    }

    public static string? ToStringInvariant(this long? value)
    {
        return value?.ToString(CultureInfo.InvariantCulture);
    }

    public static string? ToStringInvariant(this ulong? value)
    {
        return value?.ToString(CultureInfo.InvariantCulture);
    }

    public static string? ToStringInvariant(this float? value)
    {
        return value?.ToString(CultureInfo.InvariantCulture);
    }

    public static string? ToStringInvariant(this double? value)
    {
        return value?.ToString(CultureInfo.InvariantCulture);
    }

    public static string? ToStringInvariant(this decimal? value)
    {
        return value?.ToString(CultureInfo.InvariantCulture);
    }

    public static string ToAmericanDateTime(this DateTime value)
    {
        // 11/3/2016 12:15:53 PM
        return value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
    }

    public static string? ToAmericanDateTime(this DateTime? value)
    {
        // 11/3/2016 12:15:53 PM
        return value?.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
    }
}
