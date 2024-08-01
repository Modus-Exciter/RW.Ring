using System.Diagnostics;
using System.Linq;
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
}