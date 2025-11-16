using Microsoft.Extensions.DependencyInjection;
using WinSyncScroll.VisualTestUtil.ViewModels;

namespace WinSyncScroll.VisualTestUtil.Utils;

public static class ContainerConfigurationUtils
{
    private static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        return services
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
