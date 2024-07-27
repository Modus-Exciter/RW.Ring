using System.Linq;
using Schicksal.Basic;

namespace Schicksal.Anova
{
  /// <summary>
  /// Анализ данных с использованием критерия Фишера
  /// </summary>
  public static class FisherCriteria
  {
    /// <summary>
    /// Сравнение внутригрупповой и межгрупповой дисперсии тестом Фишера
    /// </summary>
    /// <param name="sample">Выборка данных, разделённая на группы</param>
    /// <returns>Результат сравнения в виде специальной структуры</returns>
    public static FisherMetrics CalculateCriteria(IDividedSample sample)
    {
      var join = new JoinedSample(sample);
      var means = new double[sample.Count];

      var degrees = default(FisherMetrics);
      degrees.Kdf = (uint)sample.Count - 1;
      degrees.Ndf = (uint)(join.Count - sample.Count);

      if (degrees.Kdf == 0 || degrees.Ndf == 0)
        return degrees;

      for (int i = 0; i < means.Length; i++)
        means[i] = DescriptionStatistics.Mean(sample[i]);

      double g_mean = DescriptionStatistics.Mean(join);

      double outer_dispersion = 0;

      for (int i = 0; i < sample.Count; i++)
        outer_dispersion += (means[i] - g_mean) * (means[i] - g_mean) * sample[i].Count;

      outer_dispersion /= degrees.Kdf;

      double inner_dispersion = 0;

      for (int i = 0; i < sample.Count; i++)
        inner_dispersion += DescriptionStatistics.SquareDerivation(sample[i]);

      inner_dispersion /= degrees.Ndf;

      degrees.MSb = outer_dispersion;
      degrees.MSw = inner_dispersion;

      return degrees;
    }

    public static FisherMetrics CalculateConjugate(IDividedSample sample)
    {
      var degrees = default(FisherMetrics);

      int n = sample.Sum(g => g.Count);
      double sum_all = sample.SelectMany(g => g).Sum();
      double c = sum_all * sum_all / n;
      double cy = sample.SelectMany(g => g).Sum(a => a * a) - c;
      int vars = sample[0].Count;

      double cp = 0;
      
      for (int i = 0; i < vars; i++)
      {
        double gp = sample.Select(g => g[i]).Sum();
        cp += gp * gp;
      }

      cp /= sample.Count;
      cp -= c;

      double cv = sample.Sum(g => System.Math.Pow(g.Sum(), 2)) / vars - c;

      double cz = cy - cp - cv;

      degrees.Kdf = (uint)(sample.Count - 1);
      degrees.Ndf = (uint)(n - degrees.Kdf - vars);
      degrees.MSb = cy / degrees.Kdf;
      degrees.MSw = cz / degrees.Ndf;

      return degrees;
    }

    public static FisherMetrics CalculateConjugate(IComplexSample sample)
    {
      var degrees = default(FisherMetrics);

      int n = sample.SelectMany(g => g).Sum(g => g.Count);
      double sum_all = sample.SelectMany(g => g).SelectMany(g => g).Sum();
      double c = sum_all * sum_all / n;
      double cy = sample.SelectMany(g => g).SelectMany(g => g).Sum(a => a * a) - c;
      int vars = sample[0][0].Count;

      double cp = 0;

      for (int i = 0; i < vars; i++)
      {
        double gp = sample.SelectMany(g => g).Select(g => g[i]).Sum();
        cp += gp * gp;
      }
      cp /= sample.Sum(g => g.Count);
      cp -= c;
      double cv = sample.SelectMany(g => g).Sum(g => System.Math.Pow(g.Sum(), 2)) / vars - c;

      double cz = cy - cp - cv;

      double ca = 0;

      foreach (var sub_sample in sample)
      {
        var sum = sub_sample.SelectMany(g => g).Sum();
        ca += sum * sum / sub_sample.Sum(g => g.Count);
      }

      ca -= c;

      degrees.Kdf = (uint)(sample.Count - 1);
      degrees.Ndf = (uint)(n - sample.Sum(g => g.Count) - vars + 1);
      degrees.MSb = ca / degrees.Kdf;
      degrees.MSw = cz / degrees.Ndf;

      return degrees;
    }

    /// <summary>
    /// Сравнение внутригрупповой и межгрупповой дисперсии
    /// тестом Фишера (для многофакторного анализа)
    /// </summary>
    /// <param name="sample">Выборка данных, разделённая на группы</param>
    /// <returns>Результат сравнения в виде специальной структуры</returns>
    public static FisherMetrics CalculateMultiplyCriteria(IComplexSample sample)
    {
      var degrees = default(FisherMetrics);

      degrees.Kdf = (uint)sample.Count - 1;
      degrees.Ndf = GetWithinDegreesOfFreedom(sample);

      if (degrees.Kdf == 0 || degrees.Ndf == 0)
        return degrees;

      var average_multi_group = new double[sample.Count];
      var average = sample.SelectMany(g => g).SelectMany(g => g).Average();

      for (int i = 0; i < sample.Count; i++)
        average_multi_group[i] = sample[i].SelectMany(g => g).Any() ? sample[i].SelectMany(g => g).Average() : average;

      double outer_dispersion = 0;
      double inner_dispersion = 0;

      for (int i = 0; i < average_multi_group.Length; i++)
        outer_dispersion += (average_multi_group[i] - average) * (average_multi_group[i] - average) * sample[i].Sum(g => g.Count);

      foreach (var group in sample.SelectMany(g => g))
        inner_dispersion += DescriptionStatistics.SquareDerivation(group);

      degrees.MSb = outer_dispersion / degrees.Kdf;
      degrees.MSw = inner_dispersion / degrees.Ndf;

      return degrees;
    }

    internal static uint GetWithinDegreesOfFreedom(IComplexSample sample)
    {
      return (uint)sample.SelectMany(g => g).Sum(g => g.Count) - (uint)sample.Sum(g => g.Count);
    }

    internal static double GetWithinDispersion(IComplexSample sample)
    {
      double inner_dispersion = 0;

      foreach (var group in sample.SelectMany(g => g))
        inner_dispersion += DescriptionStatistics.SquareDerivation(group);

      return inner_dispersion / GetWithinDegreesOfFreedom(sample);
    }

    /// <summary>
    /// Расчёт вероятости того, что все группы в выборке относятся к одной генеральной совокупности
    /// </summary>
    /// <param name="sample">Выборка данных, разделённая на группы</param>
    /// <returns>Вероятность нулевой гипотезы</returns>
    public static double GetProbability(IDividedSample sample)
    {
      FisherMetrics degrees = CalculateCriteria(sample);
      return SpecialFunctions.fcdistribution((int)degrees.Kdf, (int)degrees.Ndf, degrees.F);
    }

    /// <summary>
    /// Расчёт вероятости того, что все группы в выборке относятся к одной генеральной совокупности
    /// </summary>
    /// <param name="degrees">Результат сравнения внутригрупповой и межгрупповой дисперсии</param>
    /// <returns>Вероятность нулевой гипотезы</returns>
    public static double GetProbability(FisherMetrics degrees)
    {
      return SpecialFunctions.fcdistribution((int)degrees.Kdf, (int)degrees.Ndf, degrees.F);
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
}
