using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Notung.Data;
using Notung.Logging;
using Notung.Threading;

namespace Notung.Loader
{
  public sealed class ApplicationLoadingTask
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(ApplicationLoadingTask));

    private readonly ILoadingQueue m_queue;
    private readonly DependencyContainer m_container;

    public ApplicationLoadingTask(ILoadingQueue queue, DependencyContainer container)
    {
      if (queue == null)
        throw new ArgumentNullException("queue");

      if (container == null)
        throw new ArgumentNullException("container");

      m_queue = queue;
      m_container = container;
    }

    public ApplicationLoadingResult Run(ISynchronizeInvoke invoker, IProgressIndicator worker)
    {
      if (invoker == null) 
        throw new ArgumentNullException("invoker");

      if (worker == null) 
        throw new ArgumentNullException("worker");

      var ret = new ApplicationLoadingResult();

      var items = new TopologicalSort<Type>(m_queue.GetLoaders()).Sort();

      if (items.Count > 0)
      {
        var context = new LoadingContext(m_container, invoker, worker);

        ret.Buffer = context.Buffer;

        foreach (IApplicationLoader item in items)
        {
          worker.ReportProgress(0, "");

          try
          {
            var loaded = item.Load(context);

            ret[item.Key] = loaded ? new EmptyApplicationLoader(item.Key) : item;

            if (!loaded)
              ret.Success = false;
          }
          catch (Exception ex)
          {
            context.Buffer.Add(ex);
            _log.Error("Run(): exception", ex);
            ret[item.Key] = item;
            ret.Success = false;
          }
        }
      }

      return ret;
    }

    /// <summary>
    /// Заместитель реального загрузчика для случаев, когда компоненты уже инициализированы
    /// </summary>
    private class EmptyApplicationLoader : IApplicationLoader
    {
      private readonly Type m_type;

      public EmptyApplicationLoader(Type keyType)
      {
        m_type = keyType;
      }

      #region IApplicationLoader Members

      public bool Load(LoadingContext context)
      {
        if (context == null) 
          throw new ArgumentNullException("context");

        return true;
      }

      #endregion

      #region IDependencyItem<Type> Members

      public Type Key
      {
        get { return m_type; }
      }

      public IList<Type> MandatoryDependencies
      {
        get { return ArrayExtensions.Empty<Type>(); }
      }

      public IList<Type> OptionalDependencies
      {
        get { return ArrayExtensions.Empty<Type>(); }
      }

      #endregion
    }

  }

  public sealed class ApplicationLoadingResult : ILoadingQueue
  {
    private readonly Dictionary<Type, IApplicationLoader> m_loaders = new Dictionary<Type, IApplicationLoader>();

    public ApplicationLoadingResult()
    {
      this.Success = true;
    }

    public IApplicationLoader this[Type key]
    {
      get
      {
        if (key == null) 
          throw new ArgumentNullException("key");

        return m_loaders[key];
      }
      internal set
      {
        if (key == null) 
          throw new ArgumentNullException("key");

        m_loaders[key] = value;
      }
    }

    public bool Success { get; internal set; }

    public InfoBuffer Buffer { get; internal set; }

    public IApplicationLoader[] GetLoaders()
    {
      return m_loaders.Values.ToArray();
    }
  }
}
