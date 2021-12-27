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

      double x_average = factor.Average();
      double y_average = result.Average();
      double up_sum = 0;
      double x_sum = 0;
      double y_sum = 0;

      for (int i = 0; i < factor.Count; i++)
      {
        up_sum += (factor[i] - x_average) * (result[i] - y_average);
        x_sum += (factor[i] - x_average) * (factor[i] - x_average);
        y_sum += (result[i] - y_average) * (result[i] - y_average);
      }

      return up_sum / Math.Sqrt(x_sum * y_sum);
    }

    public static CorrelationMetrics CalculateMetrics(string factor, IDataGroup x, IDataGroup y)
    {
      var result = new CorrelationMetrics
      {
        Factor = factor,
        R = CalculateR(x, y),
        N = x.Count
      };

      result.Z = 0.5 * Math.Log((1 + result.R) / (1 - result.R));
      result.T = Math.Abs(result.R) / Math.Sqrt((1 - result.R * result.R) / (result.N - 2));
      result.R001 = SpecialFunctions.invstudenttdistribution(x.Count - 2, 1 - 0.01 / 2);
      result.R005 = SpecialFunctions.invstudenttdistribution(x.Count - 2, 1 - 0.05 / 2);
      result.P = 1 - SpecialFunctions.studenttdistribution(x.Count - 2, result.T);

      return result;
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

      List<string> expressions = GetFilterExpressions(table, factors, result, filter);

      if (expressions.Count > 0)
        m_filter = string.Join(" and ", expressions);
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

      var y_column = new DataColumnGroup(m_table.Columns[m_result], m_filter);

      for (int i = 0; i < m_factors.Length; i++)
      {
        this.Results[i] = CorrelationTest.CalculateMetrics(m_factors[i],
          new DataColumnGroup(m_table.Columns[m_factors[i]], m_filter), y_column);
      }
    }
  }
}