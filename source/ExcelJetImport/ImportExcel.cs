using System;
using System.IO;
using System.Windows.Forms;
using JetExcelOleDbImport.Properties;
using Schicksal.Exchange;
using System.Collections.Generic;
using System.Data;

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
  /*
  public class ImportCalcPoints : MarshalByRefObject, ITableImport
  {
    public ImportResult Import(object context)
    {
      using (var dlg = new ExcelImportForm())
      {
        if (dlg.ShowDialog() == DialogResult.OK)
        {

          var table = Utils.NormalizeTable(Utils.LoadTable(dlg.ExcelFileName, dlg.TableName));
          table.AcceptChanges();

          var res = table.Clone();

          var rem = new HashSet<string>();

          foreach (DataColumn col in res.Columns)
          {
            if (col.ColumnName.StartsWith("Minimal") 
              || col.ColumnName.StartsWith("Intensive") 
              || col.ColumnName.StartsWith("Semi-Intensive"))
              rem.Add(col.ColumnName);
          }

          foreach (var name in rem)
            res.Columns.Remove(name);

          res.Columns.Add("Model", typeof(string));
          res.Columns.Add("Technology", typeof(string));
          res.Columns.Add("Sort", typeof(string));
          res.Columns.Add("ModelResult", typeof(double));

          var models = new string[] { "ORYZA", "WOFOST" };
          var technologies = new string[] { "Minimal", "Semi-Intensive" , "Intensive"};
          var sorts = new string[] { "Basmati", "Non-basmati" };

          foreach (DataRow source in table.Rows)
          {
            foreach (var model in models)
            {
              foreach (var sort in sorts)
              {
                string suffix = string.Empty;

                if (model == "ORYZA")
                {
                  if (sort != "Basmati") 
                    suffix = "1";
                }
                else
                {
                  if (sort == "Basmati")
                    suffix = "2";
                  else
                    suffix = "3";
                }

                foreach (var technology in technologies)
                {
                  if (model == "WOFOST" && technology == "Minimal")
                    continue;
                  
                  var row = res.NewRow();

                  foreach (DataColumn col in table.Columns)
                  {
                    if (!rem.Contains(col.ColumnName))
                      row[col.ColumnName] = source[col.ColumnName];
                  }

                  row["Model"] = model;
                  row["Technology"] = technology;
                  row["Sort"] = sort;

                  row["ModelResult"] = source[technology + suffix];

                  res.Rows.Add(row);
                }
              }
            }
          }

          res.AcceptChanges();

          return new ImportResult
          {
            Table = res,
            Description = string.Format(Resources.IMPORT_FROM, Path.GetFileName(dlg.ExcelFileName))
          };
        }
      }

      return null;
    }
  }
  */
}