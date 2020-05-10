using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Notung.Log
{
  internal sealed class LogProcess : IDisposable
  {
    private readonly EventWaitHandle m_signal = new EventWaitHandle(false, EventResetMode.AutoReset);
    private readonly Queue<LoggingData> m_data = new Queue<LoggingData>();
    private readonly HashSet<ILogAcceptor> m_acceptors = new HashSet<ILogAcceptor>();    
    private volatile bool m_stop;
    private LoggingData[] m_current_data; // Чтобы не создавать лишних объектов

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
      lock (m_data)
        m_data.Enqueue(data);

      m_signal.Set();
    }

    public void Dispose()
    {
      m_stop = true;
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
