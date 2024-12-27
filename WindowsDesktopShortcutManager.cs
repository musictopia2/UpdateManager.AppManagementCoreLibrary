namespace UpdateManager.AppManagementCoreLibrary;
public class WindowsDesktopShortcutManager : IShortcutManager
{
    BasicList<ShortcutModel> IShortcutManager.ListShortcuts()
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        // Check if the Desktop path exists
        if (ff1.DirectoryExists(desktopPath) == false)
        {
            throw new CustomBasicException($"{desktopPath} does not exist");
        }
        BasicList<string> list = ff1.GetSeveralSpecificFiles(desktopPath, "lnk");
        BasicList<ShortcutModel> output = [];
        foreach (var item in list)
        {
            //you know the path of an item.
            InternalWindowsShortcut temp = InternalWindowsShortcut.Load(item);
            string? target = temp.Path;
            if (target is null)
            {
                continue;
            }
            if (target.Contains(@"\bin\release", StringComparison.CurrentCultureIgnoreCase))
            {
                ShortcutModel model = new()
                {
                    ShortcutName = ff1.FileName(item),
                    TargetPath = target,
                    ProjectName = ff1.FileName(target)
                };
                output.Add(model);
            }
        }
        return output;
    }

    void IShortcutManager.UpdateShortcut(string name, string newTargetPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(newTargetPath);

        string shortcutPath = Helpers.GetShortcutPath(name);

        // Load the existing shortcut from the specified file
        using InternalWindowsShortcut shortcut = InternalWindowsShortcut.Load(shortcutPath);

        // Update the target path of the shortcut
        shortcut.Path = newTargetPath;

        shortcut.Save(shortcutPath);
    }
}