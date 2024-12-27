namespace UpdateManager.AppManagementCoreLibrary;
internal sealed class InternalWindowsShortcut : IDisposable
{
    private const uint CLSCTX_INPROC_SERVER = 0x1;
    private const uint SLGP_RAWPATH = 4;
    private readonly unsafe ShellLinkInst* inst;
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyWindowsShortcut"/> class.
    /// </summary>
    public InternalWindowsShortcut()
    {
        unsafe
        {
            ShellLinkInst* inst = null;
            var clsid = Guids.CLSID_ShellLink;
            var iid = Guids.IID_IShellLinkW;
            uint res = NativeMethods.CoCreateInstance(&clsid, null, CLSCTX_INPROC_SERVER, &iid, (void**)&inst);
            if (res != 0)
            {
                throw new COMException("Unable to create ShellLink object.", (int)res);
            }

            this.inst = inst;
        }
    }



    private unsafe InternalWindowsShortcut(ShellLinkInst* inst) => this.inst = inst;

    ~InternalWindowsShortcut() => Dispose(false);

    // Gets or sets the path of the target.
    public string? Path
    {
        get
        {
            unsafe
            {
                var buffer = stackalloc char[260];
                uint res = inst->Vtbl->GetPath(inst, (byte*)buffer, 260, null, SLGP_RAWPATH);
                if (res == 0)
                {
                    return Marshal.PtrToStringUni(new nint(buffer));
                }
                else
                {
                    return null;
                }
            }
        }
        set
        {
            unsafe
            {
                SetString(inst->Vtbl->SetPath, value);
            }
        }
    }

    // Load an existing shortcut from disk.
    internal static InternalWindowsShortcut Load(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException();
        }

        var shortcut = new InternalWindowsShortcut();
        try
        {
            shortcut.LoadInternal(fileName);
            return shortcut;
        }
        catch
        {
            shortcut.Dispose();
            throw;
        }
    }

    // Save the shortcut to the file.
    public void Save(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        unsafe
        {
            var iid = Guids.IID_IPersistFile;
            PersistFileInst* persistInst = null;

            uint res = inst->Vtbl->QueryInterface(inst, &iid, (void**)&persistInst);
            if (res != 0)
            {
                throw new COMException("Unable to get IPersistFile interface.", (int)res);
            }

            try
            {
                var ptr = Marshal.StringToCoTaskMemUni(fileName);
                try
                {
                    res = persistInst->Vtbl->Save(persistInst, ptr, (uint)fileName.Length);
                    if (res != 0)
                    {
                        throw new COMException("Unable to save shortcut file.", (int)res);
                    }
                }
                finally
                {
                    Marshal.FreeCoTaskMem(ptr);
                }
            }
            finally
            {
                persistInst->Vtbl->Release(persistInst);
            }
        }
    }

    // Update the shortcut's target path
    public void UpdateTargetPath(string newTargetPath)
    {
        Path = newTargetPath;
        Console.WriteLine($"Shortcut path updated to: {newTargetPath}");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!disposed)
        {
            unsafe
            {
                inst->Vtbl->Release(inst);
            }

            disposed = true;
        }
    }

    // Internal loading of a shortcut
    private void LoadInternal(string fileName)
    {
        unsafe
        {
            var iid = Guids.IID_IPersistFile;
            PersistFileInst* persistInst = null;

            uint res = inst->Vtbl->QueryInterface(inst, &iid, (void**)&persistInst);
            if (res != 0)
            {
                throw new COMException("Unable to get IPersistFile interface.", (int)res);
            }

            try
            {
                var ptr = Marshal.StringToCoTaskMemUni(fileName);
                try
                {
                    res = persistInst->Vtbl->Load(persistInst, ptr, 0);
                    if (res != 0)
                    {
                        throw new COMException("Unable to load shortcut file.", (int)res);
                    }
                }
                finally
                {
                    Marshal.FreeCoTaskMem(ptr);
                }
            }
            finally
            {
                persistInst->Vtbl->Release(persistInst);
            }
        }
    }

    // Helper method to set strings using unsafe pointers.
    private unsafe void SetString(delegate* unmanaged[Stdcall]<ShellLinkInst*, nint, uint> setMethod, string? value)
    {
        if (value != null)
        {
            var ptr = Marshal.StringToCoTaskMemUni(value);
            try
            {
                uint res = setMethod(inst, ptr);
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }
        else
        {
            setMethod(inst, 0);
        }
    }
}