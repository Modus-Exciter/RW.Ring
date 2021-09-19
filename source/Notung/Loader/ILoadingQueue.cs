using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Notung.Loader
{
  /// <summary>
  /// Очередь, выполняющаяся при загрузке приложения
  /// </summary>
  public interface ILoadingQueue
  {
    IApplicationLoader[] GetLoaders();
  }

  /// <summary>
  /// Базовый класс для  очереди загрузки
  /// </summary>
  public class LoadingQueue : ILoadingQueue
  {
    public IApplicationLoader[] GetLoaders()
    {
      var list = new ApplicationLoaderList();

      this.FillLoaders(list.Add, list.Contains);

      return list.ToArray();
    }

    protected virtual void FillLoaders(Action<IApplicationLoader> add, Func<Type, bool> contains) { }

    private class ApplicationLoaderList : KeyedCollection<Type, IApplicationLoader>
    {
      protected override Type GetKeyForItem(IApplicationLoader item)
      {
        if (item == null)
          throw new ArgumentNullException("item");

        return item.Key;
      }
    }
  }

  /// <summary>
  /// Составная очередь для загрузки приложения
  /// </summary>
  public sealed class LoadingQueueComposite : ILoadingQueue
  {
    private readonly HashSet<ILoadingQueue> m_queues;

    public LoadingQueueComposite(IEnumerable<ILoadingQueue> queues)
    {
      if (queues == null)
        throw new ArgumentNullException("queues");

      m_queues = new HashSet<ILoadingQueue>(queues.Where(q => q != null));
    }

    #region ILoadingQueue Members -----------------------------------------------------------------

    public IApplicationLoader[] GetLoaders()
    {
      var keys = new HashSet<Type>();
      var loaders = new List<IApplicationLoader>();

      foreach (var queue in m_queues)
      {
        foreach (var loader in queue.GetLoaders())
        {
          if (keys.Add(loader.Key))
            loaders.Add(loader);
        }
      }

      return loaders.ToArray();
    }

    #endregion
  }
}