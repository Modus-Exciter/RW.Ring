using Notung;
using Notung.Services;
using Schicksal.Anova;
using Schicksal.Basic;
using Schicksal.Helm.Dialogs;
using Schicksal.Helm.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Schicksal.Regression;

namespace Schicksal.Helm.IAnalyze
{
  public class Regression : IAnalyze
  {
    public void BindTheResultForm(RunBase processor, object table_form, AnovaDialogData data)
    {
      var results_form = new AncovaResultsForm();
      var currentProcessor = (CorrelationTestProcessor)processor;
      var tf = (TableForm)table_form;
      var table = tf.DataSource;
      results_form.Text = string.Format("{0}: {1}, p={2}; {3}",
              Resources.ANCOVA, table_form.GetType().GetProperty("Text").GetValue(table_form, null)); //??????????????
      results_form.DataSource = currentProcessor.Results;
      results_form.Factors = data.Predictors.ToArray();
      results_form.ResultColumn = data.Result;
      results_form.Filter = data.Filter;
      results_form.Probability = data.Probability;
      results_form.SourceTable = table;
      results_form.Show();
    }

    public LaunchParameters GetLaunchParameters()
    {
      return new LaunchParameters
      {
        Caption = Resources.ANCOVA,
        Bitmap = Resources.column_chart
      };
    }

    public RunBase GetProcessor(DataTable table, AnovaDialogData data)
    {
      return new CorrelationTestProcessor(table,
            data.Predictors.ToArray(), data.Result, data.Filter, data.Probability);
    }

    public Dictionary<string, string[]> GetSettings()
    {
      return AppManager.Configurator.GetSection<Program.Preferences>().AncovaSettings;
    }
  }
}
