using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Notung;
using Schicksal.Basic;
using Schicksal.Properties;

namespace Schicksal.Regression
{
  public class CorrelationTest
  {
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
        throw new ArgumentOutOfRangeException("factor.Count");

      var sorted = EnumeratePoints(factor, result).OrderBy(p => p.X).ToList();

      List<int> borders = RangeToGroups(sorted);

      if (borders.Count < 3)
        return CalculateR(factor, result);

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
          int last = 0;

          if (borders.Count > 0)
            last = borders[borders.Count - 1];

          int down = 0;
          int up = 0;
          bool down_found = false;
          bool up_found = false;

          for (int j = i - 1; j > last + 1; j--)
          {
            down++;

            if (sorted[j - 1].X != sorted[j].X)
            {
              down_found = true;
              break;
            }
          }

          for (int j = i + 1; j < sorted.Count; j++)
          {
            up++;
            if (sorted[j - 1].X != sorted[j].X)
            {
              up_found = true;
              break;
            }
          }

          if (!down_found && up_found)
          {
            i += up;
          }
          else if (down_found && !up_found)
          {
            i -= down;
          }
          else if (down_found && up_found)
          {
            if (down < up)
              i -= down;
            else
              i += up;
          }
          else
          {
            break;
          }
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

    public static CorrelationMetrics CalculateMetrics(string factor, string effect, IDataGroup x, IDataGroup y)
    {
      var result = new CorrelationMetrics
      {
        Factor = factor,
        R = CalculateR(x, y),
        N = x.Count
      };

      result.Z = 0.5 * Math.Log((1 + result.R) / (1 - result.R));
      result.TR = Math.Abs(result.R) / Math.Sqrt((1 - result.R * result.R) / (result.N - 2));
      result.T001 = SpecialFunctions.invstudenttdistribution(x.Count - 2, 1 - 0.01 / 2);
      result.T005 = SpecialFunctions.invstudenttdistribution(x.Count - 2, 1 - 0.05 / 2);
      result.PR = (1 - SpecialFunctions.studenttdistribution(x.Count - 2, result.TR)) * 2;

      if (x.Count > 6)
        result.Eta = CalculateEta(x, y);
      else
        result.Eta = result.R;

      result.TH = Math.Abs(result.Eta) / Math.Sqrt((1 - result.Eta * result.Eta) / (result.N - 2));
      result.PH = (1 - SpecialFunctions.studenttdistribution(x.Count - 2, result.TH)) * 2;
      result.Correlations = new CorrelationResults(x, y).Run(factor, effect);

      foreach (var dep in result.Correlations.Dependencies)
        CaclulateHeteroscedasticity(x, y, dep);

      return result;
    }

    private static void CaclulateHeteroscedasticity(IDataGroup x, IDataGroup y, RegressionDependency dependency)
    {
      var point_list = EnumeratePoints(x, y).Select(p => new Point2D
      {
        X = p.X,
        Y = Math.Abs(p.Y - dependency.Calculate(p.X))
      }).OrderBy(p => p.X).ToList();

      var groups = point_list.GroupBy(p => p.Y).OrderBy(g => g.Key);

      double rank = 0;
      Dictionary<double, double> ranks = new Dictionary<double, double>();

      foreach (var group in groups)
      {
        var count = group.Count();
        ranks[group.Key] = rank + (count - 1.0) / 2;
        rank += count;
      }

      double dsum = 0;

      for (int i = 0; i < x.Count; i++)
      {
        double d = i - ranks[point_list[i].Y];
        dsum += d * d;
      }

      var r = 1 - 6 * dsum / (x.Count * (x.Count * x.Count - 1));
      var t = Math.Abs(r) * Math.Sqrt(x.Count - 2) / Math.Sqrt(1 - r * r);

      dependency.Heteroscedasticity = 2 * SpecialFunctions.studenttdistribution(x.Count - 2, t) - 1;
    }

    private static IEnumerable<Point2D> EnumeratePoints(IDataGroup factor, IDataGroup result)
    {
      for (int i = 0; i < factor.Count; i++)
        yield return new Point2D { X = factor[i], Y = result[i] };
    }
  }

  public class CorrelationTestProcessor : RunBase
  {
    private readonly DataTable m_table;
    private readonly string[] m_factors;
    private readonly string m_result;
    private readonly string m_filter;

    public string Filter
    {
      get { return m_filter; }
    }

    public CorrelationTestProcessor(DataTable table, string[] factors, string result, string filter)
    {
      if (table == null)
        throw new ArgumentNullException("table");

      if (factors == null)
        throw new ArgumentNullException("factors");

      if (string.IsNullOrEmpty(result))
        throw new ArgumentNullException("result");

      m_table = table;
      m_factors = factors;
      m_result = result;
      m_filter = string.IsNullOrWhiteSpace(filter) ? null : filter;
    }

    private string GetFilter(string factor)
    {
      List<string> expressions = GetFilterExpressions(m_table, new string[] { factor }, m_result, m_filter);

      if (expressions.Count > 0)
        return string.Join(" and ", expressions);
      else
        return null;
    }

    private static List<string> GetFilterExpressions(DataTable table, string[] factors, string result, string filter)
    {
      List<string> expressions = new List<string>(factors.Length + 1);

      foreach (var f in factors)
      {
        if (!table.Columns[f].DataType.IsPrimitive || table.Columns[f].DataType == typeof(bool))
          throw new ArgumentException("Factor column must be numeric");

        if (table.Columns[f].AllowDBNull)
          expressions.Add(string.Format("[{0}] is not null", f));
      }

      if (!table.Columns[result].DataType.IsPrimitive || table.Columns[result].DataType == typeof(bool))
        throw new ArgumentException("Result column must be numeric");

      if (table.Columns[result].AllowDBNull)
        expressions.Add(string.Format("[{0}] is not null", result));

      expressions.RemoveAll((filter ?? string.Empty).Contains);

      if (!string.IsNullOrEmpty(filter))
        expressions.Add(filter);

      return expressions;
    }

    public CorrelationMetrics[] Results { get; private set; }

    public override void Run()
    {
      this.Results = new CorrelationMetrics[m_factors.Length];

      for (int i = 0; i < m_factors.Length; i++)
      {
        var x_column = new DataColumnGroup(m_table.Columns[m_factors[i]], GetFilter(m_factors[i]));
        var y_column = new DataColumnGroup(m_table.Columns[m_result], GetFilter(m_factors[i]));

        this.Results[i] = CorrelationTest.CalculateMetrics(m_factors[i], m_result, x_column, y_column);
      }
    }
  }
}