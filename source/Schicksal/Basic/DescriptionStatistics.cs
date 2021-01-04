using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Notung;

namespace Schicksal.Basic
{
  /// <summary>
  /// Описательные статистики для выборок
  /// </summary>
  public static class DescriptionStatistics
  {
    public static double Mean(IDataGroup group)
    {
      Debug.Assert(group != null);

      return group.Average();
    }

    public static double Median(IDataGroup group)
    {
      return group.OrderBy(d => d).Skip(group.Count / 2).First();
    }

    /// <summary>
    /// Сумма квадратов отклонений
    /// </summary>
    public static double SquareDerivation(IDataGroup group)
    {
      Debug.Assert(group != null);

      var mean = Mean(group);
      double sum = 0;

      foreach (var value in group)
      {
        var derivation = value - mean;
        sum += derivation * derivation;
      }

      return sum;
    }

    /// <summary>
    /// Выборочная дисперсия
    /// </summary>
    public static double Dispresion(IDataGroup group)
    {
      Debug.Assert(group != null);
      Debug.Assert(group.Count > 1);

      return SquareDerivation(group) / (group.Count - 1);
    }
  }

  public class DescriptionStatisticsCalculator : RunBase
  {
    private readonly DataTable m_table;
    private readonly string m_result;
    private readonly string[] m_factors;
    private readonly string m_filter;
    private readonly double m_probability;

    public DescriptionStatisticsCalculator(DataTable table, string[] factors, string result, string filter, double probability)
    {
      m_table = table;
      m_factors = factors;
      m_result = result;
      m_filter = filter;
      m_probability = probability;
    }

    public DescriptionStatisticsEntry[] Result { get; private set; }
    
    public override void Run()
    {
      var group = new TableMultyDataGroup(m_table, m_factors, m_result, m_filter);

      var res = new DescriptionStatisticsEntry[group.Count];

      for (int i = 0; i < res.Length; i++)
      {
        var name = group.GetKey(i);
        
        if (name.Contains(" AND "))
          name = name.Substring(name.IndexOf(" AND ") + 5);

        res[i] = new DescriptionStatisticsEntry
        {
          Description = name,
          Mean = DescriptionStatistics.Mean(group[i]),
          Median = DescriptionStatistics.Median(group[i]),
          Min = group[i].Min(),
          Max = group[i].Max(),
          Count = group[i].Count
        };

        if (res[i].Count > 1)
        {
          res[i].StdError = Math.Sqrt(DescriptionStatistics.Dispresion(group[i]));
          res[i].ConfidenceInterval = res[i].StdError / Math.Sqrt(group[i].Count) *
            SpecialFunctions.invstudenttdistribution(group[i].Count - 1, 1 - m_probability / 2);
        }
        else
        {
          res[i].StdError = double.NaN;
          res[i].ConfidenceInterval = double.NaN;
        }
      }

      this.Result = res;
    }
  }


  public class DescriptionStatisticsEntry
  {
    public string Description { get; internal set; }

    public double Mean { get; internal set; }

    public double Median { get; internal set; }

    public double Min { get; internal set; }

    public double Max { get; internal set; }

    public int Count { get; internal set; }

    public double StdError { get; internal set; }

    public double ConfidenceInterval { get; internal set; }
  }
}
