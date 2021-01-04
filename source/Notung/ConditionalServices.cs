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

#if APPLICATION_INFO
    public static string GetWorkingPath()
    {
      return ApplicationInfo.Instance.GetWorkingPath();
    }

    internal sealed class ProductPath
    {
      private readonly ApplicationInfo m_application;

      public ProductPath(Assembly productAssembly)
      {
        m_application = new ApplicationInfo(productAssembly);
      }

      public string GetPath(string basePath, bool version = true)
      {
        if (!string.IsNullOrWhiteSpace(m_application.Company))
          basePath = Path.Combine(basePath, m_application.Company);

        basePath = Path.Combine(basePath, m_application.Product);

        if (version)
          basePath = Path.Combine(basePath, m_application.Version.ToString());

        return basePath;
      }

      public Version Version
      {
        get { return m_application.Version; }
      }
    }
#else
    private static ProductPath m_default_path;

    public static string GetWorkingPath()
    {
      var base_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

      var prod_path = m_default_path ?? (m_default_path = new ProductPath(
        Assembly.GetEntryAssembly() ?? typeof(ConditionalServices).Assembly));

      return prod_path.GetPath(base_path, false);
    }

    internal sealed class ProductPath
    {
      private readonly Assembly m_assembly;

      public ProductPath(Assembly productAssembly)
      {
        if (productAssembly == null)
          throw new ArgumentNullException("productAssembly");

        m_assembly = productAssembly;
      }

      public string GetPath(string basePath, bool version = true)
      {
        var company = GetCustomAttribute<AssemblyCompanyAttribute>(m_assembly);

        if (company != null && !string.IsNullOrWhiteSpace(company.Company))
          basePath = Path.Combine(basePath, company.Company);

        var product = GetCustomAttribute<AssemblyProductAttribute>(m_assembly);

        if (product != null && !string.IsNullOrWhiteSpace(product.Product))
          basePath = Path.Combine(basePath, product.Product);
        else
          basePath = Path.Combine(basePath, m_assembly.GetName().Name);

        if (version)
          basePath = Path.Combine(basePath, m_assembly.GetName().Version.ToString());

        return basePath;
      }

      public Version Version
      {
        get { return m_assembly.GetName().Version; }
      }

      private static A GetCustomAttribute<A>(Assembly assembly) where A : Attribute
      {
        if (assembly.IsDefined(typeof(A), false))
          return assembly.GetCustomAttributes(typeof(A), false)[0] as A;

        return null;
      }
    }
#endif
  }
}