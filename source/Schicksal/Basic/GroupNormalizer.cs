using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Schicksal.Basic
{
  /// <summary>
  /// Методы нормирования данных
  /// </summary>
  public static class GroupNormalizer
  {
    /// <summary>
    /// Расчёт рангов чисел в числовой последовательности
    /// </summary>
    /// <param name="data">Числовая последовательность</param>
    /// <param name="round">Количество знаков после запятой для округления</param>
    /// <returns>Каждому значению из числовой последовательности сопоставляется его ранг</returns>
    public static Dictionary<double, float> CalculateRanks(IEnumerable<double> data, int round = -1)
    {
      Debug.Assert(data != null, "data cannot be null");

      float rank = 1;
      var ranks = new Dictionary<double, float>();

      if (round >= 0)
        data = data.Select(a => Math.Round(a, round));

      foreach (var group in data.GroupBy(p => p).OrderBy(g => g.Key))
      {
        var count = group.Count();
        ranks[group.Key] = rank + (count - 1f) / 2f;
        rank += count;
      }

      return ranks;
    }

    /// <summary>
    /// Преобразование группы значений в группу рангов
    /// </summary>
    /// <param name="group">Исходная группа</param>
    /// <param name="round">Количество знаков после запятой для округления</param>
    /// <returns>Преобразованная группа</returns>
    public static IDataGroup NormalizeByRanks(IDataGroup group, int round = -1)
    {
      Debug.Assert(group != null, "group cannot be null");

      if (group is RankedGroup)
      {
        if (((RankedGroup)group).Round == round && ((RankedGroup)group).Internal)
          return group;

        return new RankedGroup(((RankedGroup)group).Source, round);
      }

      return new RankedGroup(group, round);
    }

    /// <summary>
    /// Преобразование группы значений второго порядка в группу рангов
    /// </summary>
    /// <param name="group">Исходная группа</param>
    /// <param name="round">Количество знаков после запятой для округления</param>
    /// <returns>Преобразованная группа</returns>
    public static IMultyDataGroup NormalizeByRanks(IMultyDataGroup multyGroup, int round = -1)
    {
      Debug.Assert(multyGroup != null, "multyGroup cannot be null");

      if (RecreateRequired(multyGroup, round))
      {
        var ranks = CalculateRanks(multyGroup.SelectMany(g => g));
        var groups = new IDataGroup[multyGroup.Count];

        for (int i = 0; i < groups.Length; i++)
          groups[i] = new RankedGroup(multyGroup[i], round, ranks);

        return new MultiArrayDataGroup(groups);
      }
      else
        return multyGroup;
    }

    /// <summary>
    /// Преобразование группы значений третьего порядка в группу рангов
    /// </summary>
    /// <param name="group">Исходная группа</param>
    /// <param name="round">Количество знаков после запятой для округления</param>
    /// <returns>Преобразованная группа</returns>
    public static ISetMultyDataGroup NormalizeByRanks(ISetMultyDataGroup group, int round = -1)
    {
      Debug.Assert(group != null, "group cannot be null");

      if (RecreateRequired(group.SelectMany(g => g), round))
      {
        var ranks = CalculateRanks(group.SelectMany(g => g).SelectMany(g => g));
        var groups = new IMultyDataGroup[group.Count];

        for (int i = 0; i < group.Count; i++)
        {
          var array = new IDataGroup[group[i].Count];

          for (int j = 0; j < group[i].Count; j++)
            array[j] = new RankedGroup(group[i][j], round, ranks);

          groups[i] = new MultiArrayDataGroup(array);
        }

        return new SetMultiArrayDataGroup(groups);
      }
      else
        return group;
    }

    /// <summary>
    /// Расчёт изначального значения по рангу
    /// </summary>
    /// <param name="group">Группа рангов</param>
    /// <returns>Метод, преобразующий ранг в значение с таким рангом</returns>
    public static Func<double, double> CreateInverseHandler(IDataGroup group)
    {
      Debug.Assert(group != null, "group cannot be null");

      var ranked = group as RankedGroup;

      if (ranked == null)
        return _default_handler;

      return new InverseStore(ranked.Ranks).Find;
    }

    #region Implementation ------------------------------------------------------------------------

    private static Func<double, double> _default_handler = a => a;

    private static bool RecreateRequired(IEnumerable<IDataGroup> multyGroup, int round)
    {
      Dictionary<double, float> ranks = null;
      bool first = true;

      foreach (var group in multyGroup)
      {
        if (!(group is RankedGroup))
          return true;

        if (((RankedGroup)group).Round != round)
          return true;

        if (first)
        {
          first = false;
          ranks = ((RankedGroup)group).Ranks;
        }
        else if (!object.ReferenceEquals(((RankedGroup)group).Ranks, ranks))
          return true;
      }

      return false;
    }

    private class InverseStore
    {
      private readonly float[] m_ranks;
      private readonly double[] m_values;
      private readonly double m_min = double.NaN;
      private readonly double m_max;
      private readonly double m_max_rank = 1;

      public InverseStore(Dictionary<double, float> ranks)
      {
        m_ranks = new float[ranks.Count];
        m_values = new double[ranks.Count];

        int i = 0;
        foreach (var kv in ranks.OrderBy(kvp => kvp.Value))
        {
          m_ranks[i] = kv.Value;
          m_values[i++] = kv.Key;

          if (double.IsNaN(m_min))
          {
            m_min = kv.Key;
            m_max = kv.Key;
          }
          else
          {
            if (m_min > kv.Key)
              m_min = kv.Key;
            else if (m_max < kv.Key)
              m_max = kv.Key;
          }

          if (m_max_rank < kv.Value)
            m_max_rank = kv.Value;
        }
      }

      public double Find(double value)
      {
        int l = 0;
        int r = m_ranks.Length - 1;

        if (value < 1)
          return m_min + (value - 1) * (m_max - m_min) / m_max_rank;

        if (value > m_max_rank)
          return m_max + (value - m_max_rank) * (m_max - m_min) / m_max_rank;

        while (l <= r)
        {
          int mid = (l + r) / 2;

          if (m_ranks[mid] < value)
            l = mid + 1;
          else if (m_ranks[mid] > value)
            r = mid - 1;
          else
            return m_values[mid];
        }

        while (l > 0 && m_ranks[l] > value)
          l--;

        return (value - m_ranks[l]) / (m_ranks[l + 1] - m_ranks[l]) * (m_values[l + 1] - m_values[l]) + m_values[l];
      }
    }

    private sealed class RankedGroup : IDataGroup
    {
      public readonly Dictionary<double, float> Ranks;
      public readonly int Round;
      public readonly IDataGroup Source;
      public readonly bool Internal;

      public RankedGroup(IDataGroup source, int round, Dictionary<double, float> ranks)
      {
        this.Source = source;
        this.Round = round;
        this.Ranks = ranks;
      }

      public RankedGroup(IDataGroup source, int round)
        : this(source, round, CalculateRanks(source, round))
      {
        this.Internal = true;
      }

      public double this[int index]
      {
        get { return this.Ranks[this.Source[index]]; }
      }

      public int Count
      {
        get { return this.Source.Count; }
      }

      public IEnumerator<double> GetEnumerator()
      {
        return this.Source.Select(a => (double)this.Ranks[a]).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return this.GetEnumerator();
      }

      public override string ToString()
      {
        return string.Format("Ranked {0}", this.Source);
      }
    }

    #endregion
  }
}