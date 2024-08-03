using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Notung.Data;
using Schicksal.Basic;

namespace Schicksal.Anova
{
  /// <summary>
  /// Анализ данных с использованием критерия Фишера
  /// </summary>
  public static class FisherTest
  {
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
        if (sub_sample.Count == 0)
          continue;
        
        double mean = sub_sample.Average();

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
    /// Расчёт взаимодействий факторов
    /// </summary>
    /// <param name="list">Информация о взаимодействующих факторах</param>
    /// <param name="errorHandler">Вывод сообщений о невозможности вычислить взаимодействие факторов</param>
    /// <returns>True, если удалось посчитать взаимодействие факторов. Иначе False</returns>
    public static bool FactorInteraction(IList<FactorVariance> list, Action<FactorInfo> errorHandler = null)
    {
      if (list == null)
        throw new ArgumentNullException("betweenVariances");

      var keys = new KeyedArray<FactorInfo>(list.Count, i => list[i].Factor);

      if (!CheckGradationsCount(list, errorHandler))
        return false;

      foreach (int index in GetIndexOrder(list, keys))
      {
        FactorVariance result = list[index];

        if (result.Factor.Count == 1)
          continue;

        foreach (FactorInfo p in result.Factor.Split(false))
        {
          if (!keys.Contains(p))
            continue;

          FactorVariance res = list[keys.GetIndex(p)];

          result.Variance.DegreesOfFreedom -= res.Variance.DegreesOfFreedom;
          result.Variance.SumOfSquares -= res.Variance.SumOfSquares;

          if (result.Variance.DegreesOfFreedom <= 1)
            result.Variance.DegreesOfFreedom = 1;

          if (result.Variance.SumOfSquares < 0)
            result.Variance.SumOfSquares = 0;

          list[index] = result;
        }
      }

      return true;
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

    /// <summary>
    /// Расчёт вероятости того, что все группы в выборке относятся к одной генеральной совокупности
    /// </summary>
    /// <param name="between">Межгрупповая дисперсия выборки</param>
    /// <param name="within">Внутригрупповая дисперсия выборки</param>
    /// <returns>Вероятность нулевой гипотезы</returns>
    public static double GetProbability(SampleVariance between, SampleVariance within)
    {
      var f = between.MeanSquare / within.MeanSquare;

      if (f == 0)
        return 1;

      return SpecialFunctions.fcdistribution(between.DegreesOfFreedom, within.DegreesOfFreedom, f);
    }

    private static int[] GetIndexOrder(IList<FactorVariance> list, KeyedArray<FactorInfo> keys)
    {
      var graph = new UnweightedListGraph(list.Count, true);

      for (int i = 0; i < list.Count; i++)
      {
        var predictors = list[i].Factor;

        if (predictors.Count == 1)
          continue;

        foreach (var p in predictors.Split(false))
        {
          if (keys.Contains(p))
            graph.AddArc(keys.GetIndex(p), i);
        }
      }

      return TopologicalSort.Kahn(graph);
    }

    private static bool CheckGradationsCount(IList<FactorVariance> list, Action<FactorInfo> errorHandler)
    {
      var grad_count = new Dictionary<string, int>();
      bool ok = true;

      foreach (var sv in list)
      {
        if (sv.Factor.Count == 1)
          grad_count.Add(sv.Factor.First(), sv.Variance.DegreesOfFreedom + 1);
      }

      foreach (FactorVariance result in list)
      {
         if (result.Factor.Count == 1)
          continue;

        int expected_count = 1;

        foreach (var f in result.Factor)
        {
          int count;

          if (!grad_count.TryGetValue(f, out count))
          {
            if (errorHandler != null)
              errorHandler(new FactorInfo(new string[] { f }));

            ok = false;
          }

          expected_count *= count;
        }

        expected_count--;

        if (expected_count != result.Variance.DegreesOfFreedom)
        {
          if (errorHandler != null)
            errorHandler(result.Factor);

          ok = false;
        }
      }

      return ok;
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
  /// Дисперсия выборки с указанием того, по каким предикторам она получена
  /// </summary>
  public struct FactorVariance
  {
    private readonly FactorInfo m_factor;

    /// <summary>
    /// Инициализация нового экземпляра дисперсии выборки
    /// </summary>
    /// <param name="factor">Набор предикторов для выборки</param>
    /// <param name="variance">Дисперсия выборки</param>
    public FactorVariance(FactorInfo factor, SampleVariance variance)
    {
      if (factor == null) 
        throw new ArgumentNullException("factor");

      m_factor = factor;
      Variance = variance;
    }

    /// <summary>
    /// Набор предикторов для выборки
    /// </summary>
    public FactorInfo Factor
    {
      get { return m_factor ?? FactorInfo.Empty; }
    }

    /// <summary>
    /// Дисперсия выборки
    /// </summary>
    public SampleVariance Variance;

    /// <summary>
    /// Возвращает строковое представление объекта
    /// </summary>
    /// <returns>Список предикторов и арифметическое выражение, из которого получена дисперсия</returns>
    public override string ToString()
    {
      return string.Format("{0}: {1}", this.Factor, Variance);
    }
  }
}