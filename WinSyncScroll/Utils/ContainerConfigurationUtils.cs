using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using WinSyncScroll.Services;
using WinSyncScroll.ViewModels;

namespace WinSyncScroll.Utils;

public static class ContainerConfigurationUtils
{
    private static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        return services
            /* Logging */
            .AddSingleton<ILoggerFactory>(_ =>
            {
                var loggerFactory = new LoggerFactory();
                loggerFactory.AddProvider(new NLogLoggerProvider());
                return loggerFactory;
            })
            .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
            /* Services */
            .AddScoped<MouseHook>()
            .AddScoped<WinApiService>()
            /* ViewModels */
            .AddScoped<MainViewModel>()
            /* Windows */
            .AddScoped<MainWindow>();
    }

    public static IServiceProvider CreateServiceProvider()
    {
        return new ServiceCollection()
            .ConfigureServices()
            .BuildServiceProvider();
    }
}
