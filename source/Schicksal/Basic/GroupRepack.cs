using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Schicksal.Basic
{
  /// <summary>
  /// Перепаковка выборок в базовые выборки на основе массивов
  /// </summary>
  public static class GroupRepack
  {
    /// <summary>
    /// Перепаковка выборки
    /// </summary>
    /// <param name="group">Исходная выборка</param>
    /// <returns>Выборка на основе массива</returns>
    public static IDataGroup Wrap(IDataGroup group)
    {
      if (group == null)
        throw new ArgumentNullException("group");

      return WrapIfNeeded(group);
    }

    /// <summary>
    /// Перепаковка выборки
    /// </summary>
    /// <param name="group">Исходная выборка второго порядка</param>
    /// <returns>Выборка второго порядка на основе массивов</returns>
    public static IMultyDataGroup Wrap(IMultyDataGroup group)
    {
      if (group == null)
        throw new ArgumentNullException("group");

      var check_result = Check(group);

      if (!check_result.Repack)
        return group;

      if (check_result.SingleDimension)
      {
        double[,] array = new double[group.Count, group[0].Count];

        for (int i = 0; i < group.Count; i++)
        {
          for (int j = 0; j < group[i].Count; j++)
            array[i, j] = group[i][j];
        }

        return new TwoDimensionsMultiDataGroup(array);
      }

      var groups = new IDataGroup[group.Count];

      for (int i = 0; i < groups.Length; i++)
        groups[i] = WrapIfNeeded(group[i]);

      return new MultiArrayDataGroup(groups);
    }

    /// <summary>
    /// Перепаковка выборки
    /// </summary>
    /// <param name="group">Исходная выборка третьего порядка</param>
    /// <returns>Выборка третьего порядка на основе массивов</returns>
    public static ISetMultyDataGroup Wrap(ISetMultyDataGroup group)
    {
      if (group == null)
        throw new ArgumentNullException("group");

      if (group.SelectMany(g => g).All(g => g is ArrayDataGroup))
        return group;

      var groups = new IMultyDataGroup[group.Count];

      for (int i = 0; i < group.Count; i++)
      {
        var array = new IDataGroup[group[i].Count];

        for (int j = 0; j < array.Length; j++)
          array[j] = WrapIfNeeded(group[i][j]);

        groups[i] = new MultiArrayDataGroup(array);
      }

      return new SetMultiArrayDataGroup(groups);
    }

    private struct DimensionCheckResult
    {
      public bool Repack;
      public bool SingleDimension;

      public DimensionCheckResult(bool repack, bool singleDimension)
      {
        this.Repack = repack;
        this.SingleDimension = singleDimension;
      }
    }

    private static IDataGroup WrapIfNeeded(IDataGroup group)
    {
      var adg = group as ArrayDataGroup;

      if (adg != null)
        return adg;

      double[] array = new double[group.Count];

      for (int i = 0; i < array.Length; i++)
        array[i] = group[i];

      return new ArrayDataGroup(array);
    }

    private static DimensionCheckResult Check(IMultyDataGroup group)
    {
      bool first = true;
      bool repack = false;
      bool single = true;
      int dimension = 0;

      if (group is TwoDimensionsMultiDataGroup)
        return new DimensionCheckResult(false, true);

      for (int i = 0; i < group.Count; i++)
      {
        repack |= !(group[i] is ArrayDataGroup);

        if (first)
        {
          dimension = group[i].Count;
          first = false;
        }
        else
        {
          single &= group[i].Count == dimension;

          if (!single && repack)
            break;
        }
      }

      return new DimensionCheckResult(repack, single);
    }

    private sealed class TwoDimensionsMultiDataGroup : IMultyDataGroup
    {
      private readonly TwoDimensionsDataGroup[] m_array;

      public TwoDimensionsMultiDataGroup(double[,] array)
      {
        m_array = new TwoDimensionsDataGroup[array.GetLength(0)];

        for (int i = 0; i < m_array.Length; i++)
          m_array[i] = new TwoDimensionsDataGroup(array, i);
      }

      public IDataGroup this[int index]
      {
        get { return m_array[index]; }
      }

      public int Count
      {
        get { return m_array.Length; }
      }

      public IEnumerator<IDataGroup> GetEnumerator()
      {
        return (m_array as IList<IDataGroup>).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return m_array.GetEnumerator();
      }

      public override string ToString()
      {
        return string.Format("Array muti group, count={0}", m_array.Length);
      }

      public override bool Equals(object obj)
      {
        var other = obj as TwoDimensionsMultiDataGroup;

        if (other == null)
          return false;

        if (m_array.Length == 0 && other.m_array.Length == 0)
          return true;

        return m_array[0].Array == other.m_array[0].Array;
      }

      public override int GetHashCode()
      {
        if (m_array.Length == 0)
          return 0;
        else
          return m_array[0].Array.GetHashCode();
      }

      private sealed class TwoDimensionsDataGroup : IDataGroup
      {
        public readonly double[,] Array;
        private readonly int m_index;

        public TwoDimensionsDataGroup(double[,] array, int index)
        {
          this.Array = array;
          m_index = index;
        }

        public double this[int index]
        {
          get { return this.Array[m_index, index]; }
        }

        public int Count
        {
          get { return this.Array.GetLength(1); }
        }

        public IEnumerator<double> GetEnumerator()
        {
          int size = this.Array.GetLength(1);

          for (int i =0; i < size; i++)
            yield return this.Array[m_index, i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
          return this.GetEnumerator();
        }

        public override string ToString()
        {
          return string.Format("Array group, count={0}", this.Array.GetLength(1));
        }

        public override bool Equals(object obj)
        {
          var other = obj as TwoDimensionsDataGroup;

          if (other == null)
            return false;

          return this.Array == other.Array && m_index == other.m_index;
        }

        public override int GetHashCode()
        {
          return (m_index + 1) ^ this.Array.GetHashCode();
        }
      }
    }
  }
}