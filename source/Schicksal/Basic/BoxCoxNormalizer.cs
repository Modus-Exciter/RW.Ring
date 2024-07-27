using Schicksal.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Schicksal.Basic
{
  /// <summary>
  /// Преобразователь данных для нормирования методом Бокса-Кокса
  /// </summary>
  public sealed class BoxCoxNormalizer : INormalizer
  {
    /// <summary>
    /// Расчёт изначального значения по преобразованному
    /// </summary>
    /// <param name="sample">Группа нормированных значений</param>
    /// <returns>Обратный преобразователь нормированных данных</returns>
    public IDenormalizer GetDenormalizer(IPlainSample sample)
    {
      Debug.Assert(sample != null, "sample cannot be null");

      var box_cox = sample as BoxCoxSample;

      if (box_cox != null)
        return new BoxCoxInverse(box_cox.Lambda, box_cox.Delta);
      else
        return DummyNormalizer.Denormalizer;
    }

    /// <summary>
    /// Расчёт изначального значения по преобразованному
    /// </summary>
    /// <param name="sample">Группа нормированных значений второго порядка</param>
    /// <returns>Обратный преобразователь нормированных данных</returns>
    public IDenormalizer GetDenormalizer(IDividedSample sample)
    {
      Debug.Assert(sample != null, "sample cannot be null");

      var sub_sample = sample.FirstOrDefault();
      var box_cox = sub_sample as BoxCoxSample;

      if (box_cox != null)
      {
        if (RecreateRequired(sample))
          throw new InvalidOperationException(Resources.NO_JOINT_BOX_COX);
        else
          return new BoxCoxInverse(box_cox.Lambda, box_cox.Delta);
      }
      else
        return DummyNormalizer.Denormalizer;
    }

    /// <summary>
    /// Расчёт изначального значения по преобразованному
    /// </summary>
    /// <param name="sample">Группа нормированных значений третьего порядка</param>
    /// <returns>Обратный преобразователь нормированных данных</returns>
    public IDenormalizer GetDenormalizer(IComplexSample sample)
    {
      Debug.Assert(sample != null, "sample cannot be null");

      var sub_sample = sample.SelectMany(g => g).FirstOrDefault();
      var box_cox = sub_sample as BoxCoxSample;

      if (box_cox != null)
      {
        if (RecreateRequired(sample.SelectMany(g => g)))
          throw new InvalidOperationException(Resources.NO_JOINT_BOX_COX);
        else
          return new BoxCoxInverse(box_cox.Lambda, box_cox.Delta);
      }
      else
        return DummyNormalizer.Denormalizer;
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
      bcg.Lambda = TwoStageOptimization(-10, 10, 1e-5, bcg.GetLikehood);

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

        var lambda = TwoStageOptimization(-10, 10, 1e-5, l =>
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

        var lambda = TwoStageOptimization(-10, 10, 1e-5, l =>
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

      return TwoStageOptimization(-10, 10, 1e-5, bcg.GetLikehood);
    }

    /// <summary>
    /// Текстовая информация об объекте
    /// </summary>
    public override string ToString()
    {
      return "Normalizer(value => BoxCox(value, λ, δ))";
    }

    /// <summary>
    /// Вычисление фиксированного коэффициента смещения для преобразования Бокса-Кокса
    /// </summary>
    /// <param name="sample">Числовая последовательность</param>
    /// <returns>Коэффициент для смещения отрицательных значений</returns>
    public static double CalculateDelta(IEnumerable<double> sample)
    {
      Debug.Assert(sample != null, "sample cannot be null");

      if (!sample.Any())
        return 0;

      double min = sample.First();
      double max = min;
      int count = 0;

      foreach (var value in sample.Skip(1))
      {
        if (max < value)
          max = value;

        if (min > value)
          min = value;

        count++;
      }

      if (min > 0)
        return 0;
      else if (count == 0)
        return 1 - min;
      else
        return (max - min) / count - min;
    }

    /// <summary>
    /// Преобразование Бокса-Кокса
    /// </summary>
    /// <param name="value">Преобразуемое значение</param>
    /// <param name="lambda">Коэффициент для преобразования</param>
    /// <param name="delta">Коэффициент для смещения отрицательных значений</param>
    /// <returns>Преобразованное значение</returns>
    public static double BoxCoxTransform(double value, double lambda, double delta = 0)
    {
      if (delta == 0)
      {
        if (lambda == 0)
          return Math.Log(value) + 1;
        else
          return (Math.Pow(value, lambda) - 1) / lambda + 1;
      }
      else
      {
        if (lambda == 0)
          return Math.Log(value + delta) + 1 - delta;
        else
          return (Math.Pow(value + delta, lambda) - 1) / lambda + 1 - delta;
      }
    }

    #region Implementation ------------------------------------------------------------------------

    private static bool RecreateRequired(IEnumerable<IPlainSample> samples)
    {
      double delta = 0;
      double lambda = 1;
      bool first = true;

      foreach (var sample in samples)
      {
        var bcg = sample as BoxCoxSample;

        if (bcg == null)
          return true;

        if (first)
        {
          first = false;
          delta = bcg.Delta;
          lambda = bcg.Lambda;
        }
        else if (delta != bcg.Delta || lambda != bcg.Lambda)
          return true;
      }

      return false;
    }

    private static double CalculateMultipleLikehood(double l, IEnumerable<BoxCoxSample> samples)
    {
      double likehood = 0;

      foreach (var b in samples)
        likehood += b.GetLikehood(l);

      return likehood;
    }

    private static double TwoStageOptimization(double left, double right, double eps, Func<double, double> criteria)
    {
      double step = CalculateStep(left, right, eps);
      double best_argument = (left + right) / 2;
      double best_criteria = criteria(best_argument);

      for (double x = left; x <= right; x += step)
      {
        double value = criteria(x);

        if (best_criteria < value)
        {
          best_criteria = value;
          best_argument = x;
        }
      }

      if (criteria(best_argument - step) > criteria(best_argument + step))
      {
        left = best_argument - step + eps;
        right = best_argument;
      }
      else
      {
        left = best_argument;
        right = best_argument + step - eps;
      }

      return BinaryOptimization(left, right, eps, criteria);
    }

    private static double CalculateStep(double left, double right, double eps)
    {
      double difference = right - left;

      Debug.Assert(difference > 0);
      Debug.Assert(eps > 0 && eps < difference / 2);

      return difference * Math.Log(2) / Math.Log(difference / eps);
    }

    private static double BinaryOptimization(double left, double right, double eps, Func<double, double> criteria)
    {
      ChangedBorder changed_border = ChangedBorder.Both;
      double criteria_left = 0;
      double criteria_right = 0;

      while (right - left > eps)
      {
        if (changed_border != ChangedBorder.Right)
          criteria_left = criteria(left);

        if (changed_border != ChangedBorder.Left)
          criteria_right = criteria(right);

        if (criteria_left > criteria_right)
        {
          right = (left + right) / 2;
          changed_border = ChangedBorder.Right;
        }
        else if (criteria_left < criteria_right)
        {
          left = (left + right) / 2;
          changed_border = ChangedBorder.Left;
        }
        else
        {
          left += (left + right) / 4;
          right -= (left + right) / 4;
          changed_border = ChangedBorder.Both;
        }
      }

      return (left + right) / 2;
    }

    private enum ChangedBorder : byte
    {
      Both,
      Left,
      Right
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

    private sealed class BoxCoxInverse : IDenormalizer
    {
      private readonly double m_lambda;
      private readonly double m_delta;

      public BoxCoxInverse(double lambda, double delta)
      {
        m_lambda = lambda;
        m_delta = delta;
      }

      public double Denormalize(double value)
      {
        if (m_lambda == 0)
          return Math.Exp(value - 1 + m_delta);
        else
          return Math.Pow((value - 1 + m_delta) * m_lambda + 1, 1 / m_lambda);
      }

      public override string ToString()
      {
        return "Denormalizer(value => InverseBoxCox(value, λ, δ))";
      }
    }

    #endregion
  }
}