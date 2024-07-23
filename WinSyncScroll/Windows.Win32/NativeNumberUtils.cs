﻿// ReSharper disable CheckNamespace

using Windows.Win32.UI.WindowsAndMessaging;
using WinSyncScroll.Models;

namespace Windows.Win32;

public static class NativeNumberUtils
{
    public static nuint CreateWParam(int hiWord, int loWord)
    {
        // Ensure the words fit within their respective 16-bit spaces
        if (hiWord < 0 || hiWord > 0xFFFF)
            throw new ArgumentOutOfRangeException(nameof(hiWord), "HIWORD must be between 0 and 65535.");
        if (loWord < 0 || loWord > 0xFFFF)
            throw new ArgumentOutOfRangeException(nameof(loWord), "LOWORD must be between 0 and 65535.");

        // Combine HIWORD and LOWORD into a single nuint value
        nuint wParam = (nuint)((hiWord << 16) | (loWord & 0xFFFF));
        return wParam;
    }

    public static nint CreateLParam(int hiWord, int loWord)
    {
        // Ensure the words fit within their respective 16-bit spaces
        if (hiWord < 0 || hiWord > 0xFFFF)
            throw new ArgumentOutOfRangeException(nameof(hiWord), "HIWORD must be between 0 and 65535.");
        if (loWord < 0 || loWord > 0xFFFF)
            throw new ArgumentOutOfRangeException(nameof(loWord), "LOWORD must be between 0 and 65535.");

        // Combine HIWORD and LOWORD into a single nint value
        nint lParam = (hiWord << 16) | (loWord & 0xFFFF);
        return lParam;
    }

    public static (short Low, short High) GetHiLoWords(uint value)
    {
        var low = BitConverter.ToInt16(BitConverter.GetBytes(value), 0);
        var high = BitConverter.ToInt16(BitConverter.GetBytes(value), 2);
        return (Low: low, High: high);
    }

    public static (short Low, short High) GetHiLoWordsV2(uint value)
    {
        var low = (short)(value & 0xFFFF);
        var high = (short)(value >> 16);
        return (Low: low, High: high);
    }

    public static bool IsKeyDown(nuint wParam, int key)
    {
        return (wParam & (nuint)key) != 0;
    }

    public static bool PointInRect(WindowRect windowRect, int x, int y)
    {
        return x >= windowRect.Left
               && x <= windowRect.Right
               && y >= windowRect.Top
               && y <= windowRect.Bottom;
    }

    public static int CalculateAbsoluteCoordinateX(int x, int smCxScreen)
    {
        return (x * 65536) / smCxScreen;
    }

    public static int CalculateAbsoluteCoordinateY(int y, int smCyScreen)
    {
        return (y * 65536) / smCyScreen;
    }
}