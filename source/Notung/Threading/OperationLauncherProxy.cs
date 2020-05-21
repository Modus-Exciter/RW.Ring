﻿using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Notung.ComponentModel;

namespace Notung.Threading
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

  /// <summary>
  /// Индикатор прогресса задачи, выполняющейся в другом домене
  /// </summary>
  internal interface IProgressIndicator
  {
    /// <summary>
    /// Отображает прогресс выполнения задачи
    /// </summary>
    /// <param name="percentage">Процент выполнения задачи</param>
    /// <param name="state">Текстовое описание состояния задачи. Object нельзя, потому что Remoting</param>
    void ReportProgress(int percentage, string state);

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
    
    public bool SupportPercentNotification
    {
      get { return m_run_base.GetType().IsDefined(typeof(PercentNotificationAttribute), false); }
    }

    public IProgressIndicator ProgressIndicator { get; set; }

    public string GetCaption()
    {
      return LaunchParameters.GetDefaultCaption(m_run_base);
    }

    public object GetService(Type serviceType)
    {
      if (m_run_base is IServiceProvider)
        return ((IServiceProvider)m_run_base).GetService(serviceType);

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

    public event ProgressChangedEventHandler ProgressChanged { add { } remove { } }

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
  internal class RunBaseProxyWrapper : MarshalByRefObject, IRunBase, IProgressIndicator, IServiceProvider
  {
    protected readonly RunBaseCallerWrapper m_caller;

    public RunBaseProxyWrapper(RunBaseCallerWrapper caller)
    {
      if (caller == null)
        throw new ArgumentNullException("caller");

      m_caller = caller;

      this.SupportsPercentNotification = caller.SupportPercentNotification;
      this.Caption = caller.GetCaption();
    }

    public bool SupportsPercentNotification { get; private set; }

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
      return m_caller.GetService(serviceType);
    }

    void IProgressIndicator.ReportProgress(int percentage, string state)
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