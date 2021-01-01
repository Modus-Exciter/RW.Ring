using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Notung.Threading;

namespace Notung.Data
{
  public class WeakSet <T> : IEnumerable<T> where T : class
  {
    private readonly SharedLock m_lock = new SharedLock(false);
    private readonly HashSet<GCHandle> m_set = new HashSet<GCHandle>(new GCHandleEqualityComparer());
    private readonly List<GCHandle> m_removee = new List<GCHandle>();

    private void Fix(T item = null)
    {
#if DEBUG
      if (m_lock.LockState != LockState.Write)
        throw new InvalidOperationException();
#endif
      foreach (var handle in m_set)
      {
        var target = handle.IsAllocated ? handle.Target : null;

        if (target != null)
          m_removee.Add(handle);
        else
          handle.Free();
      }

      if (m_removee.Count < m_set.Count)
      {
        m_set.Clear();

        foreach (var handle in m_removee)
          m_set.Add(handle);
      }

      m_removee.Clear();
    }

    public void Add(T item)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      using (m_lock.WriteLock())
      {
        this.Fix();

        m_set.Add(GCHandle.Alloc(item, GCHandleType.Weak));
      }
    }

    public void Clear()
    {
      using (m_lock.WriteLock())
      {
        foreach (var removee in m_set)
          removee.Free();

        m_set.Clear();
      }
    }

    public bool Contains(T item)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      using (m_lock.ReadLock())
      {
        var ptr = GCHandle.Alloc(item, GCHandleType.Weak);
        var ret = m_set.Contains(ptr);

        ptr.Free();

        return ret;
      }
    }

    public bool Remove(T item)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      using (m_lock.WriteLock())
      {
        this.Fix(item);

        var ptr = GCHandle.Alloc(item, GCHandleType.Weak);
        var ret = m_set.Remove(ptr);

        ptr.Free();

        return ret;
      }
    }

    public IEnumerator<T> GetEnumerator()
    {
      using (m_lock.ReadLock())
      {
        foreach (var reference in m_set)
        {
          var tg = reference.Target as T;

          if (tg != null)
            yield return tg;
        }
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    private class GCHandleEqualityComparer : IEqualityComparer<GCHandle>
    {
      public bool Equals(GCHandle x, GCHandle y)
      {
        var x_target = x.IsAllocated ? x.Target : null;
        var y_target = y.IsAllocated ? y.Target : null;

        return object.Equals(x_target, y_target);
      }

      public int GetHashCode(GCHandle obj)
      {
        var target = obj.IsAllocated ? obj.Target : null;

        return (target ?? obj).GetHashCode();
      }
    }
  }
}