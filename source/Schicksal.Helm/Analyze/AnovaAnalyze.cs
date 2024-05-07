using Notung;
using Notung.Services;
using Schicksal.Anova;
using Schicksal.Helm.Dialogs;
using Schicksal.Helm.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Schicksal.Helm.Analyze
{
  public class AnovaAnalyze : IAnalyze
  {
    public Type OptionsType
    {
      get { return null /*typeof(AnovaOptionsDialog)*/; }
    }

    public override string ToString()
    {
      return Resources.ANOVA;
    }

    public void BindTheResultForm(IRunBase processor, object table_form, StatisticsParameters data)
    {
      var results_form = new AnovaResultsForm();
      var currentProcessor = (FisherTableProcessor)processor;
      var tf = (TableForm)table_form;
      var table = tf.DataSource;
      results_form.Text = string.Format("{0}: {1}, p={2}",
              Resources.ANOVA, tf.Text, data.Probability);
      results_form.DataSource = currentProcessor.Result;
      results_form.SourceTable = table;
      results_form.ResultColumn = data.Result;
      results_form.Filter = data.Filter;
      results_form.Probability = data.Probability;
      results_form.Factors = data.Predictors.ToArray();
      results_form.Show(tf.MdiParent);
    }

    public LaunchParameters GetLaunchParameters()
    {
      return new LaunchParameters
      {
        Caption = Resources.ANOVA,
        Bitmap = Resources.column_chart
      };
    }

    public IRunBase GetProcessor(DataTable table, StatisticsParameters data)
    {
      var processor = new FisherTableProcessor(table, data.Predictors.ToArray(),
             data.Result, data.Probability);

      if (!string.IsNullOrEmpty(data.Filter))
        processor.Filter = data.Filter;

      processor.RunInParrallel = true;

      return processor;
    }

    public Dictionary<string, string[]> GetSettings()
    {
      return AppManager.Configurator.GetSection<Program.Preferences>().AnovaSettings;
    }
  }
}
