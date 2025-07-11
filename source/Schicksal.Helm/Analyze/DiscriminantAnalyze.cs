using Notung;
using Notung.Services;
using Schicksal.Helm.Dialogs;
using Schicksal.Helm.Properties;
using Schicksal.Regression;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Schicksal.Helm.Analyze
{
  public class DiscriminantAnalyze : IAnalyze
  {
    public Type OptionsType
    {
      get { return null; }
    }

    public override string ToString()
    {
      return Resources.DISCRIMINANT_ANALYSIS;
    }
    
    public void BindTheResultForm(IRunBase processor, object table_form, StatisticsParameters data)
    {
      var currentProcessor = (CorrelationTestProcessor)processor;
      var tf = (TableForm)table_form;
      var table = tf.DataSource;
      var results_form = new RegressionResultsForm();
      results_form.Text = string.Format("{0}: {1}, p={2}; {3}",
        Resources.REGRESSION_ANALYSIS, tf.Text, data.Probability, data.Result);
      results_form.DataSource = currentProcessor.Results;
      results_form.Factors = data.Predictors.ToArray();
      results_form.ResultColumn = data.Result;
      results_form.Filter = data.Filter;
      results_form.Probability = data.Probability;
      results_form.SourceTable = table;
      results_form.Show(tf.MdiParent);
    }

    public LaunchParameters GetLaunchParameters()
    {
      return new LaunchParameters
      {
        Caption = Resources.DISCRIMINANT_ANALYSIS,
        Bitmap = Resources.column_chart
      };
    }
    
    public IRunBase GetProcessor(DataTable table, StatisticsParameters data)
    {
      return new CorrelationTestProcessor(
        new CorrelationParameters
        (
          table,
          data.Filter,
          new Basic.FactorInfo(data.Predictors),
          data.Result,
          data.Probability
        ));
    }
    
    public Dictionary<string, string[]> GetSettings()
    {
      return AppManager.Configurator.GetSection<Program.Preferences>().DiscriminantSettings;
    }
  }
}
