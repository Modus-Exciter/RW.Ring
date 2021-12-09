using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml;
using Notung.ComponentModel;
using Notung.Logging;

namespace LogAnalyzer
{
  class MainWindowContext : ObservableObject
  {
    private string m_separator = "===============================================";
    private string m_template = "[{Date}] [{Level}] [{Process}] [{Source}]\r\n{Message}";
    private readonly Dictionary<string, WeakReference> m_tables = new Dictionary<string, WeakReference>();

    private static readonly object _last_access_key = new object();

    public event EventHandler<MessageEventArgs> MessageRecieved;

    public string Separator
    {
      get { return m_separator; }
      set { this.ChangeValue(ref m_separator, value, "Separator"); }
    }

    public string MessageTemplate
    {
      get { return m_template; }
      set { this.ChangeValue(ref m_template, value, "MessageTemplate"); }
    }

    public void OpenConfig(string fileName)
    {
      var doc = new XmlDocument();

      doc.Load(fileName);

      var nodeList = doc.SelectNodes("/configuration/applicationSettings/Notung.Logging.LogSettings/setting");

      foreach (XmlElement element in nodeList)
      {
        if (element.GetAttribute("name") == "Separator")
          this.Separator = element.SelectSingleNode("value").InnerText;

        if (element.GetAttribute("name") == "MessageTemplate")
          this.MessageTemplate = element.SelectSingleNode("value").InnerText;
      }

      if (this.MessageRecieved != null)
        this.MessageRecieved(this, new MessageEventArgs("Конфигурационный файл загружен", false));
    }

    public FileEntry Refresh(string path)
    {
      if (File.Exists(path))
        return this.OpenLog(path);
      else if (Directory.Exists(path))
        return this.OpenDirectory(path);
      else
      {
        return new FileEntry
        {
          FileName = path,
          Table = m_tables.ContainsKey(path) ? m_tables[path].Target as DataTable : null
        };
      }
    }

    public FileEntry OpenLog(string fileName)
    {
      try
      {
        return new FileEntry
        {
          FileName = fileName,
          Table = this.GetDataTable(fileName, this.LoadLogTable)
        };
      }
      catch (Exception ex)
      {
        if (this.MessageRecieved != null)
          this.MessageRecieved(this, new MessageEventArgs(ex.Message));
      }

      return null;
    }

    public FileEntry OpenDirectory(string selectedPath)
    {
      if (!Directory.EnumerateFiles(selectedPath, "*.log").Any() &&
        Directory.Exists(Path.Combine(selectedPath, "Logs")))
      {
        selectedPath = Path.Combine(selectedPath, "Logs");
      }

      try
      {
        var entry = new FileEntry
        {
          FileName = selectedPath,
          Table = this.GetDataTable(selectedPath, this.LoadLogDirectory)
        };

        if (entry.Table.Columns.Count == 0)
        {
          m_tables.Remove(selectedPath);

          if (this.MessageRecieved != null)
            this.MessageRecieved(this, new MessageEventArgs("В указанной папке протоколы не найдены"));
        }
        else
          return entry;
      }
      catch (Exception ex)
      {
        if (this.MessageRecieved != null)
          this.MessageRecieved(this, new MessageEventArgs(ex.Message));
      }

      return null;
    }

    #region Implementation ------------------------------------------------------------------------

    private DataTable LoadLogDirectory(string path)
    {
      var table = new DataTable();

      table.BeginLoadData();

      foreach (var fileName in Directory.EnumerateFiles(path, "*.log"))
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

    private DataTable GetDataTable(string path, Func<string, DataTable> createCallback)
    {
      WeakReference reference;

      if (m_tables.TryGetValue(path, out reference))
      {
        var table = reference.Target as DataTable;

        if (table != null)
        {
          if (GetLastWriteTime(path).Equals(table.ExtendedProperties[_last_access_key]))
            return table;
          else
            m_tables.Remove(path);
        }
      }

      foreach (var kv in m_tables.ToArray())
      {
        if (!kv.Value.IsAlive)
          m_tables.Remove(kv.Key);
      }

      var ret = createCallback(path);

      reference = new WeakReference(ret);
      m_tables[path] = reference;
      ret.ExtendedProperties.Add(_last_access_key, GetLastWriteTime(path));

      return ret;
    }

    private static DateTime GetLastWriteTime(string path)
    {
      if (File.Exists(path))
        return File.GetLastWriteTime(path);
      else if (Directory.Exists(path))
        return Directory.EnumerateFiles(path, "*.log").Max(File.GetLastWriteTime);
      else
        return DateTime.Now.Date;
    }

    private void FillTable(string fileName, DataTable table)
    {
      var lines = new List<string>();
      var builder = new LogStringBuilder(this.MessageTemplate);

      using (var reader = new StreamReader(fileName))
      {
        string line = null;

        while ((line = reader.ReadLine()) != null)
        {
          if (line.StartsWith(m_separator))
          {
            builder.FillRow(string.Join(Environment.NewLine, lines), table, true);
            lines.Clear();

            if (table.Columns.Contains("Message"))
            {
              var message = table.Rows[table.Rows.Count - 1]["Message"].ToString();
              var message_lines = message.Split(new string[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

              table.Rows[table.Rows.Count - 1]["Message"] = message_lines[0];

              if (message_lines.Length > 1)
                table.Rows[table.Rows.Count - 1]["Details"] = string.Join(
                  Environment.NewLine, message_lines.Skip(1));
            }
          }
          else
            lines.Add(line);
        }
      }
    }

    #endregion
  }

  public class FileEntry
  {
    private static readonly string _user_directory = Environment.GetFolderPath(
      Environment.SpecialFolder.LocalApplicationData);

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

  public class MessageEventArgs : EventArgs
  {
    public MessageEventArgs(string error, bool isError = true)
    {
      this.Message = error;
      this.IsError = isError;
    }

    public bool IsError { get; private set; }

    public string Message { get; private set; }
  }

  public class MainWindowCommands
  {
    public static readonly ICommand OpenConfig = new RoutedUICommand
    {
      Text = "Открыть конфигурационный файл",
      InputGestures =
      {
        new KeyGesture(Key.F, ModifierKeys.Control)
      }
    };

    public static readonly ICommand OpenFolder = new RoutedUICommand
    {
      Text = "Открыть папку с протоколами",
      InputGestures =
      {
        new KeyGesture(Key.D, ModifierKeys.Control)
      }
    };

    public static readonly ICommand OpenLogFile = new RoutedUICommand
    {
      Text = "Открыть отдельный протокол",
      InputGestures =
      {
        new KeyGesture(Key.L, ModifierKeys.Control)
      }
    };

    public static readonly ICommand RefreshTable = new RoutedUICommand
    {
      Text = "Обновить протокол",
      InputGestures =
      {
        new KeyGesture(Key.F5, ModifierKeys.None)
      }
    };
  }
}