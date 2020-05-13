using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Notung.Log
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

      public abstract void WriteMessage(LoggingData data);

      public abstract void Stop();
    }

    private sealed class SyncLogProcess : LogProcess
    {
      private readonly LoggingData[] m_data = new LoggingData[1];

      public SyncLogProcess(HashSet<ILogAcceptor> acceptors) : base(acceptors) { }

      public override void WaitUntilStop()
      {
        this.Stop();
      }

      public override void WriteMessage(LoggingData data)
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

      public override void Stop()
      {
        m_stop = true;
      }
    }

    private sealed class AsyncLogProcess : LogProcess
    {
      private readonly EventWaitHandle m_signal = new EventWaitHandle(false, EventResetMode.AutoReset);
      private readonly Queue<LoggingData> m_data = new Queue<LoggingData>();
      private readonly IMainThreadInfo m_info;
      private readonly Thread m_work_thread;
      private volatile bool m_shutdown;
      private LoggingData[] m_current_data; // Чтобы не создавать лишних объектов

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
        if (m_stop)
          return;

        m_shutdown = true;

        while (m_data.Count > 0)
          m_signal.Set();

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
        int size = 0;

        lock (m_data)
        {
          size = m_data.Count;
          if (size > 0)
          {
            if (m_current_data == null || m_current_data.Length != size)
              m_current_data = new LoggingData[m_data.Count];

            int i = 0;

            while (i < size)
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

      public override void WriteMessage(LoggingData data)
      {
        if (m_shutdown)
          return;

        lock (m_data)
          m_data.Enqueue(data);

        m_signal.Set();
      }

      public override void Stop()
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