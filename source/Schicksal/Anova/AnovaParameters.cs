using Schicksal.Basic;
using System;
using System.Data;

namespace Schicksal.Anova
{
  /// <summary>
  /// Набор параметров для дисперсионного анализа данных в таблице
  /// </summary>
  public sealed class AnovaParameters : PredictedWithProbabilityResponseParameters
  {
    private readonly INormalizer m_normalizer;
    private readonly string m_conjugation;
    private readonly bool m_individual_error;

    /// <summary>
    /// Инициализация набора параметров для дисперсионного анализа данных в таблице
    /// </summary>
    /// <param name="table">Таблица</param>
    /// <param name="filter">Фильтр в таблице</param>
    /// <param name="predictors">Набор имён колонок, которые будут предикторами</param>
    /// <param name="response">Имя колонки, которая будет откликом</param>
    /// <param name="probability">Уровень значимости для анализа</param>
    /// <param name="normalizer">Алгоритм нормирования данных</param>
    /// <param name="conjugation">Имя колонки, идентифицирующей сопряжённые наблюдения</param>
    public AnovaParameters
    (
      DataTable table,
      string filter,
      FactorInfo predictors,
      string response,
      double probability,
      INormalizer normalizer,
      string conjugation,
      bool individualError
    ) : base(table, filter, predictors, response, probability)
    {
      if (normalizer == null)
        throw new ArgumentNullException("normalizer");

      if (!string.IsNullOrEmpty(conjugation) && !table.Columns.Contains(conjugation))
        throw new ArgumentException(string.Format("Conjugation column {0} not found in the table", response));

      if (!IsNumeric(table.Columns[response].DataType))
        throw new ArgumentException("Result column must be numeric");

      m_normalizer = normalizer;
      m_conjugation = string.IsNullOrEmpty(conjugation) ? null : conjugation;
      m_individual_error = individualError;
    }

    /// <summary>
    /// Алгоритм нормирования данных
    /// </summary>
    public INormalizer Normalizer
    {
      get { return m_normalizer; }
    }

    /// <summary>
    /// Имя колонки, идентифицирующей сопряжённые наблюдения
    /// </summary>
    public string Conjugation
    {
      get { return m_conjugation; }
    }

    /// <summary>
    /// Использовать индивидуальное значение дисперсии при расчёте наименьшей существенной разницы
    /// </summary>
    public bool IndividualError
    {
      get { return m_individual_error; }
    }
  }
}
