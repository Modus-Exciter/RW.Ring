using System;
using System.IO;
using System.Reflection;

namespace Notung
{
  /// <summary>
  /// Сведения о текущем продукте
  /// </summary>
  [Serializable]
  [AppDomainShare]
  public sealed class ApplicationInfo
  {
    private readonly Assembly m_product_assembly;

    private string m_company;
    private string m_product;
    private string m_description;
    private string m_copyright;
    private Version m_version;
    private Version m_file_version;

    private static ApplicationInfo _instance;
    private static readonly object _lock = new object();

    /// <summary>
    /// Инициализация сведений о продукте с указанной сборкой
    /// </summary>
    /// <param name="productAssembly">Главная сборка продукта</param>
    public ApplicationInfo(Assembly productAssembly)
    {
      if (productAssembly == null)
        throw new ArgumentNullException("productAssembly");

      m_product_assembly = productAssembly;
    }

    /// <summary>
    /// Инициализация сведений о текущем продукте
    /// </summary>
    public ApplicationInfo() : this(Global.MainAssembly) { }

    /// <summary>
    /// Получение сведений о текущем продукте
    /// </summary>
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
            return new ApplicationInfo(Global.BaseAssembly);
        }
      }
    }

    /// <summary>
    /// Сборка, из которой получаются сведения о продукте
    /// </summary>
    public Assembly ProductAssembly
    {
      get { return m_product_assembly; }
    }

    /// <summary>
    /// Компания, которая произвела продукт
    /// </summary>
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

    /// <summary>
    /// Товарный знак компании, которая произвела продукт
    /// </summary>
    public string Copyright
    {
      get
      {
        if (m_copyright == null)
        {
          string copyright = null;
          var attr = m_product_assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();

          if (attr != null && !string.IsNullOrWhiteSpace(attr.Copyright))
            copyright = attr.Copyright;

          if (copyright == null)
            copyright = string.Format("Copyright @ {0} {1}", this.Company, DateTime.Today.Year);

          m_copyright = copyright;
        }

        return m_copyright;
      }
    }

    /// <summary>
    /// Название продукта
    /// </summary>
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

    /// <summary>
    /// Описание продукта
    /// </summary>
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

    /// <summary>
    /// Версия сборки продукта
    /// </summary>
    public Version Version
    {
      get
      {
        if (m_version == null)
          m_version = m_product_assembly.GetName().Version;

        return m_version;
      }
    }

    /// <summary>
    /// Версия файла продукта
    /// </summary>
    public Version FileVersion
    {
      get
      {
        if (m_file_version == null)
        {
          var attr = m_product_assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();

          if (attr == null || string.IsNullOrWhiteSpace(attr.Version)
            || !Version.TryParse(attr.Version, out m_file_version))
            m_file_version = this.Version;
        }

        return m_file_version;
      }
    }

    /// <summary>
    /// Получение папки текущего пользователя, предназначенной для хранения данных продукта
    /// </summary>
    /// <returns>Полный путь к директории</returns>
    public string GetWorkingPath()
    {
      var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

      if (!string.IsNullOrWhiteSpace(this.Company))
        basePath = Path.Combine(basePath, this.Company);

      return Path.Combine(basePath, this.Product);
    }

    /// <summary>
    /// Получение  общей папки, предназначенной для хранения данных продукта
    /// </summary>
    /// <returns>Полный путь к директории</returns>
    public string GetCommonDataPath()
    {
      var basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

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

    private class DomainAcceptor : MarshalByRefObject
    {
      public void Accept(ApplicationInfo info)
      {
        _instance = info;
      }
    }

    private static void Share(AppDomain newDomain)
    {
      var acceptor = (DomainAcceptor)newDomain.CreateInstanceAndUnwrap(
        Assembly.GetExecutingAssembly().FullName, typeof(DomainAcceptor).FullName);

      acceptor.Accept(Instance);
    }
  }
}