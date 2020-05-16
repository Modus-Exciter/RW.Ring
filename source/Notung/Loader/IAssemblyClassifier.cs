using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Notung.Data;
using Notung.Log;
using Notung.Properties;

namespace Notung.Loader
{
  public interface IAssemblyClassifier : IDisposable
  {
    Collection<string> ExcludePrefixes { get; }

    ReadOnlyCollection<Assembly> TrackingAssemblies { get; }

    ReadOnlyCollection<PluginInfo> Plugins { get; }

    string PluginsDirectory { get; set; }

    ReadOnlySet<string> UnmanagedAsemblies { get; }

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
    private readonly string m_default_plugins_directory;

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

      m_default_plugins_directory = this.FindDefaultPluginsDirectory();
      this.PluginsDirectory = m_default_plugins_directory;
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

    private string FindDefaultPluginsDirectory()
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
                      var dirs = (reader.GetAttribute("privatePath") ?? string.Empty).Split(';');

                      if (dirs.Length == 1 && !string.IsNullOrEmpty(dirs[0]))
                      {
                        return dirs[0];
                      }
                      else
                      {
                        for (int i = 0; i < dirs.Length; i++)
                        {
                          if (dirs[i].ToLower() == "plugins")
                            return dirs[i];
                        }
                      }
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
      var x_doc = new XmlDocument();
      x_doc.Load(path);      
      
      var asm_node = x_doc.SelectSingleNode("/plugin/@assembly");

      if (asm_node != null)
      {
        var asm_file = asm_node.InnerText;

        if (!Path.IsPathRooted(asm_file))
          asm_file = Path.Combine(Path.GetDirectoryName(path), asm_file);

        var name_node = x_doc.SelectSingleNode("/plugin/@name");

        return new PluginInfo(name_node != null ? name_node.InnerText : 
          Path.GetFileNameWithoutExtension(asm_file), asm_file);
      }
      else
        return null;
    }

    private string GetPluginsSearchPath()
    {
      var path = Path.GetDirectoryName(ApplicationInfo.Instance.CurrentProcess.MainModule.FileName);

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
        if (m_default_plugins_directory != null && Path.GetDirectoryName(fileName) ==
          Path.Combine(AppDomain.CurrentDomain.BaseDirectory, m_default_plugins_directory))
        {
          return Assembly.Load(Path.GetFileNameWithoutExtension(fileName));
        }
        else
          return Assembly.LoadFile(fileName);
      }
      catch (Exception ex)
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
