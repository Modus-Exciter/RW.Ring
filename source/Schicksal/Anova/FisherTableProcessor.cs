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
    private readonly double m_probability;

    public FisherTableProcessor(DataTable table, string[] factors, string result, double p)
    {
      m_source = table;
      m_factors = factors;
      m_result_column = result;
      m_probability = p;
    }

    public string Filter { get; set; }

    public string Conjugate { get; set; }

    public bool RunInParrallel { get; set; }

    public FisherTestResult[] Result { get; private set; }

    public override void Run()
    {
      if (m_source == null)
        return;

      int group_count = (1 << m_factors.Length);

      var result = new List<FisherTestResult>(group_count);

      if (this.RunInParrallel)
      {
        Parallel.For(1, group_count, (i) => this.ProcessFactor(group_count, result, i));
      }
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
      var ignoredFactors = new List<string>();

      for (int j = 0; j < m_factors.Length; j++)
      {
        if ((i & (1 << j)) != 0)
          factors.Add(m_factors[j]);
        else
          ignoredFactors.Add(m_factors[j]);
      }

      lock (result)
        this.ReportProgress(result.Count * 100 / groupCount, string.Join("+", factors));

      if (ignoredFactors.Count > 0)
      {
        if (!this.DoubleFactorAnalysis(result, factors, ignoredFactors))
          this.SingleFactorAnalysis(result, factors);
      }
      else
        this.SingleFactorAnalysis(result, factors);
    }

    private bool DoubleFactorAnalysis(List<FisherTestResult> result, List<string> factors, List<string> ignoredFactors)
    {
      using (var groups = new TableSetDataGroup(m_source, factors.ToArray(), ignoredFactors.ToArray(), m_result_column, this.Filter))
      {
        FisherMetrics degrees = string.IsNullOrEmpty(this.Conjugate)
          ? FisherCriteria.CalculateMultiplyCriteria(groups)
          : FisherCriteria.CalculateConjugate(groups);

        if (degrees.Ndf != 0)
        {
          var row = new FisherTestResult
          {
            F = degrees.F,
            Kdf = degrees.Kdf,
            Ndf = degrees.Ndf,
            Factor = string.Join("+", factors),
            IgnoredFactor = string.Join("+", this.GetIgnoredFactors(factors)),
            Conjugate = this.Conjugate,
            FCritical = FisherCriteria.GetCriticalValue(m_probability, degrees.Kdf, degrees.Ndf),
            P = FisherCriteria.GetProbability(degrees)
          };

          lock (result)
            result.Add(row);

          return true;
        }
        else
          return false;
      }
    }

    private string[] GetIgnoredFactors(List<string> factors)
    {
      var result = new string[m_factors.Length - factors.Count];
      int j = 0;

      for (int i = 0; i < m_factors.Length; i++)
      {
        if (!factors.Contains(m_factors[i]))
          result[j++] = m_factors[i];
      }

      return result;
    }

    private void SingleFactorAnalysis(List<FisherTestResult> result, List<string> factors)
    {
      using (var group = new TableMultyDataGroup(m_source, factors.ToArray(), m_result_column, this.Filter, this.Conjugate))
      {
        FisherMetrics degrees = string.IsNullOrEmpty(this.Conjugate) 
          ? FisherCriteria.CalculateCriteria(group)
          : FisherCriteria.CalculateConjugate(group);

        if (degrees.Ndf != 0)
        {
          var row = new FisherTestResult
          {
            F = degrees.F,
            Kdf = degrees.Kdf,
            Ndf = degrees.Ndf,
            Factor = string.Join("+", factors),
            IgnoredFactor = string.Empty,
            Conjugate = this.Conjugate,
            FCritical = FisherCriteria.GetCriticalValue(m_probability, degrees.Kdf, degrees.Ndf),
            P = FisherCriteria.GetProbability(degrees)
          };

          lock (result)
            result.Add(row);
        }
      }
    }
  }
}