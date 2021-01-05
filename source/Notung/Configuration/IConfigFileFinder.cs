using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Notung.Properties;

namespace Notung.Configuration
{
  public interface IConfigFileFinder
  {
    string InputFilePath { get; }

    string OutputFilePath { get; }

    string WorkingPath { get; }
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

    public string WorkingPath
    {
      get { return Path.GetFullPath(Path.GetDirectoryName(m_path)); }
    }
  }

  public sealed class ProductVersionConfigFileFinder : IConfigFileFinder
  {
    private readonly ApplicationInfo m_product_info;
    private readonly string m_working_path;
    private readonly string m_file_name;

    public const string DEFAULT_FILE = "settings.config";

    public ProductVersionConfigFileFinder(Assembly productAssembly, string fileName = DEFAULT_FILE)
    {
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentNullException("fileName");

      if (Path.GetInvalidFileNameChars().Any(fileName.Contains))
        throw new ArgumentException(Resources.WRONG_FILE_SYMBOLS);

      m_file_name = fileName;
      m_product_info = new ApplicationInfo(productAssembly);
      m_working_path = Path.Combine(m_product_info.GetWorkingPath(), m_product_info.Version.ToString());
    }

    public ProductVersionConfigFileFinder(string fileName = DEFAULT_FILE) 
      : this(Assembly.GetEntryAssembly() ?? typeof(IConfigFileFinder).Assembly, fileName) { }

    private string FindLastConfigFile(string path)
    {
      Version last = null;

      foreach (var directory in Directory.GetDirectories(path))
      {
        Version v;

        if (Version.TryParse(Path.GetFileName(directory), out v)
          && v <= m_product_info.Version && File.Exists(Path.Combine(directory, m_file_name)))
        {
          if (last == null || last < v)
            last = v;
        }
      }

      if (last != null)
        return Path.Combine(path, last.ToString(), m_file_name);
      else
        return null;
    }

    public string InputFilePath
    {
      get
      {
        if (File.Exists(this.OutputFilePath))
          return this.OutputFilePath;

        var path = m_product_info.GetWorkingPath();

        if (Directory.Exists(path))
          return FindLastConfigFile(path);

        path = m_product_info.GetCommonDataPath();

        if (Directory.Exists(path))
          return FindLastConfigFile(path);

        return null;
      }
    }

    public string OutputFilePath
    {
      get { return Path.Combine(this.WorkingPath, m_file_name); }
    }

    public string WorkingPath
    {
      get { return m_working_path; }
    }
  }
}