using System.ComponentModel;

namespace Schicksal.Anova
{
  /// <summary>
  /// Результат сравнения дисперсий в тесте Фишера
  /// </summary>
  public struct FisherMetrics
  {
    /// <summary>
    /// Число степеней свободы для межгрупповой дисперсии
    /// </summary>
    public uint Kdf { get; internal set; }

    /// <summary>
    /// Число степеней свободы для внутригрупповой дисперсии
    /// </summary>
    public uint Ndf { get; internal set; }

    /// <summary>
    /// Межгрупповая дисперсия
    /// </summary>
    public double MSb { get; internal set; }

    /// <summary>
    /// Суммарная внутригрупповая дисперсия
    /// </summary>
    public double MSw { get; internal set; }

    /// <summary>
    /// Отношение межгрупповой дисперсии к внутригрупповой
    /// </summary>
    public double F
    {
      get { return this.MSb / this.MSw; }
    }
  }

  /// <summary>
  /// Результат теста Фишера по указанному фактору
  /// </summary>
  public sealed class FisherTestResult
  {
    /// <summary>
    /// Фактор, влияние которого оценивается
    /// </summary>
    public string Factor { get; internal set; }

    /// <summary>
    /// Фактор, влияние которого игнорируется
    /// </summary>
    public string IgnoredFactor { get; internal set; }

    /// <summary>
    /// Число степеней свободы для межгрупповой дисперсии
    /// </summary>
    public uint Kdf { get; internal set; }

    /// <summary>
    /// Число степеней свободы для внутригрупповой дисперсии
    /// </summary>
    public uint Ndf { get; internal set; }

    /// <summary>
    /// Отношение межгрупповой дисперсии к внутригрупповой
    /// </summary>
    public double F { get; internal set; }

    /// <summary>
    /// Критическое значение критерия F для 5% вероятности нулевой гипотезы
    /// </summary>
    [DisplayName("F 5%")]
    public double F005 { get; internal set; }

    /// <summary>
    /// Критическое значение критерия F для 1% вероятности нулевой гипотезы
    /// </summary>
    [DisplayName("F 1%")]
    public double F001 { get; internal set; }

    /// <summary>
    /// Вероятность нулевой гипотезы для полученного значения критерия F
    /// </summary>
    [DisplayName("p")]
    public double P { get; internal set; }
  }
}
