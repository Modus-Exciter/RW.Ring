using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Schicksal.Regression
{
  public sealed class CorrelationMetrics
  {
    /// <summary>
    /// Фактор, для которого рассчитывается влияние
    /// </summary>
    public string Factor { get; set; }
    
    /// <summary>
    /// Коэффициент корреляции
    /// </summary>
    [DisplayName("r")]
    public double R { get; set; }

    /// <summary>
    /// Нормализованный коэффициент корреляции
    /// </summary>
    [DisplayName("z")]
    public double Z { get; set; }

    /// <summary>
    /// Критическое значение коэффициента корреляции для уровня значимости 0,05
    /// </summary>
    [DisplayName("R 5%")]
    public double R005 { get; set; }
    
    /// <summary>
    /// Критическое значение коэффициента корреляции для уровня значимости 0,01
    /// </summary>
    [DisplayName("R 1%")]
    public double R001 { get; set; }

    /// <summary>
    /// Вероятность нулевой гипотезы для полученного значения критерия R
    /// </summary>
    [DisplayName("p")]
    public double P { get; set; }
  }
}
