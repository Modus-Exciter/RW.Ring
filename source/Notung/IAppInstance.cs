using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Notung.Data;
using Notung.Logging;
using Notung.Threading;

namespace Notung
{
  public interface IAppInstance : ISynchronizeProvider, IMainThreadInfo
  {
    /// <summary>
    /// Аргументы командной строки
    /// </summary>
    ReadOnlyCollection<string> CommandLineArgs { get; }

    /// <summary>
    /// Декоратор операций, позволяющий их выполнять в однопоточном апартаменте
    /// </summary>
    IOperationWrapper ApartmentWrapper { get; }

    /// <summary>
    /// Находится ли приложение в состоянии перезагрузки
    /// </summary>
    bool Restarting { get; }

    /// <summary>
    /// Находится ли приложение в состоянии завершения
    /// </summary>
    bool Terminating { get; }

    /// <summary>
    /// Путь к файлу, который запустил приложение
    /// </summary>
    string StartupPath { get; }

    /// <summary>
    /// Происходит в момент выхода из приложения
    /// </summary>
    event EventHandler Exit;

    /// <summary>
    /// Этот метод следует вызвать при старте приложения, если можно запускать только одну копию
    /// </summary>
    void AllowOnlyOneInstance();

    /// <summary>
    /// Перезагрузка приложения
    /// </summary>
    void Restart();
  }

  public class AppInstance : MarshalByRefObject, IAppInstance
  {
    private readonly IAppInstanceView m_view;
    private Thread m_main_thread;
    private Thread m_mutex_thread;
    private volatile bool m_terminating;
    private volatile bool m_restarting;
    private readonly ReadOnlyCollection<string> m_args;
    private readonly ApartmentStateOperationWrapper m_apartment_wrapper = new ApartmentStateOperationWrapper();

#if APPLICATION_INFO
    private readonly string m_main_module_file_name = ApplicationInfo.Instance.CurrentProcess.MainModule.FileName;
#else
    private readonly string m_main_module_file_name = GetCurrentProcessModuleName();
#endif

    private static readonly ILog _log = LogManager.GetLogger(typeof(AppInstance));

    public AppInstance(IAppInstanceView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      var args = Environment.GetCommandLineArgs();

      if (args.Length > 0 && Path.GetFullPath(args[0]) == m_main_module_file_name)
      {
        var tmp = new string[args.Length - 1];

        for (int i = 0; i < tmp.Length; i++)
          tmp[i] = args[i + 1];

        args = tmp;
      }

      m_view = view;
      m_args = Array.AsReadOnly(args);

      if (m_view.ReliableThreading)
        LogManager.SetMainThreadInfo(this);
      else
        m_main_thread = Thread.CurrentThread;
    }

    internal AppInstance() : this(new ProcessAppInstanceView()) { }

    public System.ComponentModel.ISynchronizeInvoke Invoker
    {
      get { return m_view.Invoker; }
    }

    public bool IsAlive
    {
      get { return MainThread.IsAlive; }
    }

    public bool ReliableThreading
    {
      get { return m_view.ReliableThreading; }
    }

    public ReadOnlyCollection<string> CommandLineArgs
    {
      get { return m_args; }
    }

    public IOperationWrapper ApartmentWrapper
    {
      get { return m_apartment_wrapper; }
    }

    public bool Restarting
    {
      get { return m_restarting; }
    }

    public bool Terminating
    {
      get { return m_terminating; }
    }

    public string StartupPath
    {
      get { return m_main_module_file_name; }
    }

    public event EventHandler Exit
    {
      add { AppDomain.CurrentDomain.ProcessExit += value; }
      remove { AppDomain.CurrentDomain.ProcessExit -= value; }
    }

    public void AllowOnlyOneInstance()
    {
      if (m_mutex_thread != null)
        return;

      m_mutex_thread = new Thread(CreateBlockingMutex);
      m_mutex_thread.SetApartmentState(ApartmentState.STA);
      m_mutex_thread.Start();
    }

