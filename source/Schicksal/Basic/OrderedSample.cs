﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Schicksal.Basic
{
  /// <summary>
  /// Выборка данных, отсортированная по возрастанию или убыванию
  /// </summary>
  public abstract class OrderedSample : IPlainSample
  {
    private readonly IPlainSample m_source;
    private readonly ListSortDirection m_direction;

    private OrderedSample(IPlainSample source, ListSortDirection direction)
    {
      m_source = source;
      m_direction = direction;
    }

    /// <summary>
    /// Обращение к элементу выборки по номеру
    /// </summary>
    /// <param name="index">Порядковый номер элемента выборки</param>
    /// <returns>Значение элемента выборки</returns>
    public double this[int index]
    {
      get { return m_source[this.GetIndex(index)]; }
    }

    /// <summary>
    /// Объём выборки
    /// </summary>
    public int Count
    {
      get { return m_source.Count; }
    }

    /// <summary>
    /// Порядок, в котором отсортирована выборка
    /// </summary>
    public ListSortDirection SortDirection
    {
      get { return m_direction; }
    }

    /// <summary>
    /// Получение итератора для перебора всех значений в выборке
    /// </summary>
    /// <returns>Итератор элементов выборки</returns>
    public abstract IEnumerator<double> GetEnumerator();

    /// <summary>
    /// Получение итератора для перебора всех значений в выборке
    /// </summary>
    /// <returns>Итератор элементов выборки</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    /// <summary>
    /// Строковое представление отсортированной выборки
    /// </summary>
    /// <returns>Источник с направлением сортировки</returns>
    public override string ToString()
    {
      return string.Format("{0}, ordered by {1}", m_source, m_direction);
    }

    /// <summary>
    /// Сравнение на эквивалентность
    /// </summary>
    /// <param name="obj">Объект, с которым сравнивается выборка</param>
    /// <returns>True, если другой объект - это та же выборка, отсортированная в том же направлении. Иначе, False</returns>
    public override bool Equals(object obj)
    {
      var other = obj as OrderedSample;

      if (other == null || ReferenceEquals(this, obj))
        return other != null;

      if (m_direction != other.m_direction)
        return false;

      return m_source.Equals(other.m_source);
    }

    /// <summary>
    /// Получение хеш-кода для отсортированной выборки
    /// </summary>
    /// <returns>Число, зависящее от исходной выборки и направления сортировки</returns>
    public override int GetHashCode()
    {
      return m_direction.GetHashCode() ^ m_source.GetHashCode();
    }

    /// <summary>
    /// Запуск повторной сортировки (требуется при изменении данных в источнике)
    /// </summary>
    public abstract void Resort();

    protected abstract int GetIndex(int index);

    /// <summary>
    /// Создание нового экземпляра отсортированной выборки
    /// </summary>
    /// <param name="sample">Не отсортированная выборка</param>
    /// <param name="direction">Направление сортировки</param>
    /// <returns>Выборка, отсортированная в указанном направлении</returns>
    public static OrderedSample Construct(IPlainSample sample, ListSortDirection direction = ListSortDirection.Ascending)
    {
      if (sample == null)
        throw new ArgumentNullException("sample");

      if (sample is OrderedSample && ((OrderedSample)sample).m_direction == direction)
        return (OrderedSample)sample;
      
      if (sample.Count <= byte.MaxValue)
        return new ByteOrderedSample(sample, direction);

      if (sample.Count <= ushort.MaxValue)
        return new UInt16OrderedSample(sample, direction);

      return new Int32OrderedSample(sample, direction);
    }

    #region Implementation

    private sealed class Int32OrderedSample : OrderedSample
    {
      private readonly int[] m_indexes;

      public Int32OrderedSample(IPlainSample source, ListSortDirection direction) : base(source, direction)
      {
        m_indexes = new int[source.Count];

        for (int i = 1; i < m_indexes.Length; i++)
          m_indexes[i] = i;

        this.Resort();
      }

      private int Compare(int a, int b)
      {
        return m_source[a].CompareTo(m_source[b]);
      }

      private int CompareBack(int a, int b)
      {
        return m_source[b].CompareTo(m_source[a]);
      }

      protected override int GetIndex(int index)
      {
        return m_indexes[index];
      }

      public override IEnumerator<double> GetEnumerator()
      {
        return m_indexes.Select(i => m_source[i]).GetEnumerator();
      }

      public sealed override void Resort()
      {
        if (m_direction == ListSortDirection.Ascending)
          Array.Sort(m_indexes, this.Compare);
        else
          Array.Sort(m_indexes, this.CompareBack);
      }
    }

    private sealed class UInt16OrderedSample : OrderedSample
    {
      private readonly ushort[] m_indexes;

      public UInt16OrderedSample(IPlainSample source, ListSortDirection direction) : base(source, direction)
      {
        m_indexes = new ushort[source.Count];

        for (ushort i = 1; i < m_indexes.Length; i++)
          m_indexes[i] = i;

        this.Resort();
      }

      private int Compare(ushort a, ushort b)
      {
        return m_source[a].CompareTo(m_source[b]);
      }

      private int CompareBack(ushort a, ushort b)
      {
        return m_source[b].CompareTo(m_source[a]);
      }

      protected override int GetIndex(int index)
      {
        return m_indexes[index];
      }

      public override IEnumerator<double> GetEnumerator()
      {
        return m_indexes.Select(i => m_source[i]).GetEnumerator();
      }

      public sealed override void Resort()
      {
        if (m_direction == ListSortDirection.Ascending)
          Array.Sort(m_indexes, this.Compare);
        else
          Array.Sort(m_indexes, this.CompareBack);
      }
    }

    private sealed class ByteOrderedSample: OrderedSample
    {
      private readonly byte[] m_indexes;

      public ByteOrderedSample(IPlainSample source, ListSortDirection direction) : base(source, direction)
      {
        m_indexes = new byte[source.Count];

        for (byte i = 1; i < m_indexes.Length; i++)
          m_indexes[i] = i;

        this.Resort();
      }

      private int Compare(byte a, byte b)
      {
        return m_source[a].CompareTo(m_source[b]);
      }

      private int CompareBack(byte a, byte b)
      {
        return m_source[b].CompareTo(m_source[a]);
      }

      protected override int GetIndex(int index)
      {
        return m_indexes[index];
      }

      public override IEnumerator<double> GetEnumerator()
      {
        return m_indexes.Select(i => m_source[i]).GetEnumerator();
      }

      public sealed override void Resort()
      {
        if (m_direction == ListSortDirection.Ascending)
          Array.Sort(m_indexes, this.Compare);
        else
          Array.Sort(m_indexes, this.CompareBack);
      }
    }

    #endregion
  }
}