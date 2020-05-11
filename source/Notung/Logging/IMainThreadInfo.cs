using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Notung.Log
{
  /// <summary>
  /// Информация о главном потоке, необходимая для работы логов
  /// </summary>
  public interface IMainThreadInfo
  {
    /// <summary>
    /// Ссылка на главный поток приложения, чтобы отслеживать его завершение
    /// </summary>
    Thread MainThread { get; }

    /// <summary>
    /// Надёжна ли информация о главном потоке
    /// </summary>
    bool ReliableThreading { get; }
  }

  internal sealed class SyncLogProcess : LogManager.ILogProcess
  {
    private readonly HashSet<ILogAcceptor> m_acceptors;
    private readonly LoggingData[] m_data = new LoggingData[1];
    private volatile bool m_stop;

    public SyncLogProcess(HashSet<ILogAcceptor> acceptors)
    {
      if (acceptors == null)
        throw new ArgumentNullException("acceptors");

      m_acceptors = acceptors;
    }

    public bool Stopped
    {
      get { return m_stop; }
    }

    public void WaitUntilStop()
    {
      this.Dispose();
    }

    public void WriteMessage(LoggingData data)
    {
      if (m_stop)
        return;

      lock (m_acceptors)
      {
        m_data[0] = data;
        foreach (var acceptor in m_acceptors)
          acceptor.WriteLog(m_data);
      }
    }

    public void Dispose()
    {
      m_stop = true;
    }
  }

  internal sealed class AsyncLogProcess : LogManager.ILogProcess
  {
    private readonly EventWaitHandle m_signal = new EventWaitHandle(false, EventResetMode.AutoReset);
    private readonly Queue<LoggingData> m_data = new Queue<LoggingData>();
    private readonly HashSet<ILogAcceptor> m_acceptors;
    private readonly IMainThreadInfo m_info;
    private readonly Thread m_work_thread;
    private volatile bool m_stop;
    private volatile bool m_shutdown;
    private LoggingData[] m_current_data; // Чтобы не создавать лишних объектов

    public AsyncLogProcess(IMainThreadInfo info, HashSet<ILogAcceptor> acceptors)
    {
      if (info == null)
        throw new ArgumentNullException("info");

      if (acceptors == null)
        throw new ArgumentNullException("acceptors");

      m_info = info;
      m_acceptors = acceptors;

      (m_work_thread = new Thread(this.Process)).Start();
      new Thread(this.Watch).Start();
    }

    public bool Stopped
    {
      get { return m_stop; }
    }

    private void Watch()
    {
      while (m_info.MainThread.IsAlive)
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
      if (m_stop)
        return;

      m_stop = true;
      m_shutdown = true;
      m_signal.Set();
    }
  }
}
