using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;
using Schicksal.Exchange;

namespace JetExcelOleDbImport
{
  public class ImportExcel : MarshalByRefObject, ITableImport
  {
    public ImportResult Import(object context)
    {
      using (var dlg = new ExcelImportForm())
      {
        if (dlg.ShowDialog() == DialogResult.OK)
        {
          var mask = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"{0}\";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";

          using (var connection = new OleDbConnection(string.Format(mask, dlg.ExcelFileName)))
          {
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = dlg.TableName;
            cmd.CommandType = CommandType.TableDirect;

            var table = new DataTable();

            using (var reader = cmd.ExecuteReader())
              table.Load(reader);

            table = NormalizeTable(table);

            return new ImportResult
            {
              Table = table,
              Description = string.Format("Import from {0}", Path.GetFileName(dlg.ExcelFileName))
            };
          }
        }
      }

      return null;
    }

    private static DataTable NormalizeTable(DataTable table)
    {
      HashSet<string> nulls = new HashSet<string>();
      HashSet<string> unconvertable = new HashSet<string>();

      bool reload_required = CollectNormalizationData(table, nulls, unconvertable);

      var copy = reload_required? table.Clone() : table;

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

    public override string ToString()
    {
      return "Импорт данных из Excel";
    }
  }
}