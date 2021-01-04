using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Notung
{
  internal static class Utils
  {
    public static readonly Process CurrentProcess = Process.GetCurrentProcess();
    public static readonly string StartupPath = CurrentProcess.MainModule.FileName;
    public static ISynchronizeInvoke Invoker = CurrentProcess.SynchronizingObject;
#if !APPLICATION_INFO
    private static ProductPath m_default_path;
#endif

    public static string GetWorkingPath()
    {
#if APPLICATION_INFO
      return ApplicationInfo.Instance.GetWorkingPath();
#else
      var base_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      var prod_path = m_default_path ?? (m_default_path = new ProductPath(
        Assembly.GetEntryAssembly() ?? typeof(Utils).Assembly));

      return prod_path.GetPath(base_path, false);
#endif
    }
  }

#if APPLICATION_INFO

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
      var company = m_assembly.GetCustomAttribute<AssemblyCompanyAttribute>();

      if (company != null && !string.IsNullOrWhiteSpace(company.Company))
        basePath = Path.Combine(basePath, company.Company);

      var product = m_assembly.GetCustomAttribute<AssemblyProductAttribute>();

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
  }

#endif
}