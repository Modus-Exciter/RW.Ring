using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Notung.Threading;
using Schicksal.Basic;

namespace Schicksal.Anova
{
  /// <summary>
  /// Дисперсионный анализ
  /// </summary>
  public interface IResudualsCalculator
  {
    void Start(AnovaParameters parameters, IDividedSample<GroupKey> sample, IProgressIndicator progress);

    IEnumerable<FactorInfo> GetSupportedFactors();

    bool SingleWihinVariance { get; }

    SampleVariance GetWithinVariance(FactorInfo factor, IProgressIndicator progress);

    SampleVariance GetStandardDerivation(IDividedSample group, IPlainSample joined);

    ErrorInfo GetErrorInfo(LSDHelper sample1, LSDHelper sample2);
  }

  public struct LSDHelper
  {
    public IDividedSample Sample;

    public int InnerCount;

    public double ErrorValue;
  }

  public struct ErrorInfo
  {
    public double Value;
    public int DegreesOfFreedom;
  }


  /// <summary>
  /// Многофакторный анализ независимых наблюдений с повторностями
  /// </summary>
  public sealed class IndenepdentResudualsCalculator : IResudualsCalculator
  {
    private SampleVariance m_within_variance;
    private AnovaParameters m_parameters;

    public bool SingleWihinVariance
    {
      get { return true; }
    }

    public void Start(AnovaParameters parameters, IDividedSample<GroupKey> sample, IProgressIndicator progress)
    {
      Debug.Assert(parameters != null);

      m_parameters = parameters;
      m_within_variance = FisherTest.MSw(sample);
    }

    public SampleVariance GetWithinVariance(FactorInfo factor, IProgressIndicator progress)
    {
      return m_within_variance;
    }

    public IEnumerable<FactorInfo> GetSupportedFactors()
    {
      return m_parameters.Predictors.Split();
    }

    public SampleVariance GetStandardDerivation(IDividedSample group, IPlainSample joined)
    {
      return new SampleVariance
      {
        SumOfSquares = group.Sum(b => DescriptionStatistics.SquareDerivation(b)),
        DegreesOfFreedom = joined.Count - group.Count
      };
    }

    public ErrorInfo GetErrorInfo(LSDHelper sample1, LSDHelper sample2)
    {
      return new ErrorInfo
      {
        Value = Math.Sqrt(sample1.ErrorValue / sample1.InnerCount + sample2.ErrorValue / sample2.InnerCount),
        DegreesOfFreedom = sample1.InnerCount + sample2.InnerCount - sample1.Sample.Count - sample2.Sample.Count
      };
    }
  }

  /// <summary>
  /// Многофакторный анализ сопряжённых наблюдений
  /// </summary>
  public sealed class ConjugatedResudualsCalculator : IResudualsCalculator
  {
    private SampleVariance m_within_variance;
    private AnovaParameters m_parameters;
    private int m_repetitions_count;

    public bool SingleWihinVariance
    {
      get { return true; }
    }

    public SampleVariance GetWithinVariance(FactorInfo factor, IProgressIndicator progress)
    {
      return m_within_variance;
    }

    public void Start(AnovaParameters parameters, IDividedSample<GroupKey> sample, IProgressIndicator progress)
    {
      Debug.Assert(parameters != null);

      if (!(sample is IEqualSubSamples && sample.Count > 1 && sample[0].Count > 1))
        throw new ArgumentException("Sample must be IEqualSubSamples and full");

      m_within_variance = FisherTest.MSw(sample);
      m_parameters = parameters;

      m_repetitions_count = sample[0].Count;
      double g_mean = sample.SelectMany(g => g).Average();
      double r_var = 0;

      for (int i = 0; i < m_repetitions_count; i++)
      {
        var avg = sample.Select(s => s[i]).Average();
        r_var += (avg - g_mean) * (avg - g_mean);
      }

      m_within_variance.SumOfSquares -= r_var * sample.Count;
      m_within_variance.DegreesOfFreedom = (sample.Count - 1) * (m_repetitions_count - 1);
    }

    public IEnumerable<FactorInfo> GetSupportedFactors()
    {
      return m_parameters.Predictors.Split();
    }

    public SampleVariance GetStandardDerivation(IDividedSample group, IPlainSample joined)
    {
      return new SampleVariance
      {
        SumOfSquares = group.Sum(b => DescriptionStatistics.SquareDerivation(b)),
        DegreesOfFreedom = joined.Count - group.Count
      };
    }

    public ErrorInfo GetErrorInfo(LSDHelper sample1, LSDHelper sample2)
    {
      double[] diffs = new double[sample1.Sample.Count * sample1.Sample[0].Count];
      int index = 0;

      for (int i = 0; i < sample1.Sample.Count && i < sample2.Sample.Count; i++)
      {
        for (int j = 0; j < sample1.Sample[i].Count && j < sample2.Sample[i].Count; j++)
          diffs[index++] = sample1.Sample[i][j] - sample2.Sample[i][j];
      }

      return new ErrorInfo
      {
        Value = Math.Sqrt(DescriptionStatistics.Dispresion(new ArrayPlainSample(diffs)) / diffs.Length),
        DegreesOfFreedom = diffs.Length - 1
      };
    }
  }

  public sealed class UnrepeatedResudualsCalculator : IResudualsCalculator
  {
    private AnovaParameters m_parameters;

    public IEnumerable<FactorInfo> GetSupportedFactors()
    {
      return m_parameters.Predictors.Split().Where(p => p != m_parameters.Predictors);
    }

    public bool SingleWihinVariance
    {
      get { return false; }
    }

    public SampleVariance GetWithinVariance(FactorInfo factor, IProgressIndicator progress)
    {
      var parameters = new PredictedResponseParameters(m_parameters.Table, m_parameters.Filter,
        m_parameters.Predictors - factor, m_parameters.Response);

      using (var tableSample = new TableDividedSample(parameters, null))
      {
        IDividedSample sample = SampleRepack.Wrap(m_parameters.Normalizer.Normalize(tableSample));
        return FisherTest.MSw(sample);
      }
    }

    public void Start(AnovaParameters parameters, IDividedSample<GroupKey> sample, IProgressIndicator progress)
    {
      m_parameters = parameters;
    }

    public SampleVariance GetStandardDerivation(IDividedSample group, IPlainSample joined)
    {
      return new SampleVariance
      {
        SumOfSquares = DescriptionStatistics.SquareDerivation(joined),
        DegreesOfFreedom = joined.Count - 1
      };
    }

    public int GetDifferenceDegreesOfFreedom(int groupCount, int observationsCount)
    {
      return observationsCount - 1;
    }

    public ErrorInfo GetErrorInfo(LSDHelper sample1, LSDHelper sample2)
    {
      return new ErrorInfo
      {
        Value = Math.Sqrt(sample1.ErrorValue / sample1.InnerCount + sample2.ErrorValue / sample2.InnerCount),
        DegreesOfFreedom = sample1.InnerCount + sample2.InnerCount - 2
      };
    }
  }
}