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
    /// <param name="group">Выборка данных, разделённая на группы</param>
    /// <returns>Результат сравнения в виде специальной структуры</returns>
    public static FisherMetrics CalculateCriteria(IMultyDataGroup group)
    {
      var join = new JoinedDataGroup(group);
      var means = new double[group.Count];

      var degrees = default(FisherMetrics);
      degrees.Kdf = (uint)group.Count - 1;
      degrees.Ndf = (uint)(join.Count - group.Count);

      if (degrees.Kdf == 0 || degrees.Ndf == 0)
        return degrees;

      for (int i = 0; i < means.Length; i++)
        means[i] = DescriptionStatistics.Mean(group[i]);

      double g_mean = DescriptionStatistics.Mean(join);

      double outer_dispersion = 0;

      for (int i = 0; i < group.Count; i++)
        outer_dispersion += (means[i] - g_mean) * (means[i] - g_mean) * group[i].Count;

      outer_dispersion /= degrees.Kdf;

      double inner_dispersion = 0;

      for (int i = 0; i < group.Count; i++)
        inner_dispersion += DescriptionStatistics.SquareDerivation(group[i]);

      inner_dispersion /= degrees.Ndf;

      degrees.MSb = outer_dispersion;
      degrees.MSw = inner_dispersion;

      return degrees;
    }

    public static FisherMetrics CalculateMultiplyCriteria(ISetMultyDataGroup groups)
    {
      var degrees = default(FisherMetrics);

      degrees.Kdf = (uint)groups.Count - 1;
      degrees.Ndf = (uint)groups.SelectMany(g => g).Sum(g => g.Count) - (uint)groups.Sum(g => g.Count);

      if (degrees.Kdf == 0 || degrees.Ndf == 0)
        return degrees;

      var average_multi_group = new double[groups.Count];
      var average = groups.SelectMany(g => g).SelectMany(g => g).Average();

      for (int i = 0; i < groups.Count; i++)
        average_multi_group[i] = groups[i].SelectMany(g=>g).Average();

      double outer_dispersion = 0;
      double inner_dispersion = 0;

      foreach (var mg in average_multi_group)
        outer_dispersion += (mg - average) * (mg - average);

      foreach (var group in groups.SelectMany(g => g))
        inner_dispersion += DescriptionStatistics.SquareDerivation(group);

      degrees.MSb = outer_dispersion / degrees.Kdf;
      degrees.MSw = inner_dispersion / degrees.Ndf;

      return degrees;
    }

    /// <summary>
    /// Расчёт вероятости того, что все группы в выборке относятся к одной генеральной совокупности
    /// </summary>
    /// <param name="group">Выборка данных, разделённая на группы</param>
    /// <returns>Вероятность нулевой гипотезы</returns>
    public static double GetProbability(IMultyDataGroup group)
    {
      FisherMetrics degrees = CalculateCriteria(group);
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
