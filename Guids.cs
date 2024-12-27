namespace UpdateManager.AppManagementCoreLibrary;
internal static class Guids
{
    public static readonly Guid CLSID_ShellLink = new(CLSID_ShellLink_Bytes);
    public static readonly Guid IID_IShellLinkW = new(IID_IShellLinkW_Bytes);
    public static readonly Guid IID_IPersistFile = new(IID_IPersistFile_Bytes);
    private static ReadOnlySpan<byte> CLSID_ShellLink_Bytes => [0x01, 0x14, 0x02, 0, 0, 0, 0, 0, 0xC0, 0, 0, 0, 0, 0, 0, 0x46];
    private static ReadOnlySpan<byte> IID_IShellLinkW_Bytes => [0xF9, 0x14, 0x02, 0, 0, 0, 0, 0, 0xC0, 0, 0, 0, 0, 0, 0, 0x46];
    private static ReadOnlySpan<byte> IID_IPersistFile_Bytes => [0x0B, 0x01, 0, 0, 0, 0, 0, 0, 0xC0, 0, 0, 0, 0, 0, 0, 0x46];
}