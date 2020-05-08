using System;
using System.IO;
using System.Reflection;
using Notung.ComponentModel;
using System.Collections.Generic;

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
    private readonly ApplicationInfo m_application;
    private readonly string m_working_path;

    public const string FILE_NAME = "settings.config";

    public ProductVersionConfigFileFinder(Assembly productAssembly)
    {
      m_application = new ApplicationInfo(productAssembly);
      m_working_path = GetPath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
    }

    public ProductVersionConfigFileFinder() : this(Assembly.GetEntryAssembly() ?? typeof(IConfigFileFinder).Assembly) { }

    private string GetPath(string basePath, bool version = true)
    {
      if (!string.IsNullOrWhiteSpace(m_application.Company))
        basePath = Path.Combine(basePath, m_application.Company);

      basePath = Path.Combine(basePath, m_application.Product);

      if (version)
        basePath = Path.Combine(basePath, m_application.Version.ToString());

      return basePath;
    }

    public string InputFilePath
    {
      get
      {
        if (File.Exists(this.OutputFilePath))
          return this.OutputFilePath;

        var path = GetPath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), false);

        if (Directory.Exists(path))
          return FindLastConfigFile(path);

        path = GetPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), false);

        if (Directory.Exists(path))
          return FindLastConfigFile(path);

        return null;
      }
    }

    private string FindLastConfigFile(string path)
    {
      List<Version> versions = null;
      foreach (var directory in Directory.GetDirectories(path))
      {
        Version v;

        if (Version.TryParse(Path.GetFileName(directory), out v)
          && v <= m_application.Version
          && File.Exists(Path.Combine(directory, FILE_NAME)))
        {
          if (versions == null)
            versions = new List<Version>();
          versions.Add(v);
        }
      }
      if (versions != null)
      {
        versions.Sort();
        return Path.Combine(path, versions[versions.Count - 1].ToString(), FILE_NAME);
      }

      return null;
    }

    public string OutputFilePath
    {
      get { return Path.Combine(this.WorkingPath, FILE_NAME); }
    }

    public string WorkingPath
    {
      get { return m_working_path; }
    }
  }
}
