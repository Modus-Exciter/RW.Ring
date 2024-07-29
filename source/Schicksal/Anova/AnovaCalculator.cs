using System;
using System.Collections.Generic;
using System.Linq;
using Notung;
using Notung.Data;
using Notung.Threading;
using Schicksal.Basic;

namespace Schicksal.Anova
{
  /// <summary>
  /// Задача дисперсионного анализа таблицы
  /// </summary>
  public class AnovaCalculator : RunBase, IProgressIndicator
  {
    private readonly AnovaParameters m_parameters;
    private IResudualsCalculator m_residuals_calculator;
    private IValueTransform m_denormalizer;

    /// <summary>
    /// Инициализация задачи дисперсионного анализа таблицы
    /// </summary>
    /// <param name="parameters">Набор параметров - таблица с опциями</param>
    public AnovaCalculator(AnovaParameters parameters)
    {
      if (parameters == null)
        throw new ArgumentNullException("parameters");

      m_parameters = parameters;
    }

    /// <summary>
    /// Объект вычисления остатков, требуемый для анализа
    /// </summary>
    public IResudualsCalculator ResudualsCalculator
    {
      get { return m_residuals_calculator; }
    }

    /// <summary>
    /// Преобразователь нормированных данных в ненормированные
    /// </summary>
    public IValueTransform Denormalizer
    {
      get { return m_denormalizer; }
    }

    /// <summary>
    /// Результаты теста по критерию Фишера
    /// </summary>
    public FisherTestResult[] Result { get; private set; }

    /// <summary>
    /// Запуск задачи на выполнение
    /// </summary>
    public override void Run()
    {
      m_residuals_calculator = this.HasRepetitions() ?
        (string.IsNullOrEmpty(m_parameters.Conjugation) ? new IndenepdentResudualsCalculator() :
        (IResudualsCalculator)new ConjugatedResudualsCalculator()) :
        (IResudualsCalculator)new UnrepeatedResudualsCalculator();

      m_residuals_calculator.Start(m_parameters, this);

      var list = new List<TestResult>();

      foreach (var p in m_residuals_calculator.GetSupportedFactors())
      {
        var parameters = new PredictedResponseParameters(m_parameters.Table,
          m_parameters.Filter, p, m_parameters.Response);

        using (var tableSample = new TableDividedSample(parameters, m_parameters.Conjugation))
        {
          IDividedSample sample = SampleRepack.Wrap(m_parameters.Normalizer.Normalize(tableSample));

          var ms_w = m_residuals_calculator.GetWithinVariance(p, this);
          var ms_b = FisherTest.MSb(sample);

          if (ms_w.MeanSquare != 0 && ms_w.DegreesOfFreedom != 0)
          {
            list.Add(new TestResult
            {
              Between = ms_b,
              Within = ms_w,
              Factor = p
            });
          }
        }
      }

      FindInteraction(list);

      this.Result = this.ConvertResult(list);
    }

    void IProgressIndicator.ReportProgress(int percentage, string state)
    {
      base.ReportProgress(percentage, state);
    }

    private FisherTestResult[] ConvertResult(List<TestResult> list)
    {
      var ret = new FisherTestResult[list.Count];

      for (int i = 0; i < ret.Length; i++)
      {
        ret[i] = new FisherTestResult
        {
          Conjugate = m_parameters.Conjugation,
          Factor = list[i].Factor,
          IgnoredFactor = (m_parameters.Predictors - list[i].Factor).ToString(),
          F = list[i].Between.MeanSquare / list[i].Within.MeanSquare,
          Kdf = (uint)list[i].Between.DegreesOfFreedom,
          Ndf = (uint)list[i].Within.DegreesOfFreedom,
          FCritical = FisherTest.GetCriticalValue
            (
              m_parameters.Probability,
              (uint)list[i].Between.DegreesOfFreedom,
              (uint)list[i].Within.DegreesOfFreedom
            ),
          P = FisherTest.GetProbability(list[i].Between, list[i].Within)
        };
      }

      return ret;
    }

    private static void FindInteraction(List<TestResult> list)
    {
      var graph = new UnweightedListGraph(list.Count, true);
      var dic = new Dictionary<FactorInfo, int>();

      for (int i = 0; i < list.Count; i++)
        dic.Add(list[i].Factor, i);

      for (int i = 0; i < list.Count; i++)
      {
        var predictors = list[i].Factor;

        if (predictors.Count == 1)
          continue;

        foreach (var p in predictors.Split().Where(px => px != predictors))
        {
          if (dic.ContainsKey(p))
            graph.AddArc(dic[p], i);
        }
      }

      var indexes = TopologicalSort.Kahn(graph);

      foreach (var index in indexes)
      {
        var result = list[index];

        if (result.Factor.Count == 1)
          continue;

        foreach (var p in result.Factor.Split().Where(px => px != result.Factor))
        {
          if (!dic.ContainsKey(p))
            continue;

          var res = list[dic[p]];
          result.Between.DegreesOfFreedom -= res.Between.DegreesOfFreedom;
          result.Between.SumOfSquares -= res.Between.SumOfSquares;
        }
      }
    }

    private class TestResult
    {
      public FactorInfo Factor;

      public SampleVariance Between;

      public SampleVariance Within;
    }

    private bool HasRepetitions()
    {
      using (var sample = new TableDividedSample(m_parameters, m_parameters.Conjugation))
      {
        m_denormalizer = m_parameters.Normalizer.Prepare(m_parameters.Normalizer.Normalize(sample));

        return sample.Sum(g => g.Count) > sample.Count;
      }
    }
  }
}
