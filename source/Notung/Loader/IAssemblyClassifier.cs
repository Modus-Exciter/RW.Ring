using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Notung.Data;
using System.Reflection;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;

namespace Notung.Loader
{
  public interface IAssemblyClassifier : IDisposable
  {
    IList<string> ExcludePrefixes { get; }
    IList<Assembly> TrackingAssemblies { get; }
    IList<Assembly> Plugins { get; }
    void LoadPlugins(string searchPattern);
  }

  class AssemblyClassifier : IAssemblyClassifier
  {
    private readonly AppDomain m_domain;
    private readonly HashSet<Assembly> m_assemblies = new HashSet<Assembly>();

    private readonly BindingList<string> m_exclude_prefixes = new BindingList<string>();
    private readonly Collection<string> m_exclude_wrapper;

    private readonly List<Assembly> m_tracking_assemblies = new List<Assembly>();
    private readonly ReadOnlyCollection<Assembly> m_tracking_wrapper;

    private readonly List<Assembly> m_plugins = new List<Assembly>();
    private readonly ReadOnlyCollection<Assembly> m_plugins_wrapper;

    public AssemblyClassifier(AppDomain domain)
    {
      if (domain == null)
        throw new ArgumentNullException("domain");

      m_domain = domain;
      m_domain.AssemblyLoad += HandleAssemblyLoad;

      lock (m_assemblies)
      {
        foreach (var asm in m_domain.GetAssemblies())
          m_assemblies.Add(asm);
      }

      m_exclude_prefixes.ListChanged += HandleListChanged;
      m_exclude_wrapper = new Collection<string>(m_exclude_prefixes);
      m_tracking_wrapper = new ReadOnlyCollection<Assembly>(m_tracking_assemblies);
      m_plugins_wrapper = new ReadOnlyCollection<Assembly>(m_plugins);
    }

    public AssemblyClassifier() : this(AppDomain.CurrentDomain) { }

    private void HandleAssemblyLoad(object sender, AssemblyLoadEventArgs args)
    {
      Action action = delegate
      {
        lock (m_assemblies)
        {
          m_assemblies.Add(args.LoadedAssembly);

          if (!m_exclude_prefixes.Any(p => args.LoadedAssembly.FullName.StartsWith(p)))
            m_tracking_assemblies.Add(args.LoadedAssembly);
        }
      };

      action.BeginInvoke(null, null);
    }

    private void HandleListChanged(object sender, ListChangedEventArgs e)
    {
      lock (m_assemblies)
      {
        m_tracking_assemblies.Clear();

        foreach (var asm in m_assemblies)
        {
          if (!m_exclude_prefixes.Any(p => asm.FullName.StartsWith(p)))
            m_tracking_assemblies.Add(asm);
        }
      }
    }

    public IList<string> ExcludePrefixes
    {
      get { return m_exclude_wrapper; }
    }

    public IList<Assembly> TrackingAssemblies
    {
      get { return m_tracking_wrapper; }
    }

    public void LoadPlugins(string searchPattern)
    {
      var path = Path.GetDirectoryName(ApplicationInfo.Instance.CurrentProcess.MainModule.FileName);

      foreach (var plugin_file in Directory.GetFiles(path, searchPattern))
      {
        // TODO: PluginInfo from text manifest, then load assembly as plugin
      }
    }

    public IList<Assembly> Plugins
    {
      get { return m_plugins_wrapper; }
    }

    public void Dispose()
    {
      m_domain.AssemblyLoad -= HandleAssemblyLoad;
    }
  }
}
