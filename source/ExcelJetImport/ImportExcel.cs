using System;
using System.IO;
using System.Windows.Forms;
using JetExcelOleDbImport.Properties;
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

          var table = Utils.NormalizeTable(Utils.LoadTable(dlg.ExcelFileName, dlg.TableName));
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
      return Resources.EXCEL_IMPORT;
    }
  }
}