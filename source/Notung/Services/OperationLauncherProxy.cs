using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Notung.ComponentModel;
using Notung.Threading;
using System.Drawing.Imaging;

namespace Notung.Services
{
  /// <summary>
  /// Вспомогательный класс для запуска задач из другого домена
  /// </summary>
  internal sealed class OperationLauncherProxy : IOperationLauncher
  {
    private readonly IOperationLauncher m_real_launcher;

    public OperationLauncherProxy(IOperationLauncher realLauncher)
    {
      if (realLauncher == null)
        throw new ArgumentNullException("realLauncher");

      m_real_launcher = realLauncher;
    }

    public TimeSpan SyncWaitingTime
    {
      get { return m_real_launcher.SyncWaitingTime; }
      set { m_real_launcher.SyncWaitingTime = value; }
    }

    public TaskStatus Run(IRunBase runBase, LaunchParameters parameters = null)
    {
      if (runBase is ICancelableRunBase)
        return m_real_launcher.Run(new CancelableRunBaseCallerWrapper((ICancelableRunBase)runBase), parameters);
      else
        return m_real_launcher.Run(new RunBaseCallerWrapper(runBase), parameters);
    }

#if !APP_MANAGER
    event EventHandler<InfoBufferEventArgs> IOperationLauncher.MessagesRecieved { add { } remove { } }
#endif
  }

  internal interface IProgressIndicatorWithCancel : IProgressIndicator
  {
    /// <summary>
    /// Отправляет обёртке сообщение о том, что изменилась возможность отмены задачи
    /// </summary>
    void RaiseCanCancelChanged(EventArgs e);
  }

  /// <summary>
  /// Обёртка над задачей для домена, в котором создана задача
  /// </summary>
  internal class RunBaseCallerWrapper : MarshalByRefObject, IRunBase, ISynchronizeInvoke
  {
    protected readonly IRunBase m_run_base;

    private static readonly ISynchronizeInvoke _invoker = new SynchronizeProviderStub().Invoker;

    public RunBaseCallerWrapper(IRunBase runBase)
    {
      if (runBase == null)
        throw new ArgumentNullException("runBase");

      m_run_base = runBase;
    }
    
    public IProgressIndicatorWithCancel ProgressIndicator { get; set; }

    public string GetCaption()
    {
      return LaunchParameters.GetDefaultCaption(m_run_base);
    }

    public object GetService(Type serviceType)
    {
      if (m_run_base is IServiceProvider)
        return PrepareForMarshaling(((IServiceProvider)m_run_base).GetService(serviceType));

      return null;
    }

    public virtual void Run()
    {
      m_run_base.ProgressChanged += this.HandleProgressChanged;

      try
      {
        m_run_base.Run();
      }
      finally
      {
        m_run_base.ProgressChanged -= this.HandleProgressChanged;
      }
    }

    private void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      var indicator = this.ProgressIndicator;

      if (indicator != null)
        indicator.ReportProgress(e.ProgressPercentage, (e.UserState ?? string.Empty).ToString());
    }

    private static object PrepareForMarshaling(object source)
    {
      if (source is Image)
      {
        using (var ms = new MemoryStream())
        {
          ((Image)source).Save(ms, ImageFormat.Png);

          return ms.ToArray();
        }
      }

      return source;
    }

    event ProgressChangedEventHandler IRunBase.ProgressChanged { add { } remove { } }

    #region ISynchronizeInvoke members

    IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args)
    {
      return _invoker.BeginInvoke(method, args);
    }

    object ISynchronizeInvoke.EndInvoke(IAsyncResult result)
    {
      return _invoker.EndInvoke(result);
    }

    object ISynchronizeInvoke.Invoke(Delegate method, object[] args)
    {
      return method.DynamicInvoke(args);
    }

    bool ISynchronizeInvoke.InvokeRequired
    {
      get { return false; }
    }

    #endregion
  }

  /// <summary>
  /// Обёртка над задачей с поддержкой отмены для домена, в котором создана задача
  /// </summary>
  internal class CancelableRunBaseCallerWrapper : RunBaseCallerWrapper
  {
    private CancellationTokenSource m_token_source;

    public CancelableRunBaseCallerWrapper(ICancelableRunBase runBase) : base(runBase) { }     

    public bool CanCancel
    {
      get { return ((ICancelableRunBase)m_run_base).CanCancel; }
    }

    public override void Run()
    {
      m_token_source = new CancellationTokenSource();
      ((ICancelableRunBase)m_run_base).CanCancelChanged += this.HandleCanCancelChanged;

      try
      {
        ((ICancelableRunBase)m_run_base).CancellationToken = m_token_source.Token;
        base.Run();
      }
      finally
      {
        ((ICancelableRunBase)m_run_base).CanCancelChanged -= this.HandleCanCancelChanged;
        m_token_source.Dispose();
      }
    }

    private void HandleCanCancelChanged(object sender, EventArgs e)
    {
      var indicator = this.ProgressIndicator;

      if (indicator != null)
        indicator.RaiseCanCancelChanged(e);
    }

    public void Cancel()
    {
      m_token_source.Cancel();
    }
  }

  /// <summary>
  /// Обёртка над задачей для домена, в котором работает индикатор прогресса
  /// </summary>
  internal class RunBaseProxyWrapper : MarshalByRefObject, IRunBase, IProgressIndicatorWithCancel, IServiceProvider
  {
    protected readonly RunBaseCallerWrapper m_caller;

    public RunBaseProxyWrapper(RunBaseCallerWrapper caller)
    {
      if (caller == null)
        throw new ArgumentNullException("caller");

      m_caller = caller;
      this.Caption = caller.GetCaption();
    }

    public string Caption { get; private set; }

    public virtual void Run()
    {
      m_caller.ProgressIndicator = this;
      try
      {
        m_caller.Run();
      }
      finally
      {
        m_caller.ProgressIndicator = null;
      }
    }

    public event ProgressChangedEventHandler ProgressChanged;

    public object GetService(Type serviceType)
    {
      var ret = m_caller.GetService(serviceType);

      if (serviceType == typeof(Image) && ret is byte[])
      {
        using (var ms = new MemoryStream((byte[])ret))
          ret = Image.FromStream(ms);
      }

      return ret;
    }

    public void ReportProgress(int percentage, string state)
    {
      this.ProgressChanged.InvokeSynchronized(this, new ProgressChangedEventArgs(percentage, state));
    }

    public virtual void RaiseCanCancelChanged(EventArgs e) { }
  }

  /// <summary>
  /// Обёртка над задачей с поддержкой отмены для домена, в котором работает индикатор прогресса
  /// </summary>
  internal class CancelableRunBaseProxyWrapper : RunBaseProxyWrapper, ICancelableRunBase
  {
    private CancellationToken m_token;

    public CancelableRunBaseProxyWrapper(CancelableRunBaseCallerWrapper caller) : base(caller) { }
      
    public override void RaiseCanCancelChanged(EventArgs e)
    {
      this.CanCancelChanged.InvokeSynchronized(this, e);
    }

    public CancellationToken CancellationToken
    {
      get { return m_token; }
      set
      {
        m_token = value;

        if (m_token.CanBeCanceled)
          m_token.Register(((CancelableRunBaseCallerWrapper)m_caller).Cancel);
      }
    }

    public bool CanCancel
    {
      get { return ((CancelableRunBaseCallerWrapper)m_caller).CanCancel; }
    }

    public event EventHandler CanCancelChanged;
  }
}
