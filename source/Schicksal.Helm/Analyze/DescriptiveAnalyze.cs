using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Notung;
using Notung.Services;
using Schicksal.Basic;
using Schicksal.Helm.Dialogs;
using Schicksal.Helm.Properties;

namespace Schicksal.Helm.Analyze
{
  public class DescriptiveAnalyze : IAnalyze
  {
    public Type OptionsType
    {
      get { return null; }
    }

    public override string ToString()
    {
      return Resources.BASIC_STATISTICS;
    }

    public Dictionary<string, string[]> GetSettings()
    {
      return AppManager.Configurator.GetSection<Program.Preferences>().BaseStatSettings;
    }

    public IRunBase GetProcessor(DataTable table, StatisticsParameters data)
    {
      return new DescriptionStatisticsCalculator(
        new DescriptionStatisticsParameters(
          table,
          data.Filter,
          new FactorInfo(data.Predictors),
          data.Result,
          data.Probability));
    }

    public LaunchParameters GetLaunchParameters()
    {
      return new LaunchParameters
      {
        Caption = Resources.BASIC_STATISTICS,
        Bitmap = Resources.column_chart
      };
    }

    public void BindTheResultForm(IRunBase processor, object table_form, StatisticsParameters data)
    {
      var results_form = new BasicStatisticsForm();
      var currentProcessor = (DescriptionStatisticsCalculator)processor;
      var tf = (TableForm)table_form;
      results_form.Text = string.Format("{0}: {1}, p={2}; {3}",
        Resources.BASIC_STATISTICS, tf.Text, data.Probability, data.Filter);
      results_form.DataSorce = currentProcessor.Result;
      results_form.Factors = currentProcessor.Parameters.Predictors.ToArray();
      results_form.ResultColumn = data.Result;
      results_form.Show(tf.MdiParent);
    }
  }
}
