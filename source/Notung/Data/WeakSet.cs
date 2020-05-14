using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Notung.Threading;

namespace Notung.Data
{
  public class WeakSet <T> : ICollection<T> where T : class
  {
    private readonly HashSet<InnerReference> m_set = new HashSet<InnerReference>();
    private readonly SharedLock m_lock = new SharedLock(false);

    private void Fix()
    {
#if DEBUG
      if (m_lock.LockState != LockState.Write)
        throw new InvalidOperationException();
#endif
      m_set.RemoveWhere(reference => !reference.IsAlive);
    }

    public void Add(T item)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      using (m_lock.WriteLock())
      {
        Fix();
        m_set.Add(new InnerReference(item));
      }
    }

    public void Clear()
    {
      using (m_lock.WriteLock())
      {
        m_set.Clear();
      }
    }

    public bool Contains(T item)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      using (m_lock.ReadLock())
      {
        return m_set.Contains(new InnerReference(item));
      }
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      using (m_lock.WriteLock())
      {
        Fix();
        InnerReference[] refs = new InnerReference[array.Length];
        m_set.CopyTo(refs, arrayIndex);

        for (int i = arrayIndex; i < array.Length && i < arrayIndex + m_set.Count; i++)
          array[i] = refs[i].Target;
      }
    }

    int ICollection<T>.Count
    {
      get { return m_set.Count; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool Remove(T item)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      using (m_lock.WriteLock())
      {
        Fix();
        return m_set.Remove(new InnerReference(item));
      }
    }

    public IEnumerator<T> GetEnumerator()
    {
      using (m_lock.ReadLock())
      {
        foreach (var reference in m_set)
        {
          if (reference.IsAlive)
          {
            var tg = reference.Target;

            if (tg != null)
              yield return tg;
          }
        }
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    private class InnerReference : WeakReference
    {
      private int m_hash_code;

      public InnerReference(T target)
        : base(target, false)
      {
        m_hash_code = target.GetHashCode();
      }

      public override bool Equals(object obj)
      {
        InnerReference other = obj as InnerReference;

        if (other == null)
          return false;

        if (this.IsAlive)
          return other.IsAlive && this.Target.Equals(other.Target);
        else
          return !other.IsAlive && m_hash_code == other.m_hash_code;
      }

      public override int GetHashCode()
      {
        return m_hash_code;
      }

      public new T Target
      {
        get { return base.Target as T; }
      }
    }
  }
}
