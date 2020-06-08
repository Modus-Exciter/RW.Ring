using System;
using System.Threading;
using Notung.Threading;

namespace Notung.Data
{
  public class Pool<T> : IDisposable
  {
    private readonly Entry[] m_entries;
    private readonly T[] m_elements;
    private readonly EventWaitHandle m_signal = new EventWaitHandle(false, EventResetMode.AutoReset);
    private readonly SharedLock m_lock = new SharedLock(false);
    private Entry m_root;

    public Pool(T[] elements)
    {
      if (elements == null)
        throw new ArgumentNullException("elements");

      if (elements.Length < 1)
        throw new ArgumentOutOfRangeException("elements.Length");

      m_elements = elements;
      m_entries = new Entry[elements.Length];

      for (int i = 0; i < elements.Length; i++)
        m_entries[i] = new Entry(i);

      for (int i = 1; i < m_entries.Length; i++)
        m_entries[i - 1].Next = m_entries[i];

      m_root = m_entries[0];
    }

    public int Count
    {
      get { return m_elements.Length; }
    }

    public T this[int index]
    {
      get
      {
        if (index < 0 || index >= m_entries.Length)
          throw new ArgumentOutOfRangeException("index");

        using (m_lock.ReadLock())
        {
          if (!m_entries[index].Busy)
            throw new ArgumentException("Pool entry is not acquired");

          return m_elements[index];
        }
      }
    }

    public int Accuire(bool wait)
    {
      using (m_lock.WriteLock())
      {
        if (m_root != null)
          return GetRootEntryIndex();
      }

      if (wait)
      {
        m_signal.WaitOne();

        using (m_lock.WriteLock())
        {
          return GetRootEntryIndex();
        }
      }
      else
        return -1;
    }

    public void Release(int index)
    {
      if (index < 0 || index >= m_entries.Length)
        throw new ArgumentOutOfRangeException("index");

      using (m_lock.WriteLock())
      {
        m_entries[index].Next = m_root;
        m_entries[index].Busy = false;
        m_root = m_entries[index];

        m_signal.Set();
      }
   }

    private int GetRootEntryIndex()
    {
      var ret = m_root;
      ret.Busy = true;
      m_root = m_root.Next;
      return ret.Index;
    }

    public void Dispose()
    {
      m_lock.Close();
      m_signal.Dispose();
    }

    private class Entry
    {
      public Entry(int index)
      {
        this.Index = index;
      }

      public readonly int Index;
      public Entry Next;
      public bool Busy;
    }
  }

  public static class Rent
  {
    public static Rent<T> Create<T>(Pool<T> pool)
    {
      return new Rent<T>(pool);
    }
  }

  public class Rent<T> : IDisposable
  {
    private readonly Pool<T> m_pool;
    private readonly int m_index;

    internal Rent(Pool<T> pool)
    {
      if (pool == null)
        throw new ArgumentNullException("pool");

      m_pool = pool;
      m_index = pool.Accuire(true);
    }

    public T Data
    {
      get { return m_pool[m_index]; }
    }

    public void Dispose()
    {
      m_pool.Release(m_index);
    }

    public override string ToString()
    {
      return this.Data != null ? this.Data.ToString() : base.ToString();
    }
  }
}