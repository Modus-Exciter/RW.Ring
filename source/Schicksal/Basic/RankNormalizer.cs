using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Schicksal.Basic
{
  /// <summary>
  /// Преобразователь данных для нормирования с использованием рангов
  /// </summary>
  public sealed class RankNormalizer : INormalizer
  {
    private readonly int m_round;

    /// <summary>
    /// Инициализация преобразователя для нормирования данных с использованием рангов
    /// </summary>
    /// <param name="round">Количество знаков после запятой для округления</param>
    public RankNormalizer(int round = -1)
    {
      m_round = round >= -1 && round <= 15 ? round : -1;
    }

    /// <summary>
    /// Количество знаков после запятой для округления
    /// </summary>
    public int Round
    {
      get { return m_round; }
    }

    /// <summary>
    /// Текстовая информация об объекте
    /// </summary>
    /// <returns>Вид преобразования и количество знаков после запятой</returns>
    public override string ToString()
    {
      return string.Format("Normalizer(value => rank, round: {0})", m_round);
    }

    /// <summary>
    /// Сравнение двух нормализаторов на равенство
    /// </summary>
    /// <param name="obj">Второй объект</param>
    /// <returns>True, если второй объект такой же нормализатор с теми же настройками. Иначе, False</returns>
    public override bool Equals(object obj)
    {
      var other = obj as RankNormalizer;

      if (other == null)
        return false;

      return m_round == other.m_round;
    }

    /// <summary>
    /// Получение хеш-кода нормализатора
    /// </summary>
    /// <returns>Побитное исключающее "или" от хеш-кодов настроек</returns>
    public override int GetHashCode()
    {
      return this.GetType().GetHashCode() ^ m_round;
    }

    public IValueTransform Prepare(ISample sample)
    {
      if (sample == null)
        throw new ArgumentNullException("sample");

      if (sample is IPlainSample)
        return this.PrepareTransform(sample as IPlainSample);

      if (sample is IDividedSample)
        return this.PrepareTransform(sample as IDividedSample);

      if (sample is IComplexSample)
        return this.PrepareTransform(sample as IComplexSample);

      return DummyNormalizer.Transform;
    }

    /// <summary>
    /// Расчёт рангов чисел в числовой последовательности
    /// </summary>
    /// <param name="data">Числовая последовательность</param>
    /// <returns>Каждому значению из числовой последовательности сопоставляется его ранг</returns>
    public Dictionary<double, float> CalculateRanks(IEnumerable<double> data)
    {
      Debug.Assert(data != null, "data cannot be null");

      return CalculateRanks(data, out _, m_round);
    }

    /// <summary>
    /// Расчёт рангов чисел в числовой последовательности
    /// </summary>
    /// <param name="data">Числовая последовательность</param>
    /// <param name="round">Количество знаков после запятой для округления</param>
    /// <returns>Каждому значению из числовой последовательности сопоставляется его ранг</returns>
    public static Dictionary<double, float> CalculateRanks(IEnumerable<double> data, int round = -1)
    {
      Debug.Assert(data != null, "data cannot be null");

      if (round < -1 || round > 15)
        round = -1;

      return CalculateRanks(data, out _, round);
    }

    #region Implementation ------------------------------------------------------------------------

    private static RankTransform GetValueTransform(IPlainSample sample)
    {
      var ns = sample as NormalizedSample;

      if (ns != null)
        return ns.ValueTransform as RankTransform;

      return null;
    }

    private IValueTransform PrepareTransform(IPlainSample sample)
    {
      var transform = GetValueTransform(sample);

      if (transform != null && transform.Round == m_round && transform.Internal)
        return transform;

      bool has_reason;
      var ranks = CalculateRanks(sample, out has_reason, m_round);

      if (has_reason)
        return new RankTransform(ranks, m_round, true);
      else
        return transform != null ? new RankTransform(transform.Ranks, m_round, true) 
          : DummyNormalizer.Transform;
    }

    private IValueTransform PrepareTransform(IDividedSample sample)
    {
      Debug.Assert(sample != null, "sample cannot be null");

      if (RecreateRequired(sample, m_round))
      {
        bool has_reason;
        var ranks = CalculateRanks(sample.SelectMany(g => g), out has_reason, m_round);

        if (has_reason)
          return new RankTransform(ranks, m_round, false);
        else
          return DummyNormalizer.Transform;
      }
      else if (sample.Count > 0)
        return GetValueTransform(sample[0]);
      else
        return DummyNormalizer.Transform;
    }

    private IValueTransform PrepareTransform(IComplexSample sample)
    {
      Debug.Assert(sample != null, "sample cannot be null");

      if (RecreateRequired(sample.SelectMany(g => g), m_round))
      {
        bool has_reason;
        var ranks = CalculateRanks(sample.SelectMany(g => g).SelectMany(g => g), out has_reason, m_round);

        if (has_reason)
          return new RankTransform(ranks, m_round, false);
        else
          return DummyNormalizer.Transform;
      }
      else if (sample.Count > 0 && sample[0].Count > 0)
        return GetValueTransform(sample[0][0]);
      else
        return DummyNormalizer.Transform;
    }

    private static bool RecreateRequired(IEnumerable<IPlainSample> samples, int round)
    {
      Dictionary<double, float> ranks = null;
      bool first = true;

      foreach (var sample in samples)
      {
        var ranked = GetValueTransform(sample);

        if (ranked == null)
          return true;

        if (ranked.Round != round)
          return true;

        if (ranked.Internal)
          return false;

        if (first)
        {
          first = false;
          ranks = ranked.Ranks;
        }
        else if (!ReferenceEquals(ranked.Ranks, ranks))
          return true;
      }

      return false;
    }

    private static Dictionary<double, float> CalculateRanks(IEnumerable<double> data, out bool hasReason, int round)
    {
      float rank = 1;
      hasReason = false;
      var ranks = new Dictionary<double, float>();

      if (round >= 0)
        data = data.Select(a => Math.Round(a, round));

      var list = data.ToList();

      if (list.Count == 0)
        return ranks;

      list.Sort();

      double last = list[0];
      int count = 1;

      for (int i = 1; i < list.Count; i++)
      {
        if (list[i] == last)
          count++;
        else
        {
          var current_rank = rank + (count - 1f) / 2f;
          ranks[last] = current_rank;
          hasReason |= (current_rank != last);
          rank += count;
          last = list[i];
          count = 1;
        }
      }

      var last_rank = rank + (count - 1f) / 2f;
      ranks[last] = last_rank;
      hasReason |= (last_rank != last);

      return ranks;
    }

    private sealed class RankTransform : IValueTransform
    {
      private readonly Dictionary<double, float> m_ranks;
      private readonly int m_round;
      private readonly bool m_internal;

      private RankInverse m_inverse;

      public RankTransform(Dictionary<double, float> ranks, int round, bool isInternal)
      {
        m_ranks = ranks;
        m_round = round;
        m_internal = isInternal;
      }

      public Dictionary<double, float> Ranks
      {
        get { return m_ranks; }
      }

      public int Round
      {
        get { return m_round; }
      }

      public bool Internal
      {
        get { return m_internal; }
      }

      public double Normalize(double value)
      {
        return m_ranks[value];
      }

      public double Denormalize(double value)
      {
        if (m_inverse == null)
        {
          lock (m_ranks)
          {
            if (m_inverse == null)
              m_inverse = new RankInverse(m_ranks);
          }
        }

        return m_inverse.Denormalize(value);
      }

      public override string ToString()
      {
        return string.Format("Ranking, round: {0}, ranks code: {1}", m_round, m_ranks.GetHashCode());
      }

      public override bool Equals(object obj)
      {
        var other = obj as RankTransform;

        if (other == null)
          return false;
        
        return m_ranks.Equals(other.Ranks) && m_round == other.Round && m_internal == other.Internal;
      }

      public override int GetHashCode()
      {
        return m_ranks.GetHashCode() ^ m_round ^ m_internal.GetHashCode();
      }
    }

    private sealed class RankInverse
    {
      private readonly float[] m_ranks;
      private readonly double[] m_values;
      private readonly double m_min = double.NaN;
      private readonly double m_max;
      private readonly double m_max_rank = 1;

      public RankInverse(Dictionary<double, float> ranks)
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

      public double Denormalize(double value)
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

    #endregion
  }
}