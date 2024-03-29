﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Notung.Threading
{
  /// <summary>
  /// Обёртка над классом ReaderWriterLockSlim, делающая работу с ним более удобной. 
  /// <example>SharedLock locker = new SharedLock();
  /// using (locker.ReadLock()) { DoSomething(); }</example>
  /// </summary>
  [SuppressMessage("Microsoft.Design", "CA1001")]
  public sealed class SharedLock
  {
    private readonly ReaderWriterLockSlim m_lock;
    private readonly ReadLockHandle m_reader;
    private readonly WriteLockHandle m_writer;
    private readonly UpgradeableLockHandle m_upgrader;

    private volatile bool m_closed;

    /// <summary>
    /// Создаёт объект разделяемой блокировки
    /// </summary>
    /// <param name="reenterable">Будет ли блокировка реентерабельной</param>
    public SharedLock(bool reenterable = true)
    {
      m_lock = new ReaderWriterLockSlim(reenterable ?
        LockRecursionPolicy.SupportsRecursion : LockRecursionPolicy.NoRecursion);

      m_reader = new ReadLockHandle(m_lock);
      m_writer = new WriteLockHandle(m_lock);
      m_upgrader = new UpgradeableLockHandle(m_lock);
    }

    /// <summary>
    /// Устанавливает блокировку на чтение
    /// </summary>
    /// <returns>Дескриптор, позволяющий завершить блокировку</returns>   
    public IDisposable ReadLock()
    {
      m_lock.EnterReadLock();
      return m_reader;
    }

    /// <summary>
    /// Устанавливает блокировку на запись
    /// </summary>
    /// <returns>Дескриптор, позволяющий завершить блокировку</returns>
    public IDisposable WriteLock()
    {
      m_lock.EnterWriteLock();
      return m_writer;
    }

    /// <summary>
    /// Устанавливает блокировку на чтение с возможностью перехода к блокировке на запись
    /// </summary>
    /// <returns>Дескриптор, позволяющий завершить блокировку</returns>
    public IDisposable UpgradeableLock()
    {
      m_lock.EnterUpgradeableReadLock();
      return m_upgrader;
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

      if (!m_closed && m_lock.TryEnterReadLock(millisecondsTimeout))
      {
        try
        {
          action();
        }
        finally
        {
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

      if (!m_closed && m_lock.TryEnterWriteLock(millisecondsTimeout))
      {
        try
        {
          action();
        }
        finally
        {
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

    /// <summary>
    /// Статус блокировки - нет, чтение, обновляемый режим или запись
    /// </summary>
    public LockState LockState
    {
      get
      {
        if (m_lock.IsWriteLockHeld)
          return LockState.Write;
        else if (m_lock.IsUpgradeableReadLockHeld)
          return LockState.Upgradeable;
        else if (m_lock.IsReadLockHeld)
          return LockState.Read;
        else
          return LockState.None;
      }
    }

    private sealed class ReadLockHandle : IDisposable
    {
      private readonly ReaderWriterLockSlim m_lock;

      public ReadLockHandle(ReaderWriterLockSlim source)
      {
        m_lock = source;
      }

      public void Dispose()
      {
        m_lock.ExitReadLock();
      }
    }

    private sealed class WriteLockHandle : IDisposable
    {
      private readonly ReaderWriterLockSlim m_lock;

      public WriteLockHandle(ReaderWriterLockSlim source)
      {
        m_lock = source;
      }

      public void Dispose()
      {
        m_lock.ExitWriteLock();
      }
    }

    private sealed class UpgradeableLockHandle : IDisposable
    {
      private readonly ReaderWriterLockSlim m_lock;

      public UpgradeableLockHandle(ReaderWriterLockSlim source)
      {
        m_lock = source;
      }

      public void Dispose()
      {
        m_lock.ExitUpgradeableReadLock();
      }
    }
  }

  /// <summary>
  /// Статус блокировки
  /// </summary>
  public enum LockState
  {
    /// <summary>
    /// Блокировка отсутствует
    /// </summary>
    None,

    /// <summary>
    /// Блокировка на чтение
    /// </summary>
    Read,

    /// <summary>
    /// Блокировка на чтение с возможностью перехода к блокировке на запись
    /// </summary>
    Upgradeable,

    /// <summary>
    /// Блокировка на запись
    /// </summary>
    Write
  }
}