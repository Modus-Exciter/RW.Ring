using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Notung.Threading;
using Schicksal.Basic;

namespace Schicksal.Anova
{
  /// <summary>
  /// Методика расчёта остатков
  /// </summary>
  public interface IResudualsCalculator
  {
    /// <summary>
    /// Инициализация параметров расчёта перед обработкой каждого фактора
    /// </summary>
    /// <param name="parameters">Набор исходных данных для анализа с параметрами</param>
    /// <param name="sample">Преобразованный и нормированный набор данных для анализа</param>
    /// <param name="progress">Индикатор прогресса</param>
    void Start(AnovaParameters parameters, IDividedSample<GroupKey> sample, IProgressIndicator progress);

    /// <summary>
    /// Возвращает итератор, позволяющий перебрать все факторы и их комбинации, которые можно анализировать
    /// </summary>
    /// <returns>Итератор, обходящий доступные для анализа факторы</returns>
    IEnumerable<FactorInfo> GetSupportedFactors();

    /// <summary>
    /// Возвращает True, если в методике расчёта величина остатков не зависит от фактора. Иначе, возвращает False
    /// </summary>
    bool SingleWihinVariance { get; }

    /// <summary>
    /// Получение дисперсии остатков для указанного фактора
    /// </summary>
    /// <param name="factor">Фактор, для которого рассчитывается дисперсия остатков</param>
    /// <param name="progress">Индикатор прогресса</param>
    /// <returns></returns>
    SampleVariance GetWithinVariance(FactorInfo factor, IProgressIndicator progress);

    /// <summary>
    /// Расчёт стандартного отклонения по определённой градации одного фактора или сочетания факторов
    /// </summary>
    /// <param name="group">Набор данных, для которого требуется рассчитать стандартное отклонение</param>
    /// <param name="joined">Набор данных, для которого требуется рассчитать
    /// стандартное отклонение, преобразованный к числовой последовательности</param>
    /// <returns></returns>
    SampleVariance GetStandardDerivation(IDividedSample group, IPlainSample joined);

    /// <summary>
    /// Расчёт ошибки разности средних значений в двух градациях фактора
    /// </summary>
    /// <param name="group1">Первая градация фактора</param>
    /// <param name="group2">Вторая градация фактора</param>
    /// <returns></returns>
    GradationsCompareError GetError(Gradation group1, Gradation group2);
  }

  /// <summary>
  /// Одна градация фактора
  /// </summary>
  public struct Gradation
  {
    /// <summary>
    /// Выборка данных по градации фактора
    /// </summary>
    public IDividedSample Sample;

    /// <summary>
    /// Общее количество данных в выборке
    /// </summary>
    public int TotalCount;

    /// <summary>
    /// Стандартное отклонение в выборке
    /// </summary>
    public double ErrorValue;
  }

  /// <summary>
  /// Информация об ошибке для сравнения двух градаций
  /// </summary>
  public struct GradationsCompareError
  {
    /// <summary>
    /// Ошибка разности средних
    /// </summary>
    public double Value;

    /// <summary>
    /// Количество степеней свободы
    /// </summary>
    public int DegreesOfFreedom;
  }

  /// <summary>
  /// Методика расчёта остатков для независимых наблюдений с повторностями
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
      return FisherTest.MSw(group);
    }

    public GradationsCompareError GetError(Gradation grad1, Gradation grad2)
    {
      return new GradationsCompareError
      {
        Value = Math.Sqrt(grad1.ErrorValue / grad1.TotalCount + grad2.ErrorValue / grad2.TotalCount),
        DegreesOfFreedom = grad1.TotalCount + grad2.TotalCount - grad1.Sample.Count - grad2.Sample.Count
      };
    }
  }

  /// <summary>
  /// Методика расчёта остатков для сопряжённых наблюдений
  /// </summary>
  public sealed class ConjugatedResudualsCalculator : IResudualsCalculator
  {
    private SampleVariance m_within_variance;
    private AnovaParameters m_parameters;

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

      if (!(sample is IEqualSubSamples && sample.Count > 0 && sample[0].Count > 1))
        throw new ArgumentException("Sample must be IEqualSubSamples and full");

      m_parameters = parameters;
      m_within_variance = this.GetStandardDerivation(sample, null);
    }

    public IEnumerable<FactorInfo> GetSupportedFactors()
    {
      return m_parameters.Predictors.Split();
    }

    public SampleVariance GetStandardDerivation(IDividedSample group, IPlainSample joined)
    {
      if (group.Count == 1)
        return FisherTest.MSw(group);

      var repetitions_count = group[0].Count;
      double g_mean = group.SelectMany(g => g).Average();
      double r_var = 0;

      for (int i = 0; i < repetitions_count; i++)
      {
        var avg = group.Select(s => s[i]).Average();
        r_var += (avg - g_mean) * (avg - g_mean);
      }

      return new SampleVariance
      {
        DegreesOfFreedom = (group.Count - 1) * (repetitions_count - 1),
        SumOfSquares = FisherTest.MSw(group).SumOfSquares - r_var * group.Count
      };
    }

    public GradationsCompareError GetError(Gradation grad1, Gradation grad2)
    {
      var ms = this.GetStandardDerivation(new DoubleDividedSample(grad1.Sample, grad2.Sample), null);

      return new GradationsCompareError
      {
        Value = Math.Sqrt(ms.MeanSquare / grad1.TotalCount + ms.MeanSquare / grad2.TotalCount),
        DegreesOfFreedom = ms.DegreesOfFreedom
      };
    }

    private class DoubleDividedSample : IDividedSample
    {
      private readonly IDividedSample m_first;
      private readonly IDividedSample m_second;

      public DoubleDividedSample(IDividedSample first, IDividedSample second)
      {
        if (first == null) 
          throw new ArgumentNullException("first");

        if (second == null)
          throw new ArgumentNullException("second");

        m_first = first;
        m_second = second;
      }

      public IPlainSample this[int index]
      {
        get { return index < m_first.Count ? m_first[index] : m_second[index - m_first.Count]; }
      }

      public int Count
      {
        get { return m_first.Count + m_second.Count; }
      }

      public IEnumerator<IPlainSample> GetEnumerator()
      {
        return m_first.Concat(m_second).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return this.GetEnumerator();
      }
    }
  }

  /// <summary>
  /// Методика расчёта остатков для многофакторного эксперимента без повторностей
  /// </summary>
  public sealed class UnrepeatedResudualsCalculator : IResudualsCalculator
  {
    private AnovaParameters m_parameters;
    private IDividedSample<GroupKey> m_data;

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
      return FisherTest.MSw(GroupKey.Repack(m_data, factor));
    }

    public void Start(AnovaParameters parameters, IDividedSample<GroupKey> sample, IProgressIndicator progress)
    {
      if (parameters == null)
        throw new ArgumentNullException("parameters");

      m_parameters = parameters;
      m_data = sample;
    }

    public SampleVariance GetStandardDerivation(IDividedSample group, IPlainSample joined)
    {
      return new SampleVariance
      {
        SumOfSquares = DescriptionStatistics.SquareDerivation(joined),
        DegreesOfFreedom = joined.Count - 1
      };
    }

    public GradationsCompareError GetError(Gradation grad1, Gradation grad2)
    {
      return new GradationsCompareError
      {
        Value = Math.Sqrt(grad1.ErrorValue / grad1.TotalCount + grad2.ErrorValue / grad2.TotalCount),
        DegreesOfFreedom = grad1.TotalCount + grad2.TotalCount - 2
      };
    }
  }
}