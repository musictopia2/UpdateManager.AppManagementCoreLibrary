namespace UpdateManager.AppManagementCoreLibrary;
public static class ServiceExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection RegisterShortcutServices()
        {
            services.AddSingleton<IShortcutManager, WindowsDesktopShortcutManager>();
            return services;
        }
        public IServiceCollection RegisterAppServices(bool useFileBased = true)
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
}