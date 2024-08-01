using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Notung;
using Schicksal.Basic;

namespace Schicksal.Regression
{
  /// <summary>
  /// Задача регрессионного анализа таблицы
  /// </summary>
  public class CorrelationTestProcessor : RunBase
  {
    private readonly CorrelationParameters m_parameters;

    /// <summary>
    /// Инициализация задачи регрессионного анализа данных в таблице
    /// </summary>
    /// <param name="parameters">Набор параметров - таблица с опциями</param>
    public CorrelationTestProcessor(CorrelationParameters parameters)
    {
      if (parameters == null)
        throw new ArgumentNullException("parameters");

      m_parameters = parameters;
    }

    /// <summary>
    /// Набор параметров - таблица с опциями
    /// </summary>
    public CorrelationParameters Parameters
    {
      get { return m_parameters; }
    }

    /// <summary>
    /// Результаты анализа прямолинейной и криволинейной корреляции
    /// </summary>
    public CorrelationMetrics[] Results { get; private set; }

    /// <summary>
    /// Запуск задачи на выполнение
    /// </summary>
    public override void Run()
    {
      this.Results = new CorrelationMetrics[m_parameters.Predictors.Count];

      int i = 0;

      foreach (var p in m_parameters.Predictors)
      {
        this.ReportProgress(p);

        var x_column = new DataColumnSample(m_parameters.Table.Columns[p], this.GetFilter(p));
        var y_column = new DataColumnSample(m_parameters.Table.Columns[m_parameters.Response], this.GetFilter(p));

        this.Results[i] = CorrelationTest.CalculateMetrics
        (
          p,
          m_parameters.Response,
          x_column,
          y_column,
          m_parameters.Probability
        );

        if (m_parameters.Predictors.Count > 1)
          this.ReportProgress((i + 1) * 100 / m_parameters.Predictors.Count);

        i++;
      }
    }

    private string GetFilter(string predictor)
    {
      List<string> expressions = GetFilterExpressions
      (
        m_parameters.Table, 
        new string[] { predictor }, 
        m_parameters.Response, 
        m_parameters.Filter
      );

      if (expressions.Count > 0)
        return string.Join(" and ", expressions);
      else
        return null;
    }

    private static List<string> GetFilterExpressions(DataTable table, string[] predictors, string response, string filter)
    {
      var expressions = new List<string>(predictors.Length + 1);

      foreach (var f in predictors)
      {
        if (table.Columns[f].AllowDBNull)
          expressions.Add(string.Format("[{0}] is not null", f));
      }

      if (table.Columns[response].AllowDBNull)
        expressions.Add(string.Format("[{0}] is not null", response));

      expressions.RemoveAll((filter ?? string.Empty).Contains);

      if (!string.IsNullOrEmpty(filter))
        expressions.Add(filter);

      return expressions;
    }
  }
}