namespace UpdateManager.AppManagementCoreLibrary;
public class AppDiscoveryService(IShortcutManager shortcutManager, IAppsContext context, IAppDiscoveryHandler handler)
{
    public async Task AddAppAsync(AppModel app)
    {
        await context.AddAppAsync(app);
    }
    public async Task<BasicList<AppModel>> DiscoverMissingAppsAsync()
    {
        BasicList<AppModel> output = [];
        BasicList<AppModel> existingApps = await context.GetAllAppsAsync(); //must be all apps.
        var existingAppNames = new HashSet<string>(existingApps.Select(p => p.ProjectName));
        BasicList<string> folders = await handler.GetAppDirectoriesAsync();
        BasicList<ShortcutModel> shortcuts = shortcutManager.ListShortcuts();
        foreach (var folder in folders)
        {
            if (ff1.DirectoryExists(folder) == false)
            {
                continue; //does not exist. continue
            }
            BasicList<string> toCheck = await ff1.DirectoryListAsync(folder, SearchOption.AllDirectories);
            toCheck.RemoveAllAndObtain(d =>
            {
                if (d.Contains("Archived", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                return false;
            });
            foreach (var dir in toCheck)
            {
                var projectFiles = await ff1.GetSeveralSpecificFilesAsync(dir, "csproj");
                foreach (var projectFile in projectFiles)
                {
                    if (Path.GetFileName(projectFile).Contains(".backup", StringComparison.OrdinalIgnoreCase))
                    {
                        continue; // Skip this file
                    }
                    string appName = ff1.FileName(projectFile);
                    // **Skip the extraction if the package already exists**
                    if (existingAppNames.Contains(appName))
                    {
                        continue; // Skip this package
                    }
                    if (handler.CanIncludeApp(projectFile) == false)
                    {
                        continue;
                    }
                    AppModel app = ExtractAppInfo(projectFile, appName, shortcuts);
                    output.Add(app);
                }
            }
        }
        return output;
    }
    private AppModel ExtractAppInfo(string projectFile, string appName, BasicList<ShortcutModel> shortcuts)
    {
        CsProjEditor editor = new(projectFile);
        AppModel model = new();
        // Set initial properties from project file
        model.CsProj = projectFile;
        model.ProjectName = appName;
        handler.CustomizeAppModel(model);
        // Ensure that the properties are set back to their original values, if modified by the handler
        // This is important because the handler might modify these properties, and we want to restore them.
        model.CsProj = projectFile;
        model.ProjectName = appName;
        // If the app is using Windows-specific technologies (WPF/Blazor), mark it as such
        if (editor.IsWindows())
        {
            model.WindowsWPFBlazor = true; // Assuming WPF and Blazor are being used for this app
        }
        // Check if a shortcut is associated with the app
        ShortcutModel? shortcut = shortcuts.SingleOrDefault(x => x.ProjectName.Equals(appName, StringComparison.CurrentCultureIgnoreCase));
        if (shortcut is not null)
        {
            model.ShortcutName = shortcut.ShortcutName;
        }
        // Return the fully constructed app model
        return model;
    }
}