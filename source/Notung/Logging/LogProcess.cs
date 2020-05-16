using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Notung.Logging
{
  partial class LogManager
  {
    private abstract class LogProcess
    {
      protected readonly HashSet<ILogAcceptor> m_acceptors;
      protected volatile bool m_stop;

      public LogProcess(HashSet<ILogAcceptor> acceptors)
      {
#if DEBUG
        if (acceptors == null)
          throw new ArgumentNullException("acceptors");
#endif
        m_acceptors = acceptors;
      }

      public bool Stopped
      {
        get { return m_stop; }
      }

      public abstract void WaitUntilStop();

      public abstract void WriteMessage(LoggingEvent data);

      public abstract void Stop();
    }

    private sealed class SyncLogProcess : LogProcess
    {
      private readonly LoggingEvent[] m_data = new LoggingEvent[1];

      public SyncLogProcess(HashSet<ILogAcceptor> acceptors) : base(acceptors) { }

      public override void WaitUntilStop()
      {
        this.Stop();
      }

      public override void WriteMessage(LoggingEvent data)
      {
        if (m_stop)
          return;

        lock (m_acceptors)
        {
          m_data[0] = data;
          foreach (var acceptor in m_acceptors)
            acceptor.WriteLog(new LoggingData(m_data, 1));
        }
      }

      public override void Stop()
      {
        m_stop = true;
      }
    }

    private sealed class AsyncLogProcess : LogProcess
    {
      private readonly EventWaitHandle m_signal = new EventWaitHandle(false, EventResetMode.AutoReset);
      private readonly Queue<LoggingEvent> m_data = new Queue<LoggingEvent>();
      private readonly IMainThreadInfo m_info;
      private readonly object m_close_lock = new object();
      private volatile int m_size = 0;
      private readonly Thread m_work_thread;
      private volatile bool m_shutdown;
      private LoggingEvent[] m_current_data; // Чтобы не создавать лишних объектов

      public AsyncLogProcess(IMainThreadInfo info, HashSet<ILogAcceptor> acceptors)
        : base(acceptors)
      {
#if DEBUG
        if (info == null)
          throw new ArgumentNullException("info");
#endif
        m_info = info;

        (m_work_thread = new Thread(this.Process)).Start();
      }

      public override void WaitUntilStop()
      {
        lock (m_close_lock)
        {
          if (m_stop)
            return;

          m_shutdown = true;

          while (m_data.Count > 0)
            m_signal.Set();
        }

        this.Stop();

        m_work_thread.Join();
      }

      private void Process()
      {
        using (m_signal)
        {
          using (var timer = new Timer(Watch, m_work_thread, 256, 128))
          {
            while (m_signal.WaitOne(Timeout.Infinite))
            {
              if (m_stop)
                return;

              this.ProcessPendingEvents();
            }
          }
        }
      }

      private void Watch(object state)
      {
        if (!m_info.MainThread.IsAlive)
          this.Stop();
      }

      private void ProcessPendingEvents()
      {
        lock (m_data)
        {
          m_size = m_data.Count;
          if (m_size > 0)
          {
            if (m_current_data == null || m_current_data.Length < m_size 
              || m_current_data.Length / m_size > 0x100)
              m_current_data = new LoggingEvent[m_data.Count];

            int i = 0;

            while (i < m_size)
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
        acceptor.WriteLog(new LoggingData(m_current_data, m_size));
      }

      public override void WriteMessage(LoggingEvent data)
      {
        if (m_shutdown)
          return;

        lock (m_data)
          m_data.Enqueue(data);

        m_signal.Set();
      }

      public override void Stop()
      {
        lock (m_close_lock)
        {
          if (m_stop)
            return;

          m_stop = true;
          m_shutdown = true;
          m_signal.Set();
        }
      }
    }
  }
}