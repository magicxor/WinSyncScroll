# WinSyncScroll
Scroll two windows simultaneously

[![release](https://github.com/magicxor/WinSyncScroll/actions/workflows/release.yml/badge.svg)](https://github.com/magicxor/WinSyncScroll/actions/workflows/release.yml)

## Demo

https://github.com/user-attachments/assets/ba52c20e-1099-4e20-9b8d-f164d283c639

https://github.com/user-attachments/assets/f2cc2020-cc07-411e-a4c7-7ef227d9cdda

## How it works

The program uses the `SetWindowsHookEx` function to install a hook procedure that monitors low-level mouse events. When the user scrolls the source window, the program uses `SendInput` to simulate the same scroll event on the target window.

## Configuration

The program reads the configuration from the `appsettings.json` file. The configuration file must be in the same directory as the executable file.

### Legacy mode

When `"IsLegacyModeEnabled": true`, the program uses `SendMessage` (instead of `SendInput`) to send the `WM_MOUSEWHEEL` (or `WM_MOUSEHWHEEL`) message to the target window.

### Strict process id check

When `"IsStrictProcessIdCheckEnabled": true`, the program uses `WindowFromPoint` + `GetWindowThreadProcessId` to prevent scrolling the target window if the target or source window is currently not in the foreground.

## See also:
- https://badecho.com/index.php/2024/01/13/external-window-messages/
- https://badecho.com/index.php/2024/01/17/message-queue-messages/
- https://github.com/microsoft/CsWin32
- https://github.com/dahall/vanara
- https://www.codeproject.com/Articles/6362/Global-System-Hooks-in-NET
- https://github.com/rvknth043/Global-Low-Level-Key-Board-And-Mouse-Hook
- https://sharphook.tolik.io/v5.3.7/
- https://github.com/TolikPylypchuk/libuiohook
- https://github.com/jaredpar/pinvoke-interop-assistant
- [KatMouse](https://ehiti.de/katmouse/)
- [WizMouse](https://antibody-software.com/wizmouse)
- [X-Mouse Controls](https://github.com/joelpurra/xmouse-controls)
- [AlwaysMouseWheel](http://www.softwareok.com/?Download=AlwaysMouseWheel)
