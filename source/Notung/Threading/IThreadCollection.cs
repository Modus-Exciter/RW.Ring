using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Notung.Threading
{
  /// <summary>
  /// Список зарегистрированных потоков приложения
  /// </summary>
  public interface IThreadCollection : IEnumerable<Thread>
  {
    /// <summary>
    /// Количество зарегистрированных потоков
    /// </summary>
    int Count { get; }
  }

  /// <summary>
  /// Хранилище потоков для отслеживания их состояния
  /// </summary>
  public static class ThreadTracker
  {
    private static Action<Thread> _thread_registered;
    private static Action<Thread> _thread_removed;
    private static Action<Thread, Exception> _thread_error;

    private static readonly HashSet<Thread> _threads = new HashSet<Thread> { Thread.CurrentThread };
    private static readonly ThreadCollection _thread_collection = new ThreadCollection();

    /// <summary>
    /// Список зарегистрированных потоков
    /// </summary>
    public static IThreadCollection Threads
    {
      get { return _thread_collection; }
    }

    /// <summary>
    /// Добавление действий, которые будут выполняться при регистрации или удалении потоков
    /// </summary>
    /// <param name="registered">Действие, выполняемое при регистрации потока</param>
    /// <param name="removed">Действие, выполняемое при удалении потока</param>
    /// <param name="onError">Действие, выполняемое при ошибке</param>
    public static void AddThreadHandlers(Action<Thread> registered,
      Action<Thread> removed, Action<Thread, Exception> onError)
    {
      if (registered != null)
        _thread_registered += registered;

      if (removed != null)
        _thread_removed += removed;

      if (onError != null)
        _thread_error += onError;

      if (registered != null)
      {
        lock (_threads)
        {
          foreach (var thread in _threads)
          {
            if (thread.IsAlive)
              ProcessThread(thread, _thread_registered);
          }
        }
      }
    }

    /// <summary>
    /// Регистрация потока
    /// </summary>
    /// <param name="thread">Регистрируемый поток</param>
    public static void RegisterThread(Thread thread)
    {
      if (thread == null)
        return;

      lock (_threads)
      {
        if (_threads.Add(thread))
          ProcessThread(thread, _thread_registered);
      }
    }

    /// <summary>
    /// Удаление потока из списка зарегистрированных
    /// </summary>
    /// <param name="thread">Удаляемый поток</param>
    public static void RemoveThread(Thread thread)
    {
      if (thread == null)
        return;

      lock (_threads)
      {
        if (_threads.Remove(thread))
          ProcessThread(thread, _thread_removed);
      }
    }

    /// <summary>
    /// Очистка завершившихся потоков
    /// </summary>
    public static void ClearExpiredThreads()
    {
      lock (_threads)
        _threads.RemoveWhere(thread => !thread.IsAlive);
    }

    #region Implementation ------------------------------------------------------------------------

    private static void ProcessThread(Thread thread, Action<Thread> action)
    {
      if (action == null)
        return;

      var on_error = _thread_error;

      if (on_error != null)
      {
        try
        {
          action(thread);
        }
        catch (Exception ex)
        {
          on_error(thread, ex);
        }
      }
      else
        action(thread);
    }

    private sealed class ThreadCollection : IThreadCollection
    {
      public int Count
      {
        get { return _threads.Count; }
      }

      public IEnumerator<Thread> GetEnumerator()
      {
        return _threads.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return _threads.GetEnumerator();
      }

      public override string ToString()
      {
        return string.Format("{0} threads registered", _threads.Count);
      }
    }

    #endregion
  }
}