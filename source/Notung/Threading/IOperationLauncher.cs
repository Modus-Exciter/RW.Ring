using System;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using Notung.Logging;
using Notung.ComponentModel;

namespace Notung.Threading
{
  /// <summary>
  /// Управляет задачами, которые можно запустить в диалоге с индикатором прогресса
  /// </summary>
  public interface IOperationLauncher 
  {
    /// <summary>
    /// Время ожидания до показа индикатора прогресса
    /// </summary>
    TimeSpan SyncWaitingTime { get; set; }

    /// <summary>
    /// Запуск задачи на выполнение
    /// </summary>
    /// <param name="runBase">Задача, которую требуется выполнить</param>
    /// <param name="parameters">Параметры отображения задачи в пользовательском интерфейсе</param>
    /// <returns>Результат выполнения задачи</returns>
    TaskStatus Run(IRunBase runBase, LaunchParameters parameters = null);

#if !APP_MANAGER
    event EventHandler<InfoBufferEventArgs> MessagesRecieved;
#endif
  }

  public class OperationLauncher : MarshalByRefObject, IOperationLauncher
  {
    private readonly IOperationLauncherView m_view;

    public OperationLauncher(IOperationLauncherView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      m_view = view;
    }

    internal OperationLauncher() : this(new TaskManagerViewStub()) { }

    public TimeSpan SyncWaitingTime { get; set; }

    public TaskStatus Run(IRunBase runBase, LaunchParameters parameters = null)
    {
      if (runBase == null)
        throw new ArgumentNullException("runBase");

      if (parameters == null)
        parameters = new LaunchParameters();

      parameters.Setup(runBase);

      if (runBase is IChangeLaunchParameters)
        ((IChangeLaunchParameters)runBase).SetLaunchParameters(parameters);

      var operation = new LengthyOperation(runBase);
#if !APP_MANAGER
      operation.Invoker = m_view.Invoker;
#endif
      operation.Start();

      var ret = Complete(operation, parameters);

      if (runBase is IServiceProvider)
      {
        var messages = ((IServiceProvider)runBase).GetService<InfoBuffer>();

        if (messages != null && messages.Count != 0)
          this.OnMessagesRecieved(messages);
      }
      else if (operation.Error != null)
        this.OnError(operation.Error);

      return ret;
    }

    private TaskStatus Complete(LengthyOperation operation, LaunchParameters parameters)
    {
      if (this.SyncWaitingTime > TimeSpan.Zero && operation.Wait(this.SyncWaitingTime))
        return operation.Status;

      if (m_view.Invoker.InvokeRequired)
      {
        m_view.Invoker.Invoke(new Action<LengthyOperation, LaunchParameters>
          (m_view.ShowProgressDialog), new object[] { operation, parameters });
      }
      else
        m_view.ShowProgressDialog(operation, parameters);

      return operation.Status;
    }

#if APP_MANAGER

    private void OnMessagesRecieved(InfoBuffer buffer)
    {
      AppManager.Notificator.Show(buffer);
    }

    private void OnError(Exception ex)
    {
      AppManager.Notificator.Show(new Info(ex));
    }

    #else

    private void OnMessagesRecieved(InfoBuffer buffer)
    {
      this.MessagesRecieved.InvokeSynchronized(this, new InfoBufferEventArgs(buffer), m_view.Invoker);
    }

    private void OnError(Exception ex)
    {
      throw ex;
    }

    public event EventHandler<InfoBufferEventArgs> MessagesRecieved;

#endif
  }

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

    public event EventHandler Completed;

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

    private void Run()
    {
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
      this.TaskCompleted.InvokeSynchronized(this, EventArgs.Empty, this.Invoker);
    }

    private void OnProgressChanged(ProgressChangedEventArgs e)
    {
      this.ProgressChanged.InvokeSynchronized(this, e, this.Invoker);
    }

    internal ISynchronizeInvoke Invoker { get; set; }

#endif
  }

  public interface IOperationLauncherView : ISynchronizeProvider
  {
    void ShowProgressDialog(LengthyOperation task, LaunchParameters parameters);
  }

  public class TaskManagerViewStub : SynchronizeProviderStub, IOperationLauncherView
  {
    private class ProgressInConsole
    {
      private LaunchParameters m_parameters;

      public ProgressInConsole(LaunchParameters parameters)
      {
        m_parameters = parameters;
      }

      public void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
      {
        if (m_parameters.SupportsPercentNotification)
          Console.WriteLine("{0,3} %, {1}", e.ProgressPercentage, e.UserState);
        else
          Console.WriteLine(e.UserState);
      }

      public void HandleTaskCompleted(object sender, EventArgs e)
      {
        LengthyOperation operation = sender as LengthyOperation;

        if (operation == null)
          return;

        operation.ProgressChanged -= this.HandleProgressChanged;
        operation.Completed -= this.HandleTaskCompleted;
      }
    }

    public void ShowProgressDialog(LengthyOperation operation, LaunchParameters parameters)
    {
      var progress = new ProgressInConsole(parameters);

      operation.ProgressChanged += progress.HandleProgressChanged;
      operation.Completed += progress.HandleTaskCompleted;
      operation.Wait();
    }
  }
}
