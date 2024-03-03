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

namespace Schicksal.Helm.Analyze
{
  public class ANOVA : IAnalyze
  {
    public override string ToString()
    {
      return Resources.ANOVA;
    }
    public void BindTheResultForm(RunBase processor, object table_form, AnovaDialogData data)
    {
      var results_form = new AnovaResultsForm();
      var currentProcessor = (FisherTableProcessor)processor;
      var tf = (TableForm)table_form;
      var table = tf.DataSource;
      results_form.Text = string.Format("{0}: {1}, p={2}",
              Resources.ANOVA, table, data.Probability);
      results_form.DataSource = currentProcessor.Result;
      results_form.SourceTable = table;
      results_form.ResultColumn = data.Result;
      results_form.Filter = data.Filter;
      results_form.Probability = data.Probability;
      results_form.Factors = data.Predictors.ToArray();
      results_form.Show();
    }

    public LaunchParameters GetLaunchParameters()
    {
      return new LaunchParameters
      {
        Caption = Resources.ANOVA,
        Bitmap = Resources.column_chart
      };
    }

    public RunBase GetProcessor(DataTable table, AnovaDialogData data)
    {
      return new FisherTableProcessor(table, data.Predictors.ToArray(),
             data.Result, data.Probability);
    }

    public Dictionary<string, string[]> GetSettings()
    {
      return AppManager.Configurator.GetSection<Program.Preferences>().AnovaSettings;
    }
  }
}
