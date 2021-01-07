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

namespace Notung.Services
{
  /// <summary>
  /// Текущий экземпляр приложения
  /// </summary>
  public interface IAppInstance : IMainThreadInfo
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
    /// Ссылка на текущий процесс
    /// </summary>
    Process CurrentProcess { get; }

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

    private static readonly ILog _log = LogManager.GetLogger(typeof(AppInstance));

    public AppInstance(IAppInstanceView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      var args = Environment.GetCommandLineArgs();

      if (args.Length > 0 && Path.GetFullPath(args[0]) == Global.StartupPath)
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

    public Process CurrentProcess
    {
      get { return Global.CurrentProcess; }
    }

    public string StartupPath
    {
      get { return Global.StartupPath; }
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

      m_view.Restart(Global.StartupPath, m_args);
    }

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
        if (new_instance)
        {
          while (!m_terminating && MainThread.IsAlive)
            Thread.Sleep(200);

          mutex.ReleaseMutex();
        }
      }

      if (!new_instance)
      {
        _log.Debug("CreateBlockingMutex(): exit");

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
    }

    private bool SendArgsToPreviousProcess()
    {
      var procList = Process.GetProcessesByName(Global.CurrentProcess.ProcessName);

      foreach (var proc in procList)
      {
        if (proc.Id == Global.CurrentProcess.Id)
          continue;

        if (proc.StartInfo.UserName != Global.CurrentProcess.StartInfo.UserName)
          continue;

        return m_view.SendArgsToProcess(proc, m_args);
      }

      return false;
    }

    private string GetMutexName()
    {
      var path = Global.StartupPath.ToCharArray();

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

  public class ProcessAppInstanceView : EmptySynchronizeProvider, IAppInstanceView
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