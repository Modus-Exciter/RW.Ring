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
  public interface ITaskManager 
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
  }

  public class TaskManager : MarshalByRefObject, ITaskManager
  {
    private readonly ITaskManagerView m_view;
    private Action<InfoBuffer> m_notification_delegate;

    public TaskManager(ITaskManagerView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      m_view = view;
    }

    internal TaskManager() : this(new TaskManagerViewStub()) { }

    public TimeSpan SyncWaitingTime { get; set; }

    public TaskStatus Run(IRunBase work, LaunchParameters parameters)
    {
      if (work == null)
        throw new ArgumentNullException("runBase");

      if (parameters == null)
        parameters = new LaunchParameters();

      parameters.Setup(work);

      if (work is IChangeLaunchParameters)
        ((IChangeLaunchParameters)work).SetLaunchParameters(parameters);

      var task_info = new TaskInfo(work, parameters);
      var action = new Action(task_info.Run);
      var async = action.BeginInvoke(null, task_info);

      var ret = FinishTask(task_info, parameters, async);

      if (work is IServiceProvider && m_notification_delegate != null)
      {
        var messages = ((IServiceProvider)work).GetService<InfoBuffer>();

        if (messages != null && messages.Count != 0)
          m_notification_delegate(messages);
      }

      return ret;
    }

    internal void SetNotificationDelegate(Action<InfoBuffer> action)
    {
      m_notification_delegate = action;
    }

    private TaskStatus FinishTask(TaskInfo taskInfo, LaunchParameters parameters, IAsyncResult async)
    {
      if (this.SyncWaitingTime > TimeSpan.Zero && async.AsyncWaitHandle.WaitOne(this.SyncWaitingTime))
        return taskInfo.Status;

      if (m_view.Invoker.InvokeRequired)
      {
        m_view.Invoker.Invoke(new Action<LaunchParameters, IAsyncResult>
          (m_view.ShowProgressDialog), new object[] { parameters, async });
      }

      m_view.ShowProgressDialog(parameters, async);

      return taskInfo.Status;
    }
  }
  
  public sealed class TaskInfo : MarshalByRefObject, IProgressIndicator
  {
    private readonly IRunBase m_run_base;
    private readonly LaunchParameters m_parameters;
    private int m_current_progress;
    private string m_current_state;

    private static readonly ILog _log = LogManager.GetLogger(typeof(TaskInfo));

    public TaskInfo(IRunBase runBase, LaunchParameters parameters)
    {
      if (runBase == null)
        throw new ArgumentNullException("runBase");

      if (parameters == null)
        throw new ArgumentNullException("parameters");

      m_run_base = runBase;
      m_parameters = parameters;
      m_run_base.SetProgressIndicator(this);
      this.Status = TaskStatus.Created;
    }

    public IRunBase RunBase
    {
      get { return m_run_base; }
    }

    public TaskStatus Status { get; private set; }

    public Exception Error { get; private set; }

    public bool WasCanceled
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

    public event EventHandler TaskCompleted;

    internal void Run()
    {
      try
      {
        this.Status = TaskStatus.Running;
        m_run_base.Run();
        this.Status = WasCanceled ? TaskStatus.Canceled : TaskStatus.RanToCompletion;
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
        this.TaskCompleted.InvokeSynchronized(this, EventArgs.Empty);
      }
    }

    public void ShowCurrentProgress()
    {
      this.ProgressChanged.InvokeSynchronized(m_run_base, new ProgressChangedEventArgs(m_current_progress, m_current_state));
    }

    void IProgressIndicator.ReportProgress(int percentage, string state)
    {
      m_current_progress = percentage;
      m_current_state = state;
      this.ProgressChanged.InvokeSynchronized(m_run_base, new ProgressChangedEventArgs(percentage, state));
    }
  }

  /// <summary>
  /// Обёртка над токеном отмены задачи, позволяющая делать это через границы доменов приложения
  /// </summary>
  public sealed class CancellationTokenRef : MarshalByRefObject
  {
    private readonly CancellationToken m_token;

    public static readonly CancellationTokenRef None = new CancellationTokenRef(CancellationToken.None);

    public CancellationTokenRef(CancellationToken token)
    {
      m_token = token;
    }

    public bool CanBeCanceled
    {
      get { return m_token.CanBeCanceled; }
    }

    public bool IsCancellationRequested
    {
      get { return m_token.IsCancellationRequested; }
    }

    public void ThrowIfCancellationRequested()
    {
      m_token.ThrowIfCancellationRequested();
    }

    public static implicit operator CancellationTokenRef(CancellationToken token)
    {
      return new CancellationTokenRef(token);
    }
  }

  public interface ITaskManagerView : ISynchronizeProvider
  {
    void ShowProgressDialog(LaunchParameters parameters, IAsyncResult wait);
  }

  public class TaskManagerViewStub : SynchronizeProviderStub, ITaskManagerView
  {
    public void ShowProgressDialog(LaunchParameters parameters, IAsyncResult wait)
    {
      TaskInfo work = wait.AsyncState as TaskInfo;
      try
      {
        
        work.ProgressChanged += HandleProgressChanged;
        work.ShowCurrentProgress();
        wait.AsyncWaitHandle.WaitOne();
      }
      finally
      {
        work.ProgressChanged -= HandleProgressChanged;
      }
    }

    private void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      if (sender.GetType().IsDefined(typeof(PercentNotificationAttribute), false))
        Console.WriteLine("{0,3} %, {1}", e.ProgressPercentage, e.UserState);
      else
        Console.WriteLine(e.UserState);
    }
  }
}
