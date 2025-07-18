using Notung;
using Notung.Services;
using Schicksal.Helm.Dialogs;
using Schicksal.Helm.Properties;
using Schicksal.Discriminant;
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
      get { return typeof(DiscriminantOptionsDialog); }
    }

    public override string ToString()
    {
      return Resources.DISCRIMINANT_ANALYSIS;
    }
    public void BindTheResultForm(IRunBase processor, object table_form, StatisticsParameters data)
    {
      var currentProcessor = (DiscriminantProcessor)processor;
      var tf = (TableForm)table_form;
      var table = tf.DataSource;

      var results_form = new DiscriminantResultsForm();
      results_form.Text = string.Format("{0}: {1}, p={2}; {3}",
          Resources.DISCRIMINANT_ANALYSIS, tf.Text, data.Probability, data.Result);

      results_form.DataSource = currentProcessor.Result;
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
    
    private DiscriminantParameters.SplitCriterion GetCriterionFromXml(string xml)
    {
      if (string.IsNullOrEmpty(xml))
        return DiscriminantParameters.SplitCriterion.Entropy;

      var doc = new System.Xml.XmlDocument();
      doc.LoadXml(xml);

      var criterionNode = doc.DocumentElement?.Attributes["Criterion"];
      if (criterionNode == null)
        return DiscriminantParameters.SplitCriterion.Entropy;

      switch (criterionNode.Value)
      {
        case "Gini":
          return DiscriminantParameters.SplitCriterion.Gini;
        default:
          return DiscriminantParameters.SplitCriterion.Entropy;
      }
    }
    public IRunBase GetProcessor(DataTable table, StatisticsParameters data)
    {
      var criterion = GetCriterionFromXml(data.OptionsXML);
      return new DiscriminantProcessor(
          new DiscriminantParameters(
              table,
              data.Filter,
              new Basic.FactorInfo(data.Predictors),
              data.Result,
              data.Probability,
              criterion));
    }

    public Dictionary<string, string[]> GetSettings()
    {
      return AppManager.Configurator.GetSection<Program.Preferences>().DiscriminantSettings;
    }
  }
}
