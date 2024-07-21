using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using WinSyncScroll.Services;
using WinSyncScroll.ViewModels;

namespace WinSyncScroll.Utils;

public static class ContainerConfigurationUtils
{
        private static IServiceCollection ConfigureServices(this IServiceCollection services, string[] commandLineOptions)
        {
            return services
                // Logging
                .AddSingleton(s => new LoggerFactory().AddNLog())
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                // Services
                .AddScoped<WinApiManager>()
                // ViewModels
                .AddScoped<MainViewModel>()
                // Windows
                .AddScoped<MainWindow>();
        }

        public static IServiceProvider CreateServiceProvider(string[] commandLineOptions)
        {
            return new ServiceCollection()
                .ConfigureServices(commandLineOptions)
                .BuildServiceProvider();
        }
    }
