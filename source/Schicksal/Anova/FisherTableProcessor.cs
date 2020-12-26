using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Notung;
using Schicksal.Basic;

namespace Schicksal.Anova
{
  public class FisherTableProcessor : RunBase
  {
    private readonly DataTable m_source;
    private readonly string[] m_factors;
    private readonly string m_result_column;

    public FisherTableProcessor(DataTable table, string[] factors, string result)
    {
      m_source = table;
      m_factors = factors;
      m_result_column = result;
    }

    public string Filter { get; set; }

    public bool RunInParrallel { get; set; }

    public FisherTestResult[] Result { get; private set; }

    public override void Run()
    {
      if (m_source == null)
        return;

      int group_count = (1 << m_factors.Length);

      var result = new List<FisherTestResult>(group_count);

      if (this.RunInParrallel)
        Parallel.For(1, group_count, (i) => this.ProcessFactor(group_count, result, i));
      else
      {
        for (int i = 1; i < group_count; i++)
          this.ProcessFactor(group_count, result, i);
      }

      this.Result = result.ToArray();
    }

    private void ProcessFactor(int groupCount, List<FisherTestResult> result, int i)
    {
      var factors = new List<string>();

      for (int j = 0; j < m_factors.Length; j++)
      {
        if ((i & (1 << j)) != 0)
          factors.Add(m_factors[j]);
      }

      lock (result)
        this.ReportProgress(result.Count * 100 / groupCount, string.Join("+", factors));

      FisherMetrics degrees = FisherCriteria.CalculateCriteria(
        new TableMultyDataGroup(m_source, factors.ToArray(), m_result_column, this.Filter));

      if (degrees.Ndf != 0)
      {
        var row = new FisherTestResult();

        row.F = degrees.F;
        row.Kdf = degrees.Kdf;
        row.Ndf = degrees.Ndf;
        row.Factor = string.Join("+", factors);
        row.F005 = FisherCriteria.GetCriticalValue(0.05, degrees.Kdf, degrees.Ndf);
        row.F001 = FisherCriteria.GetCriticalValue(0.01, degrees.Kdf, degrees.Ndf);
        row.P = FisherCriteria.GetProbability(degrees);

        lock (result)
          result.Add(row);
      }
    }
  }
}
