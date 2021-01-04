using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Notung.ComponentModel;
using Notung.Threading;

namespace Notung.Services
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
  }

  public class OperationLauncher : MarshalByRefObject, IOperationLauncher
  {
    private readonly IOperationLauncherView m_view;

    public OperationLauncher(IOperationLauncherView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      m_view = view;

      if (ConditionalServices.CurrentProcess.SynchronizingObject == null)
        ConditionalServices.CurrentProcess.SynchronizingObject = m_view.Invoker;
    }

    internal OperationLauncher() : this(new TaskManagerViewStub()) { }

    public TimeSpan SyncWaitingTime { get; set; }

    public TaskStatus Run(IRunBase runBase, LaunchParameters parameters = null)
    {
      if (runBase == null)
        throw new ArgumentNullException("runBase");

      var operation = CreateOperation(ref runBase, ref parameters);

      operation.Start();

      var ret = Complete(operation, parameters);

      if (operation.Error != null)
        ConditionalServices.RecieveError(operation.Error);
      else if (runBase is IServiceProvider)
        ConditionalServices.RecieveMessages(((IServiceProvider)runBase).GetService<InfoBuffer>(), OnMessageRecieved);

      return ret;
    }

    private LengthyOperation CreateOperation(ref IRunBase runBase, ref LaunchParameters parameters)
    {
      if (parameters == null)
        parameters = new LaunchParameters();

      if (runBase is CancelableRunBaseCallerWrapper)
        runBase = new CancelableRunBaseProxyWrapper((CancelableRunBaseCallerWrapper)runBase);
      else if (runBase is RunBaseCallerWrapper)
        runBase = new RunBaseProxyWrapper((RunBaseCallerWrapper)runBase);

      parameters.Setup(runBase);

      return new LengthyOperation(runBase);
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

    private void OnMessageRecieved(object e)
    {
      this.MessagesRecieved.InvokeSynchronized(this, new InfoBufferEventArgs((InfoBuffer)e));
    }

    public event EventHandler<InfoBufferEventArgs> MessagesRecieved;
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
        if (e.ProgressPercentage != ProgressPercentage.Unknown)
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