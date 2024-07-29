using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Schicksal.Basic
{
  /// <summary>
  /// Преобразователь данных для нормирования методом Бокса-Кокса
  /// </summary>
  public sealed class BoxCoxNormalizer : INormalizer
  {
    private readonly double m_min;
    private readonly double m_max;
    private readonly double m_eps;
    private readonly string m_method = "MaxLikehood";

    /// <summary>
    /// Инициализация преобразователя данных для нормирования методом Бокса-Кокса
    /// </summary>
    /// <param name="min">Минимальное значение показателя степени для трансформации</param>
    /// <param name="max">Максмальное значение показателя степени для трансформации</param>
    /// <param name="eps">Точность вычисления показателя степени для трансформации</param>
    public BoxCoxNormalizer(double min = -10, double max = 10, double eps = 1e-5)
    {
      Debug.Assert(min < max);
      Debug.Assert(eps < (max - min) / 2);

      m_min = min;
      m_max = max;
      m_eps = eps;
    }

    /// <summary>
    /// Минимальное значение показателя степени для трансформации
    /// </summary>
    public double MinLambda
    {
      get { return m_min; }
    }

    /// <summary>
    /// Максмальное значение показателя степени для трансформации
    /// </summary>
    public double MaxLambda
    {
      get { return m_max; }
    }

    /// <summary>
    /// Точность вычисления показателя степени для трансформации
    /// </summary>
    public double Epsilon
    {
      get { return m_eps; }
    }

    /// <summary>
    /// Настройка преобразователя для нормирования данных
    /// </summary>
    /// <param name="sample">Выборка нормированных данных</param>
    /// <returns>Преобразователь нормированных значений в ненормированные</returns>
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

      return DummyNormalizer.Instance.Prepare(sample);
    }

    /// <summary>
    /// Текстовая информация об объекте
    /// </summary>
    public override string ToString()
    {
      return "Normalizer(value => BoxCox(value, λ, δ))";
    }

    /// <summary>
    /// Сравнение двух нормализаторов на равенство
    /// </summary>
    /// <param name="obj">Второй объект</param>
    /// <returns>True, если второй объект такой же нормализатор с теми же настройками. Иначе, False</returns>
    public override bool Equals(object obj)
    {
      var other = obj as BoxCoxNormalizer;

      if (other == null)
        return false;

      return m_min == other.m_min && m_max == other.m_max
        && m_eps == other.m_eps && m_method.Equals(other.m_method);
    }

    /// <summary>
    /// Получение хеш-кода нормализатора
    /// </summary>
    /// <returns>Побитное исключающее "или" от хеш-кодов настроек</returns>
    public override int GetHashCode()
    {
      return m_min.GetHashCode() ^ m_max.GetHashCode() ^ m_eps.GetHashCode() ^ m_method.GetHashCode();
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

      var bcg = new LikehoodCalculator(sample, delta);

      return TwoStageOptimization(m_min, m_max, m_eps, bcg.GetLikehood);
    }

    /// <summary>
    /// Вычисление коэффициента для преобразования Бокса-Кокса
    /// </summary>
    /// <param name="samples">Набор числовых последовательностей</param>
    /// <param name="delta">Коэффициент для смещения отрицательных значений</param>
    /// <returns>Коэффициент для преобразования Бокса-Кокса</returns>
    public double CalculateLambda(IEnumerable<IPlainSample> samples, double delta)
    {
      Debug.Assert(samples != null, "samples cannot be null");
      Debug.Assert(delta >= 0, "delta must be positive");

      var bcg = samples.Select(s => new LikehoodCalculator(s, delta));

      return TwoStageOptimization(m_min, m_max, m_eps, l => CalculateMultipleLikehood(l, bcg));
    }

    private static double CalculateMultipleLikehood(double l, IEnumerable<LikehoodCalculator> calculators)
    {
      double likehood = 0;

      foreach (var b in calculators)
        likehood += b.GetLikehood(l);

      return likehood;
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

    private IValueTransform PrepareTransform(IPlainSample sample)
    {
      var transform = GetValueTransform(sample);

      if (transform != null && transform.Method.Equals(m_method))
        return transform;

      double delta = CalculateDelta(sample);
      double lambda = this.CalculateLambda(sample, delta);

      return new BoxCoxValueTransform(lambda, delta, m_method);
    }

    private IValueTransform PrepareTransform(IDividedSample sample)
    {
      Debug.Assert(sample != null, "sample cannot be null");

      if (RecreateRequired(sample, m_method))
      {
        double delta = CalculateDelta(sample.SelectMany(g => g));
        double lambda = this.CalculateLambda(sample, delta);

        return new BoxCoxValueTransform(lambda, delta, m_method);
      }
      else if (sample.Count > 0)
        return GetValueTransform(sample[0]);
      else
        return DummyNormalizer.Instance.Prepare(sample);
    }

    private IValueTransform PrepareTransform(IComplexSample sample)
    {
      Debug.Assert(sample != null, "sample cannot be null");

      if (RecreateRequired(sample.SelectMany(g => g), m_method))
      {
        double delta = CalculateDelta(sample.SelectMany(g => g).SelectMany(g => g));
        double lambda = this.CalculateLambda(sample.SelectMany(g => g), delta);

        return new BoxCoxValueTransform(lambda, delta, m_method);
      }
      else if (sample.Count > 0 && sample[0].Count > 0)
        return GetValueTransform(sample[0][0]);
      else
        return DummyNormalizer.Instance.Prepare(sample);
    }

    private static bool RecreateRequired(IEnumerable<IPlainSample> samples, string method)
    {
      double delta = 0;
      double lambda = 1;
      bool first = true;

      foreach (var sample in samples)
      {
        var bcg = GetValueTransform(sample);

        if (bcg == null)
          return true;

        if (!method.Equals(bcg.Method))
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

    private static BoxCoxValueTransform GetValueTransform(IPlainSample sample)
    {
      var ns = sample as NormalizedSample;

      if (ns != null)
        return ns.ValueTransform as BoxCoxValueTransform;

      return null;
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

    private class LikehoodCalculator
    {
      public readonly IPlainSample Source;
      public readonly double Delta;
      private readonly double m_log_sum;

      public LikehoodCalculator(IPlainSample source, double delta)
      {
        Source = source;
        Delta = delta;
        m_log_sum = this.Source.Sum(this.LogWithDelta);
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

      private double LogWithDelta(double value)
      {
        double a = value + this.Delta;

        if (a <= 0)
          throw new ArgumentOutOfRangeException("delta");

        return Math.Log(a);
      }
    }

    private sealed class BoxCoxValueTransform : IValueTransform
    {
      private readonly double m_lambda;
      private readonly double m_delta;
      private readonly string m_method;

      public BoxCoxValueTransform(double lambda, double delta, string method)
      {
        m_lambda = lambda;
        m_delta = delta;
        m_method = method;
      }

      public double Lambda
      {
        get { return m_lambda; }
      }

      public double Delta
      {
        get { return m_delta; }
      }

      public string Method
      {
        get { return m_method; }
      }

      public double Normalize(double value)
      {
        return BoxCoxTransform(value, m_lambda, m_delta);
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
        return string.Format("Box-Cox(λ={0}, δ={1}, method={2})", m_lambda, m_delta, m_method);
      }

      public override bool Equals(object obj)
      {
        var other = obj as BoxCoxValueTransform;

        if (other == null)
          return false;

        return m_lambda == other.m_lambda && m_delta == other.m_delta && m_method.Equals(other.m_method);
      }

      public override int GetHashCode()
      {
        return m_delta.GetHashCode() ^ m_lambda.GetHashCode() ^ m_method.GetHashCode();
      }
    }

    #endregion
  }
}