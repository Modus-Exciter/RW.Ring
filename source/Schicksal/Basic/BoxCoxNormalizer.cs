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
    /// <param name="group">Группа нормированных значений</param>
    /// <returns>Обратный преобразователь нормированных данных</returns>
    public IDenormalizer GetDenormalizer(IDataGroup group)
    {
      Debug.Assert(group != null, "group cannot be null");

      var box_cox = group as BoxCoxGroup;

      if (box_cox != null)
        return new BoxCoxInverse(box_cox.Lambda, box_cox.Delta);
      else
        return DummyNormalizer.Denormalizer;
    }

    /// <summary>
    /// Расчёт изначального значения по преобразованному
    /// </summary>
    /// <param name="group">Группа нормированных значений второго порядка</param>
    /// <returns>Обратный преобразователь нормированных данных</returns>
    public IDenormalizer GetDenormalizer(IMultyDataGroup group)
    {
      Debug.Assert(group != null, "group cannot be null");

      var sub_group = group.FirstOrDefault();
      var box_cox = sub_group as BoxCoxGroup;

      if (box_cox != null)
      {
        if (RecreateRequired(group))
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
    /// <param name="group">Группа нормированных значений третьего порядка</param>
    /// <returns>Обратный преобразователь нормированных данных</returns>
    public IDenormalizer GetDenormalizer(ISetMultyDataGroup group)
    {
      Debug.Assert(group != null, "group cannot be null");

      var sub_group = group.SelectMany(g => g).FirstOrDefault();
      var box_cox = sub_group as BoxCoxGroup;

      if (box_cox != null)
      {
        if (RecreateRequired(group.SelectMany(g => g)))
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
    /// <param name="group">Исходная группа</param>
    /// <returns>Преобразованная группа</returns>
    public IDataGroup Normalize(IDataGroup group)
    {
      Debug.Assert(group != null, "group cannot be null");

      var bcg = new BoxCoxGroup(group, CalculateDelta(group));
      bcg.Lambda = TwoStageOptimization(-10, 10, 1e-5, bcg.GetLikehood);

      return bcg;
    }

    /// <summary>
    /// Преобразование группы значений второго порядка в группу рангов
    /// </summary>
    /// <param name="group">Исходная группа</param>
    /// <returns>Преобразованная группа</returns>
    public IMultyDataGroup Normalize(IMultyDataGroup group)
    {
      Debug.Assert(group != null, "group cannot be null");

      if (RecreateRequired(group))
      {
        var groups = new BoxCoxGroup[group.Count];
        double delta = CalculateDelta(group.SelectMany(g => g));

        for (int i = 0; i < groups.Length; i++)
          groups[i] = new BoxCoxGroup(group[i], delta);

        var lambda = TwoStageOptimization(-10, 10, 1e-5, l =>
        {
          return CalculateMultipleLikehood(l, groups);
        });

        foreach (var b in groups)
          b.Lambda = lambda;

        return new MultiArrayDataGroup(groups);
      }

      return group;
    }

    /// <summary>
    /// Преобразование группы значений третьего порядка в группу рангов
    /// </summary>
    /// <param name="group">Исходная группа</param>
    /// <returns>Преобразованная группа третьего порядка</returns>
    public ISetMultyDataGroup Normalize(ISetMultyDataGroup group)
    {
      Debug.Assert(group != null, "group cannot be null");

      if (RecreateRequired(group.SelectMany(g => g)))
      {
        var groups = new IMultyDataGroup[group.Count];
        double delta = CalculateDelta(group.SelectMany(g => g).SelectMany(g => g));

        for (int i = 0; i < groups.Length; i++)
        {
          var array = new BoxCoxGroup[group[i].Count];

          for (int j = 0; j < group[i].Count; j++)
            array[j] = new BoxCoxGroup(group[i][j], delta);

          groups[i] = new MultiArrayDataGroup(array);
        }

        var lambda = TwoStageOptimization(-10, 10, 1e-5, l =>
        {
          return CalculateMultipleLikehood(l, groups.SelectMany(g => g).Cast<BoxCoxGroup>());
        });

        for (int i = 0; i < groups.Length; i++)
        {
          for (int j = 0; j < groups[i].Count; j++)
            ((BoxCoxGroup)groups[i][j]).Lambda = lambda;
        }

        return new SetMultiArrayDataGroup(groups);
      }

      return group;
    }

    /// <summary>
    /// Вычисление коэффициента для преобразования Бокса-Кокса
    /// </summary>
    /// <param name="group">Числовая последовательность</param>
    /// <returns>Коэффициент для преобразования</returns>
    public double CalculateLambda(IDataGroup group)
    {
      return this.CalculateLambda(group, CalculateDelta(group));
    }

    /// <summary>
    /// Вычисление коэффициента для преобразования Бокса-Кокса
    /// </summary>
    /// <param name="group">Числовая последовательность</param>
    /// <param name="delta">Коэффициент для смещения отрицательных значений</param>
    /// <returns>Коэффициент для преобразования</returns>
    public double CalculateLambda(IDataGroup group, double delta)
    {
      Debug.Assert(group != null, "group cannot be null");
      Debug.Assert(delta >= 0, "delta must be positive");

      var bcg = new BoxCoxGroup(group, delta);

      return TwoStageOptimization(-10, 10, 1e-5, bcg.GetLikehood);
    }

    /// <summary>
    /// Текстовая информация об объекте
    /// </summary>
    public override string ToString()
    {
      return "Normalizer(value => value ^ lambda / lambda)";
    }

    /// <summary>
    /// Вычисление фиксированного коэффициента смещения для преобразования Бокса-Кокса
    /// </summary>
    /// <param name="group">Числовая последовательность</param>
    /// <returns>Коэффициент для смещения отрицательных значений</returns>
    public static double CalculateDelta(IEnumerable<double> group)
    {
      Debug.Assert(group != null, "group cannot be null");

      if (!group.Any())
        return 0;

      double min = group.First();
      double max = min;
      int count = 0;

      foreach (var value in group.Skip(1))
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

    private static bool RecreateRequired(IEnumerable<IDataGroup> multyGroup)
    {
      double delta = 0;
      double lambda = 1;
      bool first = true;

      foreach (var group in multyGroup)
      {
        var bcg = group as BoxCoxGroup;

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

    private static double CalculateMultipleLikehood(double l, IEnumerable<BoxCoxGroup> groups)
    {
      double likehood = 0;

      foreach (var b in groups)
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

    private sealed class BoxCoxGroup : IDataGroup
    {
      public readonly IDataGroup Source;
      public readonly double Delta;
      public double Lambda;
      private readonly double m_log_sum;
      private readonly Func<double, double> m_transform;

      public BoxCoxGroup(IDataGroup group, double delta)
      {
        this.Source = group;
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
        var other = obj as BoxCoxGroup;

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
        return "Denormalizer(value => (lambda * value) ^ (1 / lambda))";
      }
    }

    #endregion
  }
}