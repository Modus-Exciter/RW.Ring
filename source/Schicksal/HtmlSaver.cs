using System;
using System.Collections;
using System.IO;
using System.ComponentModel;
using System.Text;
using System.Collections.Generic;
using Schicksal.Properties;

namespace Schicksal
{
  public class HtmlSaver
  {
    public static void Save(string fileName, IList dataSource, Dictionary<string, string> columnNames)
    {
      using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
      {
        writer.WriteLine("<html>");
        writer.WriteLine("<head>");
        writer.WriteLine("<title>{0}</title>", Resources.REPORT);
        writer.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1251\">");
        writer.WriteLine("</head>");
        writer.WriteLine("<body>");
        writer.WriteLine("<table>");

        var desciptors = (dataSource is ITypedList) ? ((ITypedList)dataSource).GetItemProperties(new PropertyDescriptor[0])
          : TypeDescriptor.GetProperties(dataSource[0]);

        writer.WriteLine("<tr>");

        foreach (PropertyDescriptor pd in desciptors)
          writer.WriteLine("<td><b>{0}</b></td>", columnNames.ContainsKey(pd.Name) ? columnNames[pd.Name] : pd.DisplayName);

        writer.WriteLine("</tr>");

        foreach (var line in dataSource)
        {
          writer.WriteLine("<tr>");
          foreach (PropertyDescriptor pd in desciptors)
            writer.WriteLine("<td>{0}</td>", pd.GetValue(line));
          writer.WriteLine("</tr>");
        }

        writer.WriteLine("</table>");
        writer.WriteLine("</body>");
        writer.WriteLine("</html>");
      }
    }
  }
}
