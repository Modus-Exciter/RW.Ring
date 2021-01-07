using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Notung.Data;
using Notung.Helm.Windows.Properties;

namespace Notung.Helm.Windows
{
  /// <summary>
  /// Работа с неуправляемыми dll и функциями, экспортированными из них
  /// </summary>
  public sealed class NativeDll : IDisposable
  {
    private readonly string m_path;
    private HandleRef m_handle;

    /// <summary>
    /// Загрузка dll для запуска объявленных в ней функций
    /// </summary>
    /// <param name="path">Путь к dll с именем файла</param>
    public NativeDll(string path)
    {
      if (!File.Exists(path))
        throw new FileNotFoundException();

      m_path = path;
      m_handle = new HandleRef(this, LoadLibrary(path));

      if (m_handle.Handle == IntPtr.Zero)
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Путь к dll, указанный при создании объекта
    /// </summary>
    public string Path
    {
      get { return m_path; }
    }

    /// <summary>
    /// Дескриптор загруженной dll
    /// </summary>
    public IntPtr Handle
    {
      get { return m_handle.Handle; }
    }

    /// <summary>
    /// Получение функции, объявленной в dll, для вызова
    /// </summary>
    /// <typeparam name="TDelegate">Тип делегата, описывающего сигнатуру требуемой функции</typeparam>
    /// <param name="function">Имя загружаемой функции</param>
    /// <returns>Делегат, ссылающийся на загруженную функцию. 
    /// Если функцию не удалось загрузить, метод возвращает null</returns>
    public TDelegate GetFunction<TDelegate>(string function) where TDelegate : class
    {
      if (m_handle.Handle == IntPtr.Zero)
        throw new InvalidOperationException(Resources.DLL_FUNCTIONS_ENUM_FAIL);

      IntPtr funcPtr = GetProcAddress(m_handle, function);

      if (funcPtr == IntPtr.Zero)
        return null;

      return (TDelegate)(object)Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(TDelegate));
    }

    /// <summary>
    /// Получение имён функций, экспортированных из dll
    /// </summary>
    /// <param name="path">Путь к dll, в которой требуется узнать имена функций</param>
    /// <returns>Массив имён функций</returns>
    public static string[] GetExportList(string path)
    {
      IntPtr procId = new IntPtr(AppManager.Instance.CurrentProcess.Id);

      SymSetOptions(SymSetOptionsType.SYMOPT_UNDNAME | SymSetOptionsType.SYMOPT_DEFERRED_LOADS);

      if (!SymInitialize(procId, null, false))
        return null;

      try
      {
        var module_handle = LoadLibraryExW(path, IntPtr.Zero, LoadLibraryFlags.LOAD_LIBRARY_AS_DATAFILE);

        if (module_handle == IntPtr.Zero)
          return null;

        try
        {
          if (!SymLoadModule(procId, 0, path, null, module_handle, 0))
            return null;

          try
          {
            return LoadSymbols(procId, module_handle);
          }
          finally
          {
            SymUnloadModule(procId, module_handle);
          }
        }
        finally
        {
          FreeLibrary(new HandleRef(null, module_handle));
        }
      }
      finally
      {
        SymCleanup(procId);
      }
    }

    #region Object destroying ---------------------------------------------------------------------

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

    #endregion

    #region Symbol enumeration --------------------------------------------------------------------

    private static string[] LoadSymbols(IntPtr procId, IntPtr moduleHandle)
    {
      var dlgt = new LoadSymbolDelegate(LoadSymbolImpl);
      GC.KeepAlive(dlgt);

      IntPtr collector = Marshal.GetFunctionPointerForDelegate(dlgt);

      var strings = new List<string>();
      GCHandle handle = GCHandle.Alloc(strings);
      try
      {
        if (SymEnumerateSymbols(procId, moduleHandle, collector, (IntPtr)handle))
          return strings.ToArray();
        else
          return ArrayExtensions.Empty<string>();
      }
      finally
      {
        handle.Free();
      }
    }

    private static bool LoadSymbolImpl(string name, IntPtr symbolAddress, uint size, IntPtr context)
    {
      var strings = (List<string>)((GCHandle)context).Target;
      
      if (strings != null)
        strings.Add(name);

      return true;
    }

    private delegate bool LoadSymbolDelegate(string name, IntPtr symbolAddress, uint size, IntPtr context);

    #endregion

    #region API Helpers ---------------------------------------------------------------------------

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

    private enum LoadLibraryFlags : uint
    {
      DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
      LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
      LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
      LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
      LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
      LOAD_LIBRARY_SEARCH_APPLICATION_DIR = 0x00000200,
      LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000,
      LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100,
      LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800,
      LOAD_LIBRARY_SEARCH_USER_DIRS = 0x00000400,
      LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008,
      LOAD_LIBRARY_REQUIRE_SIGNED_TARGET = 0x00000080,
      LOAD_LIBRARY_SAFE_CURRENT_DIRS = 0x00002000
    }

    [DllImport("Imagehlp.dll", CharSet = CharSet.Ansi)]
    private static extern bool SymEnumerateSymbols(IntPtr process, IntPtr baseDll, IntPtr callback, IntPtr context);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr LoadLibrary(string libname);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr LoadLibraryExW([MarshalAs(UnmanagedType.LPWStr)]string libname, IntPtr handle, LoadLibraryFlags flags);

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

    #endregion
  }
}