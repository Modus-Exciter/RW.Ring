using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
    /// Имена сборок, которые не удалось загрузить при загрузке плагинов
    /// </summary>
    ReadOnlySet<string> UnmanagedAsemblies { get; }

    /// <summary>
    /// Плагины, которые удалось загрузить
    /// </summary>
    ReadOnlyCollection<PluginInfo> Plugins { get; }

    /// <summary>
    /// Директория, в которой расположены плагины
    /// </summary>
    string PluginsDirectory { get; set; }

    /// <summary>
    /// Загрузить плагины, находящиеся в директории плагинов
    /// </summary>
    /// <param name="searchPattern">Фильтр для поиска файлов плагинов в директории</param>
    void LoadPlugins(string searchPattern, LoadPluginsMode mode = LoadPluginsMode.CurrentDomain);

    /// <summary>
    /// Загрузка всех зависимостей указанной сборки
    /// </summary>
    /// <param name="source">Сборка, зависимости которой следует загрузить</param>
    void LoadDependencies(Assembly source);
  }

  /// <summary>
  /// Куда загружать плагины
  /// </summary>
  public enum LoadPluginsMode
  {
    /// <summary>
    /// В текущий домен
    /// </summary>
    CurrentDomain,
    /// <summary>
    /// Все плагины в отдельный домен
    /// </summary>
    SeparateDomain,
    /// <summary>
    /// Каждый плагин в свой домен
    /// </summary>
    DomainPerPlugin
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
    private readonly PluginLoader m_plugin_loader;

    private readonly PrefixTree m_prefix_tree = new PrefixTree();

    public AssemblyClassifier(AppDomain domain)
    {
      if (domain == null)
        throw new ArgumentNullException("domain");

      m_exclude_prefixes.Add("Microsoft");
      m_exclude_prefixes.Add("System");
      m_exclude_prefixes.Add("mscorlib");
      m_exclude_prefixes.Add("Windows");
      m_prefix_tree.AddRange(m_exclude_prefixes);

      m_domain = domain;
      m_domain.AssemblyLoad += HandleAssemblyLoad;

      lock (m_assemblies)
      {
        foreach (var asm in m_domain.GetAssemblies())
          this.HandleAssemblyLoad(this, new AssemblyLoadEventArgs(asm));
      }

      m_exclude_prefixes.ListChanged += HandleListChanged;
      m_exclude_wrapper = new Collection<string>(m_exclude_prefixes);
      m_tracking_wrapper = new ReadOnlyCollection<Assembly>(m_tracking_assemblies);
      m_plugins_wrapper = new ReadOnlyCollection<PluginInfo>(m_plugins);
      m_unmanaged_wrapper = new ReadOnlySet<string>(m_unmanaged_asms);
      m_plugin_loader = new PluginLoader(m_unmanaged_asms);
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

    public void LoadPlugins(string searchPattern, LoadPluginsMode mode = LoadPluginsMode.CurrentDomain)
    {
      if (string.IsNullOrEmpty(searchPattern))
        throw new ArgumentNullException("searchPattern");

      var search_path = GetPluginsSearchPath();

      AppDomain separate_domain = mode == LoadPluginsMode.SeparateDomain ?
        CreateDomain(string.Format("Plugins ({0})", searchPattern), search_path) : null;

      try
      {
        foreach (var plugin_file in Directory.GetFiles(search_path, searchPattern))
        {
          var plugin_info = GetPluginInfo(plugin_file);

          if (!CheckPlugin(plugin_info, plugin_file))
            continue;

          plugin_info.SearchPattern = searchPattern;

          switch (mode)
          {
            case LoadPluginsMode.CurrentDomain:
              LoadPluginToCurrentDomain(plugin_info);
              break;

            case LoadPluginsMode.DomainPerPlugin:
              LoadPluginToAnotherDomain(plugin_info, CreateDomain(plugin_info.Name, search_path), true);
              break;

            case LoadPluginsMode.SeparateDomain:
              LoadPluginToAnotherDomain(plugin_info, separate_domain, false);
              break;
          }
        }
      }
      finally
      {
        if (separate_domain != null && m_plugins.Count == 0)
          AppDomain.Unload(separate_domain);
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
      m_plugin_loader.Dispose();
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
        if (e.ListChangedType == ListChangedType.ItemAdded)
          m_prefix_tree.AddPrefix(m_exclude_prefixes[e.NewIndex]);
        else
        {
          m_prefix_tree.Clear();
          m_prefix_tree.AddRange(m_exclude_prefixes);
        }

        m_tracking_assemblies.Clear();

        foreach (var asm in m_assemblies)
        {
          if (!m_prefix_tree.MatchAny(asm.FullName))
            m_tracking_assemblies.Add(asm);
        }
      }
    }

    private AppDomain CreateDomain(string friendlyName, string searchPath)
    {
      AppDomainSetup setup = new AppDomainSetup();

      foreach (var pi in typeof(AppDomainSetup).GetProperties())
      {
        if (pi.CanWrite && pi.GetIndexParameters().Length == 0)
          pi.SetValue(setup, pi.GetValue(m_domain.SetupInformation, null), null);
      }

      setup.PrivateBinPath = this.PluginsDirectory ?? setup.PrivateBinPath;

      AppDomain ret = AppDomain.CreateDomain(friendlyName, m_domain.Evidence, setup);

#if APP_MANAGER
      AppManager.Share(ret);
#else
      LogManager.Share(ret);
      LoggingContext.Share(ret);
#if APPLICATION_INFO
      ApplicationInfo.Share(ret);
#endif

#endif
      return ret;
    }

    private string GetPluginsSearchPath()
    {
      var path = m_domain.BaseDirectory;
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

    private void LoadPluginToCurrentDomain(PluginInfo pluginInfo)
    {
      lock (m_assemblies)
      {
        if (m_plugins.Contains(pluginInfo.AssemblyFile))
          return;

        Assembly asm;

        if (!m_assemblies.Contains(pluginInfo.AssemblyFile))
          asm = m_plugin_loader.LoadAssemblyFromFile(pluginInfo.AssemblyFile);
        else
          asm = m_assemblies[pluginInfo.AssemblyFile];

        if (asm != null)
        {
          pluginInfo.AssemblyName = asm.GetName();
          pluginInfo.Domain = m_domain;
          m_plugins.Add(pluginInfo);
        }
      }
    }

    private void LoadPluginToAnotherDomain(PluginInfo pluginInfo, AppDomain domain, bool unloadOnFail)
    {
      AssemblyName asm_name;

      using (var plugin_loader = (IPluginLoader)domain.CreateInstanceAndUnwrap(
        typeof(AssemblyClassifier).Assembly.FullName, typeof(PluginLoader).FullName))
      {
        asm_name = plugin_loader.LoadAssemblyFromFile(pluginInfo.AssemblyFile);

        if (asm_name != null)
        {
          lock (m_assemblies)
          {
            if (!m_plugins.Contains(pluginInfo.AssemblyFile))
            {
              pluginInfo.AssemblyName = asm_name;
              pluginInfo.Domain = domain;
              m_plugins.Add(pluginInfo);
            }
          }
        }
        else
        {
          var unmanaged = plugin_loader.GetUnmanagedAssemblies();

          if (unmanaged.Count > 0)
          {
            lock (m_assemblies)
            {
              foreach (var asm in unmanaged)
                m_unmanaged_asms.Add(asm);
            }
          }
        }
      }

      if (asm_name == null && unloadOnFail)
        AppDomain.Unload(domain);
    }

    #region Implementation types

    private interface IPluginLoader : IDisposable
    {
      AssemblyName LoadAssemblyFromFile(string fileName);

      HashSet<string> GetUnmanagedAssemblies();
    }

    private class PluginLoader : MarshalByRefObject, IPluginLoader
    {
      private string m_current_plugin;
      private readonly HashSet<string> m_unmanaged_asms;

      public PluginLoader(HashSet<string> unmanagedAsemblies)
      {
        m_unmanaged_asms = unmanagedAsemblies;
        AppDomain.CurrentDomain.AssemblyResolve += HandleAssemblyResolve;
      }

      public PluginLoader() : this(new HashSet<string>()) { }

      public Assembly LoadAssemblyFromFile(string fileName)
      {
        m_current_plugin = fileName;
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
        finally
        {
          m_current_plugin = null;
        }
      }

      private Assembly HandleAssemblyResolve(object source, ResolveEventArgs e)
      {
        if (m_current_plugin != null && e.Name.StartsWith(Path.GetFileNameWithoutExtension(m_current_plugin)))
          return Assembly.LoadFrom(m_current_plugin);

        return null;
      }

      public HashSet<string> GetUnmanagedAssemblies()
      {
        return m_unmanaged_asms;
      }

      AssemblyName IPluginLoader.LoadAssemblyFromFile(string fileName)
      {
        var asm = this.LoadAssemblyFromFile(fileName);

        return asm != null ? asm.GetName() : null;
      }

      public void Dispose()
      {
        AppDomain.CurrentDomain.AssemblyResolve -= HandleAssemblyResolve;
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

      protected override void SetItem(int index, PluginInfo item)
      {
        item.Container = this;
        base.SetItem(index, item);
      }

      protected override void InsertItem(int index, PluginInfo item)
      {
        item.Container = this;
        base.InsertItem(index, item);
      }

      protected override void RemoveItem(int index)
      {
        this.Items[index].Container = null;
        base.RemoveItem(index);
      }

      protected override void ClearItems()
      {
        foreach (var plugin in this.Items)
          plugin.Container = null;

        base.ClearItems();
      }
    }

    #endregion
  }
}