using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Notung.Data;
using Notung.Logging;
using Notung.Threading;

namespace Notung.Loader
{
  /// <summary>
  /// Загрузка компонентов приложения
  /// </summary>
  public sealed class ApplicationLoadingWork
  {
    private readonly ILoadingQueue m_queue;
    private readonly DependencyContainer m_container;

    private static readonly ILog _log = LogManager.GetLogger(typeof(ApplicationLoadingWork));

    /// <summary>
    /// Создание задачи на загрузку компонентов приложения
    /// </summary>
    /// <param name="queue">Очередь загрузки</param>
    /// <param name="container">Контейнер, в который будут загружаться компоненты</param>
    public ApplicationLoadingWork(ILoadingQueue queue, DependencyContainer container)
    {
      if (queue == null)
        throw new ArgumentNullException("queue");

      if (container == null)
        throw new ArgumentNullException("container");

      m_queue = queue;
      m_container = container;
    }

    /// <summary>
    /// Загружает компоненты приложения с возможностью показа их на указанном элементе пользовательского интерфейса
    /// </summary>
    /// <param name="invoker">Объект синхронизации для компонентов, требующих синхронизации при загрузке</param>
    /// <param name="worker">Индикатор прогресса</param>
    /// <returns>Результат загрузки компонент</returns>
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
          worker.ReportProgress(ProgressPercentage.Unknown, string.Empty);

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

      public ICollection<Type> MandatoryDependencies
      {
        get { return Type.EmptyTypes; }
      }

      public ICollection<Type> OptionalDependencies
      {
        get { return Type.EmptyTypes; }
      }

      #endregion
    }
  }

  /// <summary>
  /// Контекст загрузки компонента приложения
  /// </summary>
  public sealed class LoadingContext
  {
    private readonly DependencyContainer m_container;
    private readonly ISynchronizeInvoke m_invoker;
    private readonly IProgressIndicator m_indicator;
    private readonly InfoBuffer m_buffer = new InfoBuffer();

    private static readonly ILog _log = LogManager.GetLogger(typeof(LoadingContext));

    public LoadingContext(DependencyContainer container, ISynchronizeInvoke invoker, IProgressIndicator indicator)
    {
      if (container == null)
        throw new ArgumentNullException("container");

      if (invoker == null)
        throw new ArgumentNullException("invoker");

      if (indicator == null)
        throw new ArgumentNullException("indicator");

      m_container = container;
      m_invoker = invoker;
      m_indicator = indicator;
    }

    /// <summary>
    /// Контейнер загружаемых компонентов
    /// </summary>
    public DependencyContainer Container
    {
      get { return m_container; }
    }

    /// <summary>
    /// Объект синхронизации
    /// </summary>
    public ISynchronizeInvoke Invoker
    {
      get { return m_invoker; }
    }

    /// <summary>
    /// Индикатор прогресса операции
    /// </summary>
    public IProgressIndicator Indicator
    {
      get { return m_indicator; }
    }

    /// <summary>
    /// Буфер для сбора сообщений загрузки
    /// </summary>
    public InfoBuffer Buffer
    {
      get { return m_buffer; }
    }

    /// <summary>
    /// Запуск задачи в процессе загрузки приложения
    /// </summary>
    /// <param name="runBase">Запущенная задача</param>
    /// <param name="loader">Загрузчик компонента, запустившмй задачу</param>
    /// <returns>True, если задача выполнилась успешно. Иначе, false</returns>
    public bool Run(IRunBase runBase, IApplicationLoader loader)
    {
      if (runBase == null)
        throw new ArgumentNullException("runBase");

      if (loader == null)
        throw new ArgumentNullException("loader");

      runBase.ProgressChanged += this.HandleProgressChanged;

      try
      {
        runBase.Run();
        return true;
      }
      catch (Exception ex)
      {
        _log.Error(string.Format("Run(): error loaing {0}", loader), ex);
        return false;
      }
      finally
      {
        runBase.ProgressChanged -= this.HandleProgressChanged;
      }
    }

    private void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      m_indicator.ReportProgress(e.ProgressPercentage, (e.UserState ?? string.Empty).ToString());
    }
  }

  /// <summary>
  /// Результат загрузки компонентов приложения
  /// </summary>
  public sealed class ApplicationLoadingResult : ILoadingQueue
  {
    private readonly Dictionary<Type, IApplicationLoader> m_loaders = new Dictionary<Type, IApplicationLoader>();

    public ApplicationLoadingResult()
    {
      this.Success = true;
    }

    /// <summary>
    /// Обращение к загрузчику по его типу
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
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

        if (value == null)
          throw new ArgumentNullException("value");

        if (key != value.Key)
          throw new ArgumentException();

        m_loaders[key] = value;
      }
    }

    /// <summary>
    /// Успех загрузки
    /// </summary>
    public bool Success { get; internal set; }

    /// <summary>
    /// Сообщения, возникше в ходе загрузки
    /// </summary>
    public InfoBuffer Buffer { get; internal set; }

    /// <summary>
    /// Загрузчики для компонентов, которые ещё нужно загрузить
    /// </summary>
    /// <returns>Массив загрузчиков</returns>
    public IApplicationLoader[] GetLoaders()
    {
      return m_loaders.Values.ToArray();
    }
  }
}