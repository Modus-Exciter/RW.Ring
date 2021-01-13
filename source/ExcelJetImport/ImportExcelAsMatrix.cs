using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using JetExcelOleDbImport.Properties;
using Schicksal.Exchange;

namespace JetExcelOleDbImport
{
  public class ImportExcelAsMatrix : MarshalByRefObject, ITableImport
  {
    public ImportResult Import(object context)
    {
      using (var dlg = new ExcelImportAsMatrixForm())
      {
        if (dlg.ShowDialog() == DialogResult.OK)
        {
          var table = Utils.LoadTable(dlg.ExcelFileName, dlg.TableName);

          var result = new DataTable();

          result.Columns.Add(dlg.RowName, typeof(string));
          result.Columns.Add(dlg.ColumnName, typeof(string));
          result.Columns.Add(dlg.CellName, typeof(double));

          foreach (DataRow row in table.Rows)
          {
            for (int i = 1; i < table.Columns.Count; i++)
            {
              var new_row = result.NewRow();
              new_row[0] = row[0];
              new_row[1] = table.Columns[i].ColumnName;
              new_row[2] = row[i];

              result.Rows.Add(new_row);
            }
          }

          table = Utils.NormalizeTable(result);
          table.AcceptChanges();

          return new ImportResult
          {
            Table = table,
            Description = string.Format(Resources.IMPORT_FROM, Path.GetFileName(dlg.ExcelFileName))
          };
        }
      }

      return null;
    }

    public override string ToString()
    {
      return Resources.MATRIX_EXCEL_IMPORT;
    }
  }
}