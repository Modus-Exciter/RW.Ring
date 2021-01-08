using System;
using System.Collections;
using System.IO;
using System.ComponentModel;
using System.Text;
using System.Collections.Generic;
using Schicksal.Properties;
using Notung;
using Schicksal.Basic;

namespace Schicksal
{
  public class HtmlSaver : RunBase
  {
    private readonly string m_file_name;
    private readonly DescriptionStatisticsEntry[] m_descriptions;

    public HtmlSaver(string fileName, DescriptionStatisticsEntry[] descriptions)
    {
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentNullException("fileName");

      if (descriptions == null)
        throw new ArgumentNullException("description");

      m_file_name = fileName;
      m_descriptions = descriptions;
    }

    public override void Run()
    {
      int table_number = 1;

      using (var writer = new HtmlWriter(m_file_name, Encoding.UTF8, Resources.REPORT))
      {
        writer.WriteHeader(Resources.REPORT, 1);

        //TODO: таблица со списком проварьированных параметров

        using (writer.CreateParagraph())
          writer.WriteText(string.Format("{0} {1}. {2}", Resources.TABLE, table_number++, Resources.BASIC_METRICS));

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

        writer.WriteTable(m_descriptions, columnNames);
      }
    }
  }
}
