using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using Notung.ComponentModel;
using Notung.Logging;

namespace LogAnalyzer
{
  class MainWindowContext : ObservableObject
  {
    private string m_file_name = string.Empty;
    private string m_separator = "===============================================";
    private string m_template = "[{Date}] [{Level}] [{Process}] [{Source}]\r\n{Message}";

    public event EventHandler<ExceptionEventArgs> ExceptionOccured;

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

    public string GetDirectoryPath()
    {
      var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

      if (Directory.Exists(Path.Combine(path, "ARI")))
        return Path.Combine(path, "ARI");

      var dirs = Directory.GetDirectories(path);

      if (dirs.Length > 0)
        return dirs[0];
      else
        return path;
    }

    public void OpenConfig(string fileName)
    {
      XmlDocument doc = new XmlDocument();

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

    public FileEntry OpenLog(string fileName)
    {
      try
      {
        return new FileEntry
        {
          FileName = fileName,
          Table = this.LoadLogTable(fileName)
        };
      }
      catch (Exception ex)
      {
        if (this.ExceptionOccured != null)
          this.ExceptionOccured(this, new ExceptionEventArgs(ex.Message));
      }

      return null;
    }

    public FileEntry OpenDirectory(string selectedPath)
    {
      if (Directory.GetFiles(selectedPath, "*.log").Length == 0 &&
        Directory.Exists(Path.Combine(selectedPath, "Logs")))
        selectedPath = Path.Combine(selectedPath, "Logs");

      try
      {
        var entry = new FileEntry
        {
          FileName = selectedPath,
          Table = this.LoadLogDirectory(selectedPath)
        };

        if (entry.Table.Columns.Count > 0)
        {
          return entry;
        }
        else if (this.ExceptionOccured != null)
          this.ExceptionOccured(this, new ExceptionEventArgs("В указанной папке протоколы не найдены"));
      }
      catch (Exception ex)
      {
        if (this.ExceptionOccured != null)
          this.ExceptionOccured(this, new ExceptionEventArgs(ex.Message));
      }

      return null;
    }

    private DataTable LoadLogDirectory(string path)
    {
      var table = new DataTable();
      table.BeginLoadData();

      foreach (var fileName in Directory.GetFiles(path, "*.log"))
        this.FillTable(fileName, table);

      table.EndLoadData();

      return table;
    }

    private DataTable LoadLogTable(string fileName)
    {
      var table = new DataTable();
      table.BeginLoadData();

      this.FillTable(fileName, table);

      table.EndLoadData();

      return table;
    }

    private void FillTable(string fileName, DataTable table)
    {
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
    }
  }

  public class FileEntry
  {
    private static readonly string _user_directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public string FileName { get; set; }

    public DataTable Table { get; set; }

    public override string ToString()
    {
      string file = this.FileName;

      if (file == null)
        return "log.log";

      if (file.StartsWith(_user_directory))
        file = file.Substring(_user_directory[_user_directory.Length - 1] == '\\' ?
          _user_directory.Length : _user_directory.Length + 1);

      if (Path.GetFileName(Path.GetDirectoryName(file)).Equals("Logs", StringComparison.OrdinalIgnoreCase))
        file = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(file)), Path.GetFileName(file));

      return file;
    }
  }

  public class ExceptionEventArgs : EventArgs
  {
    public ExceptionEventArgs(string error)
    {
      this.Error = error;
    }

    public string Error { get; private set; }
  }
}