using System;
using System.Threading;
using Notung.Threading;

namespace Notung.Data
{
  /// <summary>
  /// Пул объектов заданного размера
  /// </summary>
  /// <typeparam name="T">Тип объектов, хранящихся в пуле</typeparam>
  public class Pool<T> : IDisposable
  {
    private readonly Entry[] m_entries;
    private readonly EventWaitHandle m_signal = new EventWaitHandle(false, EventResetMode.AutoReset);
    private readonly SharedLock m_lock = new SharedLock(false);
    private volatile int m_trottle;
    private Entry m_root;

    /// <summary>
    /// Инициализация пула объектов
    /// </summary>
    /// <param name="elements">Объекты, которые будут помещены в пул</param>
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

        m_entries[i] = new Entry(i, elements[i]);
      }

      for (int i = 1; i < m_entries.Length; i++)
        m_entries[i - 1].Next = m_entries[i];

      m_root = m_entries[0];
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
    public int Trottle
    {
      get { return m_trottle; }
    }

    /// <summary>
    /// Получение объекта из пула по дескриптору
    /// </summary>
    /// <param name="handle">Дескриптор объекта в пуле</param>
    /// <returns>Объект с указанным дескриптором</returns>
    public T this[int handle]
    {
      get
      {
        if (handle < 0 || handle >= m_entries.Length)
          throw new ArgumentOutOfRangeException("index");

        using (m_lock.ReadLock())
        {
          if (m_entries[handle].Free)
            throw new ArgumentException("Pool entry is not acquired");

          return m_entries[handle].Data;
        }
      }
    }

    /// <summary>
    /// Запрос дескриптора нового объекта, который требуется занять
    /// </summary>
    /// <param name="wait">Что делать, если в пуле не осталось объектов: 
    /// true, чтобы дождаться освобождения, false, чтобы отказаться от получения объекта</param>
    /// <returns></returns>
    public int Accuire(bool wait)
    {
      using (m_lock.WriteLock())
      {
        if (m_root != null)
          return GetRootEntryHandle();
      }

      if (wait)
      {
        while (true)
        {
          m_signal.WaitOne();

          using (m_lock.WriteLock())
          {
            if (m_root != null) 
              return GetRootEntryHandle();
          }
        }
      }
      else
        return PoolItem.InvalidHandle;
    }

    /// <summary>
    /// Освобождение объекта, взятого из пула
    /// </summary>
    /// <param name="handle">Десриптор освобождаемого объекта</param>
    public void Release(int handle)
    {
      if (handle < 0 || handle >= m_entries.Length)
        throw new ArgumentOutOfRangeException("handle");

      using (m_lock.WriteLock())
      {
        if (m_entries[handle].Free)
          return;

        m_entries[handle].Next = m_root;
        m_entries[handle].Free = true;
        m_root = m_entries[handle];
        m_trottle--;
        m_signal.Set();
      }
   }

    /// <summary>
    /// Очистка ресурсов, используемых пулом
    /// </summary>
    public void Dispose()
    {
      m_lock.Close();
      m_signal.Dispose();
    }

    #region Implementation ------------------------------------------------------------------------

    private int GetRootEntryHandle()
    {
      var ret = m_root;
      ret.Free = false;
      m_root = m_root.Next;
      m_trottle++;

      m_signal.Reset();

      return ret.Handle;
    }

    private class Entry
    {
      public Entry(int index, T data)
      {
        this.Handle = index;
        this.Data = data;
      }

      public readonly int Handle;
      public readonly T Data;
      public bool Free = true;
      public Entry Next;
    }

    #endregion
  }

  /// <summary>
  /// Обёртка над объектом, взятым из пула
  /// </summary>
  public static class PoolItem
  {
    /// <summary>
    /// Запрос объекта из пула
    /// </summary>
    /// <typeparam name="T">Тип объектов, хранящихся в пуле</typeparam>
    /// <param name="pool">Пул объектов нужного типа</param>
    /// <returns>Объект, получаемый из пула</returns>
    public static PoolItem<T> Create<T>(Pool<T> pool)
    {
      return new PoolItem<T>(pool);
    }

    /// <summary>
    /// Дескриптор объекта, который не удалось взять из пула
    /// </summary>
    public const int InvalidHandle = -1;
  }

  /// <summary>
  /// Обёртка над объектом, взятым из пула
  /// </summary>
  /// <typeparam name="T">Тип объектов, хранящихся в пуле</typeparam>
  public sealed class PoolItem<T> : IDisposable
  {
    private readonly Pool<T> m_pool;
    private readonly int m_index;

    internal PoolItem(Pool<T> pool)
    {
      if (pool == null)
        throw new ArgumentNullException("pool");

      m_pool = pool;
      m_index = pool.Accuire(true);
    }

    /// <summary>
    /// Объект, взятый из пула
    /// </summary>
    public T Data
    {
      get { return m_pool[m_index]; }
    }

    /// <summary>
    /// Возврат объекта в пул
    /// </summary>
    public void Dispose()
    {
      m_pool.Release(m_index);
    }

    public override string ToString()
    {
      return this.Data.ToString();
    }
  }
}