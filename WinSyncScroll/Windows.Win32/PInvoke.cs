using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using WinSyncScroll.Exceptions;
using WinSyncScroll.Models;

// ReSharper disable CheckNamespace
namespace Windows.Win32;

internal partial class PInvoke
{
    internal static unsafe (uint ThreadId, uint ProcessId, int errorCode) GetWindowThreadProcessId(HWND hwnd)
    {
        uint lpdwProcessId;
        uint* lpdwProcessIdPtr = &lpdwProcessId;

        var threadId = GetWindowThreadProcessId(hwnd, lpdwProcessIdPtr);
        if (threadId == 0)
        {
            var errorCode = Marshal.GetLastWin32Error();
            return (threadId, lpdwProcessId, errorCode);
        }

        return (threadId, lpdwProcessId, 0);
    }

    internal static unsafe string GetWindowText(HWND hWnd)
    {
        var bufferSize = GetWindowTextLength(hWnd) + 1;

        fixed (char* windowNameChars = new char[bufferSize])
        {
            var windowTextLength = GetWindowText(hWnd, windowNameChars, bufferSize);

            if (windowTextLength == 0)
            {
                var errorCode = Marshal.GetLastWin32Error();
                if (errorCode != 0)
                {
                    throw new Win32Exception(errorCode);
                }

                return string.Empty;
            }

            var resultLength = windowTextLength > bufferSize
                ? bufferSize
                : windowTextLength;

            return new string(windowNameChars, 0, resultLength);
        }
    }

    internal static unsafe string GetClassName(HWND hWnd)
    {
        const int bufferSize = 256;

        fixed (char* classNameChars = new char[bufferSize])
        {
            var classNameLength = GetClassName(hWnd, classNameChars, bufferSize);

            if (classNameLength == 0)
            {
                var errorCode = Marshal.GetLastWin32Error();
                if (errorCode != 0)
                {
                    throw new Win32Exception(errorCode);
                }

                return string.Empty;
            }

            var resultLength = classNameLength > bufferSize
                ? bufferSize
                : classNameLength;

            return new string(classNameChars, 0, resultLength);
        }
    }

    public static WindowRect GetWindowRect(HWND windowHandle)
    {
        if (GetWindowRect(windowHandle, out var rectStruct))
        {
            return new WindowRect(
                Left: rectStruct.left,
                Top: rectStruct.top,
                Right: rectStruct.right,
                Bottom: rectStruct.bottom
            );
        }
        else
        {
            var errorCode = Marshal.GetLastWin32Error();
            if (errorCode != 0)
            {
                throw new Win32Exception(errorCode);
            }
            else
            {
                throw new ServiceException("Failed to get window rect");
            }
        }
    }
}
