using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace WinSyncScroll.Extensions;

internal static class InputExtensions
{
    internal static string ToLogString(this INPUT input)
    {
        return input.type switch
        {
            INPUT_TYPE.INPUT_MOUSE => $"Mouse: dx={input.Anonymous.mi.dx.ToStringInvariant()}, dy={input.Anonymous.mi.dy.ToStringInvariant()}, mouseData={input.Anonymous.mi.mouseData.ToStringInvariant()}, dwFlags={input.Anonymous.mi.dwFlags}, time={input.Anonymous.mi.time.ToStringInvariant()}, dwExtraInfo={input.Anonymous.mi.dwExtraInfo.ToStringInvariant()}",
            INPUT_TYPE.INPUT_KEYBOARD => $"Keyboard: wVk={input.Anonymous.ki.wVk}, wScan={input.Anonymous.ki.wScan.ToStringInvariant()}, dwFlags={input.Anonymous.ki.dwFlags}, time={input.Anonymous.ki.time.ToStringInvariant()}, dwExtraInfo={input.Anonymous.ki.dwExtraInfo.ToStringInvariant()}",
            _ => $"Input type: {input.type}",
        };
    }
}
