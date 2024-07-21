﻿using System.Reflection;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WinSyncScroll.Enums;
using WinSyncScroll.Extensions;
using WinSyncScroll.Utils;

namespace WinSyncScroll;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ILogger<App>? Logger { get; set; }

    private MainWindow ResolveMainWindow(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MainWindow>();
    }

    private void ShowMainWindow(MainWindow mainWindow)
    {
        mainWindow.Show();
    }

    private void App_OnStartup(object sender, StartupEventArgs startupEventArgs)
    {
        try
        {
            var serviceProvider = ContainerConfigurationUtils.CreateServiceProvider(startupEventArgs.Args);
            Logger = serviceProvider.GetRequiredService<ILogger<App>>();
            Logger?.LogDebug("Start {Name} on {MachineName} as {UserName}", Assembly.GetExecutingAssembly().GetName().Name, Environment.MachineName, Environment.UserName);

            try
            {
                var mainWindow = ResolveMainWindow(serviceProvider);
                ShowMainWindow(mainWindow);
            }
            catch (Exception e)
            {
                Logger?.LogError(ServiceErrorCode.StartupError.ToEventId(), e, "Startup error");
                throw;
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, "An error occurred", MessageBoxButton.OK, MessageBoxImage.Error);
            this.Shutdown();
        }
    }

    private void App_OnExit(object sender, ExitEventArgs exitEventArgs)
    {
    }
}