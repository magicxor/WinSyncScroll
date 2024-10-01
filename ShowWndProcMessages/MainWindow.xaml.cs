using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Windows.Win32;

namespace ShowWndProcMessages;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    private Point GetMousePos() => PointToScreen(Mouse.GetPosition(this));

    private volatile bool _isClosing;

    private readonly Timer _timer;

    public MainWindow(MainViewModel viewModel)
    {
        _viewModel = viewModel;

        InitializeComponent();

        DataContext = _viewModel;

        _timer = new Timer(state =>
                Dispatcher.Invoke(() =>
                {
                    if (_isClosing) return;
                    var pos = GetMousePos();
                    _viewModel.UpdateCurrentCursorPosition((int)pos.X, (int)pos.Y);
                }),
            null,
            0,
            100);
    }

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
        _viewModel.Initialize();
    }


    private void WindowClosing(object sender, CancelEventArgs e)
    {
        _isClosing = true;
        _timer.Dispose();

        _viewModel.HandleWindowClosing();
    }

    /// <summary>
    /// AddHook Handle WndProc messages in WPF
    /// This cannot be done in a Window's constructor as a handle window handle won't at that point, so there won't be a HwndSource.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
        {
            hwndSource.AddHook(WndProc);
        }
    }

    /// <summary>
    /// WndProc matches the HwndSourceHook delegate signature so it can be passed to AddHook() as a callback. This is the same as overriding a Windows.Form's WncProc method.
    /// </summary>
    /// <param name="hwnd">The window handle</param>
    /// <param name="msg">The message ID</param>
    /// <param name="wParam">The message's wParam value, historically used in the win32 api for handles and integers</param>
    /// <param name="lParam">The message's lParam value, historically used in the win32 api to pass pointers</param>
    /// <param name="handled">A value that indicates whether the message was handled</param>
    /// <returns></returns>
    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (_isClosing) return IntPtr.Zero;

        if (msg is WinApiConstants.WM_MOUSEWHEEL or WinApiConstants.WM_MOUSEHWHEEL)
        {
            var (x, y) = WinApiUtils.GetHiLoWords((uint)lParam);
            var wpfPoint = PointFromScreen(new Point(x, y));
            _viewModel.UpdateLatestScrollCoordinates(x, y, (int)wpfPoint.X, (int)wpfPoint.Y);
        }

        return IntPtr.Zero;
    }
}
