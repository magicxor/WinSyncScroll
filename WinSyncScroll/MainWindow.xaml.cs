using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Microsoft.Extensions.Logging;
using WinSyncScroll.ViewModels;

namespace WinSyncScroll;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly ILogger<MainWindow> _logger;
    private readonly MainViewModel _viewModel;

    public MainWindow(
        ILogger<MainWindow> logger,
        MainViewModel viewModel)
    {
        _logger = logger;
        _viewModel = viewModel;

        InitializeComponent();

        DataContext = _viewModel;
    }

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
        _viewModel.Initialize(Dispatcher);

        var dpi = VisualTreeHelper.GetDpi(this);
        _logger.LogInformation("Main window DPI Scale X: {DpiScaleX}, Y: {DpiScaleY}, PixelsPerDip: {PixelsPerDip}",
            dpi.DpiScaleX,
            dpi.DpiScaleY,
            dpi.PixelsPerDip);
    }

    private void WindowClosing(object sender, CancelEventArgs e)
    {
        _viewModel.HandleWindowClosing();
    }
}
