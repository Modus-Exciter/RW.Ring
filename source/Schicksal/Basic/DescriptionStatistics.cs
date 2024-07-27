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
    /// <summary>
    /// Среднее арифметическое
    /// </summary>
    public static double Mean(IPlainSample sample)
    {
      Debug.Assert(sample != null);

      return sample.Count > 0 ? sample.Average() : double.NaN;
    }

    /// <summary>
    /// Медиана
    /// </summary>
    public static double Median(IPlainSample sample)
    {
      var ordered = OrderedSample.Construct(sample);

      if (sample.Count % 2 == 0)
        return (ordered[sample.Count / 2 - 1] + ordered[sample.Count / 2]) / 2;
      else
        return ordered[sample.Count / 2];
    }

    /// <summary>
    /// Сумма квадратов отклонений
    /// </summary>
    public static double SquareDerivation(IPlainSample sample)
    {
      Debug.Assert(sample != null);

      var mean = Mean(sample);
      double sum = 0;

      foreach (var value in sample)
      {
        var derivation = value - mean;
        sum += derivation * derivation;
      }

      return sum;
    }

    /// <summary>
    /// Сумма модулей отклонений
    /// </summary>
    public static double PlainDerivation(IPlainSample sample)
    {
      Debug.Assert(sample != null);

      var mean = Mean(sample);
      double sum = 0;

      foreach (var value in sample)
        sum += Math.Abs(value - mean);

      return sum;
    }

    /// <summary>
    /// Выборочная дисперсия
    /// </summary>
    public static double Dispresion(IPlainSample sample)
    {
      Debug.Assert(sample != null);
      Debug.Assert(sample.Count > 1);

      return SquareDerivation(sample) / (sample.Count - 1);
    }
    /// <summary>
    /// Упрощенная выборочная дисперсия
    /// </summary>
    public static double PlainDispersion(IPlainSample sample)
    {
      Debug.Assert(sample != null);
      Debug.Assert(sample.Count > 1);

      return PlainDerivation(sample) / Math.Sqrt(sample.Count - 1);
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

    public string[] Factors
    {
      get { return m_factors; }
    }

    public override void Run()
    {
      using (var sample = new TableDividedSample(m_table, m_factors, m_result, m_filter))
      {
        var res = new DescriptionStatisticsEntry[sample.Count];

        for (int i = 0; i < res.Length; i++)
        {
          var name = sample.GetKey(i);

          if (name.Contains(" AND "))
            name = name.Substring(name.IndexOf(" AND ") + 5);

          if (!string.IsNullOrEmpty(m_filter))
            name = name.Replace(" AND " + m_filter, "");

          res[i] = new DescriptionStatisticsEntry
          {
            Description = name.Replace(" AND ", ", ").Replace("[", "").Replace("]", ""),
            Mean = DescriptionStatistics.Mean(sample[i]),
            Median = DescriptionStatistics.Median(sample[i]),
            Min = sample[i].Min(),
            Max = sample[i].Max(),
            Count = sample[i].Count
          };

          if (res[i].Count > 1)
          {
            res[i].StdError = Math.Sqrt(DescriptionStatistics.Dispresion(sample[i]));
            res[i].ConfidenceInterval = res[i].StdError / Math.Sqrt(sample[i].Count) *
              SpecialFunctions.invstudenttdistribution(sample[i].Count - 1, 1 - m_probability / 2);
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
  }

  public class DescriptionStatisticsEntry
  {
    internal DescriptionStatisticsEntry() { }

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