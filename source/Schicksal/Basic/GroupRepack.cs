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

      if (check_result == DimensionCheckResult.NoRepack)
        return group;

      if (check_result == DimensionCheckResult.SingleDimensions)
      {
        double[,] array = new double[group.Count, group[0].Count];

        for (int i = 0; i < group.Count; i++)
        {
          for (int j = 0; j < group[i].Count; j++)
            array[i, j] = group[i][j];
        }

        return new TwoDimensionMultiDataGroup(array);
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

    private enum DimensionCheckResult
    {
      NoRepack,
      SingleDimensions,
      DifferentDimensions
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

      if (!repack)
        return DimensionCheckResult.NoRepack;
      else if (single)
        return DimensionCheckResult.SingleDimensions;
      else
        return DimensionCheckResult.DifferentDimensions;
    }

    private sealed class TwoDimensionMultiDataGroup : IMultyDataGroup
    {
      private readonly TwoDimensionDataGroup[] m_array;

      public TwoDimensionMultiDataGroup(double[,] array)
      {
        m_array = new TwoDimensionDataGroup[array.GetLength(0)];

        for (int i = 0; i < m_array.Length; i++)
          m_array[i] = new TwoDimensionDataGroup(array, i);
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

      private sealed class TwoDimensionDataGroup : IDataGroup
      {
        private readonly double[,] m_array;
        private readonly int m_index;

        public TwoDimensionDataGroup(double[,] array, int index)
        {
          m_array = array;
          m_index = index;
        }

        public double this[int index]
        {
          get { return m_array[m_index, index]; }
        }

        public int Count
        {
          get { return m_array.GetLength(1); }
        }

        public IEnumerator<double> GetEnumerator()
        {
          for (int i =0; i < m_array.GetLength(1); i++)
            yield return m_array[m_index, i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
          return this.GetEnumerator();
        }

        public override string ToString()
        {
          return string.Format("Array group, count={0}", m_array.GetLength(1));
        }
      }
    }
  }
}