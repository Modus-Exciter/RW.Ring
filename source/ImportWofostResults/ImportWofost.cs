using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Schicksal.Exchange;

namespace ImportWofostResults
{
  public class ImportWofost : MarshalByRefObject, ITableImport
  {
    public ImportResult Import(object context)
    {
      using (var dlg = new WofostResultsImportForm())
      {
        if (dlg.ShowDialog() == DialogResult.OK)
        {
          var table = ReadFromExcel(dlg.ExcelFileName);
          Dictionary<int, double> third, second;

          third = ReadJson(dlg.ThirdLevelFileName);
          second = ReadJson(dlg.SecondLevelFileName);

          foreach (DataRow row in table.Rows)
          {
            if (row["Сорт"].ToString() != "RIC501.CAB")
              continue;

            int variant = (int)row["Вариант"];
            int level = (int)row["Уровень продуктивности"];
            var level_dic = (level == 2 ? second : third);

            if (level_dic.ContainsKey(variant))
              row["Урожай"] = level_dic[variant];
          }

          table.AcceptChanges();

          return new ImportResult
          {
            Table = table,
            Description = string.Format("Import from {0}", Path.GetFileName(dlg.ExcelFileName))
          };
        }
        else
          return null;
      }
    }

    private Dictionary<int, double> ReadJson(string fileName)
    {
      var ret = new Dictionary<int, double>();
      var j_object = JObject.Parse(File.ReadAllText(fileName));

      foreach (JProperty property in j_object["data"]["results"])
        ret[int.Parse(property.Name)] = (double)property.Value["data"]["yield"];

      return ret;
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

        var dt = CreateTableStructure();

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
                row["Вариант"] = variant;
                row["Почва"] = "[" + current_soil + "]";
                row["Местность"] = current_weather.Split(',')[0];
                row["Сезон"] = current_weather.Contains("Раби") ? "Раби" : "Харифа";
                row["Год"] = "20" + (current_weather.Contains("Раби") ?
                  current_weather.Substring(current_weather.IndexOf("Раби") + 4).Trim()
                  : current_weather.Substring(current_weather.IndexOf("Харифа") + 6).Trim());
                row["Сорт"] = sort.Replace('#', '.');
                row["Уровень продуктивности"] = second ? 2 : 3;
                row["Урожай"] = ReadTWSO(dr[sort]);
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

    private static DataTable CreateTableStructure()
    {
      var dt = new DataTable();
      dt.Columns.Add("Вариант", typeof(int)).AllowDBNull = true;
      dt.Columns.Add("Почва", typeof(string)).AllowDBNull = true; ;
      dt.Columns.Add("Местность", typeof(string)).AllowDBNull = true; ;
      dt.Columns.Add("Сезон", typeof(string)).AllowDBNull = true; ;
      dt.Columns.Add("Год", typeof(string)).AllowDBNull = true; ;
      dt.Columns.Add("Сорт", typeof(string)).AllowDBNull = true;
      dt.Columns.Add("Уровень продуктивности", typeof(int)).AllowDBNull = true;
      dt.Columns.Add("Урожай", typeof(double));

      foreach (DataColumn col in dt.Columns)
      {
        if (col.DataType == typeof(string))
          col.DefaultValue = string.Empty;
      }

      dt.PrimaryKey = new[] { dt.Columns["Вариант"], dt.Columns["Сорт"], dt.Columns["Уровень продуктивности"] };

      return dt;
    }

    private object ReadTWSO(object value)
    {
      if (value is DBNull)
        return value;

      return value.ToString().Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
    }

    public override string ToString()
    {
      return "Результаты обсчёта по WOFOST";
    }
  }
}
