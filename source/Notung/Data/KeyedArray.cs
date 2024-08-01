using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notung.Data
{
  public class KeyedArray<T> : IEnumerable<T>
  {
    private readonly T[] m_array;
    private readonly Dictionary<T, int> m_indexes;

    public KeyedArray(int count, Func<int, T> getter, IEqualityComparer<T> comparer)
    {
      if (count == 0)
        throw new ArgumentOutOfRangeException("count");

      if (getter == null)
        throw new ArgumentNullException("getter");

      if (comparer == null)
        throw new ArgumentNullException("comparer");

      m_array = new T[count];
      m_indexes = new Dictionary<T, int>(PrimeHelper.GetPrime(count), comparer);

      for (int i = 0; i < count; i++)
      {
        var key = getter(i);

        if (key == null)
          throw new ArgumentNullException(string.Format("getter({0})", i));

        m_indexes.Add(key, i);
        m_array[i] = key;
      }
    }

    public KeyedArray(IList<T> collection, IEqualityComparer<T> comparer)
      : this(collection.Count, i => collection[i], comparer) { }

    public KeyedArray(int count, Func<int, T> getter)
      : this(count, getter, EqualityComparer<T>.Default) { }

    public KeyedArray(IList<T> collection) 
      : this(collection, EqualityComparer<T>.Default) { }

    public int Count
    {
      get { return m_array.Length; }
    }

    public int GetIndex(T key)
    {
      return m_indexes[key];
    }

    public T GetKey(int index)
    {
      return m_array[index];
    }

    public IEnumerator<T> GetEnumerator()
    {
      return (m_array as IList<T>).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }
}
