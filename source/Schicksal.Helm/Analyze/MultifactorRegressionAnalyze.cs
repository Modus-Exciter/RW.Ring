using Notung;
using Notung.Services;
using Schicksal.Helm.Dialogs;
using Schicksal.Helm.Properties;
using Schicksal.Regression;
using System;
using System.Collections.Generic;
using System.Data;

namespace Schicksal.Helm.Analyze
{
  public class MultifactorRegressionAnalyze : IAnalyze
  {
    public Type OptionsType
    {
      get { return null; }
    }

    public override string ToString()
    {
      return Resources.MULTIFACTOR_REGRESSION;
    }

    /// <summary>
    /// Привязывает результаты анализа к форме отображения.
    /// </summary>
    /// <param name="processor">Процессор анализа.</param>
    /// <param name="tableForm">Форма с исходной таблицей.</param>
    /// <param name="data">Параметры анализа.</param>
    public void BindTheResultForm(IRunBase processor, object table_form, StatisticsParameters data)
    {
      var currentProcessor = (MultifactorRegressionProcessor)processor;
      var form = (TableForm)table_form;
      var resultsForm = new MultifactorRegressionResultsForm
      {
        Text = $"{Resources.MULTIFACTOR_REGRESSION}: {form.Text}, p={data.Probability}; {data.Result}",
      };

      resultsForm.SetLinearRegressionResults(currentProcessor.Results, data.Probability);
      resultsForm.SetParabolicRegressionResults(currentProcessor.ParabolicResults, data.Probability);

      resultsForm.Show(form.MdiParent);
    }

    /// <summary>
    /// Возвращает параметры запуска анализа.
    /// </summary>
    public LaunchParameters GetLaunchParameters()
    {
      return new LaunchParameters
      {
        Caption = Resources.MULTIFACTOR_REGRESSION,
        Bitmap = Resources.column_chart
      };
    }


    /// <summary>
    /// Создает процессор для выполнения анализа.
    /// </summary>
    /// <param name="table">Исходная таблица данных.</param>
    /// <param name="data">Параметры анализа.</param>
    public IRunBase GetProcessor(DataTable table, StatisticsParameters data)
    {
      return new MultifactorRegressionProcessor(
        new CorrelationParameters(
          table,
          data.Filter,
          new Basic.FactorInfo(data.Predictors),
          data.Result,
          data.Probability
        ));
    }

    public Dictionary<string, string[]> GetSettings()
    {
      return AppManager.Configurator.GetSection<Program.Preferences>().RegressionSettings;
    }
  }
}