using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Notung;
using Schicksal.Properties;

namespace Schicksal.Anova
{
  public class AnovaHtmlSaver : RunBase, IServiceProvider
  {
    private readonly string m_file;
    private readonly DataTable m_table;
    private readonly FisherTestResult[] m_results;
    private readonly double m_probability;
    private readonly string m_header;

    public AnovaHtmlSaver(string file, DataTable table, FisherTestResult[] results, double probability, string header)
    {
      if (string.IsNullOrEmpty(file))
        throw new ArgumentNullException("file");

      if (table == null)
        throw new ArgumentNullException("table");

      if (results == null)
        throw new ArgumentNullException("results");

      if (string.IsNullOrEmpty(header))
        header = Resources.REPORT;

      m_file = file;
      m_table = table;
      m_results = results;
      m_probability = probability;
      m_header = header;
    }

    public string[] Factors { get; set; }

    public string Result { get; set; }

    public string Filter { get; set; }

    public override void Run()
    {
      this.ReportProgress(Resources.BASIC_METRICS);

      using (var writer = new HtmlWriter(m_file, Encoding.UTF8, Resources.REPORT))
      {
        this.WriteStartTable(writer);

        this.WriteCommonTestDescription(writer);
        writer.WriteHeader(Resources.SIGNIFICAT_FACTORS, 1);

        var significant = m_results.Where(r => r.P <= m_probability).ToList();
        var descriptions = new Dictionary<string, string>
        {
          { "Count", SchicksalResources.COUNT },
          { "Mean", SchicksalResources.MEAN },
          { "Std error", SchicksalResources.STD_ERROR },
          { "Interval", SchicksalResources.INTERVAL },
          { "Factor1", string.Format("{0} 1", Resources.FACTOR) },
          { "Mean1", string.Format("{0} 1", SchicksalResources.MEAN) },
          { "Factor2", string.Format("{0} 2", Resources.FACTOR) },
          { "Mean2", string.Format("{0} 1", SchicksalResources.MEAN) },
          { "ActualDifference", Resources.ACTUAL_DIFFERENCE },
          { "MinimalDifference", Resources.CRITICAL_DIFFERENCE },
          { "Probability", "P" }
        };

        for (int i = 0; i < significant.Count; i++)
        {
          this.ReportProgress(i * 100 / significant.Count, significant[i].Factor);

          this.WriteFactorResults(writer, significant[i], descriptions, i);
        }

        using (writer.CreateParagraph())
          writer.WriteSpace();
      }
    }

    public override string ToString()
    {
      return Resources.EXPORT;
    }

    public override object GetService(Type serviceType)
    {
      if (serviceType == typeof(Image))
        return Resources.Export_Picture;

      return base.GetService(serviceType);
    }

