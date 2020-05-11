using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Notung.Log
{
  /// <summary>
  /// Базовые параметры текущего процесса, необходимые для работы логов
  /// </summary>
  public interface ILogBaseSettings
  {
    /// <summary>
    /// Ссылка на главный поток приложения, чтобы отслеживать его завершение
    /// </summary>
    Thread MainThread { get; }

    /// <summary>
    /// Директория, в которой программа хранит свои данные
    /// </summary>
    string WorkingPath { get; }
  }
  
  internal sealed class LogProcess : IDisposable
  {
    private readonly EventWaitHandle m_signal = new EventWaitHandle(false, EventResetMode.AutoReset);
    private readonly Queue<LoggingData> m_data = new Queue<LoggingData>();
    private readonly HashSet<ILogAcceptor> m_acceptors = new HashSet<ILogAcceptor>();
    private readonly ILogBaseSettings m_settings;
    private readonly Thread m_work_thread;
    private volatile bool m_stop;
    private volatile bool m_shutdown;
    private LoggingData[] m_current_data; // Чтобы не создавать лишних объектов

    public LogProcess(ILogBaseSettings settings)
    {
      if (settings == null)
        throw new ArgumentNullException("settings");

      m_settings = settings;

      m_acceptors.Add(new FileLogAcceptor(System.IO.Path.Combine(m_settings.WorkingPath, "Logs")));
      
      (m_work_thread = new Thread(this.Process)).Start();
      new Thread(this.Watch).Start();
    }

    public bool Stopped
    {
      get { return m_stop; }
    }

    private void Watch()
    {
      while (m_settings.MainThread.IsAlive)
        Thread.Sleep(512);
      
      this.Dispose();
    }

    public void WaitUntilStop()
    {
      if (m_stop)
        return;

      m_shutdown = true;

      while (m_data.Count > 0)
        m_signal.Set();

      this.Dispose();

      m_work_thread.Join();
    }

    private void Process()
    {
      using (m_signal)
      {
        while (m_signal.WaitOne(Timeout.Infinite))
        {
          if (m_stop)
            return;

          this.ProcessPendingEvents();
        }
      }
    }

    private void ProcessPendingEvents()
    {
      int size = 0;

      lock (m_data)
      {
        size = m_data.Count;
        if (size > 0)
        {
          if (m_current_data == null || m_current_data.Length != size)
            m_current_data = new LoggingData[m_data.Count];

          int i = 0;

          while (m_data.Count > 0)
            m_current_data[i++] = m_data.Dequeue();
        }
        else
          m_current_data = null;
      }

      lock (m_acceptors)
      {
        if (m_current_data != null)
          Parallel.ForEach(m_acceptors, this.Accept);
      }
    }

    private void Accept(ILogAcceptor acceptor)
    {
      acceptor.WriteLog(m_current_data);
    }

    public void AddAcceptor(ILogAcceptor acceptor)
    {
      if (acceptor == null)
        throw new ArgumentNullException("acceptor");

      lock (m_acceptors)
        m_acceptors.Add(acceptor);
    }

    public void WriteMessage(LoggingData data)
    {
      if (m_shutdown)
        return;
      
      lock (m_data)
        m_data.Enqueue(data);

      m_signal.Set();
    }

    public void Dispose()
    {
      m_stop = true;
      m_shutdown = true;
      m_signal.Set();

      lock (m_acceptors)
      {
        foreach (var acceptor in m_acceptors)
        {
          var disposable = acceptor as IDisposable;

          if (disposable != null)
            disposable.Dispose();
        }

        m_acceptors.Clear();
      }
    }
  }
}
