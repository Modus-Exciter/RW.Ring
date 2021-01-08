using System;
using System.Collections.Generic;

namespace Notung.Threading
{
  /// <summary>
  /// Заглушка, имитирующая разделяемую блокировку
  /// </summary>
  public sealed class SharedLockStub : ISharedLock
  {
    private readonly LockStubCloser m_closer = new LockStubCloser();
    
    public LockState LockState
    {
      get { return m_closer.CurrentState; }
    }

    public IDisposable ReadLock()
    {
      m_closer.Push(m_closer.CurrentState);
      m_closer.CurrentState = LockState.Read;
      return m_closer;
    }

    public IDisposable UpgradeableLock()
    {
      m_closer.Push(m_closer.CurrentState);
      m_closer.CurrentState = LockState.Upgradeable;
      return m_closer;
    }

    public IDisposable WriteLock()
    {
      m_closer.Push(m_closer.CurrentState);
      m_closer.CurrentState = LockState.Write;
      return m_closer;
    }

    public void RunInReadLock(Action action, int millisecondsTimeout)
    {
      if (action != null)
      {
        using (this.ReadLock())
          action();
      }
    }

    public void RunInWriteLock(Action action, int millisecondsTimeout)
    {
      if (action != null)
      {
        using (this.WriteLock())
          action();
      }
    }

    public void Close() { }

    private class LockStubCloser : Stack<LockState>, IDisposable
    {
      public LockState CurrentState = LockState.None;

      public void Dispose()
      {
        this.CurrentState = this.Pop();
      }
    }
  }
}