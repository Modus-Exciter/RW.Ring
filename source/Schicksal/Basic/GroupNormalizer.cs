using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Schicksal.Properties;

namespace Schicksal.Basic
{
  /// <summary>
  /// Методы нормирования данных
  /// </summary>
  public static class GroupNormalizer
  {
    #region Box-Cox -------------------------------------------------------------------------------

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
    /// Вычисление коэффициента для преобразования Бокса-Кокса
    /// </summary>
    /// <param name="group">Числовая последовательность</param>
    /// <param name="delta">Коэффициент для смещения отрицательных значений</param>
    /// <returns>Коэффициент для преобразования</returns>
    public static double CalculateLambda(IDataGroup group, double delta = 0)
    {
      Debug.Assert(group != null, "group cannot be null");
      Debug.Assert(delta >= 0, "delta must be positive");

      var bcg = new BoxCoxGroup(group, delta);

      return TwoStageOptimization(-10, 10, 1e-5, bcg.GetLikehood);
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

    /// <summary>
    /// Нормирование группы значений методом Бокса-Кокса
    /// </summary>
    /// <param name="group">Исходная группа</param>
    /// <returns>Нормированная группа</returns>
    public static IDataGroup NormalizeByBoxCox(IDataGroup group)
    {
      Debug.Assert(group != null, "group cannot be null");

      var bcg = new BoxCoxGroup(group, CalculateDelta(group));
      bcg.Lambda = TwoStageOptimization(-10, 10, 1e-5, bcg.GetLikehood);
      return bcg;
    }

    /// <summary>
    /// Нормирование группы значений второго порядка методом Бокса-Кокса
    /// </summary>
    /// <param name="group">Исходная группа</param>
    /// <returns>Нормированная группа</returns>
    public static IMultyDataGroup NormalizeByBoxCox(IMultyDataGroup group)
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
    /// Нормирование группы значений третьего порядка методом Бокса-Кокса
    /// </summary>
    /// <param name="group">Исходная группа</param>
    /// <returns>Нормированная группа</returns>
    public static ISetMultyDataGroup NormalizeByBoxCox(ISetMultyDataGroup group)
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

    #endregion

    #region Kruskal-Wallis ------------------------------------------------------------------------

    /// <summary>
    /// Расчёт рангов чисел в числовой последовательности
    /// </summary>
    /// <param name="data">Числовая последовательность</param>
    /// <param name="round">Количество знаков после запятой для округления</param>
    /// <returns>Каждому значению из числовой последовательности сопоставляется его ранг</returns>
    public static Dictionary<double, float> CalculateRanks(IEnumerable<double> data, int round = -1)
    {
      Debug.Assert(data != null, "data cannot be null");
      return CalculateRanks(data, out _, round);
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
        bool has_reason;
        var ranks = CalculateRanks(multyGroup.SelectMany(g => g), out has_reason, round);

        if (has_reason)
        {
          var groups = new IDataGroup[multyGroup.Count];

          for (int i = 0; i < groups.Length; i++)
            groups[i] = new RankedGroup(multyGroup[i], round, ranks);

          return new MultiArrayDataGroup(groups);
        }
      }
      
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
        bool has_reason;
        var ranks = CalculateRanks(group.SelectMany(g => g).SelectMany(g => g), out has_reason, round);

        if (has_reason)
        {
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
      }

      return group;
    }

    #endregion

    #region Inverse -------------------------------------------------------------------------------

    /// <summary>
    /// Расчёт изначального значения по преобразованному
    /// </summary>
    /// <param name="group">Группа преобразованных значений</param>
    /// <returns>Значение в исходной группе</returns>
    public static Func<double, double> CreateInverseHandler(IDataGroup group)
    {
      Debug.Assert(group != null, "group cannot be null");

      var ranked = group as RankedGroup;
      var box_cox = group as BoxCoxGroup;

      if (ranked != null)
        return new RankInverse(ranked.Ranks).Find;
      else if (box_cox != null)
        return new BoxCoxInverse(box_cox.Lambda, box_cox.Delta).Find;
      else
        return _default_handler;
    }

