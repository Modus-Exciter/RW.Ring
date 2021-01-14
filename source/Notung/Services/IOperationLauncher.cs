using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Notung.ComponentModel;
using Notung.Threading;

namespace Notung.Services
{
  /// <summary>
  /// Управляет задачами, которые можно запустить в диалоге с индикатором прогресса
  /// </summary>
  public interface IOperationLauncher : ISynchronizeProvider
  {
    /// <summary>
    /// Время ожидания до показа индикатора прогресса
    /// </summary>
    TimeSpan SyncWaitingTime { get; set; }

    /// <summary>
    /// Контекст синхронизации
    /// </summary>
    SynchronizationContext SynchronizationContext { get; }

    /// <summary>
    /// Запуск задачи на выполнение
    /// </summary>
    /// <param name="runBase">Задача, которую требуется выполнить</param>
    /// <param name="closeOnFinish">Закрывать ли диалог с прогрессом операции по завершении</param>
    /// <returns>Результат выполнения задачи</returns>
    TaskStatus Run(IRunBase runBase);

    /// <summary>
    /// Запуск задачи на выполнение
    /// </summary>
    /// <param name="runBase">Задача, которую требуется выполнить</param>
    /// <param name="parameters">Дополнительные параметры отображения задачи</param>
    /// <returns>Результат выполнения задачи</returns>
    TaskStatus Run(IRunBase runBase, LaunchParameters parameters);
  }

  /// <summary>
  /// Базовая реализация IOperationLauncher
  /// </summary>
  public class OperationLauncher : MarshalByRefObject, IOperationLauncher
  {
    private readonly IOperationLauncherView m_view;
    private readonly Action m_set_context;
    private SynchronizationContext m_context;

    /// <summary>
    /// Инициализациа нового сервиса по управлению задачами
    /// </summary>
    /// <param name="view">Представление для сервивса</param>
    public OperationLauncher(IOperationLauncherView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      m_view = view;
      m_set_context = this.SetContext;
    }

    internal OperationLauncher() : this(new TaskManagerViewStub()) { }

    public TimeSpan SyncWaitingTime { get; set; }

    public ISynchronizeInvoke Invoker
    {
      get { return m_view.Invoker; }
    }

    public SynchronizationContext SynchronizationContext
    {
      get
      {
        if (m_context != null)
          return m_context;

        if (m_view.Invoker.InvokeRequired)
          m_view.Invoker.Invoke(m_set_context, Global.EmptyArgs);
        else
          this.SetContext();

        return m_context;
      }
    }

    public TaskStatus Run(IRunBase runBase)
    {
      if (runBase == null)
        throw new ArgumentNullException("runBase");

      return this.Run(WrapIfRemote(runBase), true);
    }

    public TaskStatus Run(IRunBase runBase, LaunchParameters parameters)
    {
      if (runBase == null)
        throw new ArgumentNullException("runBase");

      if (parameters == null)
        return this.Run(WrapIfRemote(runBase), true);

      using (var wrapper = parameters.Wrap(WrapIfRemote(runBase)))
        return this.Run(wrapper, parameters.CloseOnFinish);
    }

    #region Implementation ------------------------------------------------------------------------

    private IRunBase WrapIfRemote(IRunBase runBase)
    {
      if (runBase is CancelableRunBaseCallerWrapper)
        runBase = new CancelableRunBaseProxyWrapper((CancelableRunBaseCallerWrapper)runBase);
      else if (runBase is RunBaseCallerWrapper)
        runBase = new RunBaseProxyWrapper((RunBaseCallerWrapper)runBase);

      return runBase;
    }

    private TaskStatus Run(IRunBase runBase, bool closeOnFinish)
    {
      using (var operation = new LengthyOperation(runBase, m_view.Invoker))
      {
        operation.Start();

        var ret = Complete(operation, closeOnFinish);

        if (operation.Error != null)
          m_view.ShowError(operation.Error);
        else if (runBase is IServiceProvider)
        {
          var messages = ((IServiceProvider)runBase).GetService<InfoBuffer>();

          if (messages != null && messages.Count != 0)
            m_view.ShowMessages(messages);
        }

        return ret;
      }
    }

    private TaskStatus Complete(LengthyOperation operation, bool closeOnFinish)
    {
      if (this.SyncWaitingTime > TimeSpan.Zero && operation.Wait(this.SyncWaitingTime))
        return operation.Status;

      if (m_view.Invoker.InvokeRequired)
      {
        m_view.Invoker.Invoke(new Action<LengthyOperation, bool>
          (m_view.ShowProgressDialog), new object[] { operation, closeOnFinish });
      }
      else
        m_view.ShowProgressDialog(operation, closeOnFinish);

      return operation.Status;
    }

    private void SetContext()
    {
      if (m_context == null)
        m_context = System.Threading.SynchronizationContext.Current ?? new SynchronizationContext();
    }

    #endregion
  }

  public interface IOperationLauncherView : ISynchronizeProvider
  {
    void ShowProgressDialog(LengthyOperation task, bool closeOnFinish);

    void ShowError(Exception error);

    void ShowMessages(InfoBuffer messages);
  }

  public class TaskManagerViewStub : EmptySynchronizeProvider, IOperationLauncherView
  {
    private class ProgressInConsole
    {
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

    public void ShowProgressDialog(LengthyOperation operation, bool closeOnFinish)
    {
      var progress = new ProgressInConsole();

      operation.ProgressChanged += progress.HandleProgressChanged;
      operation.Completed += progress.HandleTaskCompleted;
      operation.Wait();
    }

    public void ShowError(Exception error)
    {
      Console.WriteLine(error);
    }

    public void ShowMessages(InfoBuffer messages)
    {
      foreach (var message in messages)
        Console.WriteLine("{0}: {1}", message.Level, message.Message);
    }
  }
}