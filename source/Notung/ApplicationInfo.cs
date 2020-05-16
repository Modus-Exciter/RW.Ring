using System;
using System.IO;
using System.Reflection;
using Process = System.Diagnostics.Process;

namespace Notung
{
  [Serializable]
  public sealed class ApplicationInfo
  {
    private readonly Assembly m_product_assembly;

    private string m_company;
    private string m_product;
    private string m_description;
    private Version m_version;
    private Version m_file_version;

    [NonSerialized]
    private Process m_current_process;

    private static ApplicationInfo _instance;
    private static object _lock = new object();

    public ApplicationInfo(Assembly productAssembly)
    {
      if (productAssembly == null)
        throw new ArgumentNullException("productAssembly");

      m_product_assembly = productAssembly;
    }

    public ApplicationInfo() : this(Assembly.GetEntryAssembly() ?? typeof(ApplicationInfo).Assembly) { }

    public static ApplicationInfo Instance
    {
      get
      {
          if (_instance != null)
            return _instance;

        lock (_lock)
        {
          if (_instance != null)
            return _instance;

          if (Assembly.GetEntryAssembly() != null)
          {
            _instance = new ApplicationInfo(Assembly.GetEntryAssembly());
            return _instance;
          }
          else
            return new ApplicationInfo(typeof(ApplicationInfo).Assembly);
        }
      }
    }

    public Assembly ProductAssembly
    {
      get { return m_product_assembly; }
    }

    public string Company
    {
      get
      {
        if (m_company == null)
        {
          string company = null;
          var attr = m_product_assembly.GetCustomAttribute<AssemblyCompanyAttribute>();

          if (attr != null && !string.IsNullOrWhiteSpace(attr.Company))
            company = attr.Company;

          if (company == null)
            company = string.Empty;

          m_company = company;
        }

        return m_company;
      }
    }

    public string Product
    {
      get
      {
        if (m_product == null)
        {
          string product = null;
          var attr = m_product_assembly.GetCustomAttribute<AssemblyProductAttribute>();

          if (attr != null && !string.IsNullOrWhiteSpace(attr.Product))
            product = attr.Product;

          if (string.IsNullOrWhiteSpace(product))
            product = m_product_assembly.GetName().Name;

          m_product = product;
        }

        return m_product;
      }
    }

    public string Description
    {
      get
      {
        if (m_description == null)
        {
          string description = null;
          var attr = m_product_assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();

          if (attr != null && !string.IsNullOrWhiteSpace(attr.Description))
            description = attr.Description;

          if (description == null)
            description = string.Empty;

          m_description = description;
        }

        return m_description;
      }
    }

    public Version Version
    {
      get
      {
        if (m_version == null)
          m_version = m_product_assembly.GetName().Version;

        return m_version;
      }
    }

    public Version FileVersion
    {
      get
      {
        if (m_file_version == null)
        {
          var attr = m_product_assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();

          if (attr == null || string.IsNullOrWhiteSpace(attr.Version) || !Version.TryParse(attr.Version, out m_file_version))
            m_file_version = this.Version;
        }

        return m_file_version;
      }
    }

    public Process CurrentProcess
    {
      get
      {
        if (m_current_process == null)
          m_current_process = Process.GetCurrentProcess();

        return m_current_process;
      }
    }

    public string GetWorkingPath()
    {
      var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

      if (!string.IsNullOrWhiteSpace(this.Company))
        basePath = Path.Combine(basePath, this.Company);

      return Path.Combine(basePath, this.Product);
    }

    public override string ToString()
    {
      return m_product_assembly.ToString();
    }

    public override bool Equals(object obj)
    {
      ApplicationInfo other = obj as ApplicationInfo;

      if (other == null)
        return false;

      return m_product_assembly.Equals(other.m_product_assembly);
    }

    public override int GetHashCode()
    {
      return m_product_assembly.GetHashCode();
    }

    private interface IDomainAcceptor
    {
      void Accept(ApplicationInfo info);
    }

    private class DomainAcceptor : MarshalByRefObject, IDomainAcceptor
    {
      public void Accept(ApplicationInfo info)
      {
        _instance = info;
      }
    }

    internal static void SetupNewDomain(AppDomain newDomain)
    {
      var acceptor = (IDomainAcceptor)newDomain.CreateInstanceAndUnwrap(
        Assembly.GetExecutingAssembly().FullName, typeof(DomainAcceptor).FullName);

      acceptor.Accept(Instance);
    }
  }
}
