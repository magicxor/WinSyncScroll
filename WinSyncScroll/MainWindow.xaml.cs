using System.ComponentModel;
using System.Windows;
using WinSyncScroll.ViewModels;

namespace WinSyncScroll;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        _viewModel = viewModel;

        InitializeComponent();

        DataContext = _viewModel;
    }

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
        _viewModel.Initialize();
    }

    private void WindowClosing(object sender, CancelEventArgs e)
    {
        _viewModel.HandleWindowClosing();
    }
}
