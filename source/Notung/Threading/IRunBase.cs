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
    /// Происходит при изменении прогресса выполнения задачи
    /// </summary>
    event ProgressChangedEventHandler ProgressChanged;
  }

  /// <summary>
  /// Выполняемая задача с индикатором проргресса и возможностью отмены
  /// </summary>
  public interface ICancelableRunBase : IRunBase
  {
    /// <summary>
    /// Токен отмены задачи
    /// </summary>
    CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// Возможно ли отменить задачу в текущий момент
    /// </summary>
    bool CanCancel { get; }

    /// <summary>
    /// Происходит, когда возможность отмены задачи меняется
    /// </summary>
    event EventHandler CanCancelChanged;
  }
  
  /// <summary>
  /// Этим атрибутом следует пометить задачу, если она поддерживает
  /// оповещение о прогрессе операции в процентах
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
  public sealed class PercentNotificationAttribute : Attribute { }

  /// <summary>
  /// Базовая реализация интерфейса IRunBase
  /// </summary>
  public abstract class RunBase : IRunBase
  {
    private volatile int m_percent;
    private volatile object m_state;
    private readonly bool m_percent_notification;
    private readonly InfoBuffer m_infolog = new InfoBuffer();

    protected RunBase()
    {
      m_percent_notification = this.GetType().IsDefined(typeof(PercentNotificationAttribute), false);
    }

    /// <summary>
    /// Поддерживается ли оповещение о прогрессе операции
    /// </summary>
    protected bool SupportsPercentNotification
    {
      get { return m_percent_notification; }
    }

    /// <summary>
    /// Буфер, в который можно писать сообщения, накапливающиеся во время выполнения задачи
    /// </summary>
    protected InfoBuffer Infolog
    {
      get { return m_infolog; }
    }

    public abstract void Run();

    public event ProgressChangedEventHandler ProgressChanged;

    protected void ReportProgress(int percent, object state)
    {
      if (m_percent_notification)
      {
        if (m_percent == percent && object.Equals(m_state, state))
          return;

        m_percent = percent;
      }
      else if (object.Equals(state, m_state))
        return;

      m_state = state;

      this.OnProgressChanged();
    }

    protected void ReportProgress(int percent)
    {
      if (!m_percent_notification || m_percent == percent)
        return;

      m_percent = percent;

      this.OnProgressChanged();
    }

    protected void ReportProgress(object state)
    {
      if (object.Equals(m_state, state))
        return;

      m_state = state;

      this.OnProgressChanged();
    }

    private void OnProgressChanged()
    {
      if (this.ProgressChanged != null)
        this.ProgressChanged.InvokeSynchronized(this, new ProgressChangedEventArgs(m_percent, m_state));
    }

    public virtual object GetService(Type serviceType)
    {
      if (serviceType == typeof(InfoBuffer))
        return m_infolog;
      else
        return null;
    }
  }

  public abstract class CancelableRunBase : RunBase, ICancelableRunBase
  {
    private volatile bool m_can_cancel = true;
    
    public CancellationToken CancellationToken { get; set; }

    public bool CanCancel
    {
      get { return m_can_cancel; }
      protected set
      {
        if (m_can_cancel == value)
          return;

        m_can_cancel = value;
        this.CanCancelChanged.InvokeSynchronized(this, EventArgs.Empty);
      }
    }

    public event EventHandler CanCancelChanged;
  }
}