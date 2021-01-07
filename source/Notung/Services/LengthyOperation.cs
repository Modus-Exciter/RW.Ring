using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Notung.ComponentModel;
using Notung.Logging;
using Notung.Threading;

namespace Notung.Services
{
  /// <summary>
  /// Обёртка над объектом RunBase, позволяющая отслеживать его состояние выполнения
  /// </summary>
  public sealed class LengthyOperation : IDisposable
  {
    private readonly IRunBase m_run_base;
    private readonly IOperationWrapper m_wrapper;
    private readonly Action m_progress_changed;
    private readonly Action m_can_cancel_changed;
    private ProgressChangedEventArgs m_current_progress;
    private IAsyncResult m_operation;
    private CancellationTokenSource m_cancel_source;

    private readonly object m_lock = new object();

    private static readonly ILog _log = LogManager.GetLogger(typeof(LengthyOperation));

    /// <summary>
    /// Создание новой длительной операции на основе задачи
    /// </summary>
    /// <param name="runBase">Задача, которую требуется выполнить</param>
    /// <param name="invoker">Объект синхронизации для оповезения о прогрессе операции</param>
    public LengthyOperation(IRunBase runBase, ISynchronizeInvoke invoker)
    {
      if (runBase == null)
        throw new ArgumentNullException("runBase");

      m_run_base = runBase;

      if (invoker != null)
        m_wrapper = new SynchronizeOperationWrapper(invoker, true);
      else
        m_wrapper = new EmptyOperationWrapper();

      m_current_progress = new ProgressChangedEventArgs(ProgressPercentage.Unknown, string.Empty);
      m_progress_changed = new Action(this.OnProgressChanged);

      if (runBase is ICancelableRunBase)
        m_can_cancel_changed = new Action(this.OnCanCancelChanged);

      this.Status = TaskStatus.Created;
    }

    public TaskStatus Status { get; private set; }

    public Exception Error { get; private set; }

    public bool CanCancel
    {
      get { return m_run_base is ICancelableRunBase && ((ICancelableRunBase)m_run_base).CanCancel; }
    }

    public bool IsCanceled
    {
      get
      {
        var cancelable = m_run_base as ICancelableRunBase;

        if (cancelable == null)
          return false;

        return cancelable.CancellationToken.IsCancellationRequested;
      }
    }

    public event ProgressChangedEventHandler ProgressChanged;

    public event EventHandler CanCancelChanged;

    public event EventHandler Completed;

    public void Start()
    {
      lock (m_lock)
      {
        if (m_operation != null)
          throw new InvalidOperationException();

        m_operation = new Action(this.Run).BeginInvoke(CloseHandle, this);
      }
    }

    public void Wait()
    {
      var operation = m_operation;

      if (operation == null)
        return;

      operation.AsyncWaitHandle.WaitOne();
    }

    public bool Wait(TimeSpan duration)
    {
      var operation = m_operation;

      if (operation == null)
        return true;

      return operation.AsyncWaitHandle.WaitOne(duration);
    }

    public void ShowCurrentProgress()
    {
      m_wrapper.Invoke(m_progress_changed);
    }

    public CancellationTokenSource GetCancellationTokenSource()
    {
      if (!(m_run_base is ICancelableRunBase))
        return null;

      if (m_cancel_source != null)
        return m_cancel_source;

      lock (m_lock)
      {
        if (m_cancel_source == null)
        {
          m_cancel_source = new CancellationTokenSource();
          ((ICancelableRunBase)m_run_base).CancellationToken = m_cancel_source.Token;
        }

        return m_cancel_source;
      }
    }

    public string GetWorkCaption()
    {
      if (m_run_base is RunBaseProxyWrapper)
        return ((RunBaseProxyWrapper)m_run_base).Caption;
      else
        return LaunchParameters.GetDefaultCaption(m_run_base);
    }

    public Image GetWorkImage()
    {
      IServiceProvider provider = m_run_base as IServiceProvider;

      return provider != null ? provider.GetService<Image>() : null;
    }

    public void Dispose()
    {
      if (m_cancel_source != null)
      {
        m_cancel_source.Dispose();
        m_cancel_source = null;
      }
    }

    private void Run()
    {
      ThreadTracker.RegisterThread(Thread.CurrentThread);

      m_run_base.ProgressChanged += HandleProgressChanged;

      if (m_run_base is ICancelableRunBase)
        ((ICancelableRunBase)m_run_base).CanCancelChanged += HandleCanCancelChanged;

      try
      {
        this.Status = TaskStatus.Running;
        m_run_base.Run();
        this.Status = IsCanceled ? TaskStatus.Canceled : TaskStatus.RanToCompletion;
      }
      catch (OperationCanceledException)
      {
        this.Status = TaskStatus.Canceled;
      }
      catch (Exception ex)
      {
        _log.Error("Run(): exception", ex);

        this.Error = ex;
        this.Status = TaskStatus.Faulted;
      }
      finally
      {
        m_run_base.ProgressChanged -= HandleProgressChanged;

        if (m_run_base is ICancelableRunBase)
          ((ICancelableRunBase)m_run_base).CanCancelChanged -= HandleCanCancelChanged;

        m_wrapper.Invoke(this.OnTaskCompleted);
      }
    }

    private void CloseHandle(IAsyncResult result)
    {
      if (!ReferenceEquals(m_operation, result))
        throw new InvalidOperationException();

      lock (m_lock)
      {
        result.AsyncWaitHandle.Dispose();
        m_operation = null;

        this.Dispose();
      }
    }

    private void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      bool changed = false;

      if (m_current_progress.ProgressPercentage != e.ProgressPercentage)
      {
        m_current_progress = e;
        changed = true;
      }

      if (!object.Equals(m_current_progress.UserState, e.UserState))
      {
        m_current_progress = e;
        changed = true;
      }

      if (changed)
        m_wrapper.Invoke(m_progress_changed);
    }

    private void HandleCanCancelChanged(object sender, EventArgs e)
    {
      m_wrapper.Invoke(m_can_cancel_changed);
    }

    private void OnCanCancelChanged()
    {
      var handler = this.CanCancelChanged;

      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    private void OnProgressChanged()
    {
      var handler = this.ProgressChanged;

      if (handler != null)
        handler(this, m_current_progress);
    }

    private void OnTaskCompleted()
    {
      var handler = this.Completed;

      if (handler != null)
        handler(this, EventArgs.Empty);
    }
  }
}