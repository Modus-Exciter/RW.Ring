using System;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using Notung.Logging;
using Notung.ComponentModel;

namespace Notung.Threading
{
  public interface ITaskManager 
  {
    TimeSpan SyncWaitingTime { get; set; }

    TaskStatus Run(IRunBase runBase, LaunchParameters parameters);
  }

  public class TaskManager : MarshalByRefObject, ITaskManager
  {
    private readonly ITaskManagerView m_view;

    public TaskManager(ITaskManagerView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      m_view = view;
    }

    internal TaskManager() : this(new TaskManagerViewStub()) { }

    public TaskStatus Run(IRunBase work, LaunchParameters parameters)
    {
      if (work == null)
        throw new ArgumentNullException("runBase");

      if (parameters == null)
        parameters = new LaunchParameters(work);

      var taskInfo = new TaskInfo(work, parameters);
      var action = new Action(taskInfo.Run);
      var async = action.BeginInvoke(null, taskInfo);

      if (this.SyncWaitingTime > TimeSpan.Zero && async.AsyncWaitHandle.WaitOne(this.SyncWaitingTime))
        return taskInfo.Status;

      m_view.ShowProgressDialog(taskInfo, parameters, async);

      return taskInfo.Status;
    }

    public TimeSpan SyncWaitingTime { get; set; }
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

    public void Run()
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
        if (m_parameters != null)
          m_parameters.Finish();

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

  public sealed class Cancellation : MarshalByRefObject
  {
    private readonly CancellationToken m_token;

    public Cancellation(CancellationToken token)
    {
      m_token = token;
    }

    public bool IsCancellationRequested
    {
      get { return m_token.IsCancellationRequested; }
    }

    public void ThrowIfCancellationRequested()
    {
      m_token.ThrowIfCancellationRequested();
    }

    public static implicit operator Cancellation(CancellationToken token)
    {
      return new Cancellation(token);
    }
  }


  public interface ITaskManagerView : ISynchronizeProvider
  {
    bool ShowProgressDialog(TaskInfo work, LaunchParameters parameters, IAsyncResult wait);
  }

  public class TaskManagerViewStub : SynchronizeProviderStub, ITaskManagerView
  {
    public bool ShowProgressDialog(TaskInfo work, LaunchParameters parameters, IAsyncResult wait)
    {
      try
      {
        work.ProgressChanged += HandleProgressChanged;
        work.ShowCurrentProgress();
        wait.AsyncWaitHandle.WaitOne();
        return work.Status == TaskStatus.RanToCompletion;
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

  public static class RunBaseExtensions
  {
    public static bool Run(this ITaskManager context, IRunBase runBase)
    {
      return context.Run(runBase, new LaunchParameters(runBase)) == TaskStatus.RanToCompletion;
    }
  }
}