    /// <summary>
    /// Расчёт изначального значения по преобразованному
    /// </summary>
    /// <param name="group">Группа преобразованных значений второго порядка</param>
    /// <returns>Значение в исходной группе</returns>
    public static Func<double, double> CreateInverseHandler(IMultyDataGroup group)
    {
      Debug.Assert(group != null, "group cannot be null");
      
      var sub_group = group.FirstOrDefault();
      var ranked = sub_group as RankedGroup;
      var box_cox = sub_group as BoxCoxGroup;

      if (ranked != null)
      {
        if (RecreateRequired(group, ranked.Round))
          throw new InvalidOperationException(Resources.NO_JOINT_RANKS);
        else
          return new RankInverse(ranked.Ranks).Find;
      }
      else if (box_cox != null)
      {
        if (RecreateRequired(group))
          throw new InvalidOperationException(Resources.NO_JOINT_BOX_COX);
        else
          return new BoxCoxInverse(box_cox.Lambda, box_cox.Delta).Find;
      }
      else
        return _default_handler;
    }

    /// <summary>
    /// Расчёт изначального значения по преобразованному
    /// </summary>
    /// <param name="group">Группа преобразованных значений третьего порядка</param>
    /// <returns>Значение в исходной группе</returns>
    public static Func<double, double> CreateInverseHandler(ISetMultyDataGroup group)
    {
      Debug.Assert(group != null, "group cannot be null");

      var sub_group = group.SelectMany(g => g).FirstOrDefault();
      var ranked = sub_group as RankedGroup;
      var box_cox = sub_group as BoxCoxGroup;

      if (ranked != null)
      {
        if (RecreateRequired(group.SelectMany(g => g), ranked.Round))
          throw new InvalidOperationException(Resources.NO_JOINT_RANKS);
        else
          return new RankInverse(ranked.Ranks).Find;
      }
      else if (box_cox != null)
      {
        if (RecreateRequired(group.SelectMany(g => g)))
          throw new InvalidOperationException(Resources.NO_JOINT_BOX_COX);
        else
          return new BoxCoxInverse(box_cox.Lambda, box_cox.Delta).Find;
      }
      else
        return _default_handler;
    }

    #endregion

    #region Implementation ------------------------------------------------------------------------

    private static readonly Func<double, double> _default_handler = a => a;

    private static Dictionary<double, float> CalculateRanks(IEnumerable<double> data, out bool hasReason, int round = -1)
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

    private static bool RecreateRequired(IEnumerable<IDataGroup> multyGroup, int round)
    {
      Dictionary<double, float> ranks = null;
      bool first = true;

      foreach (var group in multyGroup)
      {
        var ranked = group as RankedGroup;
        
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

      public override bool Equals(object obj)
      {
        var other = obj as RankedGroup;

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

        m_log_sum = this.Source.Sum(Math.Log);
        m_transform = a => this.Transform(a, this.Lambda);
      }

      public double this[int index]
      {
        get
        {
          return this.Transform(this.Source[index], this.Lambda);
        }
      }

      public int Count
      {
        get { return this.Source.Count; }
      }

      public double Transform(double value, double lambda)
      {
        return BoxCoxTransform(value, lambda, this.Delta);
      }

      public double GetLikehood(double lambda)
      {
        IEnumerable<double> tmp = this.Source.Select(a => this.Transform(a, lambda));
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

    private sealed class BoxCoxInverse
    {
      private readonly double m_lambda;
      private readonly double m_delta;

      public BoxCoxInverse(double lambda, double delta)
      {
        m_lambda = lambda;
        m_delta = delta;
      }

      public double Find(double value)
      {
        if (m_lambda == 0)
          return Math.Exp(value - 1 + m_delta);
        else
          return Math.Pow((value - 1 + m_delta) * m_lambda + 1, 1 / m_lambda);
      }
    }

    #endregion
  }
}