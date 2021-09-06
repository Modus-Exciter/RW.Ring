using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace Notung.Feuerzauber
{
  public class DispatcherSynchronizeInvoke : ISynchronizeInvoke
  {
    private readonly Dispatcher m_dispatcher;

    public DispatcherSynchronizeInvoke(Dispatcher dispatcher)
    {
      if (dispatcher == null)
        throw new ArgumentNullException("dispatcher");

      m_dispatcher = dispatcher;
    }

    public IAsyncResult BeginInvoke(Delegate method, object[] args)
    {
      return new DispatcherAsyncResult(m_dispatcher.BeginInvoke(method, args));
    }

    public object EndInvoke(IAsyncResult result)
    {
      var res = result as DispatcherAsyncResult;

      if (res == null)
        throw new ArgumentException();

      res.EndInvoke();

      return res.AsyncState;
    }

    public object Invoke(Delegate method, object[] args)
    {
      return m_dispatcher.Invoke(method, args);
    }

    public bool InvokeRequired
    {
      get { return !m_dispatcher.CheckAccess(); }
    }

    private class DispatcherAsyncResult : IAsyncResult
    {
      private readonly DispatcherOperation m_operation;
      private readonly object m_lock = new object();
      private readonly bool m_sync;
      private volatile EventWaitHandle m_handle;
      private volatile bool m_completed;

      public DispatcherAsyncResult(DispatcherOperation operation)
      {
        m_operation = operation;
        m_operation.Completed += HandleCompleted;

        lock (m_lock)
          m_completed = m_operation.Status == DispatcherOperationStatus.Completed;

        m_sync = m_operation.Dispatcher.CheckAccess();
      }

      private void HandleCompleted(object sender, EventArgs args)
      {
        m_operation.Completed -= HandleCompleted;

        lock (m_lock)
        {
          m_completed = true;

          if (m_handle != null)
            m_handle.Set();
        }
      }

      public object AsyncState
      {
        get { return m_operation.Result; }
      }

      public WaitHandle AsyncWaitHandle
      {
        get
        {
          if (m_handle != null)
            return m_handle;

          lock (m_lock)
          {
            if (m_handle == null)
              m_handle = new EventWaitHandle(m_completed, EventResetMode.ManualReset);

            return m_handle;
          }
        }
      }

      public bool CompletedSynchronously
      {
        get { return m_sync; }
      }

      public bool IsCompleted
      {
        get { return m_completed; }
      }

      public void EndInvoke()
      {
        m_operation.Wait();

        lock (m_lock)
        {
          if (m_handle != null)
            m_handle.Close();
        }
      }
    }
  }
}
