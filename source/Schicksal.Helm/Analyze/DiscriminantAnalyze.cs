using Notung;
using Notung.Services;
using Schicksal.Helm.Dialogs;
using Schicksal.Helm.Properties;
using Schicksal.Regression;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Schicksal.Discriminant;

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
      var treeProcessor = (Processor)processor;
      var tf = (TableForm)table_form;
      var table = tf.DataSource;

      var resultsForm = new RegressionResultsForm
      {
        Text = string.Format("{0}: {1}", Resources.DISCRIMINANT_ANALYSIS, tf.Text),
        DataSource = treeProcessor.Results,
        Factors = data.Predictors.ToArray(),
        ResultColumn = data.Result,
        Filter = data.Filter,
        Probability = data.Probability,
        SourceTable = table
      };

      resultsForm.Show(tf.MdiParent);
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
      return new Processor(
        new Parameterscs(
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
