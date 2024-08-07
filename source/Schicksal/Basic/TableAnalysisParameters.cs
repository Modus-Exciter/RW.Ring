﻿using System;
using System.Data;
using System.Diagnostics;

namespace Schicksal.Basic
{
  /// <summary>
  /// Набор параметров для анализа данных в таблице
  /// </summary>
  public class TableAnalysisParameters
  {
    private readonly DataTable m_table;
    private readonly string m_filter;

    /// <summary>
    /// Инициализация набора параметров для анализа данных в таблице
    /// </summary>
    /// <param name="table">Таблица</param>
    /// <param name="filter">Фильтр в таблице</param>
    public TableAnalysisParameters(DataTable table, string filter)
    {
      if (table == null)
        throw new ArgumentNullException("table");

      new DataView(table, filter, null, DataViewRowState.OriginalRows).Dispose(); //Проверка корректности фильтра

      m_table = table;
      m_filter = string.IsNullOrWhiteSpace(filter) ? null : filter;
    }

    /// <summary>
    /// Таблица
    /// </summary>
    public DataTable Table
    {
      get { return m_table; }
    }

    /// <summary>
    /// Фильтр в таблице
    /// </summary>
    public string Filter
    {
      get { return m_filter; }
    }

    /// <summary>
    /// Проверка на то, что тип данных является числовым
    /// </summary>
    /// <param name="type">Тип, который надо проверить на то, является ли он числовым</param>
    /// <returns>True, если тип является числовым, иначе False</returns>
    /// <param name="checkNullable">Считать ли nullable-типы числовыми</param>
    protected static bool IsNumeric(Type type, bool checkNullable = false)
    {
      Debug.Assert(type != null);

      if (checkNullable && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        type = type.GetGenericArguments()[0];

      return (type.IsPrimitive && type != typeof(bool)) || type == typeof(decimal);
    }
  }

  /// <summary>
  /// Набор параметров для анализа данных в таблице с
  /// использованием методов, в которых есть предикторы и отклик
  /// </summary>
  public class PredictedResponseParameters : TableAnalysisParameters
  {
    private readonly FactorInfo m_predictors;
    private readonly string m_response;

    /// <summary>
    /// Инициализация набора параметров для анализа данных в таблице
    /// </summary>
    /// <param name="table">Таблица</param>
    /// <param name="filter">Фильтр в таблице</param>
    /// <param name="predictors">Набор имён колонок, которые будут предикторами</param>
    /// <param name="response">Имя колонки, которая будет откликом</param>
    public PredictedResponseParameters(DataTable table, string filter, FactorInfo predictors, string response)
      : base(table, filter)
    {
      if (predictors == null || predictors.Equals(FactorInfo.Empty))
        throw new ArgumentNullException("predictors");

      if (string.IsNullOrEmpty(response))
        throw new ArgumentNullException("response");

      foreach (var predictor in predictors)
      {
        if (!table.Columns.Contains(predictor))
          throw new ArgumentException(string.Format("Predictor column {0} not found in the table", predictor));
      }

      if (!table.Columns.Contains(response))
        throw new ArgumentException(string.Format("Response column {0} not found in the table", response));

      if (predictors.Contains(response))
        throw new ArgumentException("Response column intercects with predictor columns");

      m_predictors = predictors;
      m_response = response;
    }

    /// <summary>
    /// Набор имён колонок, которые будут предикторами
    /// </summary>
    public FactorInfo Predictors
    {
      get { return m_predictors; }
    }

    /// <summary>
    /// Имя колонки, которая будет откликом
    /// </summary>
    public string Response
    {
      get { return m_response; }
    }

    /// <summary>
    /// Получение фильтра, дополненного условием обязательности заполнения колонки отклика
    /// </summary>
    /// <returns>Дополненный фильтр</returns>
    public string GetFullFilter()
    {
      if (!this.Table.Columns[m_response].AllowDBNull)
        return this.Filter;

      string response_filter = string.Format("[{0}] IS NOT NULL", m_response);

      if (this.Filter == null)
        return response_filter;
      else if (this.Filter.IndexOf(response_filter, StringComparison.CurrentCultureIgnoreCase) >= 0)
        return this.Filter;

      return string.Format("{0} AND {1}", this.Filter, response_filter);
    }
  }

  /// <summary>
  /// Набор параметров для анализа данных в таблице с использованием методов,
  /// в которых есть предикторы и отклик, с указанным уровнем значимости
  /// </summary>
  public class PredictedWithProbabilityResponseParameters : PredictedResponseParameters
  {
    private readonly float m_probability;

    /// <summary>
    /// Инициализация набора параметров для анализа данных в таблице
    /// </summary>
    /// <param name="table">Таблица</param>
    /// <param name="filter">Фильтр в таблице</param>
    /// <param name="predictors">Набор имён колонок, которые будут предикторами</param>
    /// <param name="response">Имя колонки, которая будет откликом</param>
    /// <param name="probability">Уровень значимости для анализа</param>
    public PredictedWithProbabilityResponseParameters
    (
      DataTable table,
      string filter,
      FactorInfo predictors,
      string response,
      float probability
    ) : base(table, filter, predictors, response)
    {
      if (probability < 0 || probability > 1)
        throw new ArgumentOutOfRangeException("probability");

      m_probability = probability;
    }

    /// <summary>
    /// Уровень значимости для анализа
    /// </summary>
    public float Probability
    {
      get { return m_probability; }
    }
  }
}