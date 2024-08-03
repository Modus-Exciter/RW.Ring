using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Schicksal.Basic
{
  /// <summary>
  /// Вспомогательный класс для сопоставления выборок ключам
  /// </summary>
  /// <typeparam name="T">Тип ключа</typeparam>
  [ImmutableObject(true)]
  public sealed class ArrayDividedSample<T> : IDividedSample<T>
  {
    private readonly IPlainSample[] m_data;
    private readonly T[] m_keys;
    private readonly Dictionary<T, int> m_indexes;

    public ArrayDividedSample(IPlainSample[] data, Func<int, T> keys)
    {
      if (data == null)
        throw new ArgumentNullException("data");

      if (keys == null)
        throw new ArgumentNullException("keys");

      m_indexes = new Dictionary<T, int>(data.Length);
      m_keys = new T[data.Length];

      for (int i = 0; i < data.Length; i++)
      {
        if (data[i] == null)
          throw new ArgumentNullException(string.Format("data[{0}]", i));

        var key = keys(i);

        m_indexes.Add(key, i);
        m_keys[i] = key;
      }

      m_data = data;
    }

    public ArrayDividedSample(double[][] data, T[] keys)
    {
      if (data == null)
        throw new ArgumentNullException("data");

      if (keys == null)
        throw new ArgumentNullException("keys");

      if (data.Length != keys.Length)
        throw new ArgumentException("Data and keys count mismatch");

      m_data = new IPlainSample[data.Length];
      m_indexes = new Dictionary<T, int>(m_data.Length);

      for (int i = 0; i < data.Length; i++)
      {
        m_data[i] = new ArrayPlainSample(data[i]);

        m_indexes.Add(keys[i], i);
      }

      m_keys = keys;
    }

    public ArrayDividedSample(IDividedSample data, Func<int, T> keys)
    {
      if (data == null)
        throw new ArgumentNullException("data");

      if (keys == null)
        throw new ArgumentNullException("keys");

      m_data = new IPlainSample[data.Count];
      m_keys = new T[data.Count];
      m_indexes = new Dictionary<T, int>();

      for (int i = 0; i < m_data.Length; i++)
      {
        var key = keys(i);

        m_indexes.Add(key, i);
        m_data[i] = data[i];
        m_keys[i] = key;
      }
    }

    public IPlainSample this[T key] { get { return m_data[m_indexes[key]]; } }

    public IPlainSample this[int index] { get { return m_data[index]; } }

    public int Count { get { return m_data.Length; } }

    public T GetKey(int index)
    {
      return m_keys[index];
    }

    public int GetIndex(T key)
    {
      return m_indexes[key];
    }

    public IEnumerator<IPlainSample> GetEnumerator()
    {
      return ((IList<IPlainSample>)m_data).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return m_data.GetEnumerator();
    }

    public override string ToString()
    {
      return string.Format("Number sequence set, count={0}", m_data.Length);
    }

    public override bool Equals(object obj)
    {
      var other = obj as ArrayDividedSample<T>;

      if (other == null || ReferenceEquals(this, obj))
        return other != null;

      if (!ReferenceEquals(m_data, other.m_data))
      {
        for (int i = 0; i < m_data.Length; i++)
        {
          if (!m_data[i].Equals(other.m_data[i]))
            return false;
        }
      }

      if (!ReferenceEquals(m_indexes, other.m_indexes))
      {
        for (int i = 0; i < m_keys.Length; i++)
        {
          if (!m_keys[i].Equals(other.m_keys[i]))
            return false;
        }
      }

      return true;
    }

    public override int GetHashCode()
    {
      int res = m_data.Length;

      for (int i = 0; i < m_data.Length; i++)
        res ^= m_data[i].GetHashCode();

      for (int i = 0; i < m_keys.Length; i++)
        res ^= m_keys[i].GetHashCode();

      return res;
    }
  }
}