namespace UpdateManager.AppManagementCoreLibrary;
internal static partial class NativeMethods
{
    [LibraryImport("ole32.dll", SetLastError = false)]
    public static unsafe partial uint CoCreateInstance(Guid* rclsid, void* pUnkOuter, uint dwClsContext, Guid* riid, void** ppv);
}
