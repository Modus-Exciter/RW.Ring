using System;
using System.Collections;
using System.Collections.Generic;

namespace Notung.Data
{
  /// <summary>
  /// Коллекция, обеспечивающая доступ к элементам как по ключу, так и по значению
  /// </summary>
  /// <typeparam name="T">Тип элемента коллекции</typeparam>
  public class KeyedArray<T> : IEnumerable<T>
  {
    private readonly T[] m_array;
    private readonly Dictionary<T, int> m_indexes;

    /// <summary>
    /// Инициализация коллекции
    /// </summary>
    /// <param name="count">Размер коллекции</param>
    /// <param name="getter">Функция, возвращающая ключ для каждого индекса</param>
    /// <param name="comparer">Метод сравнения ключей</param>
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

    /// <summary>
    /// Инициализация коллекции
    /// </summary>
    /// <param name="collection">Список ключей</param>
    /// <param name="comparer">Метод сравнения ключей</param>
    public KeyedArray(IList<T> collection, IEqualityComparer<T> comparer)
      : this(collection.Count, i => collection[i], comparer) { }

    /// <summary>
    /// Инициализация коллекции
    /// </summary>
    /// <param name="count">Размер коллекции</param>
    /// <param name="getter">Функция, возвращающая ключ для каждого индекса</param>
    public KeyedArray(int count, Func<int, T> getter)
      : this(count, getter, EqualityComparer<T>.Default) { }

    /// <summary>
    /// Инициализация коллекции
    /// </summary>
    /// <param name="collection">Список ключей</param>
    public KeyedArray(IList<T> collection)
      : this(collection, EqualityComparer<T>.Default) { }

    /// <summary>
    /// Количество ключей
    /// </summary>
    public int Count
    {
      get { return m_array.Length; }
    }

    /// <summary>
    /// Получение индекса ключа
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <returns>Индекс</returns>
    public int GetIndex(T key)
    {
      return m_indexes[key];
    }

    /// <summary>
    /// Получение ключа по индексу
    /// </summary>
    /// <param name="index">Индекс ключа</param>
    /// <returns>Кюч с запрошенным индексом</returns>
    public T GetKey(int index)
    {
      return m_array[index];
    }

    /// <summary>
    /// Определяет, содержится ли указанный ключ в коллекции
    /// </summary>
    /// <param name="key">Ключ, который требуется найти в коллекции</param>
    /// <returns>true, если коллекция содержит указанный ключ, в противном случае — false.</returns>
    public bool Contains(T key)
    {
      return m_indexes.ContainsKey(key);
    }

    /// <summary>
    /// Получение итератора для обхода всех ключей
    /// </summary>
    /// <returns>Итератор, перебирающий ключи в порядке возрастания их индексов</returns>
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