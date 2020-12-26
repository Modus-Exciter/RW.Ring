using System;
using System.Collections;
using System.Collections.Generic;

namespace Schicksal.Basic
{
  /// <summary>
  /// Набор данных для статистического анализа
  /// </summary>
  public interface IDataGroup : IEnumerable<double>
  {
    /// <summary>
    /// Объём выборки
    /// </summary>
    int Count { get; }

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
  public interface IMultyDataGroup : IEnumerable<IDataGroup>
  {
    /// <summary>
    /// Количество выборок
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Получение выборки по порядковому номеру
    /// </summary>
    /// <param name="index">Порядковый номер выборки в наборе данных</param>
    /// <returns>Выборка для анализа</returns>
    IDataGroup this[int index] { get; }
  }

  /// <summary>
  /// Набор данных, состоящий из нескольких выборок, с поиском по ключу
  /// </summary>
  /// <typeparam name="T">Тип ключа выборки</typeparam>
  public interface IMultyDataGroup<T> : IMultyDataGroup
  {
    /// <summary>
    /// Получение выборки по ключу
    /// </summary>
    /// <param name="key">Значение ключа выборки</param>
    /// <returns>Выборка для анализа</returns>
    IDataGroup this[T key] { get; }

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

  public sealed class ArrayDataGroup : IDataGroup
  {
    private readonly double[] m_array;

    public ArrayDataGroup(double[] array)
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
      return ((IList<double>)m_array).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return m_array.GetEnumerator();
    }
  }

  public sealed class MultiArrayDataGroup : IMultyDataGroup
  {
    private readonly IDataGroup[] m_data;

    public MultiArrayDataGroup(IDataGroup[] data)
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

    public MultiArrayDataGroup(double[][] data)
    {
      if (data == null)
        throw new ArgumentNullException("data");

      m_data = new IDataGroup[data.Length];

      for (int i = 0; i < data.Length; i++)
        m_data[i] = new ArrayDataGroup(data[i]);
    }

    public IDataGroup this[int index]
    {
      get { return m_data[index]; }
    }

    public int Count
    {
      get { return m_data.Length; }
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
