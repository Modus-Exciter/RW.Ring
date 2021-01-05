using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Notung
{
  /* Этот класс предназначен для поддержки разных сборок Notung
   * с возможностью отключения некоторых функций для уменьшения
   * связности межу компонентами. Это может потребоваться, если
   * нужно использовать отдельные классы библиотеки, а не всю.
   */
  internal static class ProcessUtil
  {
    public static readonly Process CurrentProcess = Process.GetCurrentProcess();
    public static readonly string StartupPath = CurrentProcess.MainModule.FileName;
    public static ISynchronizeInvoke SynchronizingObject;
    public static Action<Thread> ThreadRegistrator;

    public static void RegisterCurrentThread()
    {
      var registrator = ThreadRegistrator;

      if (registrator != null)
        registrator(Thread.CurrentThread);
    }
  }
}