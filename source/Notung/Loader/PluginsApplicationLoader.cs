﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Notung.Properties;
using Notung.Services;

namespace Notung.Loader
{
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

      var old_dir = Environment.CurrentDirectory;
      Environment.CurrentDirectory = Path.GetDirectoryName(AppManager.Instance.StartupPath);

      try
      {
        if (!Directory.Exists(m_plugins_directory))
          Directory.CreateDirectory(m_plugins_directory);
      }
      finally
      {
        Environment.CurrentDirectory = old_dir;
      }

      AppManager.AssemblyClassifier.PluginsDirectory = m_plugins_directory;
      context.Container.AddService(this);

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

  public class PluginsApplicationLoader<T> : IApplicationLoader where T : class
  {
    private readonly string m_filter;
    private readonly LoadPluginsMode m_mode;
    private static readonly Type[] _dependencies = new Type[] { typeof(PluginsApplicationLoader) };

    public PluginsApplicationLoader(string filter, LoadPluginsMode mode)
    {
      m_filter = filter;
      m_mode = mode;
    }

    public string Filter
    {
      get { return m_filter; }
    }

    public bool Load(LoadingContext context)
    {
      AppManager.AssemblyClassifier.LoadPlugins(m_filter, m_mode);

      var list = new List<T>();

      foreach (var plugin in AppManager.AssemblyClassifier.Plugins)
      {
        var searcher = m_mode == LoadPluginsMode.CurrentDomain ? new TypeSearcher() :
          (TypeSearcher)plugin.Domain.CreateInstanceAndUnwrap("Notung", "Notung.Loader.TypeSearcher");

        searcher.Set(plugin.AssemblyName, typeof(T));

        foreach (T t in searcher.GetItems())
          list.Add(t);
      }

      context.Container.AddService<List<T>>(list);
      return true;
    }

    public void Setup(LoadingContext context) { }

    public Type Key
    {
      get { return typeof(IList<T>); }
    }

    public ICollection<Type> Dependencies
    {
      get { return _dependencies; }
    }
  }

  internal class TypeSearcher : MarshalByRefObject
  {
    private AssemblyName m_name;
    private Type m_type;

    public void Set(AssemblyName name, Type type)
    {
      m_name = name;
      m_type = type;
    }

    public IList GetItems()
    {
      if (m_name == null || m_type == null)
        throw new InvalidOperationException();

      var list = new List<object>();

      foreach (var type in Assembly.Load(m_name).GetAvailableTypes())
      {
        if (!type.IsAbstract && m_type.IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null)
          list.Add(Activator.CreateInstance(type));
      }

      return list;
    }
  }
}