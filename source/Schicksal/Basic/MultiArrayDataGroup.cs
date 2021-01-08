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
  public sealed class MultiArrayDataGroup<T> : IMultyDataGroup<T>
  {
    private readonly IDataGroup[] m_data;
    private readonly T[] m_keys;
    private readonly Dictionary<T, int> m_indexes;

    public MultiArrayDataGroup(IDataGroup[] data, T[] keys)
    {
      if (data == null)
        throw new ArgumentNullException("data");

      if (keys == null)
        throw new ArgumentNullException("keys");

      if (data.Length != keys.Length)
        throw new ArgumentException("Data and keys count mismatch");

      m_indexes = new Dictionary<T, int>(data.Length);

      for (int i = 0; i < data.Length; i++)
      {
        if (data[i] == null)
          throw new ArgumentNullException(string.Format("data[{0}]", i));

        m_indexes.Add(keys[i], i);
      }

      m_data = data;
      m_keys = keys;
    }

    public MultiArrayDataGroup(double[][] data, T[] keys)
    {
      if (data == null)
        throw new ArgumentNullException("data");

      if (keys == null)
        throw new ArgumentNullException("keys");

      if (data.Length != keys.Length)
        throw new ArgumentException("Data and keys count mismatch");

      m_data = new IDataGroup[data.Length];
      m_indexes = new Dictionary<T, int>(m_data.Length);

      for (int i = 0; i < data.Length; i++)
      {
        m_data[i] = new ArrayDataGroup(data[i]);

        m_indexes.Add(keys[i], i);
      }

      m_keys = keys;
    }

    public IDataGroup this[T key] { get { return m_data[m_indexes[key]]; } }

    public IDataGroup this[int index] { get { return m_data[index]; } }

    public int Count { get { return m_data.Length; } }

    public T GetKey(int index)
    {
      return m_keys[index];
    }

    public int GetIndex(T key)
    {
      return m_indexes[key];
    }

    public IEnumerator<IDataGroup> GetEnumerator()
    {
      return ((IList<IDataGroup>)m_data).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return m_data.GetEnumerator();
    }
  }
}
