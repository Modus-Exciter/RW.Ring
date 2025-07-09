using Notung;
using Schicksal.Basic;
using System;
using System.Collections.Generic;

namespace Schicksal.Regression
{
  /// <summary>
  /// Процессор для выполнения многофакторного регрессионного анализа.
  /// </summary>
  public class MultifactorRegressionProcessor : RunBase
  {
    private readonly CorrelationParameters m_parameters;

    /// <summary>
    /// Результаты многофакторного линейного регрессионного анализа.
    /// </summary>
    public MultifactorRegressionResult Results { get; private set; }

    /// <summary>
    /// Результаты многофакторного параболического регрессионного анализа.
    /// </summary>
    public MultifactorRegressionResult ParabolicResults { get; private set; }

    /// <summary>
    /// Инициализация процессора многофакторного регрессионного анализа.
    /// </summary>
    /// <param name="parameters">Параметры анализа.</param>
    public MultifactorRegressionProcessor(CorrelationParameters parameters)
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
    /// Запуск задачи на выполнение.
    /// </summary>
    public override void Run()
    {
      try
      {
        MultifactorRegression regression = new MultifactorRegression();
        var factorInfo = new FactorInfo(m_parameters.Predictors);
        var ySample = new DataColumnSample(m_parameters.Table.Columns[m_parameters.Response], m_parameters.Filter);
        var xSamples = new List<IPlainSample>();

        foreach (string predictorName in m_parameters.Predictors)
        {
          xSamples.Add(new DataColumnSample(m_parameters.Table.Columns[predictorName], m_parameters.Filter));
        }

        this.ReportProgress(25);

        this.Results = regression.Perform(factorInfo, xSamples, ySample, m_parameters.Probability);
        this.ReportProgress(50);

        if (m_parameters.Predictors.Count == 2)
        {
          this.ParabolicResults = regression.PerformParabolic(factorInfo, xSamples, ySample, m_parameters.Probability);
          this.ReportProgress(75);
        }

        this.ReportProgress(100);
      }
      catch (Exception ex)
      {
        throw new Exception($"Ошибка регрессионного анализа: {ex.Message}");
      }
    }
  }
}