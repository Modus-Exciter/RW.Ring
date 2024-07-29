using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Basic
{
  class Tmp
  {
    /*
    /// <summary>
    /// Расчёт изначального значения по преобразованному
    /// </summary>
    /// <param name="sample">Группа нормированных значений</param>
    /// <returns>Обратный преобразователь нормированных данных</returns>
    public IValueTransform Prepare(ISample sample)
    {
      Debug.Assert(sample != null, "sample cannot be null");

      return this.ExtractProperty(sample) ?? DummyNormalizer.Instance.Prepare(sample);
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


    #region ISamplePropertyExtractor<IDenormalizer> members ---------------------------------------

    bool ISamplePropertyExtractor<IValueTransform>.HasProperty(IPlainSample sample)
    {
      return sample is RankedSample;
    }

    IValueTransform ISamplePropertyExtractor<IValueTransform>.Extract(IPlainSample sample)
    {
      var ranked = sample as RankedSample;

      return new RankInverse(ranked.Ranks);
    }

    IValueTransform ISamplePropertyExtractor<IValueTransform>.Extract(IDividedSample sample)
    {
      var ranked = sample.FirstOrDefault() as RankedSample;

      if (RecreateRequired(sample, ranked.Round))
        throw new InvalidOperationException(Resources.NO_JOINT_RANKS);
      else
        return new RankInverse(ranked.Ranks);
    }

    IValueTransform ISamplePropertyExtractor<IValueTransform>.Extract(IComplexSample sample)
    {
      var ranked = sample.SelectMany(g => g).FirstOrDefault() as RankedSample;

      if (RecreateRequired(sample.SelectMany(g => g), ranked.Round))
        throw new InvalidOperationException(Resources.NO_JOINT_RANKS);
      else
        return new RankInverse(ranked.Ranks);
    }

    #endregion

    #region Implementation ------------------------------------------------------------------------

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

  }

  #endregion
*/
    
    #region Box-Cox

    /*

    /// <summary>
    /// Расчёт изначального значения по преобразованному
    /// </summary>
    /// <param name="sample">Группа нормированных значений</param>
    /// <returns>Обратный преобразователь нормированных данных</returns>
    public IDenormalizer GetDenormalizer(ISample sample)
    {
      Debug.Assert(sample != null, "sample cannot be null");

      return this.ExtractProperty(sample) ?? DummyNormalizer.Instance.GetDenormalizer(sample);
    }

    /// <summary>
    /// Преобразование группы значений в группу рангов
    /// </summary>
    /// <param name="sample">Исходная группа</param>
    /// <returns>Преобразованная группа</returns>
    public IPlainSample Normalize(IPlainSample sample)
    {
      Debug.Assert(sample != null, "sample cannot be null");

      var bcg = new BoxCoxSample(sample, CalculateDelta(sample));
      bcg.Lambda = TwoStageOptimization(m_min, m_max, m_eps, bcg.GetLikehood);

      return bcg;
    }

    /// <summary>
    /// Преобразование группы значений второго порядка в группу рангов
    /// </summary>
    /// <param name="sample">Исходная группа</param>
    /// <returns>Преобразованная группа</returns>
    public IDividedSample Normalize(IDividedSample sample)
    {
      Debug.Assert(sample != null, "sample cannot be null");

      if (RecreateRequired(sample))
      {
        var samples = new BoxCoxSample[sample.Count];

        double delta = CalculateDelta(sample.SelectMany(g => g));

        for (int i = 0; i < samples.Length; i++)
          samples[i] = new BoxCoxSample(sample[i], delta);

        var lambda = TwoStageOptimization(m_min, m_max, m_eps, l =>
        {
          return CalculateMultipleLikehood(l, samples);
        });

        foreach (var b in samples)
          b.Lambda = lambda;

        return new ArrayDividedSample(samples);
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

      if (RecreateRequired(sample.SelectMany(g => g)))
      {
        var samples = new IDividedSample[sample.Count];
        double delta = CalculateDelta(sample.SelectMany(g => g).SelectMany(g => g));

        for (int i = 0; i < samples.Length; i++)
        {
          var array = new BoxCoxSample[sample[i].Count];

          for (int j = 0; j < sample[i].Count; j++)
            array[j] = new BoxCoxSample(sample[i][j], delta);

          samples[i] = new ArrayDividedSample(array);
        }

        var lambda = TwoStageOptimization(m_min, m_max, m_eps, l =>
        {
          return CalculateMultipleLikehood(l, samples.SelectMany(g => g).Cast<BoxCoxSample>());
        });

        for (int i = 0; i < samples.Length; i++)
        {
          for (int j = 0; j < samples[i].Count; j++)
            ((BoxCoxSample)samples[i][j]).Lambda = lambda;
        }

        return new ArrayComplexSample(samples);
      }

      return sample;
    }

    /// <summary>
    /// Вычисление коэффициента для преобразования Бокса-Кокса
    /// </summary>
    /// <param name="sample">Числовая последовательность</param>
    /// <returns>Коэффициент для преобразования</returns>
    public double CalculateLambda(IPlainSample sample)
    {
      return this.CalculateLambda(sample, CalculateDelta(sample));
    }

    /// <summary>
    /// Вычисление коэффициента для преобразования Бокса-Кокса
    /// </summary>
    /// <param name="sample">Числовая последовательность</param>
    /// <param name="delta">Коэффициент для смещения отрицательных значений</param>
    /// <returns>Коэффициент для преобразования</returns>
    public double CalculateLambda(IPlainSample sample, double delta)
    {
      Debug.Assert(sample != null, "sample cannot be null");
      Debug.Assert(delta >= 0, "delta must be positive");

      var bcg = new BoxCoxSample(sample, delta);

      return TwoStageOptimization(m_min, m_max, m_eps, bcg.GetLikehood);
    }

    /// <summary>
    /// Текстовая информация об объекте
    /// </summary>
    public override string ToString()
    {
      return "Normalizer(value => BoxCox(value, λ, δ))";
    }

    #region ISamplePropertyExtractor<IDenormalizer> members----------------------------------------

    bool ISamplePropertyExtractor<IDenormalizer>.HasProperty(IPlainSample sample)
    {
      return sample is BoxCoxSample;
    }

    IDenormalizer ISamplePropertyExtractor<IDenormalizer>.Extract(IPlainSample sample)
    {
      var box_cox = sample as BoxCoxSample;

      return new BoxCoxInverse(box_cox.Lambda, box_cox.Delta);
    }

    IDenormalizer ISamplePropertyExtractor<IDenormalizer>.Extract(IDividedSample sample)
    {
      var box_cox = sample.FirstOrDefault() as BoxCoxSample;

      if (RecreateRequired(sample))
        throw new InvalidOperationException(Resources.NO_JOINT_BOX_COX);
      else
        return new BoxCoxInverse(box_cox.Lambda, box_cox.Delta);
    }

    IDenormalizer ISamplePropertyExtractor<IDenormalizer>.Extract(IComplexSample sample)
    {
      var box_cox = sample.SelectMany(g => g).FirstOrDefault() as BoxCoxSample;

      if (RecreateRequired(sample.SelectMany(g => g)))
        throw new InvalidOperationException(Resources.NO_JOINT_BOX_COX);
      else
        return new BoxCoxInverse(box_cox.Lambda, box_cox.Delta);
    }

    #endregion

    #region Implementation ------------------------------------------------------------------------

    private static double CalculateMultipleLikehood(double l, IEnumerable<BoxCoxSample> samples)
    {
      double likehood = 0;

      foreach (var b in samples)
        likehood += b.GetLikehood(l);

      return likehood;
    }



    private sealed class BoxCoxSample : IPlainSample
    {
      public readonly IPlainSample Source;
      public readonly double Delta;
      public double Lambda;
      private readonly double m_log_sum;
      private readonly Func<double, double> m_transform;

      public BoxCoxSample(IPlainSample sample, double delta)
      {
        this.Source = sample;
        this.Delta = delta;

        m_log_sum = this.Source.Sum(this.LogWithDelta);
        m_transform = a => BoxCoxTransform(a, this.Lambda, this.Delta);
      }

      public double this[int index]
      {
        get
        {
          return BoxCoxTransform(this.Source[index], this.Lambda, this.Delta);
        }
      }

      public int Count
      {
        get { return this.Source.Count; }
      }

      public double GetLikehood(double lambda)
      {
        IEnumerable<double> tmp = this.Source.Select(a => BoxCoxTransform(a, lambda, this.Delta));
        double likehood = 0;
        double average = tmp.Average();

        foreach (var value in tmp)
          likehood += (value - average) * (value - average);

        likehood /= this.Source.Count;
        likehood = Math.Log(likehood);
        likehood *= this.Source.Count / -2.0;
        likehood += m_log_sum * (lambda - 1);

        return likehood;
      }

      public IEnumerator<double> GetEnumerator()
      {
        return this.Source.Select(m_transform).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return this.GetEnumerator();
      }

      public override string ToString()
      {
        return string.Format("Box-Cox {0}", this.Source);
      }

      public override bool Equals(object obj)
      {
        var other = obj as BoxCoxSample;

        if (other == null)
          return false;

        if (!this.Source.Equals(other.Source))
          return false;

        if (this.Lambda != other.Lambda)
          return false;

        if (this.Delta != other.Delta)
          return false;

        return true;
      }

      public override int GetHashCode()
      {
        return this.Source.GetHashCode() ^ this.Lambda.GetHashCode() ^ this.Delta.GetHashCode();
      }

      private double LogWithDelta(double value)
      {
        double a = value + this.Delta;

        if (a <= 0)
          throw new ArgumentOutOfRangeException("delta");

        return Math.Log(a);
      }
    }

    #endregion
     */

    #endregion
  }
}
