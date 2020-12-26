using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Schicksal.Exchange;

namespace ImportWofostResults
{
  public class ImportWofost : ITableImport
  {
    public ImportResult Import(object context)
    {
      using (var dlg = new OpenFileDialog())
      {
        dlg.Filter = "Excel files|*.xls";

        if (dlg.ShowDialog(context as IWin32Window) == System.Windows.Forms.DialogResult.OK)
        {
          return new ImportResult
          {
            Table = ReadFromExcel(dlg.FileName),
            Description = string.Format("Import from {0}", Path.GetFileName(dlg.FileName))
          };
        }
        else
          return null;
      }
    }

    private DataTable ReadFromExcel(string fileName)
    {
      var mask = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"{0}\";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
      using (var connection = new OleDbConnection(string.Format(mask, fileName)))
      {
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "Main_DATA";
        cmd.CommandType = CommandType.TableDirect;

        var dt = new DataTable();
        dt.Columns.Add("Variant", typeof(int));
        dt.Columns.Add("Soil", typeof(string));
        dt.Columns.Add("Place", typeof(string));
        dt.Columns.Add("Season", typeof(string));
        dt.Columns.Add("Year", typeof(string));
        dt.Columns.Add("Sort", typeof(string));
        dt.Columns.Add("Level", typeof(int));
        dt.Columns.Add("TWSO", typeof(double));

        using (var dr = cmd.ExecuteReader())
        {
          string current_soil = null;
          string current_weather = null;
          int variant = 0, tmp;
          bool twso = false;
          bool second = false;

          string[] sorts = new string[dr.FieldCount - 2];
          for (int i = 2; i < dr.FieldCount; i++)
            sorts[i - 2] = dr.GetName(i);

          while (dr.Read())
          {
            if ("Почва:".Equals(dr[0]))
              current_soil = dr.GetString(1);
            else if ("Погода и сезон:".Equals(dr[0]))
              current_weather = dr.GetString(1);
            else if (int.TryParse(dr[1].ToString(), out tmp))
            {
              if (tmp == 1 && variant > 1)
                second = true;

              variant = tmp;
            }
            else if (twso)
            {
              twso = false;

              Debug.Assert(current_weather.Contains("Раби") ||
                current_weather.Contains("Харифа"));

              foreach (var sort in sorts)
              {
                var row = dt.NewRow();
                row["Variant"] = variant;
                row["Soil"] = "[" + current_soil + "]";
                row["Place"] = current_weather.Split(',')[0];
                row["Season"] = current_weather.Contains("Раби") ? "Раби" : "Харифа";
                row["Year"] = "20" + (current_weather.Contains("Раби") ?
                  current_weather.Substring(current_weather.IndexOf("Раби") + 4).Trim()
                  : current_weather.Substring(current_weather.IndexOf("Харифа") + 6).Trim());
                row["Sort"] = sort.Replace('#', '.');
                row["Level"] = second ? 2 : 3;
                row["TWSO"] = dr[sort];
                dt.Rows.Add(row);
              }
            }
            else if ("TWSO".Equals(dr[2]))
            {
              twso = true;
            }
          }
        }
        dt.AcceptChanges();

        return dt;
      }
    }

    public override string ToString()
    {
      return "Результаты обсчёта по WOFOST";
    }
  }
}
