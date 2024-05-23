using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Notung;
using Notung.Logging;
using Schicksal.Basic;
using Schicksal.Properties;
using Schicksal.VectorField;

namespace Schicksal.Regression
{
  /// <summary>
  /// Расчёт всех параметров регрессий
  /// </summary>
  public static class CorrelationTest
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(CorrelationTest));

    #region Public methods ------------------------------------------------------------------------

    public static CorrelationMetrics CalculateMetrics(string factor, string effect, IDataGroup x, IDataGroup y, double p)
    {
      var result = new CorrelationMetrics
      {
        Factor = factor,
        R = CalculateR(x, y),
        Eta = CalculateEta(x, y),
        Formula = CalculateFormula(x, y, factor, effect),
        N = x.Count
      };

      if (double.IsNaN(result.Eta))
        result.Eta = result.R;

      result.Z = 0.5 * Math.Log((1 + result.R) / (1 - result.R));
      result.TStandard = SpecialFunctions.invstudenttdistribution(x.Count - 2, 1 - p / 2);
      //result.T001 = SpecialFunctions.invstudenttdistribution(x.Count - 2, 1 - 0.01 / 2);
      //result.T005 = SpecialFunctions.invstudenttdistribution(x.Count - 2, 1 - 0.05 / 2);
      result.TR = Math.Abs(result.R) / Math.Sqrt((1 - result.R * result.R) / (result.N - 2));
      result.PR = (1 - SpecialFunctions.studenttdistribution(x.Count - 2, result.TR)) * 2;
      result.TH = Math.Abs(result.Eta) / Math.Sqrt((1 - result.Eta * result.Eta) / (result.N - 2));
      result.PH = (1 - SpecialFunctions.studenttdistribution(x.Count - 2, result.TH)) * 2;

      return result;
    }

    public static double CalculateR(IDataGroup factor, IDataGroup result)
    {
      if (factor == null)
        throw new ArgumentNullException("factor");

      if (result == null)
        throw new ArgumentNullException("result");

      if (factor.Count != result.Count)
        throw new ArgumentException(Resources.DATA_GROUP_SIZE_MISMATCH);

      if (factor.Count < 3)
        throw new ArgumentOutOfRangeException("factor.Count");

      double up_sum = 0;
      double x_sum = 0;
      double y_sum = 0;
      double x_average = factor.Average();
      double y_average = result.Average();

      for (int i = 0; i < factor.Count; i++)
      {
        up_sum += (factor[i] - x_average) * (result[i] - y_average);
        x_sum += (factor[i] - x_average) * (factor[i] - x_average);
        y_sum += (result[i] - y_average) * (result[i] - y_average);
      }

      return up_sum / Math.Sqrt(x_sum * y_sum);
    }

    public static double CalculateEta(IDataGroup factor, IDataGroup result)
    {
      if (factor == null)
        throw new ArgumentNullException("factor");

      if (result == null)
        throw new ArgumentNullException("result");

      if (factor.Count != result.Count)
        throw new ArgumentException(Resources.DATA_GROUP_SIZE_MISMATCH);

      if (factor.Count < 6)
        return double.NaN;

      var sorted = EnumeratePoints(factor, result).OrderBy(p => p.X).ToList();

      List<int> borders = RangeToGroups(sorted);

      if (borders.Count < 3)
        return double.NaN;

      int last = 0;

      Point2D[][] data = new Point2D[borders.Count][];
      double avg_y = result.Average();
      double up_sum = 0;
      double dn_sum = 0;
      int index = 0;

      foreach (int border in borders)
      {
        double group_y = 0;

        data[index] = new Point2D[border - last];

        for (int i = 0; i < data[index].Length; i++)
        {
          data[index][i] = sorted[last + i];
          group_y += sorted[last + i].Y;
        }

        group_y /= data[index].Length;
        up_sum += data[index].Length * (group_y - avg_y) * (group_y - avg_y);

        last = border;
        index++;
      }

      foreach (double y in result)
        dn_sum += (y - avg_y) * (y - avg_y);

      return Math.Sqrt(up_sum / dn_sum);
    }

    public static Dictionary<double, float> CalculateSpearmanRanks(IEnumerable<double> data)
    {
      float rank = 1;
      Dictionary<double, float> ranks = new Dictionary<double, float>();

      foreach (var group in data.GroupBy(p => p).OrderBy(g => g.Key))
      {
        var count = group.Count();
        ranks[group.Key] = rank + (count - 1f) / 2f;
        rank += count;
      }

      return ranks;
    }

    public static CorrelationFormula CalculateFormula(IDataGroup x, IDataGroup y, string factor, string effect)
    {
      double min_x = double.MaxValue;
      double max_x = double.MinValue;
      double min_y = double.MaxValue;
      double max_y = double.MinValue;

      Point2D[] points = new Point2D[x.Count];

      for (int i = 0; i < x.Count; i++)
      {
        var point = new Point2D
        {
          X = Convert.ToDouble(x[i]),
          Y = Convert.ToDouble(y[i])
        };

        points[i] = point;

        if (min_x > point.X)
          min_x = point.X;

        if (max_x < point.X)
          max_x = point.X;

        if (min_y > point.Y)
          min_y = point.Y;

        if (max_y < point.Y)
          max_y = point.Y;
      }

      List<RegressionDependency> dependencies = new List<RegressionDependency>(5);

      Parallel.ForEach(EnumerateDependencyTypes(),
        t => AddType(t, dependencies, x, y, factor, effect));

      return new CorrelationFormula
      {
        MinX = min_x,
        MaxX = max_x,
        MinY = min_y,
        MaxY = max_y,
        SourcePoints = points,
        Dependencies = dependencies.ToArray()
      };
    }

    #endregion

    #region Implementation ------------------------------------------------------------------------

    private static List<int> RangeToGroups(List<Point2D> sorted)
    {
      var borders = new List<int>();
      var group = (int)Math.Sqrt(sorted.Count);

      if (group < 2)
        group = 2;

      if (group < sorted.Count / 20)
        group = sorted.Count / 20;

      int i = group;

      while (i < sorted.Count)
      {
        if (sorted[i - 1].X == sorted[i].X && i < sorted.Count)
        {
          if (!CorrectBorder(sorted, borders, ref i))
            break;
        }

        borders.Add(i);
        i += group;
      }

      if (borders.Count > 0)
      {
        int last = borders[borders.Count - 1];

        if (last < sorted.Count - 2)
          borders.Add(sorted.Count);
        else
          borders[borders.Count - 1] = sorted.Count;
      }

      return borders;
    }

    private static bool CorrectBorder(List<Point2D> sorted, List<int> borders, ref int border)
    {
      int last = 0;

      if (borders.Count > 0)
        last = borders[borders.Count - 1];

      int down = 0;
      int up = 0;
      bool down_found = false;
      bool up_found = false;

      for (int j = border - 1; j > last + 1; j--)
      {
        down++;

        if (sorted[j - 1].X != sorted[j].X)
        {
          down_found = true;
          break;
        }
      }

      for (int j = border + 1; j < sorted.Count; j++)
      {
        up++;

        if (sorted[j - 1].X != sorted[j].X)
        {
          up_found = true;
          break;
        }
      }

      if (down_found && up_found)
      {
        if (down < up)
          border -= down;
        else
          border += up;
      }
      else if (down_found)
        border -= down;
      else if (up_found)
        border += up;
      else
        return false;

      return true;
    }

    private static void CalculateHeteroscedasticity(IDataGroup x, IDataGroup y, RegressionDependency dependency)
    {
      var x_ranks = CalculateSpearmanRanks(x);
      var y_ranks = CalculateSpearmanRanks(EnumeratePoints(x, y).Select(p =>
        Math.Abs(p.Y - dependency.Calculate(p.X))));

      double dsum = 0;

      for (int i = 0; i < x.Count; i++)
      {
        double d = x_ranks[x[i]] - y_ranks[Math.Abs(y[i] - dependency.Calculate(x[i]))];
        dsum += d * d;
      }

      var r = 1 - 6 * dsum / (x.Count * ((double)x.Count * x.Count - 1));
      var t = Math.Abs(r) * Math.Sqrt(x.Count - 2) / Math.Sqrt(1 - r * r);

      dependency.Heteroscedasticity = new Heteroscedasticity
      {
        SpearmanCoefficent = r,
        Probability = 2 * SpecialFunctions.studenttdistribution(x.Count - 2, t) - 1
      };
    }

    private static void CalculateConsistency(IDataGroup x, IDataGroup y, RegressionDependency dependency)
    {
      double down = DescriptionStatistics.SquareDerivation(y);
      double up = 0;
      int i = 0;
      Predicate<double> condition = gap => gap == x[i];

      for (; i < x.Count; i++)
      {
        if (Array.Exists(dependency.GetGaps(), condition))
          break;

        var derivation = dependency.Calculate(x[i]) - y[i];
        up += derivation * derivation;
      }

      dependency.Consistency = (down - up) / down;
    }

    private static void CalculateConsistencyWeighted(IDataGroup x, IDataGroup y, RegressionDependency dep, Dispersion disp)
    {
      double mean = DescriptionStatistics.Mean(y);
      double num = 0, denum = 0;
      double residualRegr = 0;
      double residualMid = 0;

      for (int i = 0; i < x.Count; i++)
      {
        residualRegr = (y[i] - dep.Calculate(x[i])) / disp[i];
        residualRegr *= residualRegr;
        residualMid = (y[i] - mean) / disp[i];
        residualMid *= residualMid;

        num += residualRegr;
        denum += residualMid;
      }
      dep.ConsistencyWeighted = 1 - num / denum;
    }

    private static void CalculateRMSError(IDataGroup x, IDataGroup y, RegressionDependency dep)
    {
      double result = 0;
      double residual;

      for (int i = 0; i < x.Count; i++)
      {
        residual = y[i] - dep.Calculate(x[i]);
        result += residual * residual;
      }

      dep.RMSError = Math.Sqrt(result / x.Count);
    }

    private static void CalculateRMSErrorWeighted(IDataGroup x, IDataGroup y, RegressionDependency dep, Dispersion disp)
    {
      double num = 0;
      double denum = 0;
      double residual;

      for (int i = 0; i < x.Count; i++)
      {
        residual = (y[i] - dep.Calculate(x[i])) / disp[i];
        num += residual * residual;
        denum += 1 / (disp[i]* disp[i]);
      }

      dep.RMSErrorWeighted = Math.Sqrt(num / denum);
    }

    private static void AddType(Type type, List<RegressionDependency> dependencies, IDataGroup x, IDataGroup y, string factor, string effect)
    {
      try
      {
        var dep = (RegressionDependency)Activator.CreateInstance(type, x, y);
        Dispersion disp = new Dispersion(x, y, dep.Calculate);
        dep.Factor = factor;
        dep.Effect = effect;

        CalculateHeteroscedasticity(x, y, dep);
        CalculateConsistency(x, y, dep);
        CalculateConsistencyWeighted(x, y, dep, disp);
        CalculateRMSError(x, y, dep);
        CalculateRMSErrorWeighted(x, y, dep, disp);

        lock (dependencies)
        {
          dependencies.Add(dep);
        }
      }
      catch (Exception ex)
      {
        _log.Error("Run(): exception", ex);
      }
    }

    private static IEnumerable<Type> EnumerateDependencyTypes()
    {
      foreach (var type in typeof(RegressionDependency).Assembly.GetAvailableTypes())
      {
        if (!type.IsAbstract && typeof(RegressionDependency).IsAssignableFrom(type))
          yield return type;
      }
    }

    private static IEnumerable<Point2D> EnumeratePoints(IDataGroup factor, IDataGroup result)
    {
      for (int i = 0; i < factor.Count; i++)
        yield return new Point2D { X = factor[i], Y = result[i] };
    }

    #endregion
  }
}