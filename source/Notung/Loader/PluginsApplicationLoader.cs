using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Notung.Properties;

namespace Notung.Loader
{
  public class PluginsApplicationLoader<T> : IApplicationLoader where T : class
  {
    private readonly string m_filter;
    private readonly List<Type> m_dependencies = new List<Type>();

    public PluginsApplicationLoader(string filter)
    {
      m_filter = filter;
    }

    public string Filter
    {
      get { return m_filter; }
    }

    public bool Load(LoadingContext context)
    {
      AppManager.AssemblyClassifier.LoadPlugins(m_filter);

      var list = new List<T>();

      foreach (var plugin in AppManager.AssemblyClassifier.Plugins)
      {
        foreach (var type in plugin.Domain.Load(plugin.AssemblyName).GetAvailableTypes())
        {
          if (typeof(T).IsAssignableFrom(type))
            list.Add((T)Activator.CreateInstance(type));
        }
      }

      context.Container.SetService<IList<T>>(list);
      return true;
    }

    public void Setup(LoadingContext context) { }

    public Type Key
    {
      get { return typeof(IList<T>); }
    }

    public ICollection<Type> Dependencies
    {
      get { return m_dependencies; }
    }
  }

  public sealed class PluginsApplicationLoader : IApplicationLoader
  {
    private readonly string m_plugins_directory;

    public PluginsApplicationLoader(string pluginsDirectory)
    {
      m_plugins_directory = pluginsDirectory;
    }

    public string PluginsDirectory
    {
      get { return m_plugins_directory; }
    }
    
    public bool Load(LoadingContext context)
    {
      if (Path.IsPathRooted(m_plugins_directory))
        throw new ArgumentException(Resources.INVALID_PLUGIN_DIRECTORY);

      if (!Directory.Exists(m_plugins_directory))
        Directory.CreateDirectory(m_plugins_directory);

      AppManager.AssemblyClassifier.PluginsDirectory = m_plugins_directory;
      context.Container.SetService(this);

      return true;
    }

    void IApplicationLoader.Setup(LoadingContext context) { }

    public Type Key
    {
      get { return this.GetType(); }
    }

    public ICollection<Type> Dependencies
    {
      get { return Type.EmptyTypes; }
    }
  }
}