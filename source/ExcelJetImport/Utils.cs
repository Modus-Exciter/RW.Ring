using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace JetExcelOleDbImport
{
  internal class Utils
  {
    public static void FillTableList(string excelFile, IList tableList)
    {
      tableList.Clear();
      
      var mask = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"{0}\";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";

      using (var ole_cn = new OleDbConnection(string.Format(mask, excelFile)))
      {
        ole_cn.Open();
        DataTable tables = ole_cn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

        foreach (DataRow row in tables.Rows)
          tableList.Add(row["TABLE_NAME"].ToString());
      }
    }

    public static void CheckFileIsOpen(string excelFile, IList<string> errors)
    {
      var short_name = Path.GetFileName(excelFile);
      
      using (var excel_process = Process.GetProcessesByName("EXCEL").Where(p =>
       p.MainWindowTitle.Contains(short_name)).FirstOrDefault())
      {

        if (excel_process != null)
        {
          errors.Add(string.Format(@"Файл {0} открыт в Excel. Импорт может привести к нестабильной работы системы.
Закройте Excel и повторите попытку", short_name));
        }
      }
    }

    public static DataTable LoadTable(string fileName, string tableName)
    {
      var mask = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"{0}\";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";

      using (var connection = new OleDbConnection(string.Format(mask, fileName)))
      {
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = tableName;
        cmd.CommandType = CommandType.TableDirect;

        var table = new DataTable();

        using (var reader = cmd.ExecuteReader())
          table.Load(reader);

        return table;
      }
    }

    public static DataTable NormalizeTable(DataTable table)
    {
      HashSet<string> nulls = new HashSet<string>();
      HashSet<string> unconvertable = new HashSet<string>();
      bool last_row_empty = true;

      if (table.Rows.Count > 0)
      {
        foreach (DataColumn col in table.Columns)
        {
          if (!table.Rows[table.Rows.Count - 1].IsNull(col))
          {
            last_row_empty = false;
            break;
          }
        }

        if (last_row_empty)
          table.Rows.RemoveAt(table.Rows.Count - 1);
      }

      bool reload_required = CollectNormalizationData(table, nulls, unconvertable);

      var copy = reload_required ? table.Clone() : table;

      foreach (DataColumn col in copy.Columns)
      {
        if (nulls.Contains(col.ColumnName))
          continue;

        if (col.DataType == typeof(string))
          col.DefaultValue = string.Empty;

        if (col.DataType == typeof(double) || col.DataType == typeof(float))
        {
          if (!unconvertable.Contains(col.ColumnName))
            col.DataType = typeof(int);
        }

        if (col.DataType != typeof(double) && col.DataType != typeof(float))
          col.AllowDBNull = false;
      }

      if (reload_required)
        copy.Load(table.CreateDataReader());

      return copy;
    }

    private static bool CollectNormalizationData(DataTable table, HashSet<string> nulls, HashSet<string> unconvertable)
    {
      foreach (DataRow row in table.Rows)
      {
        foreach (DataColumn col in table.Columns)
        {
          if (row.IsNull(col))
            nulls.Add(col.ColumnName);
          else
          {
            if (col.DataType == typeof(float))
            {
              var fl = (float)row[col];

              if (fl != (int)fl)
                unconvertable.Add(col.ColumnName);
            }
            if (col.DataType == typeof(double))
            {
              var fl = (double)row[col];

              if (fl != (int)fl)
                unconvertable.Add(col.ColumnName);
            }
          }
        }
      }

      bool reload_required = false;

      foreach (DataColumn col in table.Columns)
      {
        if (col.DataType == typeof(double) || col.DataType == typeof(float))
        {
          if (!unconvertable.Contains(col.ColumnName))
          {
            reload_required = true;
            break;
          }
        }
      }

      return reload_required;
    }
  }
}