using System;
using System.ComponentModel;
using System.Threading;
using Notung.ComponentModel;

namespace Notung.Threading
{
  /// <summary>
  /// Выполняемая задача с индикатором прогресса
  /// </summary>
  public interface IRunBase
  {
    /// <summary>
    /// Выполнить задачу
    /// </summary>
    void Run();

    /// <summary>
    /// Задание индикатора прогресса для задачи
    /// </summary>
    /// <param name="indicator">Индикатор прогресса</param>
    void SetProgressIndicator(IProgressIndicator indicator);
  }

  /// <summary>
  /// Выполняемая задача с индикатором проргресса и возможностью отмены
  /// </summary>
  public interface ICancelableRunBase : IRunBase
  {
    /// <summary>
    /// Обёртка над токеном отмены задачи
    /// </summary>
    CancellationTokenRef CancellationToken { get; set; }
  }

  /// <summary>
  /// Интерфейс для задач, изменяющих настройки отображения в ходе выполнения
  /// </summary>
  public interface IChangeLaunchParameters : IRunBase
  {
    /// <summary>
    /// Передаёт задаче настройки отображения, 
    /// которые она может изменять в ходе выполнения
    /// </summary>
    /// <param name="parameters">Настройки отображения</param>
    void SetLaunchParameters(LaunchParameters parameters);
  }
  
  /// <summary>
  /// Индикатор прогресса для удалённой задачи
  /// </summary>
  public interface IProgressIndicator
  {
    /// <summary>
    /// Отображает прогресс выполнения задачи
    /// </summary>
    /// <param name="percentage">Процент выполнения задачи</param>
    /// <param name="state">Текстовое описание состояния задачи</param>
    void ReportProgress(int percentage, string state);
  }

  /// <summary>
  /// Этим атрибутом следует пометить задачу, если она поддерживает
  /// оповещение о прогрессе операции в процентах
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
  public sealed class PercentNotificationAttribute : Attribute { }

  public abstract class RunBase : MarshalByRefObject, IRunBase
  {
    private volatile int m_percent;
    private volatile string m_state;
    private readonly bool m_percent_notification;
    private IProgressIndicator m_indicator = _indicator_stub;

    private static readonly ProgressIndicatorStub _indicator_stub = new ProgressIndicatorStub();

    protected RunBase()
    {
      m_percent_notification = this.GetType().IsDefined(typeof(PercentNotificationAttribute), false);
    }

    protected bool SupportsPercentNotification
    {
      get { return m_percent_notification; }
    }

    public abstract void Run();

    protected void ReportProgress(int percent, string state)
    {
      if (m_percent_notification)
        m_percent = percent;

      m_state = state;
      m_indicator.ReportProgress(m_percent, m_state);
    }

    protected void ReportProgress(int percent)
    {
      if (!m_percent_notification)
        return;

      if (m_percent == percent)
        return;

      m_percent = percent;
      m_indicator.ReportProgress(m_percent, m_state);
    }

    protected void ReportProgress(string state)
    {
      if (object.Equals(state, m_state))
        return;

      m_state = state;
      m_indicator.ReportProgress(m_percent, m_state);
    }

    void IRunBase.SetProgressIndicator(IProgressIndicator indicator)
    {
      if (indicator == null)
        throw new ArgumentNullException("indicator");

      m_indicator = indicator;
    }

    private sealed class ProgressIndicatorStub : IProgressIndicator
    {
      void IProgressIndicator.ReportProgress(int percentage, string state) { }
    }
  }

  public abstract class CancelableRunBase : RunBase, ICancelableRunBase
  {
    private static readonly CancellationTokenRef _cancellation_stub =
      new CancellationTokenRef(System.Threading.CancellationToken.None);

    private CancellationTokenRef m_cancellation = _cancellation_stub;

    protected CancelableRunBase() { }

    public CancellationTokenRef CancellationToken
    {
      get { return m_cancellation; }
      set { m_cancellation = value ?? _cancellation_stub; }
    }
  }
}
