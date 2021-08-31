using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Notung.Logging;

namespace LogAnalyzer
{
  public class TablePresenter : INotifyPropertyChanged
  {
    private string m_file_name = string.Empty;
    private string m_separator = "===============================================";
    private string m_template = "[{Date}] [{Level}] [{Process}] [{Source}]\r\n{Message}";
    private int m_current_file = -1;
    private ObservableCollection<FileEntry> m_file_entries = new ObservableCollection<FileEntry>();
    private readonly Dictionary<string, string> m_filters = new Dictionary<string, string>();

    public event PropertyChangedEventHandler PropertyChanged;

    public ObservableCollection<FileEntry> OpenedFiles
    {
      get { return m_file_entries; }
    }

    public FileEntry CurrentFile
    {
      get
      {
        if (m_current_file >= 0 && m_current_file < m_file_entries.Count)
          return m_file_entries[m_current_file];
        else
          return null;
      }
      set
      {
        if (value != null)
          ChangeCurrentFile(m_file_entries.IndexOf(value));
        else
          ChangeCurrentFile(-1);
      }
    }

    public DataTable LoadedTable
    {
      get
      {
        var file = this.CurrentFile;

        if (file != null)
          return file.Table;
        else
          return null;
      }
      set
      {
        var file = this.CurrentFile;

        if (file != null)
        {
          file.Table = value;
          this.OnPropertyChanged("LoadedTable");
        }
      }
    }

    public string FileName
    {
      get
      {
        var file = this.CurrentFile;

        if (file != null)
          return file.FileName;
        else
          return null;
      }
      set
      {
        var file = this.CurrentFile;

        if (file != null)
        {
          file.FileName = value;
          this.OnPropertyChanged("FileName");
        }
      }
    }

    private void OnPropertyChanged(string property)
    {
      if (this.PropertyChanged != null)
        this.PropertyChanged(this, new PropertyChangedEventArgs(property));
    }

    public string Separator
    {
      get { return m_separator; }
      set
      {
        if (object.Equals(m_separator, value))
          return;

        m_separator = value;
        this.OnPropertyChanged("Separator");
      }
    }

    public string MessageTemplate
    {
      get { return m_template; }
      set
      {
        if (object.Equals(m_template, value))
          return;

        m_template = value;
        this.OnPropertyChanged("MessageTemplate");
      }
    }

    public override string ToString()
    {
      return m_file_name ?? string.Empty;
    }

    public void OpenConfig(string fileName)
    {
      ConfigXmlDocument doc = new ConfigXmlDocument();
      doc.Load(fileName);

      var nodeList = doc.SelectNodes("/configuration/applicationSettings/Notung.Logging.LogSettings/setting");

      foreach (var element in nodeList.OfType<XmlElement>())
      {
        if (element.GetAttribute("name") == "Separator")
          this.Separator = element.SelectSingleNode("value").InnerText;

        if (element.GetAttribute("name") == "MessageTemplate")
          this.MessageTemplate = element.SelectSingleNode("value").InnerText;
      }
    }

    public void OpenLog(string fileName)
    {
      for (int i = 0; i < m_file_entries.Count; i++)
      {
        if (m_file_entries[i].FileName.Equals(fileName))
        {
          ChangeCurrentFile(i);
          return;
        }
      }

      m_file_entries.Add(new FileEntry
      {
        FileName = fileName,
        Table = this.LoadLogTable(fileName)
      });

      ChangeCurrentFile(m_file_entries.Count - 1);
    }

    public void CloseCurrent()
    {
      var old_value = m_current_file;
      
      if (this.CurrentFile != null)
        m_file_entries.RemoveAt(old_value--);

      if (m_file_entries.Count > 0 && old_value < 0)
        this.ChangeCurrentFile(0);
      else
        this.ChangeCurrentFile(old_value);
    }

    public void SetFilter(string column, string value)
    {
      if (string.IsNullOrEmpty(column))
        return;

      if (string.IsNullOrEmpty(value))
        m_filters.Remove(column);
      else
        m_filters[column] = value;

      if (this.CurrentFile != null)
      {
        StringBuilder sb = new StringBuilder();
        bool first = true;

        foreach (var kv in m_filters)
        {
          if (!this.CurrentFile.Table.Columns.Contains(kv.Key))
            continue;

          if (first)
            first = false;
          else
            sb.Append(" AND ");

          sb.AppendFormat("Convert({0}, System.String) LIKE '{1}%'", kv.Key, kv.Value);
        }

        CurrentFile.Table.DefaultView.RowFilter = sb.ToString();
      }
    }

    private DataTable LoadLogTable(string fileName)
    {
      var table = new DataTable();
      table.BeginLoadData();

      List<string> lines = new List<string>();
      var builder = new LogStringBuilder(this.MessageTemplate);

      using (var reader = new StreamReader(fileName))
      {
        string line = null;

        while ((line = reader.ReadLine()) != null)
        {
          if (line.StartsWith("==============================="))
          {
            builder.FillRow(string.Join(Environment.NewLine, lines), table, true);
            lines.Clear();

            if (table.Columns.Contains("Message"))
            {
              var message = table.Rows[table.Rows.Count - 1]["Message"].ToString();
              var message_lines = message.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

              table.Rows[table.Rows.Count - 1]["Message"] = message_lines[0];

              if (message_lines.Length > 1)
                table.Rows[table.Rows.Count - 1]["Details"] = string.Join(Environment.NewLine, message_lines.Skip(1));
            }
          }
          else
            lines.Add(line);
        }
      }

      table.EndLoadData();
      return table;
    }

    private void ChangeCurrentFile(int index)
    {
      if (m_current_file == index)
        return;
      
      m_current_file = index;

      m_filters.Clear();

      this.OnPropertyChanged("CurrentFile");
      this.OnPropertyChanged("LoadedTable");
      this.OnPropertyChanged("FileName");
    }
  }

  public class FileEntry
  {
    public string FileName { get; set; }

    public DataTable Table { get; set; }

    public override string ToString()
    {
      return this.FileName ?? "log.log";
    }
  }
}
