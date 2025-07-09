namespace Schicksal.Regression
{
  public class MultifactorRegressionResult
  {
    public string ModelType { get; set; } // Тип модели (Линейная, Параболическая)
    public double[] Coefficients { get; set; } // Коэффициенты
    public string[] FactorNames { get; set; }  // Имена факторов

    public double RSquared { get; set; } // Коэффициент детерминации R-квадрат
    public double AdjustedRSquared { get; set; } // Скорректированный R-квадрат
    public double FStatistic { get; set; } // F-статистика модели
    public double PValueFStatistic { get; set; } // P-значение для F-статистики

    public double[] StandardErrors { get; set; } // Стандартные ошибки для каждого коэффициента
    public double[] TStatistics { get; set; }    // T-статистики для каждого коэффициента
    public double[] PValuesTStatistics { get; set; } // P-значения для каждой T-статистики

    public double ResidualSumOfSquares { get; set; } // Сумма квадратов остатков (SSR)
    public double TotalSumOfSquares { get; set; }    // Общая сумма квадратов (SST)
    public int DegreesOfFreedomResidual { get; set; } // Степени свободы остатков
    public int DegreesOfFreedomRegression { get; set; } // Степени свободы регрессии
  }
}
