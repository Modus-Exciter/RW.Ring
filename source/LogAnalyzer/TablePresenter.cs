using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using Notung.Logging;

namespace LogAnalyzer
{
  public class TablePresenter : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    private DataTable m_loaded_table;

    public TablePresenter()
    {
      m_loaded_table = new DataTable();
      m_loaded_table.Columns.Add("Данные");
    }

    public DataTable LoadedTable
    {
      get { return m_loaded_table; }
      set
      {
        m_loaded_table = value;
        this.OnPropertyChanged("LoadedTable");
      }
    }

    private void OnPropertyChanged(string property)
    {
      if (this.PropertyChanged != null)
        this.PropertyChanged(this, new PropertyChangedEventArgs(property));
    }

    public void OpenFile(string fileName)
    {
      var table = new DataTable();
      table.BeginLoadData();

      List<string> lines = new List<string>();
      var builder = new LogStringBuilder(@"[{Date}] [{Level}] [{Process}] [{Source}]
{Message}");

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

      this.LoadedTable = table;
    }
  }
}
