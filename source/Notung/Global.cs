using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Notung
{
  /* 
   * Этот класс предназначен для поддержки разных сборок Notung
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
    public static readonly object[] EmptyArgs = Enumerable.Empty<object>() as object[] ?? new object[0];
  }

  /// <summary>
  /// Помечает класс для расшаривания находящихся в нём сервисов между доменами приложений,
  /// за это отвечает метод static void Share(AppDomain newDomain) в этом классе
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
  public sealed class AppDomainShareAttribute : Attribute { }
}