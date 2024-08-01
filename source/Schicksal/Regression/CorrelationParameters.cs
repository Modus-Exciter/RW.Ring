using Schicksal.Basic;
using System;
using System.Data;
using System.Linq;

namespace Schicksal.Regression
{
  /// <summary>
  /// Набор параметров для регрессионного анализа данных в таблице
  /// </summary>
  public sealed class CorrelationParameters : PredictedWithProbabilityResponseParameters
  {
    /// <summary>
    /// Инициализация набора параметров для регрессионного анализа данных в таблице
    /// </summary>
    /// <param name="table">Таблица</param>
    /// <param name="filter">Фильтр в таблице</param>
    /// <param name="predictors">Набор имён колонок, которые будут предикторами</param>
    /// <param name="response">Имя колонки, которая будет откликом</param>
    /// <param name="probability">Уровень значимости для анализа</param>
    public CorrelationParameters
    (
      DataTable table,
      string filter,
      FactorInfo predictors,
      string response,
      float probability
    )
      : base(table, filter, predictors, response, probability)
    {
      if (!IsNumeric(table.Columns[response].DataType))
        throw new ArgumentException("Result column must be numeric");

      if (predictors.Any(p => !IsNumeric(table.Columns[p].DataType)))
        throw new ArgumentException("All predictor columns must be numeric");
    }
  }
}