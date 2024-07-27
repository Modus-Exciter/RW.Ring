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

      if (check_result.EqualGroups)
        return RepackRectangleMultiDataGroup(group);

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

      var check_result = Check(group);

      if (!check_result.Repack)
        return group;

      if (check_result.EqualGroups && check_result.EqualMultiGroups)
        return RepackRectangleSetMultiDataGroup(group);

      var groups = new IMultyDataGroup[group.Count];

      for (int i = 0; i < group.Count; i++)
      {
        if (check_result.EqualGroups)
        {
          groups[i] = RepackRectangleMultiDataGroup(group[i]);
        }
        else
        {
          var array = new IDataGroup[group[i].Count];

          for (int j = 0; j < array.Length; j++)
            array[j] = WrapIfNeeded(group[i][j]);

          groups[i] = new MultiArrayDataGroup(array);
        }
      }

      return new SetMultiArrayDataGroup(groups);
    }

    #region Implementation ------------------------------------------------------------------------

    private struct DimensionCheckResult
    {
      public bool Repack;
      public bool EqualGroups;
      public bool EqualMultiGroups;

      public DimensionCheckResult(bool repack, bool equalGroups, bool equalMultiGroups)
      {
        this.Repack = repack;
        this.EqualGroups = equalGroups;
        this.EqualMultiGroups = equalMultiGroups;
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

    private static IMultyDataGroup RepackRectangleMultiDataGroup(IMultyDataGroup group)
    {
      int count = group.Count > 0 ? group[0].Count : 0;

      double[,] array = new double[group.Count, count];

      for (int i = 0; i < group.Count; i++)
      {
        for (int j = 0; j < count; j++)
          array[i, j] = group[i][j];
      }

      return new TwoDimensionsMultiDataGroup(array);
    }

    private static ISetMultyDataGroup RepackRectangleSetMultiDataGroup(ISetMultyDataGroup group)
    {
      int count1 = group.Count > 0 ? group[0].Count : 0;
      int count2 = count1 > 0 ? group[0][0].Count : 0;

      double[,,] array = new double[group.Count, count1, count2];

      for (int i = 0; i < group.Count; i++)
      {
        for (int j = 0; j < group[i].Count; j++)
        {
          for (int k = 0; k < group[i][j].Count; k++)
            array[i, j, k] = group[i][j][k];
        }
      }

      return new ThreeDimensionsSetDataGroup(array);
    }

    private static DimensionCheckResult Check(IMultyDataGroup group)
    {
      bool first = true;
      bool repack = false;
      bool equal = true;
      int dimension = 0;

      if (group is TwoDimensionsMultiDataGroup)
        return new DimensionCheckResult(false, true, true);

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
          equal &= group[i].Count == dimension;

          if (!equal && repack)
            break;
        }
      }

      return new DimensionCheckResult(repack, equal, true);
    }

    private static DimensionCheckResult Check(ISetMultyDataGroup group)
    {
      bool first = true;
      bool repack = false;
      bool equal1 = true;
      bool equal2 = true;
      int dimension = 0;

      for (int i = 0; i < group.Count; i++)
      {
        var inner_check = Check(group[i]);

        repack |= inner_check.Repack;
        equal1 &= inner_check.EqualGroups;

        if (first)
        {
          dimension = group[i].Count;
          first = false;
        }
        else
        {
          equal2 &= group[i].Count == dimension;

          if (!equal2 && repack)
            break;
        }
      }

      return new DimensionCheckResult(repack, equal1, equal2);
    }

    private sealed class TwoDimensionsMultiDataGroup : IMultyDataGroup, IEqualSubGroups
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

      public int SubGroupSize
      {
        get { return m_array.Length > 0 ? m_array[0].Count : 0; }
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

        for (int i = 0; i < size; i++)
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

    private sealed class ThreeDimensionsSetDataGroup : ISetMultyDataGroup, IEqualSubGroups
    {
      private readonly ThreeDimensionsMultiDataGroup[] m_groups;
      private readonly double[,,] m_data;

      public ThreeDimensionsSetDataGroup(double[,,] data)
      {
        m_data = data;
        m_groups = new ThreeDimensionsMultiDataGroup[data.GetLength(0)];

        for (int i = 0; i < m_groups.Length; i++)
          m_groups[i] = new ThreeDimensionsMultiDataGroup(data, i);
      }

      public IMultyDataGroup this[int index]
      {
        get { return m_groups[index]; }
      }

      public int Count
      {
        get { return m_groups.Length; }
      }

      public int SubGroupSize
      {
        get { return m_data.GetLength(1); }
      }

      public IEnumerator<IMultyDataGroup> GetEnumerator()
      {
        return (m_groups as IList<IMultyDataGroup>).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return this.GetEnumerator();
      }

      public override string ToString()
      {
        return string.Format("Array group set, count={0}", m_data.GetLength(0));
      }

      public override bool Equals(object obj)
      {
        var other = obj as ThreeDimensionsSetDataGroup;

        if (other == null)
          return false;

        return Equals(m_data, other.m_data);
      }

      public override int GetHashCode()
      {
        return m_data.GetHashCode();
      }
    }

    private sealed class ThreeDimensionsMultiDataGroup : IMultyDataGroup, IEqualSubGroups
    {
      private readonly ThreeDimensionsDataGroup[] m_data;

      public ThreeDimensionsMultiDataGroup(double[,,] data, int index)
      {
        m_data = new ThreeDimensionsDataGroup[data.GetLength(1)];

        for (int i = 0; i < m_data.Length; i++)
          m_data[i] = new ThreeDimensionsDataGroup(data, index, i);
      }

      public IDataGroup this[int index]
      {
        get { return m_data[index]; }
      }

      public int Count
      {
        get { return m_data.Length; }
      }

      public int SubGroupSize
      {
        get { return m_data.Length > 0 ? m_data[0].Array.GetLength(2) : 0; }
      }

      public IEnumerator<IDataGroup> GetEnumerator()
      {
        return (m_data as IList<IDataGroup>).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return this.GetEnumerator();
      }

      public override string ToString()
      {
        return string.Format("Array muti group, count={0}", m_data.Length);
      }

      public override bool Equals(object obj)
      {
        var other = obj as ThreeDimensionsMultiDataGroup;

        if (other == null)
          return false;

        if (other == null)
          return false;

        if (m_data.Length == 0 && other.m_data.Length == 0)
          return true;

        return m_data[0].Array == other.m_data[0].Array;
      }

      public override int GetHashCode()
      {
        if (m_data.Length == 0)
          return 0;
        else
          return m_data[0].Array.GetHashCode() ^ m_data[0].Index1;
      }
    }

    private sealed class ThreeDimensionsDataGroup : IDataGroup
    {
      public readonly double[,,] Array;
      private readonly int m_index1;
      private readonly int m_index2;

      public ThreeDimensionsDataGroup(double[,,] array, int index1, int index2)
      {
        this.Array = array;
        m_index1 = index1;
        m_index2 = index2;
      }

      public int Index1
      {
        get { return m_index1; }
      }

      public double this[int index]
      {
        get { return this.Array[m_index1, m_index2, index]; }
      }

      public int Count
      {
        get { return this.Array.GetLength(2); }
      }

      public IEnumerator<double> GetEnumerator()
      {
        var size = this.Array.GetLength(2);

        for (int i = 0; i < size; i++)
          yield return this.Array[m_index1, m_index2, i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return this.GetEnumerator();
      }

      public override string ToString()
      {
        return string.Format("Array group, count={0}", this.Array.GetLength(2));
      }

      public override bool Equals(object obj)
      {
        var other = obj as ThreeDimensionsDataGroup;

        if (other == null)
          return false;

        return this.Array == other.Array && m_index1 == other.m_index1 && m_index2 == other.m_index2;
      }

      public override int GetHashCode()
      {
        return (m_index1 + 1) ^ (m_index2 + 3) ^ this.Array.GetHashCode();
      }
    }

    #endregion
  }
}