using System.ComponentModel;

namespace Schicksal.Regression
{
  /// <summary>
  /// Результаты теста корреляции
  /// </summary>
  public sealed class CorrelationMetrics
  {
    /// <summary>
    /// Фактор, для которого рассчитывается влияние
    /// </summary>
    public string Factor { get; internal set; }

    /// <summary>
    /// Количество пар значений при вычислении корреляции
    /// </summary>
    public int N { get; internal set; }
    
    /// <summary>
    /// Коэффициент корреляции
    /// </summary>
    [DisplayName("r")]
    public double R { get; internal set; }

    /// <summary>
    /// Нормализованный коэффициент корреляции
    /// </summary>
    [DisplayName("z")]
    public double Z { get; internal set; }

    /// <summary>
    /// Фактический критерий значимости корреляции
    /// </summary>
    public double T { get; internal set; }

    /// <summary>
    /// Критическое значение коэффициента корреляции для уровня значимости 0,05
    /// </summary>
    [DisplayName("R 5%")]
    public double R005 { get; internal set; }
    
    /// <summary>
    /// Критическое значение коэффициента корреляции для уровня значимости 0,01
    /// </summary>
    [DisplayName("R 1%")]
    public double R001 { get; internal set; }

    /// <summary>
    /// Вероятность нулевой гипотезы для полученного значения критерия R
    /// </summary>
    [DisplayName("p")]
    public double P { get; internal set; }
  }
}
