using Notung;
using Notung.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Schicksal
{
  public class HtmlWriter : IDisposable
  {
    private readonly TextWriter m_writer;
    private bool m_closed;

    private static readonly Dictionary<string, string> _empty_columns = new Dictionary<string, string>();

    public HtmlWriter(TextWriter writer, string caption)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");

      m_writer = writer;
      WriteStartDocument(caption);
    }

    public HtmlWriter(string fileName, Encoding encoding, string caption) 
      : this(new StreamWriter(fileName, false, encoding), caption) { }

    private void WriteStartDocument(string caption)
    {
      string encoding;

      if (m_writer.Encoding == Encoding.UTF8)
        encoding = "utf-8";
      else
        encoding = string.Format("windows-{0}", m_writer.Encoding.WindowsCodePage);

      m_writer.WriteLine("<html>");
      m_writer.WriteLine("<head>");
      m_writer.WriteLine("<title>{0}</title>", caption);
      m_writer.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset={0}\">", encoding);
      m_writer.WriteLine("</head>");
      m_writer.WriteLine("<body>");
    }

    private void WriteEndDocument()
    {
      m_writer.WriteLine("</body>");
      m_writer.WriteLine("</html>");
    }

    public void WriteHeader(string caption, int level = 2)
    {
      m_writer.WriteLine("<h{0}>{1}</h{0}>", level, caption);
    }

    public void WriteTable(IList dataSource, Dictionary<string, string> columnNames = null)
    {
      if (dataSource == null)
        throw new ArgumentNullException("dataSource");

      if (columnNames == null)
        columnNames = _empty_columns;

      var desciptors = GetProperties(dataSource);

      if (desciptors.Count == 0)
        return;

      m_writer.WriteLine("\t<table border = \"1\" cellpadding=\"5\" cellspacing=\"0\">");
      m_writer.WriteLine("\t\t<tr>");

      foreach (PropertyDescriptor pd in desciptors)
      {
        if (pd.IsBrowsable)
          m_writer.WriteLine("\t\t\t<td><strong>{0}</strong></td>", GetDisplayName(columnNames, pd));
      }

      m_writer.WriteLine("\t\t</tr>");

     foreach (var line in dataSource)
      {
        m_writer.WriteLine("\t\t<tr>");

        foreach (PropertyDescriptor pd in desciptors)
        {
          if (pd.IsBrowsable)
            m_writer.WriteLine("\t\t\t<td>{0}</td>", FormatValue(pd.GetValue(line)));
        }

        m_writer.WriteLine("\t\t</tr>");
      }

      m_writer.WriteLine("\t</table>");
    }

    public void WriteList(IList list)
    {
      if (list == null)
        throw new ArgumentNullException("list");

      m_writer.WriteLine("\t<ol>");

      foreach (object item in list)
        m_writer.WriteLine("\t\t<li>{0}</li>", FormatValue(item));

      m_writer.WriteLine("\t</ol>");
    }

    public void WriteCollection(ICollection collection)
    {
      if (collection == null)
        throw new ArgumentNullException("collection");

      m_writer.WriteLine("\t<ul>");

      foreach (object item in collection)
        m_writer.WriteLine("\t\t<li>{0}</li>", FormatValue(item));

      m_writer.WriteLine("\t</ul>");
    }

    public void WriteGlossary(Dictionary<string, string> glossary)
    {
      if (glossary == null)
        throw new ArgumentNullException("glossary");

      m_writer.WriteLine("\t<ul>");

      foreach (var kv in glossary)
      {
        m_writer.WriteLine("\t\t<li>");
        m_writer.WriteLine("\t\t\t<em>{0}</em> - {1}", kv.Key, FormatValue(kv.Value));
        m_writer.WriteLine("\t\t</li>");
      }

      m_writer.WriteLine("\t</ul>");
    }

    public IDisposable CreateParagraph()
    {
      return new ParagraphWriter(m_writer);
    }

    public void WriteText(string text)
    {
      m_writer.WriteLine(FormatValue(text));
    }

    public void WriteSpace()
    {
      m_writer.Write(" &nbsp; ");
    }

    public static string FormatValue(object value)
    {
      if (value is double)
      {
        var copy = (double)value;

        if (copy > 0.00005)
        {
          return Math.Round(copy, 4).ToString();
        }
        else
        {
          var ret = copy.ToString();
          return FormatFloatValue(ret);
        }
      }

      if (value is float)
      {
        var copy = (float)value;

        if (copy > 0.00005)
          return Math.Round(copy, 4).ToString();
        else
        {
          var ret = copy.ToString();
          return FormatFloatValue(ret);
        }
      }

      if (value is string)
      {
        if (value.ToString().Contains("<br>"))
          value = value.ToString().Replace("<br>", "\n");

        return value.ToString().Replace("\n", "\n<br>\n");
      }

      if (value == null)
        return CoreResources.NULL;
      else
        return value.ToString();
    }

    private static string FormatFloatValue(string ret)
    {
      if (ret.Contains("E") || ret.Contains("e"))
      {
        var parts = ret.Split('e', 'E');

        ret = string.Format("{0:0.0000}*10<sup>{1}</sup>",
          double.Parse(parts[0]), parts[1]);
      }

      return ret;
    }

    private static string GetDisplayName(Dictionary<string, string> columnNames, PropertyDescriptor pd)
    {
      string display_name;

      if (!columnNames.TryGetValue(pd.Name, out display_name))
        display_name = pd.DisplayName;

      if (string.IsNullOrWhiteSpace(display_name))
        display_name = pd.Name;

      return display_name;
    }

    private static PropertyDescriptorCollection GetProperties(IList dataSource)
    {
      if (dataSource is ITypedList)
        return ((ITypedList)dataSource).GetItemProperties(ArrayExtensions.Empty<PropertyDescriptor>());
      else if (dataSource.Count > 0)
        return TypeDescriptor.GetProperties(dataSource[0]);

      Type element_type = null;

      foreach (var inter in dataSource.GetType().GetInterfaces())
      {
        if (inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IList<>))
        {
          element_type = inter.GetGenericArguments()[0];
          break;
        }
      }

      if (element_type != null)
        return TypeDescriptor.GetProperties(element_type);
      else
        return new PropertyDescriptorCollection(ArrayExtensions.Empty<PropertyDescriptor>());
    }

    #region Destroy -------------------------------------------------------------------------------

    protected virtual void Dispose(bool disposing)
    {
      if (m_closed)
        return;
      
      if (disposing)
      {
        this.WriteEndDocument();
        m_writer.Dispose();
        m_closed = true;
      }
    }
    
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion

    private class ParagraphWriter : IDisposable
    {
      private readonly TextWriter m_writer;
      private bool m_exited;

      public ParagraphWriter(TextWriter writer)
      {
        m_writer = writer;
        m_writer.WriteLine("\t<p>");
      }

      public void Dispose()
      {
        if (m_exited)
          return;

        m_writer.WriteLine("\t</p>");
        m_exited = true;
      }

      public override string ToString()
      {
        return "ParagraphWriter";
      }
    }
  }
}