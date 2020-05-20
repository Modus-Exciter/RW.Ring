using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using Notung.ComponentModel;
using Notung.Logging;
using System.Threading;
using System.IO;
using System.Drawing.Imaging;

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
        return m_real_launcher.Run(new CancelableRunBaseCallerWrapper((ICancelableRunBase)runBase, parameters), parameters);
      else
        return m_real_launcher.Run(new RunBaseCallerWrapper(runBase, parameters), parameters);
    }
#if !APP_MANAGER
    public event EventHandler<InfoBufferEventArgs> MessagesRecieved;
#endif
  }

  /// <summary>
  /// Индикатор прогресса для удалённой задачи
  /// </summary>  
  internal interface IProgressIndicator
  {
    /// <summary>
    /// Отображает прогресс выполнения задачи
    /// </summary>
    /// <param name="percentage">Процент выполнения задачи</param>
    /// <param name="state">Текстовое описание состояния задачи</param>
    void ReportProgress(int percentage, string state);
  }

  internal class RunBaseCallerWrapper : MarshalByRefObject, IRunBase, ISynchronizeInvoke, IChangeLaunchParameters, IServiceProvider
  {
    protected readonly IRunBase m_run_base;
    protected LaunchParameters m_parameters;
    private byte[] m_image;

    public RunBaseCallerWrapper(IRunBase runBase, LaunchParameters parameters)
    {
      if (runBase == null)
        throw new ArgumentNullException("runBase");

      m_run_base = runBase;
      m_parameters = parameters;
      if (m_parameters != null && m_parameters.Bitmap != null)
      {
        using (var ms = new MemoryStream())
        {
          parameters.Bitmap.Save(ms, ImageFormat.Png);
          m_image = ms.ToArray();
        }
      }
    }
    
    public virtual void Run()
    {
      m_run_base.ProgressChanged += this.HadnleProgressChanged;
      try
      {
        m_run_base.Run();
      }
      finally
      {
        m_run_base.ProgressChanged -= this.HadnleProgressChanged;
      }
    }

    public bool SupportPercentNotification
    {
      get { return m_run_base.GetType().IsDefined(typeof(PercentNotificationAttribute), false); }
    }

    private void HadnleProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      var indicator = this.ProgressIndicator;

      if (indicator != null)
        indicator.ReportProgress(e.ProgressPercentage, (e.UserState ?? string.Empty).ToString());
    }

    public IProgressIndicator ProgressIndicator { get; set; }

    public event ProgressChangedEventHandler ProgressChanged
    {
      add { }
      remove { }
    }

    public override string ToString()
    {
      var ret = m_run_base.ToString();

      if (object.Equals(ret, m_run_base.GetType().ToString()))
      {
        var dn = m_run_base.GetType().GetCustomAttribute<DisplayNameAttribute>(true);

        if (dn != null && !string.IsNullOrWhiteSpace(dn.DisplayName))
          ret = dn.DisplayName;
      }

      return ret;
    }

    public byte[] BitmapBytes
    {
      get { return m_image; }
    }

    public IAsyncResult BeginInvoke(Delegate method, object[] args)
    {
      var func = new Func<object[], object>(method.DynamicInvoke);
      return func.BeginInvoke(args, null, func);
    }

    public object EndInvoke(IAsyncResult result)
    {
      return ((Func<object[], object>)result.AsyncState).EndInvoke(result);
    }

    public object Invoke(Delegate method, object[] args)
    {
      return method.DynamicInvoke(args);
    }

    public bool InvokeRequired
    {
      get { return false; }
    }

    public void SetLaunchParameters(LaunchParameters parameters)
    {
      if (m_run_base is IChangeLaunchParameters)
        ((IChangeLaunchParameters)m_run_base).SetLaunchParameters(parameters);
    }

    public object GetService(Type serviceType)
    {
      if (m_run_base is IServiceProvider)
        return ((IServiceProvider)m_run_base).GetService(serviceType);

      return null;
    }
  }

  internal class CancelableRunBaseCallerWrapper : RunBaseCallerWrapper, ICancelableRunBase
  {
    private CancellationTokenSource m_token_source;

    public CancelableRunBaseCallerWrapper(ICancelableRunBase runBase, LaunchParameters parameters)
      : base(runBase, parameters)
    {
    }

    CancellationToken ICancelableRunBase.CancellationToken { get; set; }

    public override void Run()
    {
      using (m_token_source = new CancellationTokenSource())
      {
        ((ICancelableRunBase)m_run_base).CancellationToken = m_token_source.Token;
        base.Run();
      }
    }

    public void Cancel()
    {
      m_token_source.Cancel();
    }
  }

  internal class RunBaseProxyWrapper : MarshalByRefObject, IRunBase, IProgressIndicator, IChangeLaunchParameters, IServiceProvider
  {
    protected readonly RunBaseCallerWrapper m_caller;

    public RunBaseProxyWrapper(RunBaseCallerWrapper caller)
    {
      if (caller == null)
        throw new ArgumentNullException("caller");

      m_caller = caller;
    }

    public bool SupportPercentNotification
    {
      get { return m_caller.SupportPercentNotification; }
    }

    public byte[] BitmapBytes
    {
      get { return m_caller.BitmapBytes; }
    }
    
    public void Run()
    {
      m_caller.ProgressIndicator = this;
      m_caller.Run();
    }

    public event ProgressChangedEventHandler ProgressChanged;

    void IProgressIndicator.ReportProgress(int percentage, string state)
    {
      this.ProgressChanged.InvokeSynchronized(this, new ProgressChangedEventArgs(percentage, state));
    }

    public override string ToString()
    {
      return m_caller.ToString();
    }

    public void SetLaunchParameters(LaunchParameters parameters)
    {
      m_caller.SetLaunchParameters(parameters);
    }

    public object GetService(Type serviceType)
    {
      return m_caller.GetService(serviceType);
    }
  }

  internal class CancelableRunBaseProxyWrapper : RunBaseProxyWrapper, ICancelableRunBase
  {
    private CancellationToken m_token;

    public CancelableRunBaseProxyWrapper(CancelableRunBaseCallerWrapper caller) : base(caller) { }
    
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
  }
}
