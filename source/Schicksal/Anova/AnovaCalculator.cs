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
    private IValueTransform m_transform;

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
    public IValueTransform ValueTransform
    {
      get { return m_transform; }
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
      var transformed = this.GetInitialData();

      m_residuals_calculator.Start(m_parameters, transformed, this);

      var list = new List<TestResult>();

      int totals = 1 << m_parameters.Predictors.Count;
      int current = 0;

      foreach (var p in m_residuals_calculator.GetSupportedFactors())
      {
        var parameters = new PredictedResponseParameters(m_parameters.Table,
          m_parameters.Filter, p, m_parameters.Response);

        var sample = GroupKey.Repack(transformed, p);
        var ms_b = FisherTest.MSb(sample);

        this.AddPredictorResult(list, p, ms_b);

        current++;

        this.ReportProgress(current * 100 / totals, p.ToString());
      }

      if (m_residuals_calculator.SingleWihinVariance)
      {
        var ms_w = m_residuals_calculator.GetWithinVariance(m_parameters.Predictors, this);

        foreach (var item in list)
          item.Within = ms_w;

        FindInteraction(list);
      }

      this.Result = this.ConvertResult(list);
    }

    void IProgressIndicator.ReportProgress(int percentage, string state)
    {
      base.ReportProgress(percentage, state);
    }

    private void AddPredictorResult(List<TestResult> list, FactorInfo p, SampleVariance ms_b)
    {
      if (m_residuals_calculator.SingleWihinVariance)
      {
        list.Add(new TestResult
        {
          Between = ms_b,
          Factor = p
        });
      }
      else
      {
        var ms_w = m_residuals_calculator.GetWithinVariance(p, this);

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

    private IDividedSample<GroupKey> GetInitialData()
    {
      using (var table = new TableDividedSample(m_parameters, m_parameters.Conjugation))
      {
        m_transform = m_parameters.Normalizer.Prepare(m_parameters.Normalizer.Normalize(table));

        if (table.Sum(g => g.Count) > table.Count)
        {
          if (string.IsNullOrEmpty(m_parameters.Conjugation))
            m_residuals_calculator = new IndenepdentResudualsCalculator();
          else
            m_residuals_calculator = new ConjugatedResudualsCalculator();
        }
        else
          m_residuals_calculator = new UnrepeatedResudualsCalculator();

        return SampleRepack.Wrap(new ArrayDividedSample<GroupKey>(m_transform.Normalize(table), table.GetKey));
      }
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

        foreach (var p in predictors.Split(false))
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

        foreach (var p in result.Factor.Split(false))
        {
          if (!dic.ContainsKey(p))
            continue;

          var res = list[dic[p]];
          result.Between.DegreesOfFreedom -= res.Between.DegreesOfFreedom;
          result.Between.SumOfSquares -= res.Between.SumOfSquares;

          if (result.Between.DegreesOfFreedom <= 1)
            result.Between.DegreesOfFreedom = 1;

          if (result.Between.SumOfSquares < 0)
            result.Between.SumOfSquares = 0;
        }
      }
    }

    private class TestResult
    {
      public FactorInfo Factor;

      public SampleVariance Between;

      public SampleVariance Within;
    }
  }
}
