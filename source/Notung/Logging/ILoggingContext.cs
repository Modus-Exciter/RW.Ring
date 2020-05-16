using System;
using System.Collections.Generic;
using Notung.Threading;
using SysThread = System.Threading.Thread;
using Notung.Properties;

namespace Notung.Logging
{
  public interface ILoggingContext
  {
    object this[string key] { get; set; }

    bool Contains(string key);
  }

  public interface IThreadLoggingContext : ILoggingContext
  {
    SysThread CurrentThread { get; }
  }

  public static class LoggingContext
  {
    [ThreadStatic]
    private static ThreadContextData _thread;
    private static readonly GlobalContextData _global = new GlobalContextData();
    private static readonly HashSet<string> _forbidden = new HashSet<string>
    {
      "Source",
      "Date",
      "Message",
      "Level",
      "Data",
      "Process",
      "Thread"
    };

    public static ILoggingContext Global
    {
      get { return _global; }
    }

    public static IThreadLoggingContext Thread
    {
      get
      {
        if (_thread == null)
          _thread = new ThreadContextData(true);

        return _thread;
      }
    }

    private class ThreadContextData : IThreadLoggingContext
    {
      private readonly Dictionary<string, object> m_data = new Dictionary<string, object>();
      private readonly SysThread m_thread = SysThread.CurrentThread;
      private readonly bool m_check_thread;

      public ThreadContextData(bool checkThread)
      {
        m_check_thread = checkThread;
      }

      public object this[string key]
      {
        get
        {
          if (string.IsNullOrWhiteSpace(key))
            return null;

          object ret;
          m_data.TryGetValue(key, out ret);
          return ret;
        }
        set
        {
          if (m_check_thread && SysThread.CurrentThread != m_thread)
            throw new InvalidOperationException(Resources.THREAD_CONTEXT_MISMATCH);

          if (_forbidden.Contains(key))
            throw new ArgumentException(string.Format(Resources.CONTEXT_KEY_RESERVED, key));

          if (!string.IsNullOrWhiteSpace(key))
            m_data[key] = value;
        }
      }

      public bool Contains(string key)
      {
        if (string.IsNullOrWhiteSpace(key))
          return false;

        return m_data.ContainsKey(key);
      }

      public SysThread CurrentThread
      {
        get { return m_thread; }
      }

      public override string ToString()
      {
        return string.Format("Logging context of thread {0} {1}", m_thread.ManagedThreadId, m_thread.Name);
      }
    }

    private class GlobalContextData : ILoggingContext
    {
      private readonly ThreadContextData m_data = new ThreadContextData(false);
      private readonly SharedLock m_lock = new SharedLock(false);

      public object this[string key]
      {
        get
        {
          using (m_lock.ReadLock())
            return m_data[key];
        }
        set
        {
          using (m_lock.WriteLock())
            m_data[key] = value;
        }
      }

      public bool Contains(string key)
      {
        using (m_lock.ReadLock())
          return m_data.Contains(key);
      }

      public override string ToString()
      {
        return "Global logging context";
      }
    }
  }
}