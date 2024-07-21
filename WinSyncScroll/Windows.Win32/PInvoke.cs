using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;

// ReSharper disable CheckNamespace

namespace Windows.Win32;

internal partial class PInvoke
{
    internal static unsafe (uint ThreadId, uint ProcessId) GetWindowThreadProcessId(HWND hwnd)
    {
        uint lpdwProcessId;
        uint* lpdwProcessIdPtr = &lpdwProcessId;

        var threadId = GetWindowThreadProcessId(hwnd, lpdwProcessIdPtr);

        return (threadId, lpdwProcessId);
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
}
