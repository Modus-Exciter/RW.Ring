using System;
using System.Threading;

namespace Notung.Threading
{
  public sealed class LockSource
  {
    private readonly ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
    private readonly bool m_reenterable;
    private volatile bool m_closed;

    public LockSource(bool reenterable = true)
    {
      m_reenterable = reenterable;
    }

    /// <summary>
    /// Устанавливает блокировку на чтение
    /// </summary>
    /// <returns>Дескриптор, позволяющий завершить блокировку</returns>   
    public IDisposable ReadLock()
    {
      if (m_reenterable)
        return new ReenterableReadLock(m_lock);
      else
        return new SingleReadLock(m_lock);
    }

    /// <summary>
    /// Устанавливает блокировку на запись
    /// </summary>
    /// <returns>Дескриптор, позволяющий завершить блокировку</returns>
    public IDisposable WriteLock()
    {
      if (m_reenterable)
        return new ReenterableWriteLock(m_lock);
      else
        return new SingleWriteLock(m_lock);
    }

    /// <summary>
    /// Устанавливает блокировку на чтение с возможностью перехода к блокировке на запись
    /// </summary>
    /// <returns>Дескриптор, позволяющий завершить блокировку</returns>
    public IDisposable UpgradeableLock()
    {
      if (m_reenterable)
        return new ReenterableUpgradeableLock(m_lock);
      else
        return new SingleUpgradeableLock(m_lock);
    }

    /// <summary>
    /// Выполнение операции в контексте блокировки на чтение
    /// </summary>
    /// <param name="action">Выполняемая операция</param>
    /// <param name="millisecondsTimeout">Время, по истечении которого если не удалось захватить блокировку операция выполняться не будет</param>
    public void RunInReadLock(Action action, int millisecondsTimeout)
    {
      if (action == null) 
        throw new ArgumentNullException("action");

      if (m_closed)
        return;

      var lock_required = !m_lock.IsWriteLockHeld
          && !m_lock.IsUpgradeableReadLockHeld && !m_lock.IsReadLockHeld;

      if (lock_required && !m_lock.TryEnterReadLock(millisecondsTimeout))
        return;
      else
      {
        try
        {
          action();
        }
        finally
        {
          if (lock_required)
            m_lock.ExitReadLock();
        }
      }
    }

    /// <summary>
    /// Выполнение операции в контексте блокировки на запись
    /// </summary>
    /// <param name="action">Выполняемая операция</param>
    /// <param name="millisecondsTimeout">Время, по истечении которого если не удалось захватить блокировку операция выполняться не будет</param>
    public void RunInWriteLock(Action action, int millisecondsTimeout)
    {
      if (action == null) 
        throw new ArgumentNullException("action");

      if (m_closed)
        return;

      if (m_lock.IsReadLockHeld
        && !m_lock.IsUpgradeableReadLockHeld)
        throw new InvalidOperationException("IsReadLockHeld");

      var lock_required = !m_lock.IsWriteLockHeld;

      if (lock_required && !m_lock.TryEnterWriteLock(millisecondsTimeout))
        return;
      else
      {
        try
        {
          action();
        }
        finally
        {
          if (lock_required)
            m_lock.ExitWriteLock();
        }
      }
    }

    /// <summary>
    /// Завершает работу ReaderWriterLockSlim
    /// </summary>
    public void Close()
    {
      m_closed = true;
      m_lock.Dispose();
    }

    private sealed class SingleReadLock : IDisposable
    {
      private readonly ReaderWriterLockSlim m_lock;

      public SingleReadLock(ReaderWriterLockSlim source)
      {
        m_lock = source;
        m_lock.EnterReadLock();
      }

      public void Dispose()
      {
        m_lock.ExitReadLock();
      }
    }

    private sealed class SingleWriteLock : IDisposable
    {
      private readonly ReaderWriterLockSlim m_lock;

      public SingleWriteLock(ReaderWriterLockSlim source)
      {
        m_lock = source;
        m_lock.EnterWriteLock();
      }

      public void Dispose()
      {
        m_lock.ExitWriteLock();
      }
    }

    private sealed class SingleUpgradeableLock : IDisposable
    {
      private readonly ReaderWriterLockSlim m_lock;

      public SingleUpgradeableLock(ReaderWriterLockSlim source)
      {
        m_lock = source;
        m_lock.EnterUpgradeableReadLock();
      }

      public void Dispose()
      {
        m_lock.ExitUpgradeableReadLock();
      }
    }

    private sealed class ReenterableReadLock : IDisposable
    {
      private readonly ReaderWriterLockSlim m_lock;
      private readonly bool m_exit_required;

      public ReenterableReadLock(ReaderWriterLockSlim source)
      {
        m_lock = source;

        m_exit_required = !m_lock.IsWriteLockHeld
          && !source.IsUpgradeableReadLockHeld && !m_lock.IsReadLockHeld;

        if (m_exit_required)
          m_lock.EnterReadLock();
      }

      public void Dispose()
      {
        if (m_exit_required)
          m_lock.ExitReadLock();
      }
    }

    private sealed class ReenterableUpgradeableLock : IDisposable
    {
      private readonly ReaderWriterLockSlim m_lock;
      private readonly bool m_exit_required;

      public ReenterableUpgradeableLock(ReaderWriterLockSlim source)
      {
        var read_held = source.IsReadLockHeld;
        var upgradeable_held = source.IsUpgradeableReadLockHeld;

        if (read_held && !upgradeable_held)
          throw new ArgumentException("Not upgradeable", "source");

        m_lock = source;

        m_exit_required = !read_held && !upgradeable_held;

        if (m_exit_required)
          m_lock.EnterUpgradeableReadLock();
      }

      public void Dispose()
      {
        if (m_exit_required)
          m_lock.ExitUpgradeableReadLock();
      }
    }

    private sealed class ReenterableWriteLock : IDisposable
    {
      private readonly ReaderWriterLockSlim m_lock;
      private readonly bool m_exit_required;

      public ReenterableWriteLock( ReaderWriterLockSlim source)
      {
        if (source == null)
          throw new ArgumentNullException("source");

        if (source.IsReadLockHeld
          && !source.IsUpgradeableReadLockHeld)
          throw new ArgumentException("Not writable", "source");

        m_lock = source;

        m_exit_required = !m_lock.IsWriteLockHeld;

        if (m_exit_required)
          m_lock.EnterWriteLock();
      }

      public void Dispose()
      {
        if (m_exit_required)
          m_lock.ExitWriteLock();
      }
    }
  }
}