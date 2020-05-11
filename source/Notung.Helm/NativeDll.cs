using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Notung.Helm
{
  public sealed class NativeDll : IDisposable
  {
    private readonly string m_path;
    private HandleRef m_handle;

    public NativeDll(string path)
    {
      if (!File.Exists(path))
        throw new FileNotFoundException();

      m_path = path;
      m_handle = new HandleRef(this, LoadLibrary(path));

      if (m_handle.Handle == IntPtr.Zero)
        GC.SuppressFinalize(this);
    }

    public string Path
    {
      get { return m_path; }
    }

    public IntPtr Handle
    {
      get { return m_handle.Handle; }
    }

    public void Invoke(string function)
    {
      this.GetFunction<Action>(function)();
    }

    public string[] GetExportList()
    {
      IntPtr procId;

      using (var process = Process.GetCurrentProcess())
        procId = new IntPtr(process.Id);

      SymSetOptions(SymSetOptionsType.SYMOPT_UNDNAME | SymSetOptionsType.SYMOPT_DEFERRED_LOADS);

      if (!SymInitialize(procId, null, true))
        return null;

      try
      {
        if (!SymLoadModule(procId, 0, m_path, null, m_handle.Handle, 0))
          return null;

        
        try
        {
          IntPtr collector = Marshal.GetFunctionPointerForDelegate(new LoadSymbolDelegate(LoadSymbolImpl));
          var builder = new StringBuilder(1<<12);

          if (!SymEnumerateSymbols(procId, m_handle.Handle, collector, builder))
            return null;

          return builder.ToString().Split(' ');
        }
        finally
        {
          SymUnloadModule(procId, m_handle.Handle);
        }
      }
      finally
      {
        SymCleanup(procId);
      }
    }

    public TDelegate GetFunction<TDelegate>(string function) where TDelegate : class
    {
      if (m_handle.Handle == IntPtr.Zero)
        throw new InvalidOperationException();

      IntPtr funcPtr = GetProcAddress(m_handle, function);

      if (funcPtr == IntPtr.Zero)
        return null;

      return (TDelegate)(object)Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(TDelegate));
    }

    private static bool LoadSymbolImpl(string name, IntPtr symbolAddress, uint size, StringBuilder context)
    {
      if (context.Length == 0)
        context.Append(name);
      else
        context.AppendFormat(" {0}", name);

      return true;
    }

    ~NativeDll()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
      if (Handle != IntPtr.Zero)
        FreeLibrary(m_handle);

      if (disposing)
      {
        m_handle = new HandleRef();
      }
    }

    private enum SymSetOptionsType : uint
    {
      SYMOPT_CASE_INSENSITIVE = 0x00000001,
      SYMOPT_UNDNAME = 0x00000002,
      SYMOPT_DEFERRED_LOADS = 0x00000004,
      SYMOPT_LOAD_LINES = 0x00000010,
      SYMOPT_OMAP_FIND_NEAREST = 0x00000020,
      SYMOPT_FAIL_CRITICAL_ERRORS = 0x00000200,
      SYMOPT_AUTO_PUBLICS = 0x00010000,
      SYMOPT_NO_IMAGE_SEARCH = 0x00020000,
      SYMOPT_DEBUG = 0x80000000
    }

    private delegate bool LoadSymbolDelegate(string name, IntPtr symbolAddress, uint size, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder context);

    [DllImport("Imagehlp.dll", CharSet = CharSet.Ansi)]
    private static extern bool SymEnumerateSymbols(IntPtr process, IntPtr baseDll, IntPtr callback, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder context);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr LoadLibrary(string libname);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern bool FreeLibrary(HandleRef hModule);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
    private static extern IntPtr GetProcAddress(HandleRef hModule, string lpProcName);

    [DllImport("Imagehlp.dll", CharSet = CharSet.Ansi)]
    private static extern bool SymInitialize(IntPtr process, string userPath, bool fInvadeProcess);

    [DllImport("Imagehlp.dll", CharSet = CharSet.Ansi)]
    private static extern bool SymLoadModule(IntPtr process, int file, string fileName, string module, IntPtr lib, int size);

    [DllImport("Imagehlp.dll", CharSet = CharSet.Auto)]
    private static extern bool SymUnloadModule(IntPtr process, IntPtr lib);

    [DllImport("Imagehlp.dll", CharSet = CharSet.Auto)]
    private static extern bool SymCleanup(IntPtr process);

    [DllImport("Imagehlp.dll", CharSet = CharSet.Auto)]
    private static extern SymSetOptionsType SymSetOptions(SymSetOptionsType type);
  }
}
