using System;
using System.Collections.Generic;
using System.Data;
using Notung;

namespace Schicksal.Regression
{
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

    public CorrelationMetrics[] Results { get; private set; }

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

    public override void Run()
    {
      this.Results = new CorrelationMetrics[m_factors.Length];

      for (int i = 0; i < m_factors.Length; i++)
      {
        this.ReportProgress(m_factors[i]);

        var x_column = new DataColumnGroup(m_table.Columns[m_factors[i]], this.GetFilter(m_factors[i]));
        var y_column = new DataColumnGroup(m_table.Columns[m_result], this.GetFilter(m_factors[i]));

        this.Results[i] = CorrelationTest.CalculateMetrics(m_factors[i], m_result, x_column, y_column);

        if (m_factors.Length > 1)
          this.ReportProgress((i + 1) * 100 / m_factors.Length);
      }
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
  }
}
