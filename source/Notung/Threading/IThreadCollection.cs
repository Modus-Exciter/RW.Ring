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
    public static void AddThreadHandlers(Action<Thread> registered, Action<Thread> removed, Action<Thread, Exception> onError)
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
            if (!thread.IsAlive)
              continue;

            if (onError != null)
            {
              try
              {
                registered(thread);
              }
              catch (Exception ex)
              {
                onError(thread, ex);
              }
            }
            else
              registered(thread);
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
        {
          var registrator = _thread_registered;

          if (registrator != null)
          {
            var on_error = _thread_error;

            if (on_error != null)
            {
              try
              {
                registrator(thread);
              }
              catch (Exception ex)
              {
                on_error(thread, ex);
              }
            }
            else
              registrator(thread);
          }
        }
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
        {
          var remover = _thread_removed;

          if (remover != null)
            remover(thread);
        }
      }
    }

    #region Implementation ------------------------------------------------------------------------

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