    public void Restart()
    {
      if (m_terminating)
        return;

      _log.Debug("Restart()");

      LogManager.Stop();

      m_terminating = true;
      m_restarting = true;

      if (m_mutex_thread != null)
        m_mutex_thread.Join();

      m_view.Restart(m_main_module_file_name, m_args);
    }

#if !APPLICATION_INFO
    private static string GetCurrentProcessModuleName()
    {
      using (var process = Process.GetCurrentProcess())
        return process.MainModule.FileName;
    }
#endif

    private Thread MainThread
    {
      get
      {
        if (m_main_thread != null)
          return m_main_thread;

        if (m_view.Invoker.InvokeRequired)
          return m_main_thread = (Thread)m_view.Invoker.Invoke(
            new Func<Thread>(() => Thread.CurrentThread), ArrayExtensions.Empty<object>());
        else
          return Thread.CurrentThread;
      }
    }

    private void CreateBlockingMutex()
    {
      if (m_terminating)
        return;

      bool new_instance;
      using (var mutex = new Mutex(true, GetMutexName(), out new_instance))
      {
        if (!new_instance)
        {
          _log.Debug("CreateBlockingMutex(): exit");
          mutex.Close();

          if (m_args.Count > 0 && m_view.SupportSendingArgs)
          {
            if (this.SendArgsToPreviousProcess())
              _log.Debug("CreateBlockingMutex(): message to previous application copy sent");
            else
              _log.Debug("CreateBlockingMutex(): previous application copy not responding");
          }

          m_terminating = true;

          LogManager.Stop();
          Environment.Exit(2);
        }
        else
        {
          while (!m_terminating && MainThread.IsAlive)
            Thread.Sleep(200);

          mutex.ReleaseMutex();
        }
      }
    }

    private bool SendArgsToPreviousProcess()
    {
#if APPLICATION_INFO
      var currentProc = ApplicationInfo.Instance.CurrentProcess;
#else
      using (var currentProc = Process.GetCurrentProcess())
      {
#endif
      var procList = Process.GetProcessesByName(currentProc.ProcessName);
      foreach (var proc in procList)
      {
        if (proc.Id == currentProc.Id)
          continue;

        if (proc.StartInfo.UserName != currentProc.StartInfo.UserName)
          continue;

        return m_view.SendArgsToProcess(proc, m_args);
      }
#if !APPLICATION_INFO
      }
#endif

      return false;
    }

    private string GetMutexName()
    {
      var path = m_main_module_file_name.ToCharArray();

      for (int i = 0; i < path.Length; i++)
      {
        if (path[i] == '\\' || path[i] == '/' || path[i] == ':')
          path[i] = '_';
      }

      return new string(path);
    }
  }

  public interface IAppInstanceView : ISynchronizeProvider
  {
    bool ReliableThreading { get; }

    void Restart(string startPath, IList<string> args);

    bool SupportSendingArgs { get; }

    bool SendArgsToProcess(Process previous, IList<string> args);
  }

  public class ProcessAppInstanceView : SynchronizeProviderStub, IAppInstanceView
  {
    private static string CreatePathArgs(IList<string> args)
    {
      StringBuilder sb = new StringBuilder();

      bool first = true;

      foreach (var arg in args)
      {
        if (first)
          first = false;
        else
          sb.Append(" ");

        if (arg.Contains(' ') || arg.Contains('\t'))
          sb.AppendFormat("\"{0}\"", arg);
        else
          sb.Append(arg);
      }

      return sb.ToString();
    }

    public bool ReliableThreading
    {
      get { return false; }
    }

    public bool SupportSendingArgs
    {
      get { return false; }
    }

    public bool SendArgsToProcess(Process previous, IList<string> args)
    {
      return false;
    }

    public void Restart(string startPath, IList<string> args)
    {
      if (args == null)
        throw new ArgumentNullException("args");

      using (Process.Start(startPath, CreatePathArgs(args))) { }

      Environment.Exit(0);
    }
  }
}
