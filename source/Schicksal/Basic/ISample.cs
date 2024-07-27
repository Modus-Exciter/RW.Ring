using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Schicksal.Basic
{
  /// <summary>
  /// Выборка для статистического анализа
  /// </summary>
  public interface ISample : IEnumerable
  {
    /// <summary>
    /// Объём выборки
    /// </summary>
    int Count { get; }
  }
  
  /// <summary>
  /// Выборка чисел для статистического анализа
  /// </summary>
  public interface IPlainSample : IEnumerable<double>, ISample
  {
    /// <summary>
    /// Обращение к элементу выборки по номеру
    /// </summary>
    /// <param name="index">Порядковый номер элемента выборки</param>
    /// <returns>Значение элемента выборки</returns>
    double this[int index] { get; }
  }

  /// <summary>
  /// Набор данных, состоящий из нескольких выборок
  /// </summary>
  public interface IDividedSample : IEnumerable<IPlainSample>, ISample
  {
    /// <summary>
    /// Получение выборки по порядковому номеру
    /// </summary>
    /// <param name="index">Порядковый номер выборки в наборе данных</param>
    /// <returns>Выборка для анализа</returns>
    IPlainSample this[int index] { get; }
  }

  /// <summary>
  /// Множество наборов данных, состоящих из нескольких выборок
  /// </summary>
  public interface IComplexSample : IEnumerable<IDividedSample>, ISample
  {
    /// <summary>
    /// Получение выборки по порядковому номеру
    /// </summary>
    /// <param name="index">Порядковый номер набора выборок данных</param>
    /// <returns>Выборка для анализа</returns>
    IDividedSample this[int index] { get; }
  }

  /// <summary>
  /// Набор данных, состоящий из нескольких выборок, с поиском по ключу
  /// </summary>
  /// <typeparam name="T">Тип ключа выборки</typeparam>
  public interface IDividedSample<T> : IDividedSample
  {
    /// <summary>
    /// Получение выборки по ключу
    /// </summary>
    /// <param name="key">Значение ключа выборки</param>
    /// <returns>Выборка для анализа</returns>
    IPlainSample this[T key] { get; }

    /// <summary>
    /// Получение ключа выборки по порядковому номеру
    /// </summary>
    /// <param name="index">Порядковый номер выборки в наборе данных</param>
    /// <returns>Значение ключа выборки</returns>
    T GetKey(int index);

    /// <summary>
    /// Получение порядкового номера выборки по ключу
    /// </summary>
    /// <param name="key">Значение ключа выборки</param>
    /// <returns>Порядковый номер выборки в наборе данных</returns>
    int GetIndex(T key);
  }

  [ImmutableObject(true)]
  public sealed class ArrayPlainSample : IPlainSample
  {
    private readonly double[] m_array;

    public ArrayPlainSample(double[] array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      m_array = array;
    }

    public double this[int index]
    {
      get { return m_array[index]; }
    }

    public int Count
    {
      get { return m_array.Length; }
    }

    public IEnumerator<double> GetEnumerator()
    {
      return (m_array as IList<double>).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return m_array.GetEnumerator();
    }

    public override string ToString()
    {
      return string.Format("Number sequence, count={0}", m_array.Length);
    }

    public override bool Equals(object obj)
    {
      var other = obj as ArrayPlainSample;

      if (other == null)
        return false;

      return m_array.Equals(other.m_array);
    }

    public override int GetHashCode()
    {
      return m_array.GetHashCode();
    }
  }

  [ImmutableObject(true)]
  public sealed class ArrayDividedSample : IDividedSample
  {
    private readonly IPlainSample[] m_data;

    public ArrayDividedSample(IPlainSample[] data)
    {
      if (data == null)
        throw new ArgumentNullException("data");

      for (int i = 0; i < data.Length; i++)
      {
        if (data[i] == null)
          throw new ArgumentNullException(string.Format("data[{0}]", i));
      }

      m_data = data;
    }

    public ArrayDividedSample(double[][] data)
    {
      if (data == null)
        throw new ArgumentNullException("data");

      m_data = new IPlainSample[data.Length];

      for (int i = 0; i < data.Length; i++)
        m_data[i] = new ArrayPlainSample(data[i]);
    }

    public IPlainSample this[int index]
    {
      get { return m_data[index]; }
    }

    public int Count
    {
      get { return m_data.Length; }
    }

    public IEnumerator<IPlainSample> GetEnumerator()
    {
      return (m_data as IList<IPlainSample>).GetEnumerator();
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
      var other = obj as ArrayDividedSample;

      if (other == null)
        return false;

      if (m_data.Length != other.m_data.Length)
        return false;

      if (!ReferenceEquals(m_data, other.m_data))
      {
        for (int i = 0; i < m_data.Length; i++)
        {
          if (!m_data[i].Equals(other.m_data[i]))
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

      return res;
    }
  }

  [ImmutableObject(true)]
  public sealed class ArrayComplexSample : IComplexSample
  {
    private readonly IDividedSample[] m_data;

    public ArrayComplexSample(IDividedSample[] data)
    {
      if (data == null)
        throw new ArgumentNullException("data");

      for (int i = 0; i < data.Length; i++)
      {
        if (data[i] == null)
          throw new ArgumentNullException(string.Format("data[{0}]", i));
      }

      m_data = data;
    }

    public IDividedSample this[int index]
    {
      get { return m_data[index]; }
    }

    public int Count
    {
      get { return m_data.Length; }
    }

    public IEnumerator<IDividedSample> GetEnumerator()
    {
      return (m_data as IList <IDividedSample>).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return m_data.GetEnumerator();
    }

    public override string ToString()
    {
      return string.Format("Number sequence complex, count={0}", m_data.Length);
    }

    public override bool Equals(object obj)
    {
      var other = obj as ArrayComplexSample;

      if (other == null)
        return false;

      if (m_data.Length != other.m_data.Length)
        return false;

      if (ReferenceEquals(m_data, other.m_data))
        return true;

      for (int i = 0; i < m_data.Length; i++)
      {
        if (!m_data[i].Equals(other.m_data[i]))
          return false;
      }

      return true;
    }

    public override int GetHashCode()
    {
      int res = m_data.Length;

      for (int i = 0; i < m_data.Length; i++)
        res ^= m_data[i].GetHashCode();

      return res;
    }
  }
}