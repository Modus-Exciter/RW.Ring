using System;
using System.ComponentModel;
using Notung.Threading;

namespace Notung.Loader
{
  /// <summary>
  /// Загрузчик компонента приложения
  /// </summary>
  public interface IComponentLoader
  {
    /// <summary>
    /// Загрузка компонента приложения
    /// </summary>
    /// <param name="context">Контекст загрузки</param>
    /// <returns><code>true</code>, если компонент загружен. <code>false</code>, если не загружен</returns>
    bool Load(LoadingContext context);
  }

  /// <summary>
  /// Загрузчик компонента приложения с зависимостями
  /// </summary>
  public interface IApplicationLoader : IDependencyItem<Type>, IComponentLoader { }

  public class LoadingContext
  {
    private readonly DependencyContainer m_container;
    private readonly ISynchronizeInvoke m_invoker;
    private readonly IProgressIndicator m_indicator;
    private readonly InfoBuffer m_buffer = new InfoBuffer();

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
  }
}
