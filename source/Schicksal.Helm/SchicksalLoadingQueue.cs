using System;
using Notung.Loader;
using Schicksal.Exchange;

namespace Schicksal.Helm
{
  public class SchicksalLoadingQueue : LoadingQueue
  {
    protected override void FillLoaders(Action<IApplicationLoader> add, Func<Type, bool> contains)
    {
      add(new PluginsApplicationLoader("Plugins"));
      add(new PluginsApplicationLoader<ITableImport>("*.import"));
    }
  }
}
