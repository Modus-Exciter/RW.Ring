using System;
using System.ComponentModel;
using System.Threading;
using Notung.ComponentModel;

namespace Notung.Threading
{
  public interface IRunBase
  {
    void Run();

    void SetProgressIndicator(IProgressIndicator indicator);
  }

  public interface ICancelableRunBase : IRunBase
  {
    Cancellation CancellationToken { get; set; }

    bool CanCancel { get; }

    event EventHandler CanCancelChanged;
  }

  public interface IProgressIndicator
  {
    void ReportProgress(int percentage, string state);
  }

  public interface ICancellation
  {
    bool IsCancellationRequested { get; }

    void ThrowIfCancellationRequested();
  }

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
      public void ReportProgress(int percentage, string state) { }
    }
  }

  public abstract class CancelableRunBase : RunBase, ICancelableRunBase
  {
    private volatile bool m_can_cancel;
    private Cancellation m_cancellation = _cancellation_stub;

    private static readonly Cancellation _cancellation_stub = new Cancellation(System.Threading.CancellationToken.None);

    protected CancelableRunBase(bool canCancel = true)
    {
      m_can_cancel = canCancel;
    }

    public Cancellation CancellationToken
    {
      get { return m_cancellation; }
      set
      {
        m_cancellation = value ?? _cancellation_stub;
      }
    }

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
