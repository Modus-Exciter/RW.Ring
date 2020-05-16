using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Notung.Data;
using Notung.Logging;
using Notung.Properties;

namespace Notung.Loader
{
  /// <summary>
  /// Просмотр и управление сборками, загруженными в приложение
  /// </summary>
  public interface IAssemblyClassifier : IDisposable
  {
    /// <summary>
    /// Сборки, имена которых начинаются с этих префиксов, не отслеживаются
    /// </summary>
    Collection<string> ExcludePrefixes { get; }

    /// <summary>
    /// Отслеживаемые сборки
    /// </summary>
    ReadOnlyCollection<Assembly> TrackingAssemblies { get; }

    /// <summary>
    /// Плагины, которые удалось загрузить
    /// </summary>
    ReadOnlyCollection<PluginInfo> Plugins { get; }

    /// <summary>
    /// Директория, в которой расположены плагины
    /// </summary>
    string PluginsDirectory { get; set; }

    /// <summary>
    /// Имена сборок, которые не удалось загрузить при загрузке плагинов
    /// </summary>
    ReadOnlySet<string> UnmanagedAsemblies { get; }

    /// <summary>
    /// Загрузить плагины, находящиеся в директории плагинов
    /// </summary>
    /// <param name="searchPattern">Фильтр для поиска файлов плагинов в директории</param>
    void LoadPlugins(string searchPattern);
  }

