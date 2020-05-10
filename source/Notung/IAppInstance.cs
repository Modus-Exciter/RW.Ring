using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Threading;
using Notung.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Text;
using Notung.Log;

namespace Notung
{
  public interface IAppInstance : ISynchronizeProvider
  {
    Thread MainThread { get; }

    bool IsUserAnAdmin { get; }

    ReadOnlyCollection<string> CommandLineArgs { get; }

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
    /// Путь к директории, из которой стартовало приложение
    /// </summary>
    string StartupPath { get; }

    event EventHandler Exit;

    /// <summary>
    /// Создание объекта блокировки приложения для предотвращения запуска нескольких копий
    /// </summary>
    void CreateBlockingMutex();

    /// <summary>
    /// Перезагрузка приложения
    /// </summary>
    void Restart();
  }

  public class AppInstance : IAppInstance
  {
    private readonly IAppInstanceView m_view;
    private Thread m_main_thread;
    private Thread m_mutex_thread;
    private volatile bool m_terminating;
    private volatile bool m_restarting;
    private readonly bool m_is_admin;
    private readonly string m_startup_path;
    private readonly ReadOnlyCollection<string> m_args;
    private readonly ApartmentStateOperationWrapper m_apartment_wrapper = new ApartmentStateOperationWrapper();
    private readonly string m_main_module_file_name = GetMainModuleFileName();
    private readonly AppDomain m_main_domain;
    
    private static readonly ILog _log = LogManager.GetLogger(typeof(AppInstance));

    public AppInstance(IAppInstanceView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      m_view = view;
      m_args = Array.AsReadOnly(Environment.GetCommandLineArgs());
      m_is_admin = m_apartment_wrapper.Invoke(new Func<bool>(APIHelper.IsUserAnAdmin)); // TODO: уточнить, можно ли изменить это в ходе работы программы

      if (m_view is ProcessAppInstanceView)
        m_main_thread = Thread.CurrentThread;

      m_startup_path = Path.GetDirectoryName(m_main_module_file_name);
      m_main_domain = AppDomain.CurrentDomain;
    }
    
    public System.ComponentModel.ISynchronizeInvoke Invoker
    {
      get { return m_view.Invoker; }
    }

    public Thread MainThread
    {
      get
      {
        if (m_main_thread != null)
          return m_main_thread;

        if (m_view.Invoker.InvokeRequired)
          return m_main_thread = (Thread)m_view.Invoker.Invoke(
            new Func<Thread>(APIHelper.GetCurrentThread), new object[0]);
        else
          return Thread.CurrentThread;
      }
    }

    public bool IsUserAnAdmin 
    {
      get { return m_is_admin; }
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
      get { return m_startup_path; }
    }

    public event EventHandler Exit
    {
      add { AppDomain.CurrentDomain.ProcessExit += value; }
      remove { AppDomain.CurrentDomain.ProcessExit -= value; }
    }

    public void CreateBlockingMutex()
    {
      m_mutex_thread = new Thread(MutexBlockingThread);
      m_mutex_thread.SetApartmentState(ApartmentState.STA);
      m_mutex_thread.Start();
    }

    public void Restart()
    {
      if (m_terminating)
        return;

      m_terminating = true;
      m_restarting = true;

      if (m_mutex_thread != null)
        m_mutex_thread.Join();

      m_view.Restart(m_args);
    }

    private void MutexBlockingThread()
    {
      if (m_terminating)
        return;

      bool new_instance;
      using (var mutex = new Mutex(true, GetMutexName(), out new_instance))
      {
        if (!new_instance)
        {
          _log.Debug("CreateBlockingMutexImpl(): exit");         
          mutex.Close();

          // TODO: отправка сообщения другой копии приложения
          if (m_args.Count > 0)
          {
          }

          m_terminating = true;

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

    private static string GetMainModuleFileName()
    {
      using (var process = Process.GetCurrentProcess())
      {
        return process.MainModule.FileName;
      }
    }

    static private class APIHelper
    {
      [DllImport("shell32.dll")]
      public static extern bool IsUserAnAdmin();

      public static Thread GetCurrentThread()
      {
        return Thread.CurrentThread;
      }
    }
  }

  public interface IAppInstanceView : ISynchronizeProvider
  {
    void Restart(IList<string> args);
  }

  public class ProcessAppInstanceView : SynchronizeProviderStub, IAppInstanceView
  {
    private readonly string m_main_module_file_name = GetMainModuleFileName();

    private static string GetMainModuleFileName()
    {
      using (var process = Process.GetCurrentProcess())
      {
        return process.MainModule.FileName;
      }
    }

    #region IAppInstanceView Members

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

    public string StartupPath
    {
      get { return Path.GetDirectoryName(m_main_module_file_name); }
    }

    public void Restart(IList<string> args)
    {
      if (args == null) throw new ArgumentNullException("args");

      using (Process.Start(m_main_module_file_name, CreatePathArgs(args))) { }

      Environment.Exit(0);
    }

    #endregion

  }
}