    private void WriteFactorResults(HtmlWriter writer, FisherTestResult factor, Dictionary<string, string> descriptions, int number)
    {
      writer.WriteHeader(factor.Factor, 2);

      using (writer.CreateParagraph())
        writer.WriteText(string.Format("<em>{0} {1}.</em> {2}", Resources.TABLE, number + 2, Resources.BASIC_METRICS));

      var processor = new VariantsComparator(m_table, factor.Factor, factor.IgnoredFactor, this.Result, this.Filter);
      var comparator = new MultiVariantsComparator(processor, m_probability);
      comparator.Run();
      comparator.Source.Columns.Remove("Factor");

      writer.WriteTable(comparator.Source.DefaultView, descriptions, new HashSet<string> { "Ignorable" });

      writer.WriteHeader(string.Format("{0}:", Resources.SUMMARY), 3);

      using (writer.CreateParagraph())
      {
        writer.WriteText(Resources.SIGNIFICANT_DIFFERENCES);

        foreach (var di in comparator.Results.Where(r => r.Probability <= m_probability))
          writer.WriteText(string.Format("«{0}» {1} «{2}»,", di.Factor1, Resources.AND, di.Factor2));//«»

        writer.WriteText(Resources.OTHER_UNSIGNIFICAT);

        var max_dif = comparator.Results[0];
        var sig_dif = comparator.Results[0];
        var max_val = new Tuple<string, double>(comparator.Results[0].Factor1, comparator.Results[0].Mean1);  //comparator.Results[0];
        var min_val = new Tuple<string, double>(comparator.Results[0].Factor1, comparator.Results[0].Mean1); //comparator.Results[0];

        for (int i = 0; i < comparator.Results.Length; i++)
        {
          if (max_dif.ActualDifference < comparator.Results[i].ActualDifference
            && max_dif.Probability < m_probability)
            max_dif = comparator.Results[i];

          if (sig_dif.Probability > comparator.Results[i].Probability)
            sig_dif = comparator.Results[i];

          if (max_val.Item2 < comparator.Results[i].Mean1)
            max_val = new Tuple<string, double>(comparator.Results[i].Factor1, comparator.Results[i].Mean1);// comparator.Results[i];

          if (max_val.Item2 < comparator.Results[i].Mean2)
            max_val = new Tuple<string, double>(comparator.Results[i].Factor2, comparator.Results[i].Mean2);// comparator.Results[i];

          if (min_val.Item2 > comparator.Results[i].Mean1)
            min_val = new Tuple<string, double>(comparator.Results[i].Factor1, comparator.Results[i].Mean1);// comparator.Results[i];

          if (min_val.Item2 > comparator.Results[i].Mean2)
            min_val = new Tuple<string, double>(comparator.Results[i].Factor2, comparator.Results[i].Mean2);// comparator.Results[i];
        }

        writer.WriteText(string.Format("{0} «{1}» {2} «{3}»; {4} {5}.",
          Resources.MAX_DIFFERENCE, max_dif.Factor1, Resources.AND, max_dif.Factor2,
          Resources.DIFFERENCE_VALUE, HtmlWriter.FormatValue(max_dif.ActualDifference)));
        writer.WriteText(string.Format("{0} «{1}» {2} «{3}»; {4} {5}.",
          Resources.MOST_SIGNIFICAT_DIFFERENCE, sig_dif.Factor1, Resources.AND, sig_dif.Factor2,
          Resources.DIFFERENCE_VALUE, HtmlWriter.FormatValue(sig_dif.ActualDifference)));

        writer.WriteText(string.Format(Resources.MAX_VALUE, max_val.Item1, HtmlWriter.FormatValue(max_val.Item2)));
        writer.WriteText(string.Format(Resources.MIN_VALUE, min_val.Item1, HtmlWriter.FormatValue(min_val.Item2)));
      }
    }

    private void WriteStartTable(HtmlWriter writer)
    {
      writer.WriteHeader(m_header, 1);

      if (this.Factors != null && this.Factors.Length > 0)
      {
        using (writer.CreateParagraph())
          writer.WriteText(string.Format("{0}:", Resources.VARIED_FACTORS));

        writer.WriteCollection(this.Factors);
      }

      if (!string.IsNullOrEmpty(this.Result))
      {
        using (writer.CreateParagraph())
          writer.WriteText(string.Format("{0}: {1}", Resources.AFFECTED_RESULT, this.Result));
      }

      using (writer.CreateParagraph())
        writer.WriteText(string.Format("<em>{0} {1}.</em> {2}", Resources.TABLE, 1, Resources.F_CRITERIA));

      writer.WriteTable(m_results, new Dictionary<string, string>
        {
          { "Factor", Resources.FACTOR },
          { "Kdf", "K - 1" },
          { "Ndf", "N - K" },
        },
        new HashSet<string> { "IgnoredFactor" });
    }

    private void WriteCommonTestDescription(HtmlWriter writer)
    {
      using (writer.CreateParagraph())
      {
        if (m_results.Any(r => r.P > m_probability))
        {
          writer.WriteText(string.Format(Resources.UNSIGNIFICAT, m_probability * 100) + ':');
          writer.WriteText(string.Join(", ",
            m_results.Where(r => r.P > m_probability).Select(r => r.Factor)) + '.');
        }
        if (m_results.Any(r => r.P <= m_probability))
        {
          writer.WriteText(string.Format(Resources.SIGNIFICAT,
            string.Join(", ", m_results.Where(r => r.P <= m_probability).Select(r => r.Factor)),
            m_probability * 100) + '.');
        }

        if (m_results.Length > 0)
        {
          var min = m_results[0];
          var max = m_results[0];

          for (int i = 1; i < m_results.Length; i++)
          {
            if (min.P < m_results[i].P)
              min = m_results[i];

            if (max.P > m_results[i].P)
              max = m_results[i];
          }

          writer.WriteText(string.Format(Resources.MAX_INFLUENCE,
            max.Factor, HtmlWriter.FormatValue(max.P * 100)));

          writer.WriteText(string.Format(Resources.MIN_INFLUENCE,
            min.Factor, HtmlWriter.FormatValue(min.P * 100)));
        }
      }
    }
  }
}
