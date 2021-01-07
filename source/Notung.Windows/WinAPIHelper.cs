using System;
using System.Runtime.InteropServices;

namespace Notung.Helm.Windows
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

    #region Message helper

    public const uint WM_COPYDATA = 0x004A;

    public const uint SMTO_ABORTIFHUNG = 0x0002;
    public const uint SMTO_BLOCK = 0x0001;
    public const uint SMTO_NORMAL = 0x0000;
    public const uint SMTO_NOTIMEOUTIFNOTHUNG = 0x0008;
    public const uint SMTO_ERRORONEXIT = 0x0020;

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
    public static extern uint RegisterWindowMessageA(string lpString);

    /// <summary>
    /// Вызов функции Win API для асинхронного посылания собщения окну
    /// </summary>
    /// <param name="hwnd">Дескриптор окна</param>
    /// <param name="Msg">Код сообщения</param>
    /// <param name="wParam">wParam</param>
    /// <param name="lParam">lParam</param>
    /// <returns></returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int PostMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

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

    /// <summary>
    /// Отправка сообщения окну с указанием времени ожидания до того, как функция вернёт управление
    /// </summary>
    /// <param name="hwnd">Дескриптор окна</param>
    /// <param name="Msg">Код сообщения</param>
    /// <param name="wParam">wParam</param>
    /// <param name="lParam">lParam</param>
    /// <param name="flags">Флаги SMTO_XXX, управляющие поведением функции</param>
    /// <param name="timeout">Время ожидания отклика от окна в миллисекундах</param>
    /// <param name="result">Указатель на то место, куда окно должно записать отклик</param>
    /// <returns>Если произошёл таймаут, то IntPtr.Zero. Иначе значение, отличное от IntPtr.Zero</returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SendMessageTimeoutA(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam, uint flags, uint timeout, IntPtr result);

    /// <summary>
    /// Выводит указанное окно на передний план
    /// </summary>
    /// <param name="hWnd">Дескриптор окна, которое требуется вывести на первый план</param>
    /// <returns>Удалось ли выполнить операцию</returns>
    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    /// <summary>
    /// Проверяет наличие админских прав у пользователя
    /// </summary>
    /// <returns>True, если пользователь работает с правами админа. Иначе, false</returns>
    [DllImport("shell32.dll")]
    public static extern bool IsUserAnAdmin();

    #endregion
  }
}
