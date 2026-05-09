namespace Schicksal.Regression
{
  public sealed class MultifactorRegressionResult
  {
    public string ModelType { get; internal set; } // Тип модели (Линейная, Параболическая)
    public double[] Coefficients { get; internal set; } // Коэффициенты
    public string[] FactorNames { get; internal set; }  // Имена факторов

    public double RSquared { get; internal set; } // Коэффициент детерминации R-квадрат
    public double AdjustedRSquared { get; internal set; } // Скорректированный R-квадрат
    public double FStatistic { get; internal set; } // F-статистика модели
    public double PValueFStatistic { get; internal set; } // P-значение для F-статистики

    public double[] StandardErrors { get; internal set; } // Стандартные ошибки для каждого коэффициента
    public double[] TStatistics { get; internal set; }    // T-статистики для каждого коэффициента
    public double[] PValuesTStatistics { get; internal set; } // P-значения для каждой T-статистики

    public double ResidualSumOfSquares { get; internal set; } // Сумма квадратов остатков (SSR)
    public double TotalSumOfSquares { get; internal set; }    // Общая сумма квадратов (SST)
    public int DegreesOfFreedomResidual { get; internal set; } // Степени свободы остатков
    public int DegreesOfFreedomRegression { get; internal set; } // Степени свободы регрессии
  }
}
