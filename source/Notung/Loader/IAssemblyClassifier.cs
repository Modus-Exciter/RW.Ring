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

    /// <summary>
    /// Загрузка всех зависимостей указанной сборки
    /// </summary>
    /// <param name="source"></param>
    void LoadDependencies(Assembly source);
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

    private volatile bool m_loading_plugins;

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
      m_domain.AssemblyResolve += HandleAssemblyResolve;

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

      m_loading_plugins = true;

      try
      {
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
      finally
      {
        m_loading_plugins = false;
      }
    }

    public void LoadDependencies(Assembly source)
    {
      if (source == null)
        throw new ArgumentNullException("source");

      lock (m_assemblies)
      {
        if (m_prefix_tree.MatchAny(source.FullName))
          return;
      }

      foreach (var asm_name in source.GetReferencedAssemblies())
      {
        lock (m_assemblies)
        {
          if (m_prefix_tree.MatchAny(asm_name.Name))
            continue;
        }

        if (asm_name.Name.Contains(".resources"))
          continue;

        int before;
        Assembly asm = null;

        lock (m_assemblies)
        {
          before = m_assemblies.Count;
          try
          {
            asm = Assembly.Load(asm_name);
          }
          catch (Exception ex)
          {
            _log.Error("LoadDependencies(): excption", ex);
          }
        }

        if (m_assemblies.Count > before && asm != null)
          this.LoadDependencies(asm);
      }
    }

    public void Dispose()
    {
      m_domain.AssemblyLoad -= HandleAssemblyLoad;
      m_domain.AssemblyResolve -= HandleAssemblyResolve;
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

    private Assembly HandleAssemblyResolve(object source, ResolveEventArgs e)
    {
      if (m_loading_plugins && !string.IsNullOrEmpty(this.PluginsDirectory))
      {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.PluginsDirectory, e.Name + ".dll");

        if (File.Exists(path))
          return Assembly.LoadFrom(path);
      }

      return null;
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
            if (reader.NodeType == XmlNodeType.Element && reader.Depth == 0 && reader.Name == "plugin")
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
        return Assembly.Load(Path.GetFileNameWithoutExtension(fileName));
      }
      catch (BadImageFormatException ex)
      {
        _log.Error("LoadAssemblyFromFile(): exception", ex);
        m_unmanaged_asms.Add(fileName);

        return null;
      }
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