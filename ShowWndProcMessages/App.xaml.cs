using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace ShowWndProcMessages;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public sealed partial class App : Application
{
    private static MainWindow ResolveMainWindow(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MainWindow>();
    }

    private static void ShowMainWindow(MainWindow mainWindow)
    {
        mainWindow.Show();
    }

    private void App_OnStartup(object sender, StartupEventArgs startupEventArgs)
    {
        var serviceProvider = ContainerConfigurationUtils.CreateServiceProvider();
        var mainWindow = ResolveMainWindow(serviceProvider);
        ShowMainWindow(mainWindow);
    }

    private void App_OnExit(object sender, ExitEventArgs exitEventArgs)
    {
    }
}
