using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using WinSyncScroll.Enums;
using WinSyncScroll.Models;
using WinSyncScroll.Services;
using WinSyncScroll.ViewModels;

namespace WinSyncScroll.Utils;

public static class ContainerConfigurationUtils
{
    private static IConfigurationRoot GetConfigurationRoot()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
        var configurationRoot = configurationBuilder.Build();
        return configurationRoot;
    }

    private static IServiceCollection ConfigureServices(
        this IServiceCollection services,
        IConfigurationRoot configurationRoot)
    {
        services
            .AddOptions<GeneralOptions>()
            .Bind(configurationRoot.GetSection(nameof(OptionSections.General)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

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
        var configurationRoot = GetConfigurationRoot();

        return new ServiceCollection()
            .ConfigureServices(configurationRoot)
            .BuildServiceProvider();
    }
}
