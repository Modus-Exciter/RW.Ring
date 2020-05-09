using System;
using System.Collections.Generic;
using System.Threading;

namespace Notung.Log
{
  public sealed class LogProcess : IDisposable
  {
    private readonly EventWaitHandle m_signal = new EventWaitHandle(false, EventResetMode.AutoReset);
    private volatile bool m_stop;
    private readonly Queue<LoggingData> m_data = new Queue<LoggingData>();
    private readonly HashSet<ILogAcceptor> m_acceptors = new HashSet<ILogAcceptor>();

    public LogProcess()
    {
      m_acceptors.Add(new FileLogAcceptor());      
      
      new Thread(this.Process).Start();
      new Thread(this.Watch).Start();
    }

    private void Watch()
    {
      while (AppManager.Instance.MainThread.IsAlive)
        Thread.Sleep(512);
      
      this.Dispose();
    }

    private void Process()
    {
      while (m_signal.WaitOne(Timeout.Infinite))
      {
        if (m_stop)
          return;

        LoggingData[] list = null; // Не создаём список без необходимости

        lock (m_data)
        {
          if (m_data.Count > 0)
          {
            list = new LoggingData[m_data.Count];
            int i = 0;

            while (m_data.Count > 0)
              list[i++] = m_data.Dequeue();
          }
        }

        if (list != null)
        {
          lock (m_acceptors)
          {
            foreach (var acceptor in m_acceptors)
              acceptor.WriteLog(list);
          }
        }
      }
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
      lock (m_data)
        m_data.Enqueue(data);

      m_signal.Set();
    }

    public void Dispose()
    {
      m_stop = true;
      m_signal.Set();
    }
  }
}
