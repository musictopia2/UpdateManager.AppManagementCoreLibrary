namespace UpdateManager.AppManagementCoreLibrary;
internal class Helpers
{
    public static string GetShortcutPath(string name)
    {
        string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{name}.lnk");
        return shortcutPath;
    }
}