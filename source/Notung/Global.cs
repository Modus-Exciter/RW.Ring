using System.Diagnostics;
using System.Reflection;

namespace Notung
{
  /* Этот класс предназначен для поддержки разных сборок Notung
   * с возможностью отключения некоторых функций для уменьшения
   * связности межу компонентами. Это может потребоваться, если
   * нужно использовать отдельные классы библиотеки, а не всю.
   */
  internal static class Global
  {
    public static readonly Process CurrentProcess = Process.GetCurrentProcess();
    public static readonly string StartupPath = CurrentProcess.MainModule.FileName;
    public static readonly Assembly BaseAssembly = typeof(Global).Assembly;
    public static readonly Assembly MainAssembly = Assembly.GetEntryAssembly() ?? BaseAssembly;
  }
}