using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Text;


namespace Notung.Helm
{
  /// <summary>
  /// Объявления некоторых функций Win API
  /// </summary>
  public static class WinAPIHelper
  {
    #region Close button helper

    [Flags]
    enum WFlags : long
    {
      MF_BYPOSITION = 0x400,
      MF_REMOVE = 0x1000,
      MF_DISABLED = 0x2
    }

    [DllImport("user32.Dll")]
    private static extern IntPtr RemoveMenu(int hMenu, int nPosition, WFlags wFlags);

    [DllImport("User32.Dll")]
    private static extern IntPtr GetSystemMenu(int hWnd, bool bRevert);

    [DllImport("User32.Dll")]
    private static extern IntPtr GetMenuItemCount(int hMenu);

    [DllImport("User32.Dll")]
    private static extern IntPtr DrawMenuBar(int hwnd);

    /// <summary>
    /// Отключает кнопку закрытия в главном меню формы
    /// </summary>
    /// <param name="hWnd">Дескриптор окна формы</param>
    public static void DisableCloseButton(int hWnd)
    {
      IntPtr hMenu;
      IntPtr menuItemCount;

      //Obtain the handle to the form's system menu
      hMenu = GetSystemMenu(hWnd, false);

      // Get the count of the items in the system menu
      menuItemCount = GetMenuItemCount(hMenu.ToInt32());

      // Remove the close menuitem
      RemoveMenu(hMenu.ToInt32(), menuItemCount.ToInt32() - 1, WFlags.MF_REMOVE | WFlags.MF_BYPOSITION);

      // Remove the Separator 
      RemoveMenu(hMenu.ToInt32(), menuItemCount.ToInt32() - 2, WFlags.MF_REMOVE | WFlags.MF_BYPOSITION);

      // redraw the menu bar
      DrawMenuBar(hWnd);
    }

    #endregion

    #region Load library helper

    private class SymbolLoader
    {
      private readonly StringCollection m_strings = new StringCollection();

      public StringCollection Strings
      {
        get { return m_strings; }
      }

      public bool LoadSymbol(string name, IntPtr symbolAddress, uint size, IntPtr context)
      {
        m_strings.Add(name);
        return true;
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

    private delegate bool LoadSymbolDelegate(string name, IntPtr symbolAddress, uint size, IntPtr context);

    [DllImport("Imagehlp.dll", CharSet = CharSet.Ansi)]
    private static extern bool SymEnumerateSymbols(IntPtr process, IntPtr baseDll, IntPtr callback, object context);

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

    /// <summary>
    /// Возвращает список функций, находящихся в неуправляемой библиотеке
    /// </summary>
    /// <param name="fileName">Имя файла DLL</param>
    /// <returns>Список имён функций</returns>
    public static StringCollection GetDllExportList(string fileName)
    {
      var loader = new SymbolLoader();

      IntPtr procId = new IntPtr(Process.GetCurrentProcess().Id);

      SymSetOptions(SymSetOptionsType.SYMOPT_UNDNAME | SymSetOptionsType.SYMOPT_DEFERRED_LOADS);

      try
      {
        if (!SymInitialize(procId, null, true))
          return null;

        IntPtr lib = LoadLibrary(fileName);

        if (lib == IntPtr.Zero)
          return null;

        try
        {
          if (!SymLoadModule(procId, 0, fileName, null, lib, 0))
            return null;

          try
          {
            IntPtr collector = Marshal.GetFunctionPointerForDelegate(new LoadSymbolDelegate(loader.LoadSymbol));

            if (!SymEnumerateSymbols(procId, lib, collector, null))
              return null;
          }
          finally
          {
            SymUnloadModule(procId, lib);
          }
        }
        finally
        {
          FreeLibrary(new HandleRef(new object(), lib));
        }
      }
      finally
      {
        SymCleanup(procId);
      }

      return loader.Strings;
    }

    /// <summary>
    /// Вызов неуправляемой функции синхронно
    /// </summary>
    /// <param name="fileName">Имя файла DLL</param>
    /// <param name="function">Имя функции</param>
    public static void CallExternal(string fileName, string function)
    {
      IntPtr lib = LoadLibrary(fileName);

      if (lib == IntPtr.Zero)
        return;

      object wrapper = new object();
      try
      {
        IntPtr funcPtr = GetProcAddress(new HandleRef(wrapper, lib), function);

        if (funcPtr == IntPtr.Zero)
          return;

        ((Action)Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(Action)))();
      }
      finally
      {
        FreeLibrary(new HandleRef(wrapper, lib));
      }
    }

    /*
    /// <summary>
    /// Вызов неуправляемой функции асинхронно в диалоге служебной задачи
    /// </summary>
    /// <param name="work">Задача с настроенными параметрами вызова функции</param>
    /// <returns>True, если функция вызвана и успешно отработала</returns>
    public static bool RunExternal(this ExternalCallWork work, IWin32Window owner)
    {
      ILog log = LogManager.GetLogger(work.Caption);
      string fileName = Path.GetFileName(work.FileName);

      IntPtr lib = LoadLibrary(work.FileName);

      if (lib == IntPtr.Zero)
      {
        log.Alert(new Info(string.Format(Resources.DLL_LOAD_FAIL, fileName), InfoLevel.Error));
        return false;
      }

      object wrapper = new object();
      try
      {
        IntPtr funcPtr = GetProcAddress(new HandleRef(wrapper, lib), work.FunctionName);

        if (funcPtr == IntPtr.Zero)
        {
          log.Alert(new Info(string.Format(Resources.FUNCTION_LOAD_FAIL,
            work.FunctionName, fileName), InfoLevel.Error));
          return false;
        }

        work.ExternalAction = (Action)Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(Action));

        string last_dir = Environment.CurrentDirectory;
        Environment.CurrentDirectory = Path.GetDirectoryName(work.FileName);

        try
        {
          return work.RunAsync(owner);
        }
        finally
        {
          Environment.CurrentDirectory = last_dir;
        }
      }
      finally
      {
        FreeLibrary(new HandleRef(wrapper, lib));
      }
    }*/

    #endregion

    #region Message helper

    public const uint WM_COPYDATA = 0x004A;

    /// <summary>
    /// Вызов функции Win API для асинхронного посылания собщения окну
    /// </summary>
    /// <param name="hwnd">Дескриптор окна</param>
    /// <param name="Msg">Код сообщения</param>
    /// <param name="wParam">wParam</param>
    /// <param name="lParam">lParam</param>
    /// <returns></returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr PostMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Вызов функции Win API для синхронного посылания собщения окну
    /// </summary>
    /// <param name="hwnd">Дескриптор окна</param>
    /// <param name="Msg">Код сообщения</param>
    /// <param name="wParam">wParam</param>
    /// <param name="lParam">lParam</param>
    /// <returns></returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SendMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    #endregion
  }
}
