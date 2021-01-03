using System;
using System.Collections.Generic;

namespace Notung.Threading
{
  /// <summary>
  /// Интерфейс разделяемой блокировки
  /// </summary>
  public interface ISharedLock
  {
    /// <summary>
    /// Статус блокировки - нет, чтение, обновляемый режим или запись
    /// </summary>
    LockState LockState { get; }

    /// <summary>
    /// Устанавливает блокировку на чтение
    /// </summary>
    /// <returns>Дескриптор, позволяющий завершить блокировку</returns>   
    IDisposable ReadLock();

    /// <summary>
    /// Устанавливает блокировку на чтение с возможностью перехода к блокировке на запись
    /// </summary>
    /// <returns>Дескриптор, позволяющий завершить блокировку</returns>
    IDisposable UpgradeableLock();

    /// <summary>
    /// Устанавливает блокировку на запись
    /// </summary>
    /// <returns>Дескриптор, позволяющий завершить блокировку</returns>
    IDisposable WriteLock();

    /// <summary>
    /// Выполнение операции в контексте блокировки на чтение
    /// </summary>
    /// <param name="action">Выполняемая операция</param>
    /// <param name="millisecondsTimeout">Время, по истечении которого если не удалось захватить блокировку операция выполняться не будет</param>
    void RunInReadLock(Action action, int millisecondsTimeout);

    /// <summary>
    /// Выполнение операции в контексте блокировки на запись
    /// </summary>
    /// <param name="action">Выполняемая операция</param>
    /// <param name="millisecondsTimeout">Время, по истечении которого если не удалось захватить блокировку операция выполняться не будет</param>
    void RunInWriteLock(Action action, int millisecondsTimeout);
    
    /// <summary>
    /// Завершает работу объекта блокировки
    /// </summary>
    void Close();
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