using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using Notung;
using Notung.Services;
using Schicksal.Anova;
using Schicksal.Basic;
using Schicksal.Helm.Dialogs;
using Schicksal.Helm.Properties;

namespace Schicksal.Helm.Analyze
{
  public class AnovaAnalyze : IAnalyze
  {
    private static readonly Dictionary<string, string> _empty_dic = new Dictionary<string, string>();

    public Type OptionsType
    {
      get { return typeof(AnovaOptionsDialog); }
    }

    public override string ToString()
    {
      return Resources.ANOVA;
    }

    public void BindTheResultForm(IRunBase processor, object table_form, StatisticsParameters data)
    {
      var tf = (TableForm)table_form;
      var currentProcessor = (AnovaCalculator)processor;
      var results_form = new AnovaResultsForm(currentProcessor);

      results_form.Text = string.Format("{0}: {1}, p={2}", Resources.ANOVA, tf.Text, data.Probability);

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
      var dic = ReadSettings(data);
      string conjugation;
      string nomalizer_name;
      string individual;

      dic.TryGetValue("Conjugate", out conjugation);
      dic.TryGetValue("Normalization", out nomalizer_name);
      dic.TryGetValue("Individual", out individual);

      INormalizer normalizer = DummyNormalizer.Instance;

      if (nomalizer_name == "NonParametric")
        normalizer = new RankNormalizer();
      else if (nomalizer_name == "BoxCox")
        normalizer = new BoxCoxNormalizer();

      return new AnovaCalculator(
        new AnovaParameters
        (
          table, data.Filter, 
          new FactorInfo(data.Predictors),
          data.Result, data.Probability,
          normalizer,
          conjugation, true.ToString().Equals(individual))
        );
    }

    private static Dictionary<string, string> ReadSettings(StatisticsParameters data)
    {
      if (!string.IsNullOrEmpty(data.OptionsXML))
      {
        var doc = new XmlDocument();
        doc.LoadXml(data.OptionsXML);

        return doc.DocumentElement.Attributes.Cast<XmlAttribute>()
          .Where(att => !string.IsNullOrEmpty(att.Value))
          .ToDictionary(att => att.Name, att => att.Value);
      }
      
      return _empty_dic;
    }

    public Dictionary<string, string[]> GetSettings()
    {
      return AppManager.Configurator.GetSection<Program.Preferences>().AnovaSettings;
    }
  }
}
