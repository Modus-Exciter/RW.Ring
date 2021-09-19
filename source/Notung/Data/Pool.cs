using System;
using System.Threading;
using Notung.Properties;

namespace Notung.Data
{
  /// <summary>
  /// Объект, который можно взять из пула и вернуть в пул
  /// </summary>
  /// <typeparam name="T">Тип объектов, находящихся в пуле</typeparam>
  public interface IPoolItem<T> : IDisposable where T : class
  {
    /// <summary>
    /// Объект, взятый из пула
    /// </summary>
    T Data { get; }
  }

  /// <summary>
  /// Пул объектов
  /// </summary>
  /// <typeparam name="T">Тип объектов, находящихся в пуле</typeparam>
  public sealed class Pool<T> : IDisposable where T : class
  {
    private Entry m_root;
    private readonly Entry[] m_entries;
    private readonly SemaphoreSlim m_semaphore;

    /// <summary>
    /// Инициализация пула объектов
    /// </summary>
    /// <param name="elements">Объекты, которые будут храниться в пуле</param>
    public Pool(T[] elements)
    {
      if (elements == null)
        throw new ArgumentNullException("elements");

      if (elements.Length < 1)
        throw new ArgumentOutOfRangeException("elements.Length");

      m_entries = new Entry[elements.Length];

      for (int i = 0; i < elements.Length; i++)
      {
        if (elements[i] == null)
          throw new ArgumentNullException(string.Format("elements[{0}]", i));

        m_entries[i] = new Entry(elements[i]);
      }

      for (int i = 1; i < m_entries.Length; i++)
        m_entries[i - 1].Next = m_entries[i];

      m_root = m_entries[0];
      m_semaphore = new SemaphoreSlim(m_entries.Length, m_entries.Length);
    }

    /// <summary>
    /// Количество объектов в пуле
    /// </summary>
    public int Size
    {
      get { return m_entries.Length; }
    }

    /// <summary>
    /// Сколько объектов пула задействовано
    /// </summary>
    public int Throttle
    {
      get { return m_entries.Length - m_semaphore.CurrentCount; }
    }

    /// <summary>
    /// Получение объекта из пула
    /// </summary>
    /// <param name="waitIfAllBusy">True, если нужно дожидаться освобождения при исчерпании пула.
    /// False, если нужно вернуть пустой объект при исчерпании пула</param>
    /// <returns>Объект, полученный из пула</returns>
    public IPoolItem<T> Accuire(bool waitIfAllBusy = false)
    {
      if (!waitIfAllBusy)
      {
        lock (m_entries)
        {
          if (m_root == null)
            return PoolItemStub.Instance;
        }
      }

      m_semaphore.Wait();

      lock (m_entries)
      {
        var ret = m_root;
        ret.Busy = true;
        m_root = m_root.Next;

        return new PoolItem(ret, this);
      }
    }

    private void Release(Entry entry)
    {
      lock (m_entries)
      {
        entry.Next = m_root;
        entry.Busy = false;
        m_root = entry;
      }

      m_semaphore.Release();
    }

    /// <summary>
    /// Прекращает работу пула и освобождает связанные ресурсы
    /// </summary>
    public void Dispose()
    {
      m_semaphore.Dispose();
    }

    #region Inner classes -------------------------------------------------------------------------

    private class Entry
    {
      public Entry(T data)
      {
        Data = data;
      }

      public readonly T Data;
      public volatile bool Busy;
      public Entry Next;
    }

    private class PoolItemStub : IPoolItem<T>
    {
      public static readonly PoolItemStub Instance = new PoolItemStub();

      private PoolItemStub() { }

      public T Data
      {
        get { return null; }
      }

      public void Dispose() { }

      public override string ToString()
      {
        return CoreResources.NULL;
      }
    }

    private class PoolItem : IPoolItem<T>
    {
      private readonly Entry m_entry;
      private readonly Pool<T> m_pool;
      private readonly Thread m_thread;

      public PoolItem(Entry entry, Pool<T> pool)
      {
        m_entry = entry;
        m_pool = pool;
        m_thread = Thread.CurrentThread;
      }

      public T Data
      {
        get
        {
          if (!m_entry.Busy)
            throw new InvalidOperationException(Resources.POOL_ITEM_NOT_ACCQUIRED);

          return m_entry.Data;
        }
      }

      public void Dispose()
      {
        if (m_thread != Thread.CurrentThread)
          throw new InvalidOperationException(Resources.WRONG_THREAD);

        m_pool.Release(m_entry);
      }

      public override string ToString()
      {
        return m_entry.Data.ToString();
      }
    }

    #endregion
  }
}