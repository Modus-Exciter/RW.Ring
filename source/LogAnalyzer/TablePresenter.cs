using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using Notung.Logging;

namespace LogAnalyzer
{
  public class TablePresenter : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    private string m_file_name = string.Empty;
    private string m_separator = "===============================================";
    private string m_template = "[{Date}] [{Level}] [{Process}] [{Source}]\r\n{Message}";
    private ObservableCollection<FileEntry> m_file_entries = new ObservableCollection<FileEntry>();
    private int m_current_file = -1;

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

    public void OpenFile(string fileName)
    {
      for (int i = 0; i < m_file_entries.Count; i++)
      {
        if (m_file_entries[i].FileName.Equals(fileName))
        {
          ChangeCurrentFile(i);
          return;
        }
      }

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

      m_file_entries.Add(new FileEntry
      {
        FileName = fileName,
        Table = table
      });

      ChangeCurrentFile(m_file_entries.Count - 1);
    }

    private void ChangeCurrentFile(int i)
    {
      m_current_file = i;
      this.OnPropertyChanged("CurrentFile");
      this.OnPropertyChanged("LoadedTable");
      this.OnPropertyChanged("FileName");
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
