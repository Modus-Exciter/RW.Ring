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
    private readonly ISynchronizeInvoke m_invoker;
    private readonly Action m_can_cancel_changed;
    private readonly Action m_task_completed;
    private readonly object m_lock = new object();
    private IAsyncResult m_operation;
    private CancellationTokenSource m_cancel_source;
    private ProgressChangedEventArgs m_current_args;

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
      m_current_args = new ProgressChangedEventArgs(ProgressPercentage.Unknown, string.Empty);
      m_invoker = invoker ?? EmptySynchronizeProvider.Default;
      m_task_completed = new Action(this.OnTaskCompleted);

      if (runBase is ICancelableRunBase)
        m_can_cancel_changed = new Action(this.OnCanCancelChanged);

      this.Status = TaskStatus.Created;
    }

    /// <summary>
    /// Статус выполняющейся задачи
    /// </summary>
    public TaskStatus Status { get; private set; }

    /// <summary>
    /// Ошибка, возникшая в ходе выполнения задачи
    /// </summary>
    public Exception Error { get; private set; }

    /// <summary>
    /// Можно ли отменить задачу
    /// </summary>
    public bool CanCancel
    {
      get { return m_run_base is ICancelableRunBase && ((ICancelableRunBase)m_run_base).CanCancel; }
    }

    /// <summary>
    /// Была ли задача отменена
    /// </summary>
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

    /// <summary>
    /// Происходит, когда задача оповещает о своём прогрессе выполнения
    /// </summary>
    public event ProgressChangedEventHandler ProgressChanged;

    /// <summary>
    /// Происходит, когда изменяется возможность выполнения задачи
    /// </summary>
    public event EventHandler CanCancelChanged;

    /// <summary>
    /// Происходит, когда задача завершается
    /// </summary>
    public event EventHandler Completed;

    /// <summary>
    /// Запуск задачи на выполнение
    /// </summary>
    public void Start()
    {
      lock (m_lock)
      {
        if (m_operation != null)
          throw new InvalidOperationException();

        m_operation = new Action(this.Run).BeginInvoke(CloseHandle, this);
      }
    }

    /// <summary>
    /// Ожидание завершения задачи
    /// </summary>
    public void Wait()
    {
      var operation = m_operation;

      if (operation == null)
        return;

      operation.AsyncWaitHandle.WaitOne();
    }

    /// <summary>
    /// Ожидание завершения задачи в течение указанного времени
    /// </summary>
    /// <param name="duration">Максимальное время ожидания</param>
    /// <returns>True, если задача завершилась к моменту окончания ожидания. Иначе, false</returns>
    public bool Wait(TimeSpan duration)
    {
      var operation = m_operation;

      if (operation == null)
        return true;

      return operation.AsyncWaitHandle.WaitOne(duration);
    }

    /// <summary>
    /// Показывает текущий прогресс выполнения задачи
    /// </summary>
    public void ShowCurrentProgress()
    {
      this.HandleProgressChanged(this, m_current_args);
    }

    /// <summary>
    /// Получает объект, через который можно отменить задачу
    /// </summary>
    /// <returns>Объект отмены задачи</returns>
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

    /// <summary>
    /// Получает заголовок задачи
    /// </summary>
    /// <returns>Заголовок задачи</returns>
    public string GetWorkCaption()
    {
      if (m_run_base is RunBaseProxyWrapper)
        return ((RunBaseProxyWrapper)m_run_base).Caption;
      else
        return LaunchParameters.GetDefaultCaption(m_run_base);
    }

    /// <summary>
    /// Получает изображение, связанное с задачей
    /// </summary>
    /// <returns>Изображение, связанное с задачей</returns>
    public Image GetWorkImage()
    {
      IServiceProvider provider = m_run_base as IServiceProvider;
      return provider != null ? provider.GetService<Image>() : null;
    }

    /// <summary>
    /// Очищает ресурсы после выполнения задачи
    /// </summary>
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

        ThreadTracker.RemoveThread(Thread.CurrentThread);

        this.HandleCompleted();
      }
    }

    private void HandleCompleted()
    {
      if (m_invoker.InvokeRequired)
        m_invoker.BeginInvoke(m_task_completed, Global.EmptyArgs);
      else
        this.OnTaskCompleted();
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
      var current = m_current_args;
      var handler = this.ProgressChanged;

      if (e.ProgressPercentage != current.ProgressPercentage 
        || !object.Equals(e.UserState, current.UserState))
        m_current_args = e;

      if (handler != null)
      {
        if (m_invoker.InvokeRequired)
          m_invoker.BeginInvoke(handler, new object[] { this, e });
        else
          handler(this, e);
      }
    }

    private void HandleCanCancelChanged(object sender, EventArgs e)
    {
      if (m_invoker.InvokeRequired)
        m_invoker.BeginInvoke(m_can_cancel_changed, Global.EmptyArgs);
      else
        this.OnCanCancelChanged();
    }

    private void OnCanCancelChanged()
    {
      this.CanCancelChanged.InvokeIfSubscribed(this, EventArgs.Empty);
    }

    private void OnTaskCompleted()
    {
      this.Completed.InvokeIfSubscribed(this, EventArgs.Empty);
    }
  }
}