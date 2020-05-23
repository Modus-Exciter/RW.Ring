using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Notung.ComponentModel;
using Notung.Logging;

namespace Notung.Threading
{
  /// <summary>
  /// Обёртка над объектом RunBase, позволяющая отслеживать его состояние выполнения
  /// </summary>
  public sealed class LengthyOperation
  {
    private readonly IRunBase m_run_base;
    private int m_current_progress;
    private object m_current_state;
    private IAsyncResult m_operation;
    private CancellationTokenSource m_cancel_source;

    private readonly object m_lock = new object();

    private static readonly ILog _log = LogManager.GetLogger(typeof(LengthyOperation));

    /// <summary>
    /// Создание новой длительной операции на основе задачи
    /// </summary>
    /// <param name="runBase">Задача, которую требуется выполнить</param>
    public LengthyOperation(IRunBase runBase)
    {
      if (runBase == null)
        throw new ArgumentNullException("runBase");

      m_run_base = runBase;
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

    public event EventHandler CanCancelChanged
    {
      add
      {
        if (m_run_base is ICancelableRunBase)
          ((ICancelableRunBase)m_run_base).CanCancelChanged += value;
      }
      remove
      {
        if (m_run_base is ICancelableRunBase)
          ((ICancelableRunBase)m_run_base).CanCancelChanged -= value;
      }
    }

    public event EventHandler Completed;

    internal void Start()
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
      this.OnProgressChanged(new ProgressChangedEventArgs(m_current_progress, m_current_state));
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

    private void Run()
    {
#if MULTI_LANG
      LanguageSwitcher.RegisterThread(Thread.CurrentThread);
#endif
      m_run_base.ProgressChanged += HandleProgressChanged;
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
        this.OnTaskCompleted();
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

        if (m_cancel_source != null)
        {
          m_cancel_source.Dispose();
          m_cancel_source = null;
        }
      }
    }

    private void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      bool changed = false;

      if (m_current_progress != e.ProgressPercentage)
      {
        m_current_progress = e.ProgressPercentage;
        changed = true;
      }

      if (!object.Equals(m_current_state, e.UserState))
      {
        m_current_state = e.UserState;
        changed = true;
      }

      if (changed)
        this.OnProgressChanged(e);
    }

#if APP_MANAGER

    private void OnTaskCompleted()
    {
      this.Completed.InvokeSynchronized(this, EventArgs.Empty);
    }

    private void OnProgressChanged(ProgressChangedEventArgs e)
    {
      this.ProgressChanged.InvokeSynchronized(this, e);
    }

#else

    private void OnTaskCompleted()
    {
      this.Completed.InvokeSynchronized(this, EventArgs.Empty, this.Invoker);
    }

    private void OnProgressChanged(ProgressChangedEventArgs e)
    {
      this.ProgressChanged.InvokeSynchronized(this, e, this.Invoker);
    }

    internal ISynchronizeInvoke Invoker { get; set; }

#endif

  }
}
