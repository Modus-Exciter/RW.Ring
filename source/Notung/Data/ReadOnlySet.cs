using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Notung.Data
{
  [DebuggerDisplay("Count = {Count}")]
  public sealed class ReadOnlySet<T> : ISet<T>
  {
    private readonly ISet<T> m_set;

    public ReadOnlySet(ISet<T> set)
    {
      if (set == null)
        throw new ArgumentNullException("set");

      m_set = set;
    }

    #region Methods

    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
      return m_set.IsProperSubsetOf(other);
    }

    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
      return m_set.IsProperSupersetOf(other);
    }

    public bool IsSubsetOf(IEnumerable<T> other)
    {
      return m_set.IsSubsetOf(other);
    }

    public bool IsSupersetOf(IEnumerable<T> other)
    {
      return m_set.IsSupersetOf(other);
    }

    public bool Overlaps(IEnumerable<T> other)
    {
      return m_set.Overlaps(other);
    }

    public bool SetEquals(IEnumerable<T> other)
    {
      return m_set.SetEquals(other);
    }

    public bool Contains(T item)
    {
      return m_set.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      m_set.CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get { return m_set.Count; }
    }

    public bool IsReadOnly
    {
      get { return true; }
    }

    public IEnumerator<T> GetEnumerator()
    {
      return m_set.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return m_set.GetEnumerator();
    }

    #endregion

    #region Unsupported methods

    bool ISet<T>.Add(T item)
    {
      throw new NotSupportedException();
    }

    void ISet<T>.ExceptWith(IEnumerable<T> other)
    {
      throw new NotSupportedException();
    }

    void ISet<T>.IntersectWith(IEnumerable<T> other)
    {
      throw new NotSupportedException();
    }

    void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
    {
      throw new NotSupportedException();
    }

    void ISet<T>.UnionWith(IEnumerable<T> other)
    {
      throw new NotSupportedException();
    }

    void ICollection<T>.Add(T item)
    {
      throw new NotSupportedException();
    }

    void ICollection<T>.Clear()
    {
      throw new NotSupportedException();
    }

    bool ICollection<T>.Remove(T item)
    {
      throw new NotSupportedException();
    }

    #endregion
  }
}