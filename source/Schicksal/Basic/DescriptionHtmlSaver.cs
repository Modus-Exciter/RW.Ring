using System;
using System.Collections;
using System.Linq;
using System.IO;
using System.ComponentModel;
using System.Text;
using System.Collections.Generic;
using Schicksal.Properties;
using Notung;

namespace Schicksal.Basic
{
  public class DescriptionHtmlSaver : RunBase
  {
    private readonly string m_file_name;
    private readonly DescriptionStatisticsEntry[] m_descriptions;

    public DescriptionHtmlSaver(string fileName, DescriptionStatisticsEntry[] descriptions)
    {
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentNullException("fileName");

      if (descriptions == null)
        throw new ArgumentNullException("description");

      m_file_name = fileName;
      m_descriptions = descriptions;
    }

    public string Caption { get; set; }

    public string[] Factors { get; set; }

    public string Result { get; set; }

    public override void Run()
    {
      int table_number = 1;

      if (this.Caption != null)
        this.Caption = this.Caption.Replace("[", "").Replace("]", "");

      using (var writer = new HtmlWriter(m_file_name, Encoding.UTF8, Resources.REPORT))
      {
        writer.WriteHeader(this.Caption ?? Resources.REPORT, 1);

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
          writer.WriteText(string.Format("<em>{0} {1}.</em> {2}", Resources.TABLE, table_number++, Resources.BASIC_METRICS));

        writer.WriteTable(m_descriptions, GetColumnNames());
        writer.WriteHeader(string.Format("{0}:", Resources.SUMMARY), 3);
        writer.WriteGlossary(this.CalculateSummary());

        using (writer.CreateParagraph())
          writer.WriteSpace();
      }
    }

    public override string ToString()
    {
      return Resources.EXPORT;
    }

    private Dictionary<string, string> CalculateSummary()
    {
      Dictionary<string, string> summary = new Dictionary<string, string>();

      DescriptionStatisticsEntry min = m_descriptions[0];
      DescriptionStatisticsEntry max = m_descriptions[0];

      double total = 0;
      int count = 0;

      DescriptionStatisticsEntry median = m_descriptions.OrderBy(d => d.Median)
        .Skip(m_descriptions.Length / 2).First();

      for (int i = 1; i < m_descriptions.Length; i++)
      {
        if (m_descriptions[i].Min < min.Min)
          min = m_descriptions[i];

        if (m_descriptions[i].Max > max.Max)
          max = m_descriptions[i];

        total += m_descriptions[i].Mean * m_descriptions[i].Count;
        count += m_descriptions[i].Count;
      }

      summary.Add(SchicksalResources.MAX, string.Format("{0} ({1})", max.Max, max.Description));
      summary.Add(SchicksalResources.MIN, string.Format("{0} ({1})", min.Min, min.Description));
      summary.Add(SchicksalResources.MEAN, (total / count).ToString("0.0000"));
      summary.Add(SchicksalResources.MEDIAN, string.Format("{0} ({1})", median.Median, median.Description));
      summary.Add(Resources.TOTAL_COUNT, count.ToString());
      return summary;
    }

    private static Dictionary<string, string> GetColumnNames()
    {
      var columnNames = new Dictionary<string, string>();

      foreach (var pi in typeof(DescriptionStatisticsEntry).GetProperties())
      {
        if (pi.Name == "StdError")
          columnNames[pi.Name] = SchicksalResources.STD_ERROR;
        else if (pi.Name == "ConfidenceInterval")
          columnNames[pi.Name] = SchicksalResources.INTERVAL;
        else
          columnNames[pi.Name] = SchicksalResources.ResourceManager.GetString(pi.Name.ToUpper());
      }

      return columnNames;
    }
  }
}
