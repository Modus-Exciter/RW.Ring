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
    IList<string> ExcludePrefixes { get; }

    IList<Assembly> TrackingAssemblies { get; }

    IList<PluginInfo> Plugins { get; }

    string PluginsDirectory { get; set; }

    string[] GetUnmanagedAsemblies();

    void LoadPlugins(string searchPattern);
  }

  public class AssemblyClassifier : IAssemblyClassifier
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(AssemblyClassifier));
    
    private readonly AppDomain m_domain;
    private readonly AssemblyList m_assemblies = new AssemblyList();

    private readonly BindingList<string> m_exclude_prefixes = new BindingList<string>();
    private readonly Collection<string> m_exclude_wrapper;

    private readonly List<Assembly> m_tracking_assemblies = new List<Assembly>();
    private readonly ReadOnlyCollection<Assembly> m_tracking_wrapper;

    private readonly PluginList m_plugins = new PluginList();
    private readonly ReadOnlyCollection<PluginInfo> m_plugins_wrapper;

    private readonly HashSet<string> m_unmanaged_asms = new HashSet<string>();


    private readonly PrefixTree m_prefix_tree = new PrefixTree();

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
    }

    public AssemblyClassifier() : this(AppDomain.CurrentDomain) { }

    public IList<string> ExcludePrefixes
    {
      get { return m_exclude_wrapper; }
    }

    public IList<Assembly> TrackingAssemblies
    {
      get { return m_tracking_wrapper; }
    }

    public IList<PluginInfo> Plugins
    {
      get { return m_plugins_wrapper; }
    }

    public string PluginsDirectory { get; set; }

    public string[] GetUnmanagedAsemblies()
    {
      lock (m_assemblies)
        return m_unmanaged_asms.ToArray();
    }

    public void LoadPlugins(string searchPattern)
    {
      foreach (var plugin_file in Directory.GetFiles(GetPluginsSearchPath(), searchPattern))
      {
        var pluginInfo = GetPluginInfo(plugin_file);

        if (!CheckPlugin(pluginInfo, plugin_file))
          continue;        

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

    private string GetPluginsSearchPath()
    {
      var path = Path.GetDirectoryName(ApplicationInfo.Instance.CurrentProcess.MainModule.FileName);

      var dir = this.PluginsDirectory;

      if (!string.IsNullOrEmpty(dir))
      {
        if (!Path.IsPathRooted(dir))
          dir = Path.Combine(path, dir);

        if (!(AppDomain.CurrentDomain.SetupInformation.PrivateBinPath ?? "").Contains(dir))
        {
          if (string.IsNullOrEmpty(AppDomain.CurrentDomain.SetupInformation.PrivateBinPath))
            AppDomain.CurrentDomain.SetupInformation.PrivateBinPath = dir;
          else
            AppDomain.CurrentDomain.SetupInformation.PrivateBinPath += ", " + dir;
        }

        path = dir;
      }

      return path;
    }

    private void HandleAssemblyLoad(object sender, AssemblyLoadEventArgs args)
    {
      new Action(delegate
      {
        lock (m_assemblies)
        {
          m_assemblies.Add(args.LoadedAssembly);

          if (!m_prefix_tree.MatchAny(args.LoadedAssembly.FullName))
            m_tracking_assemblies.Add(args.LoadedAssembly);
        }
      }).BeginInvoke(null, null);
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

    private Assembly LoadAssemblyFromFile(string fileName)
    {
      try
      {
        return Assembly.LoadFrom(fileName);
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
