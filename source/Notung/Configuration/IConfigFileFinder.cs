using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Notung.Configuration
{
  public interface IConfigFileFinder
  {
    string InputFilePath { get; }
    string OutputFilePath { get; }
  }

  public sealed class DirectConfigFileFinder : IConfigFileFinder
  {
    private readonly string m_path;

    public DirectConfigFileFinder(string path)
    {
      if (!File.Exists(path))
        throw new FileNotFoundException("Config file not found");

      m_path = path;
    }
    
    public string InputFilePath
    {
      get { return m_path; }
    }

    public string OutputFilePath
    {
      get { return m_path; }
    }
  }

  public sealed class ProductVersionConfigFileFinder : IConfigFileFinder
  {
    private readonly Assembly m_product_assembly;

    public ProductVersionConfigFileFinder(Assembly productAssembly)
    {
      if (productAssembly == null)
        throw new ArgumentNullException("productAssembly");

      m_product_assembly = productAssembly;
    }

    public ProductVersionConfigFileFinder() : this(Assembly.GetEntryAssembly()) { }

    // TODO: Ищем подходящий путь среди прошлых версий
    public string InputFilePath
    {
      get { throw new NotImplementedException(); }
    }

    // TODO: берём папку продукта и версии именно текущего пользователя
    public string OutputFilePath
    {
      get { throw new NotImplementedException(); }
    }
  }
}
