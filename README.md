# WinSyncScroll
Scroll two windows simultaneously

## Demo

https://github.com/user-attachments/assets/ba52c20e-1099-4e20-9b8d-f164d283c639

https://github.com/user-attachments/assets/f2cc2020-cc07-411e-a4c7-7ef227d9cdda

https://github.com/user-attachments/assets/609c8ace-a694-4c2c-85fb-4628113caebc

## How it works

The program uses the `SetWindowsHookEx` function to install a hook procedure that monitors low-level mouse events. When the user scrolls the source window, the program uses `SendInput` to simulate the same scroll event on the target window.

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
