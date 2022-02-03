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
    /// Критическое значение коэффициента корреляции для уровня значимости 0,05
    /// </summary>
    [DisplayName("T 5%")]
    public double T005 { get; internal set; }

    /// <summary>
    /// Критическое значение коэффициента корреляции для уровня значимости 0,01
    /// </summary>
    [DisplayName("T 1%")]
    public double T001 { get; internal set; }

    /// <summary>
    /// Коэффициент линейной корреляции
    /// </summary>
    [DisplayName("r")]
    public double R { get; internal set; }

    /// <summary>
    /// Фактический критерий значимости линейной корреляции
    /// </summary>
    [DisplayName("tr")]
    public double TR { get; internal set; }

    /// <summary>
    /// Вероятность нулевой гипотезы для полученного значения критерия R
    /// </summary>
    [DisplayName("pr")]
    public double PR { get; internal set; }

    /// <summary>
    /// Коэффициент криволинейной корреляции
    /// </summary>
    [DisplayName("η")]
    public double Eta { get; internal set; }

    /// <summary>
    /// Фактический критерий значимости криволинейной корреляции
    /// </summary>
    [DisplayName("tη")]
    public double TH { get; internal set; }

    /// <summary>
    /// Вероятность нулевой гипотезы для полученного значения критерия η
    /// </summary>
    [DisplayName("pη")]
    public double PH { get; internal set; }

    /// <summary>
    /// Нормализованный коэффициент корреляции
    /// </summary>
    [DisplayName("z")]
    [Browsable(false)]
    public double Z { get; internal set; }
  }
}