﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Schicksal.Properties;

namespace Schicksal.Basic
{
  /// <summary>
  /// Преобразователь данных для нормирования с использованием рангов
  /// </summary>
  public sealed class RankNormalizer : INormalizer, IDenormalizerFactory
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
    /// Расчёт изначального значения по преобразованному
    /// </summary>
    /// <param name="sample">Группа нормированных значений</param>
    /// <returns>Обратный преобразователь нормированных данных</returns>
    public IDenormalizer GetDenormalizer(ISample sample)
    {
      Debug.Assert(sample != null, "sample cannot be null");

      return DenormalizationHelper.GetDenormalizer(sample, this);
    }

    /// <summary>
    /// Преобразование группы значений в группу рангов
    /// </summary>
    /// <param name="sample">Исходная группа</param>
    /// <returns>Преобразованная группа</returns>
    public IPlainSample Normalize(IPlainSample sample)
    {
      Debug.Assert(sample != null, "sample cannot be null");

      if (sample is RankedSample)
      {
        if (((RankedSample)sample).Round == m_round && ((RankedSample)sample).Internal)
          return sample;

        return new RankedSample(((RankedSample)sample).Source, m_round);
      }

      return new RankedSample(sample, m_round);
    }

    /// <summary>
    /// Преобразование группы значений второго порядка в группу рангов
    /// </summary>
    /// <param name="sample">Исходная группа</param>
    /// <returns>Преобразованная группа</returns>
    public IDividedSample Normalize(IDividedSample sample)
    {
      Debug.Assert(sample != null, "sample cannot be null");

      if (RecreateRequired(sample, m_round))
      {
        bool has_reason;
        var ranks = CalculateRanks(sample.SelectMany(g => g), out has_reason, m_round);

        if (has_reason)
        {
          var samples = new IPlainSample[sample.Count];

          for (int i = 0; i < samples.Length; i++)
            samples[i] = new RankedSample(sample[i], m_round, ranks);

          return new ArrayDividedSample(samples);
        }
      }

      return sample;
    }

    /// <summary>
    /// Преобразование группы значений третьего порядка в группу рангов
    /// </summary>
    /// <param name="sample">Исходная группа</param>
    /// <returns>Преобразованная группа третьего порядка</returns>
    public IComplexSample Normalize(IComplexSample sample)
    {
      Debug.Assert(sample != null, "sample cannot be null");

      if (RecreateRequired(sample.SelectMany(g => g), m_round))
      {
        bool has_reason;
        var ranks = CalculateRanks(sample.SelectMany(g => g).SelectMany(g => g), out has_reason, m_round);

        if (has_reason)
        {
          var samples = new IDividedSample[sample.Count];

          for (int i = 0; i < sample.Count; i++)
          {
            var array = new IPlainSample[sample[i].Count];

            for (int j = 0; j < sample[i].Count; j++)
              array[j] = new RankedSample(sample[i][j], m_round, ranks);

            samples[i] = new ArrayDividedSample(array);
          }

          return new ArrayComplexSample(samples);
        }
      }

      return sample;
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

    bool IDenormalizerFactory.IsNormalized(IPlainSample sample)
    {
      return sample is RankedSample;
    }

    IDenormalizer IDenormalizerFactory.GetDenormalizer(IPlainSample sample)
    {
      var ranked = sample as RankedSample;

      return new RankInverse(ranked.Ranks);
    }

    IDenormalizer IDenormalizerFactory.GetDenormalizer(IDividedSample sample)
    {
      var ranked = sample.FirstOrDefault() as RankedSample;

      if (RecreateRequired(sample, ranked.Round))
        throw new InvalidOperationException(Resources.NO_JOINT_RANKS);
      else
        return new RankInverse(ranked.Ranks);
    }

    IDenormalizer IDenormalizerFactory.GetDenormalizer(IComplexSample sample)
    {
      var ranked = sample.SelectMany(g => g).FirstOrDefault() as RankedSample;

      if (RecreateRequired(sample.SelectMany(g => g), ranked.Round))
        throw new InvalidOperationException(Resources.NO_JOINT_RANKS);
      else
        return new RankInverse(ranked.Ranks);
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

    private static bool RecreateRequired(IEnumerable<IPlainSample> samples, int round)
    {
      Dictionary<double, float> ranks = null;
      bool first = true;

      foreach (var sample in samples)
      {
        var ranked = sample as RankedSample;

        if (ranked == null)
          return true;

        if (ranked.Round != round)
          return true;

        if (first)
        {
          first = false;
          ranks = ranked.Ranks;
        }
        else if (!object.ReferenceEquals(ranked.Ranks, ranks))
          return true;
      }

      return false;
    }

    private sealed class RankedSample : IPlainSample
    {
      public readonly Dictionary<double, float> Ranks;
      public readonly int Round;
      public readonly IPlainSample Source;
      public readonly bool Internal;

      public RankedSample(IPlainSample source, int round, Dictionary<double, float> ranks)
      {
        this.Source = source;
        this.Round = round;
        this.Ranks = ranks;
      }

      public RankedSample(IPlainSample source, int round)
        : this(source, round, CalculateRanks(source, out _, round))
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

      public override bool Equals(object obj)
      {
        var other = obj as RankedSample;

        if (other == null)
          return false;

        if (!this.Source.Equals(other.Source))
          return false;

        if (this.Round != other.Round)
          return false;

        if (!ReferenceEquals(this.Ranks, other.Ranks))
          return false;

        return true;
      }

      public override int GetHashCode()
      {
        return this.Source.GetHashCode() ^ this.Round ^ this.Ranks.GetHashCode();
      }
    }

    private sealed class RankInverse : IDenormalizer
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

      public override string ToString()
      {
        return "Denormalizer(rank => value)";
      }
    }
  }

  #endregion
}