namespace UpdateManager.AppManagementCoreLibrary;
public static class ServiceExtensions
{
    public static IServiceCollection RegisterShortcutServices(this IServiceCollection services)
    {
        services.AddSingleton<IShortcutManager, WindowsDesktopShortcutManager>();
        return services;
    }
    public static IServiceCollection RegisterAppServices(this IServiceCollection services, bool useFileBased = true)
    {
        if (useFileBased)
        {
            services.AddSingleton<IAppsContext, FileAppsContext>();
        }
        services.AddSingleton<IShortcutManager, WindowsDesktopShortcutManager>()
            .AddSingleton<AppDiscoveryService>();
        return services;
    }
}