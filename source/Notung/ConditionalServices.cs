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
  internal static class ConditionalServices
  {
    public static readonly Process CurrentProcess = Process.GetCurrentProcess();
    public static readonly string StartupPath = CurrentProcess.MainModule.FileName;

    public static void RegisterCurrentThread()
    {
#if MULTI_LANG
      Notung.ComponentModel.LanguageSwitcher.RegisterThread(Thread.CurrentThread);
#endif
    }

#if APP_MANAGER
    public static ISynchronizeInvoke SynchronizingObject
    {
      get { return AppManager.OperationLauncher.Invoker; }
      set { Debug.Assert(false, "Attempt to change synchronizing object when APP_MANAGER is enabled"); }
    }

    public static void RecieveMessages(IEnumerable<object> messages, Action<object> caller = null)
    {
      var buffer = messages as InfoBuffer;

      if (buffer != null && buffer.Count != 0)
        AppManager.Notificator.Show(buffer);
    }

    public static void RecieveError(Exception error)
    {
      AppManager.Notificator.Show(new Info(error));
    }
#else
    public static ISynchronizeInvoke SynchronizingObject
    {
      get { return CurrentProcess.SynchronizingObject; }
      set { CurrentProcess.SynchronizingObject = value; }
    }

    public static void RecieveMessages(IEnumerable<object> messages, Action<object> caller = null)
    {
      if (messages != null && messages.Any() && caller != null)
        caller(messages);
    }

    public static void RecieveError(Exception error)
    {
      throw error;
    }
#endif
  }
}