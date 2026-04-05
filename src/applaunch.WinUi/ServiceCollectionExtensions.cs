using System.Collections.ObjectModel;
using applaunch.WinUi.Abstractions;
using applaunch.WinUi.Config;
using applaunch.WinUi.Models;
using applaunch.WinUi.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace applaunch.WinUi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        AppConfig appConfig = new();
        configuration.GetSection("AppConfig").Bind(appConfig);
        services.AddSingleton(appConfig);

        services.AddSingleton(new ObservableCollection<AppItem>());

        services.AddSingleton<IHotkeyService, HotkeyService>();
        services.AddSingleton<ISearchEngine<AppItem>, AppSearchEngine>();
        services.AddSingleton<IAppLauncher, AppLaunchService>();
        services.AddSingleton<IAppSearchService, AppSearchService>();
        services.AddSingleton<IAppNavigationService, AppNavigationService>();
        services.AddSingleton<IAppScanner, AppScanner>();

        return services;
    }
}
