using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Notung.Loader;
using Notung;
using Schicksal.Exchange;

namespace Schicksal.Helm
{
  public class SchicksalLoadingQueue : LoadingQueue
  {
    protected override void FillLoaders(Action<IApplicationLoader> add, Func<Type, bool> contains)
    {
      add(new ImportMenuLoader());
    }

    private class ImportMenuLoader : IApplicationLoader
    {
      public bool Load(LoadingContext context)
      {
        AppManager.AssemblyClassifier.PluginsDirectory = "Plugins";
        AppManager.AssemblyClassifier.LoadPlugins("*.import");

        var list = new List<ITableImport>();

        foreach (var plugin in AppManager.AssemblyClassifier.Plugins)
        {
          foreach (var type in plugin.Domain.Load(plugin.AssemblyName).GetAvailableTypes())
          {
            if (typeof(ITableImport).IsAssignableFrom(type))
              list.Add((ITableImport)Activator.CreateInstance(type));
          }
        }

        context.Container.SetService<IList<ITableImport>>(list);

        return true;
      }

      public void Setup(LoadingContext context) { }

      public Type Key
      {
        get { return typeof(IList<ITableImport>); }
      }

      public ICollection<Type> Dependencies
      {
        get { return Type.EmptyTypes; }
      }
    }
  }
}
