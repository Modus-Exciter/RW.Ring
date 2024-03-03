using Notung;
using Notung.Services;
using Schicksal.Basic;
using Schicksal.Helm.Dialogs;
using Schicksal.Helm.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Schicksal.Helm.IAnalyze
{
  public class BaseStatistics : IAnalyze
  {
    public Dictionary<string, string[]> GetSettings()
    {
      return AppManager.Configurator.GetSection<Program.Preferences>().BaseStatSettings;
    }

    public RunBase GetProcessor(DataTable table, AnovaDialogData data)
    {
      return new DescriptionStatisticsCalculator(table, data.Predictors.ToArray(),
             data.Result, data.Filter, data.Probability);
    }

    public LaunchParameters GetLaunchParameters()
    {
      return new LaunchParameters
      {
        Caption = Resources.BASIC_STATISTICS,
        Bitmap = Resources.column_chart
      };
    }

    public void BindTheResultForm(RunBase processor, object table_form, AnovaDialogData data)
    {
      var results_form = new BasicStatisticsForm();
      var currentProcessor = (DescriptionStatisticsCalculator)processor;
      results_form.Text = string.Format("{0}: {1}, p={2}; {3}",
          Resources.BASIC_STATISTICS, table_form.GetType().GetProperty("Text").GetValue(table_form, null),
          data.Probability, data.Filter);
      results_form.DataSorce = currentProcessor.Result;
      results_form.Factors = currentProcessor.Factors;
      results_form.ResultColumn = data.Result;
      results_form.Show();
    }
  }
}
