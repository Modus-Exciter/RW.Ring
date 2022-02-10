using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using Notung;
using Schicksal.Properties;

namespace Schicksal.Regression
{
  public sealed class RegressionHtmlSaver : RunBase, IServiceProvider
  {
    private readonly string m_file_name;
    private readonly DataTable m_table;
    private readonly CorrelationMetrics[] m_metrics;
    private readonly double m_probability;
    private readonly string m_header;

    public RegressionHtmlSaver(string fileName, DataTable table, CorrelationMetrics[] metrics, double probability, string header)
    {
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentNullException("fileName");

      if (table == null)
        throw new ArgumentNullException("table");

      if (metrics == null)
        throw new ArgumentNullException("metrics");

      if (string.IsNullOrEmpty(header))
        header = Resources.REPORT;

      m_file_name = fileName;
      m_table = table;
      m_metrics = metrics;
      m_probability = probability;
      m_header = header.Trim();

      if (m_header.EndsWith(","))
        m_header = m_header.Substring(0, m_header.Length - 1);
    }

    public string Result { get; set; }

    public string Filter { get; set; }

    public string[] Factors { get; set; }

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

    public override void Run()
    {
      this.ReportProgress(Resources.BASIC_METRICS);

      using (var writer = new HtmlWriter(m_file_name, Encoding.UTF8, Resources.REPORT))
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
            writer.WriteText(string.Format("{0}: {1}.", Resources.AFFECTED_RESULT, this.Result));
        }

        using (writer.CreateParagraph())
        {
          writer.WriteText(string.Format("<em>{0} {1}.</em> {2}",
            Resources.TABLE, 1, Resources.REGRESSION_RESULTS));
        }

        writer.WriteTable(m_metrics, new Dictionary<string, string>
        {
          { "Factor", Resources.FACTOR }
        });

        writer.WriteHeader(Resources.REGRESSION_DETAILS, 2);

        foreach (var metric in m_metrics)
        {
          writer.WriteHeader(metric.Factor, 3);

          using (writer.CreateParagraph())
            writer.WriteText(string.Format("<em>{0}</em>:", Resources.REGRESSION_DEPENDENCY));

          using (writer.CreateParagraph())
            writer.WriteText(new CorrelationResults(m_table, metric.Factor, this.Result, typeof(LinearDependency)).Run().ToString());
        }
      }
    }
  }
}
