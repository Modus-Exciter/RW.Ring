using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Notung.Logging
{
  partial class LogManager
  {
    private abstract class LogProcess
    {
      protected readonly HashSet<ILogAppender> m_appedners;
      protected volatile bool m_stop;

      public LogProcess(HashSet<ILogAppender> appedners)
      {
        System.Diagnostics.Debug.Assert(appedners != null, "Appenders cannot be null");
        m_appedners = appedners;
      }

      public bool Stopped
      {
        get { return m_stop; }
      }

      public abstract void WaitUntilStop();

      public abstract void WriteMessage(ref LoggingEvent data);

      public abstract void Stop();
    }

    private sealed class SyncLogProcess : LogProcess
    {
      private readonly LoggingEvent[] m_data = new LoggingEvent[1];

      public SyncLogProcess(HashSet<ILogAppender> acceptors) : base(acceptors) { }

      public override void WaitUntilStop()
      {
        this.Stop();
      }

      public override void WriteMessage(ref LoggingEvent data)
      {
        if (m_stop)
          return;

        lock (m_appedners)
        {
          m_data[0] = data;
          foreach (var acceptor in m_appedners)
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
      private EventWaitHandle m_signal;
      private readonly Queue<LoggingEvent> m_data = new Queue<LoggingEvent>();
      private readonly IMainThreadInfo m_info;
      private readonly Thread m_work_thread;
      private readonly object m_close_lock = new object();
      private volatile int m_size = 0;
      private volatile bool m_shutdown;
      private LoggingEvent[] m_current_data; // Чтобы не создавать лишних объектов

      public AsyncLogProcess(IMainThreadInfo info, HashSet<ILogAppender> acceptors) : base(acceptors)
      {
        System.Diagnostics.Debug.Assert(info != null, "Main thread info cannot be null");
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
            this.Set();
        }

        this.Stop();

        m_work_thread.Join();
      }

      private void Process()
      {
        using (m_signal = new EventWaitHandle(false, EventResetMode.AutoReset))
        {
          using (var timer = new Timer(this.Watch, m_work_thread, 256, 128))
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
        if (!m_info.IsAlive)
          this.Stop();
      }

      private void ProcessPendingEvents()
      {
        lock (m_data)
        {
          m_size = m_data.Count;

          if (m_size > 0)
          {
            if (m_current_data == null 
              || m_current_data.Length < m_size 
              || m_current_data.Length / m_size > 0x100)
              m_current_data = new LoggingEvent[m_data.Count];

            int i = 0;

            while (i < m_size)
              m_current_data[i++] = m_data.Dequeue();
          }
          else
            m_current_data = null;
        }

        lock (m_appedners)
        {
          if (m_current_data != null)
            Parallel.ForEach(m_appedners, this.Accept);
        }
      }

      private void Accept(ILogAppender appender)
      {
        appender.WriteLog(new LoggingData(m_current_data, m_size));
      }

      public override void WriteMessage(ref LoggingEvent data)
      {
        if (m_shutdown)
          return;

        lock (m_data)
          m_data.Enqueue(data);

        this.Set();
      }

      private void Set()
      {
        var signal = m_signal;

        if (signal != null)
          signal.Set();
      }

      public override void Stop()
      {
        lock (m_close_lock)
        {
          if (m_stop)
            return;

          m_stop = true;
          m_shutdown = true;

          this.Set();
        }
      }
    }
  }
}