using Schicksal.Basic;
using System.ComponentModel;

namespace Schicksal.Anova
{
  /// <summary>
  /// Результат теста Фишера по указанному фактору
  /// </summary>
  public sealed class FisherTestResult
  {
    /// <summary>
    /// Фактор, влияние которого оценивается
    /// </summary>
    public FactorInfo Factor { get; internal set; }

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
    /// Сумма квадратов отклонений внутри групп
    /// </summary>
    public double SSw { get; internal set; }

    /// <summary>
    /// Критическое значение критерия F для указанного уровня значимости
    /// </summary>
    public double FCritical { get; internal set; }

    /// <summary>
    /// Вероятность нулевой гипотезы для полученного значения критерия F
    /// </summary>
    [DisplayName("p")]
    public double P { get; internal set; }
  }
}
