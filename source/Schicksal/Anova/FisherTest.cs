using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Notung;
using Notung.Data;
using Notung.Threading;
using Schicksal.Basic;

namespace Schicksal.Anova
{
  /// <summary>
  /// Анализ данных с использованием критерия Фишера
  /// </summary>
  public static class FisherTest
  {
    /// <summary>
    /// Расчёт вероятости того, что все группы в выборке относятся к одной генеральной совокупности
    /// </summary>
    /// <param name="between">Межгрупповая дисперсия выборки</param>
    /// <param name="within">Внутригрупповая дисперсия выборки</param>
    /// <returns>Вероятность нулевой гипотезы</returns>
    public static double GetProbability(SampleVariance between, SampleVariance within)
    {
      return SpecialFunctions.fcdistribution(between.DegreesOfFreedom, within.DegreesOfFreedom,
        between.MeanSquare / within.MeanSquare);
    }

    /// <summary>
    /// Расчёт межгрупповой дисперсии
    /// </summary>
    /// <param name="sample">Выборка, разделённая на группы</param>
    /// <returns>Межгрупповая дисперсия выборки</returns>
    public static SampleVariance MSb(IDividedSample sample)
    {
      Debug.Assert(sample != null, "sample cannot be null");

      double g_mean = sample.SelectMany(g => g).Average();
      double ss_b = 0;

      foreach (var sub_sample in sample)
      {
        double mean = sub_sample.Count > 0 ? sub_sample.Average() : 0;

        ss_b += (mean - g_mean) * (mean - g_mean) * sub_sample.Count;
      }

      return new SampleVariance
      {
        SumOfSquares = ss_b,
        DegreesOfFreedom = sample.Count - 1
      };
    }

    /// <summary>
    /// Расчёт внутригрупповой дисперсии
    /// </summary>
    /// <param name="sample">Выборка, разделённая на группы</param>
    /// <returns>Внутригрупповая дисперсия выборки</returns>
    public static SampleVariance MSw(IDividedSample sample)
    {
      Debug.Assert(sample != null, "sample cannot be null");

      double ss_w = 0;
      int g_count = 0;

      foreach (var sub_sample in sample)
      {
        ss_w += DescriptionStatistics.SquareDerivation(sub_sample);
        g_count += sub_sample.Count;
      }

      return new SampleVariance
      {
        SumOfSquares = ss_w,
        DegreesOfFreedom = g_count - sample.Count
      };
    }

    /// <summary>
    /// Расчёт критического значения критерия Фишера
    /// </summary>
    /// <param name="p">Вероятность нулевой гипотезы</param>
    /// <param name="kdf">Число степеней свободы для межгрупповой дисперсии</param>
    /// <param name="ndf">Число степеней свободы для внутригрупповой дисперсии</param>
    /// <returns></returns>
    public static double GetCriticalValue(double p, uint kdf, uint ndf)
    {
      return SpecialFunctions.invfdistribution((int)kdf, (int)ndf, p);
    }
  }

  /// <summary>
  /// Дисперсия выборки
  /// </summary>
  public struct SampleVariance
  {
    /// <summary>
    /// Сумма квадратов отклонений от средних
    /// </summary>
    public double SumOfSquares;

    /// <summary>
    /// Количество степеней свободы
    /// </summary>
    public int DegreesOfFreedom;

    /// <summary>
    /// Величина дисперсии
    /// </summary>
    public double MeanSquare
    {
      get { return this.SumOfSquares / this.DegreesOfFreedom; }
    }

    /// <summary>
    /// Строковое представление объекта
    /// </summary>
    /// <returns>Арифметическое выражение, из которого получена дисперсия</returns>
    public override string ToString()
    {
      return string.Format("{0} / {1} = {2}", this.SumOfSquares, this.DegreesOfFreedom, this.MeanSquare);
    }
  }

  /// <summary>
  /// Задача дисперсионного анализа таблицы
  /// </summary>
  public class AnovaCalculator : RunBase, IProgressIndicator
  {
    private readonly AnovaParameters m_parameters;
    private IResudualsCalculator m_residuals_calculator;

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

      for (int i = 0; i < this.Result.Length; i++)
      {
        this.Result[i] = new FisherTestResult
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
        return sample.Sum(g => g.Count) > sample.Count;
      }
    }
  }
}