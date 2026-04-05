using applaunch.WinUi.Abstractions;
using applaunch.WinUi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace applaunch.WinUi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton<IHotkeyManager, HotkeyManager>();
        services.AddSingleton<IAppScanner, AppScanner>();
        services.AddSingleton<IAppSearchEngine, AppSearchEngine>();
        services.AddSingleton<IAppLauncher, AppLauncher>();

        return services;
    }
}