  public class AssemblyClassifier : IAssemblyClassifier
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(AssemblyClassifier));
    
    private readonly AppDomain m_domain;
    private readonly AssemblyList m_assemblies = new AssemblyList();

    private readonly BindingList<string> m_exclude_prefixes = new BindingList<string>();
    private readonly Collection<string> m_exclude_wrapper;

    private readonly AssemblyList m_tracking_assemblies = new AssemblyList();
    private readonly ReadOnlyCollection<Assembly> m_tracking_wrapper;

    private readonly PluginList m_plugins = new PluginList();
    private readonly ReadOnlyCollection<PluginInfo> m_plugins_wrapper;

    private readonly HashSet<string> m_unmanaged_asms = new HashSet<string>();
    private readonly ReadOnlySet<string> m_unmanaged_wrapper;

    private readonly PrefixTree m_prefix_tree = new PrefixTree();
    private readonly string[] m_private_bin_path;

    public AssemblyClassifier(AppDomain domain)
    {
      if (domain == null)
        throw new ArgumentNullException("domain");

      m_prefix_tree.AddPrefix("Microsoft");
      m_prefix_tree.AddPrefix("System");
      m_prefix_tree.AddPrefix("mscorlib");

      m_exclude_prefixes.Add("Microsoft");
      m_exclude_prefixes.Add("System");
      m_exclude_prefixes.Add("mscorlib");

      m_domain = domain;
      m_domain.AssemblyLoad += HandleAssemblyLoad;

      lock (m_assemblies)
      {
        foreach (var asm in m_domain.GetAssemblies())
        {
          m_assemblies.Add(asm);

          if (!m_prefix_tree.MatchAny(asm.FullName))
            m_tracking_assemblies.Add(asm);
        }
      }

      m_exclude_prefixes.ListChanged += HandleListChanged;
      m_exclude_wrapper = new Collection<string>(m_exclude_prefixes);
      m_tracking_wrapper = new ReadOnlyCollection<Assembly>(m_tracking_assemblies);
      m_plugins_wrapper = new ReadOnlyCollection<PluginInfo>(m_plugins);
      m_unmanaged_wrapper = new ReadOnlySet<string>(m_unmanaged_asms);

      m_private_bin_path = this.FindPrivateBinaryDirectories() ?? ArrayExtensions.Empty<string>();
      SetDefaultPluginPath();
    }

    public AssemblyClassifier() : this(AppDomain.CurrentDomain) { }

    public Collection<string> ExcludePrefixes
    {
      get { return m_exclude_wrapper; }
    }

    public ReadOnlyCollection<Assembly> TrackingAssemblies
    {
      get { return m_tracking_wrapper; }
    }

    public ReadOnlyCollection<PluginInfo> Plugins
    {
      get { return m_plugins_wrapper; }
    }

    public string PluginsDirectory { get; set; }

    public ReadOnlySet<string> UnmanagedAsemblies
    {
      get { return m_unmanaged_wrapper; }
    }

    public void LoadPlugins(string searchPattern)
    {
      if (string.IsNullOrEmpty(searchPattern))
        throw new ArgumentNullException("searchPattern");
      
      foreach (var plugin_file in Directory.GetFiles(GetPluginsSearchPath(), searchPattern))
      {
        var pluginInfo = GetPluginInfo(plugin_file);

        if (!CheckPlugin(pluginInfo, plugin_file))
          continue;

        pluginInfo.SearchPattern = searchPattern;

        lock (m_assemblies)
        {
          if (m_plugins.Contains(pluginInfo.AssemblyFile))
            continue;
          
          if (!m_assemblies.Contains(pluginInfo.AssemblyFile))
            pluginInfo.Assembly = LoadAssemblyFromFile(pluginInfo.AssemblyFile);
          else
            pluginInfo.Assembly = m_assemblies[pluginInfo.AssemblyFile];

          if (pluginInfo.Assembly != null)
            m_plugins.Add(pluginInfo);
        }
      }
    }

    public void Dispose()
    {
      m_domain.AssemblyLoad -= HandleAssemblyLoad;
    }

    private void HandleAssemblyLoad(object sender, AssemblyLoadEventArgs args)
    {
      lock (m_assemblies)
      {
        m_assemblies.Add(args.LoadedAssembly);

        if (!m_prefix_tree.MatchAny(args.LoadedAssembly.FullName))
          m_tracking_assemblies.Add(args.LoadedAssembly);
      }

      if (args.LoadedAssembly.IsDefined(typeof(LibraryInitializerAttribute), false))
      {
        new Action(delegate
        {
          args.LoadedAssembly.GetCustomAttribute<LibraryInitializerAttribute>().Perform();
        }).BeginInvoke(null, null);
      }
    }

    private void HandleListChanged(object sender, ListChangedEventArgs e)
    {
      lock (m_assemblies)
      {
        m_tracking_assemblies.Clear();
        m_prefix_tree.Clear();

        foreach (var prefix in m_exclude_prefixes)
          m_prefix_tree.AddPrefix(prefix);

        foreach (var asm in m_assemblies)
        {
          if (!m_prefix_tree.MatchAny(asm.FullName))
            m_tracking_assemblies.Add(asm);
        }
      }
    }

    private string[] FindPrivateBinaryDirectories()
    {
      if (string.IsNullOrEmpty(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile)
        || !File.Exists(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile))
        return null;

      using (var file = new FileStream(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
        FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        using (var reader = new XmlTextReader(file))
        {
          while (reader.Read())
          {
            if (reader.NodeType == XmlNodeType.Element)
            {
              if (reader.Name == "configuration")
                reader.ReadStartElement();
              else if (reader.Name != "runtime")
                reader.Skip();

              if (reader.Name == "runtime")
              {
                while (reader.Read())
                {
                  if (reader.NodeType != XmlNodeType.Element || reader.Name != "assemblyBinding"
                    || reader.NamespaceURI != "urn:schemas-microsoft-com:asm.v1")
                    continue;

                  while (reader.Read())
                  {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "probing")
                    {
                      var dirs_string = reader.GetAttribute("privatePath") ?? string.Empty;

                      if (string.IsNullOrWhiteSpace(dirs_string))
                        return null;

                      return dirs_string.Split(';').Select(d => d.Trim())
                        .Where(d => !string.IsNullOrEmpty(d)).Distinct().ToArray();
                    }
                  }
                }
              }
            }
          }
        }
      }

      return null;
    }

    protected virtual PluginInfo GetPluginInfo(string path)
    {
#if DEBUG
      if (path == null) 
        throw new ArgumentNullException("path");
#endif
      using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
      {
        using (var reader = new XmlTextReader(file))
        {
          while (reader.Read())
          {
            if (reader.NodeType == XmlNodeType.Element && reader.Name == "plugin")
            {
              var asm_file = reader.GetAttribute("assembly");

              if (!string.IsNullOrEmpty(asm_file))
              {
                if (!Path.IsPathRooted(asm_file))
                  asm_file = Path.Combine(Path.GetDirectoryName(path), asm_file);

                var name_node = reader.GetAttribute("name");

                return new PluginInfo(name_node != null ? name_node :
                  Path.GetFileNameWithoutExtension(asm_file), asm_file);
              }
            }
          }
        }
      }

      return null;
    }

    private void SetDefaultPluginPath()
    {
      if (m_private_bin_path.Length == 1)
        this.PluginsDirectory = m_private_bin_path[0];
      else
      {
        for (int i = 0; i < m_private_bin_path.Length; i++)
        {
          if (m_private_bin_path[i].ToLower() == "plugins")
          {
            this.PluginsDirectory = m_private_bin_path[i];
            break;
          }
        }
      }
    }

    private string GetPluginsSearchPath()
    {
      var path = AppDomain.CurrentDomain.BaseDirectory;

      var dir = this.PluginsDirectory;

      if (!string.IsNullOrEmpty(dir))
      {
        if (!Path.IsPathRooted(dir))
          dir = Path.Combine(path, dir);

        path = dir;
      }

      return path;
    }

    private bool CheckPlugin(PluginInfo pluginInfo, string plugin_file)
    {
      if (pluginInfo == null)
      {
        _log.Warn(string.Format(Resources.INVALID_PLUGIN_FILE, Path.GetFileName(plugin_file)));
        return false;
      }

      if (!File.Exists(pluginInfo.AssemblyFile))
      {
        _log.Warn(string.Format(Resources.PLUGIN_NOT_FOUND, Path.GetFileName(pluginInfo.AssemblyFile), Path.GetFileName(plugin_file)));
        return false;
      }

      if (m_unmanaged_asms.Contains(pluginInfo.AssemblyFile))
        return false;

      return true;
    }

    private Assembly LoadAssemblyFromFile(string fileName)
    {
      try
      {
        if (this.CanLoadWithFullContext(fileName))
        {
          return Assembly.Load(Path.GetFileNameWithoutExtension(fileName));
        }
        else
        {
          _log.Warn("LoadAssemblyFromFile(): Unable to use Assembly.Load. Trying Assembly.LoadFile");
          return Assembly.LoadFile(fileName);
        }
      }
      catch (BadImageFormatException ex)
      {
        _log.Error("LoadAssemblyFromFile(): exception", ex);
        m_unmanaged_asms.Add(fileName);

        return null;
      }
    }

    private bool CanLoadWithFullContext(string fileName)
    {
      var base_dir = AppDomain.CurrentDomain.BaseDirectory;

      if (fileName == Path.Combine(base_dir, Path.GetFileName(fileName)))
        return true;

      for (int i = 0; i < m_private_bin_path.Length; i++)
      {
        if (fileName == Path.Combine(base_dir, m_private_bin_path[i], Path.GetFileName(fileName)))
          return true;
      }

      return false;
    }

    private class AssemblyList : KeyedCollection<string, Assembly>
    {
      protected override string GetKeyForItem(Assembly item)
      {
        return item.ManifestModule.FullyQualifiedName;
      }
    }

    private class PluginList : KeyedCollection<string, PluginInfo>
    {
      protected override string GetKeyForItem(PluginInfo item)
      {
        return item.AssemblyFile;
      }
    }
  }
}
