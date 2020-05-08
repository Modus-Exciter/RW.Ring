using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Notung.Threading
{
  public sealed class LockSource
  {
    private readonly ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
    private volatile bool m_closed;

    /// <summary>
    /// Устанавливает блокировку на чтение
    /// </summary>
    /// <returns>Дескриптор, позволяющий завершить блокировку</returns>
    
    public IDisposable ReadLock()
    {
      return new ReadLockImpl(m_lock);
    }

    /// <summary>
    /// Устанавливает блокировку на запись
    /// </summary>
    /// <returns>Дескриптор, позволяющий завершить блокировку</returns>
    public IDisposable WriteLock() 
    {
      return new WriteLockImpl(m_lock);
    }

    /// <summary>
    /// Устанавливает блокировку на чтение с возможностью перехода к блокировке на запись
    /// </summary>
    /// <returns>Дескриптор, позволяющий завершить блокировку</returns>
    public IDisposable UpgradeableLock()
    {
      return new UpgradeableLockImpl(m_lock);
    }

    /// <summary>
    /// Выполнение операции в контексте блокировки на чтение
    /// </summary>
    /// <param name="action">Выполняемая операция</param>
    /// <param name="millisecondsTimeout">Время, по истечении которого если не удалось захватить блокировку операция выполняться не будет</param>
    public void RunInReadLock(Action action, int millisecondsTimeout)
    {
      if (action == null) throw new ArgumentNullException("action");

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
      if (action == null) throw new ArgumentNullException("action");

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

    private sealed class ReadLockImpl : IDisposable
    {
      private readonly ReaderWriterLockSlim m_lock;
      private readonly bool m_exit_required;

      public ReadLockImpl( ReaderWriterLockSlim source)
      {
        if (source == null)
          throw new ArgumentNullException("source");

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

    private sealed class UpgradeableLockImpl : IDisposable
    {
      private readonly ReaderWriterLockSlim m_lock;
      private readonly bool m_exit_required;

      public UpgradeableLockImpl( ReaderWriterLockSlim source)
      {
        if (source == null)
          throw new ArgumentNullException("source");

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

    private sealed class WriteLockImpl : IDisposable
    {
      private readonly ReaderWriterLockSlim m_lock;
      private readonly bool m_exit_required;

      public WriteLockImpl( ReaderWriterLockSlim source)
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
