using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Schicksal
{
  public static class StatisticsTables
  {
    public static DataTable GetStudentTable()
    {
      DataTable ret = new DataTable();

      ret.Columns.Add("N", typeof(int));
      ret.Columns.Add("0.05", typeof(double));
      ret.Columns.Add("0.01", typeof(double));
      ret.Columns.Add("0.001", typeof(double));

      var list = new List<int>(32);

      for (int i = 1; i <= 30; i++)
        list.Add(i);

      list.AddRange(new[] { 50, 100, 500, 1000, 100000, int.MaxValue });

      foreach (int i in list)
      {
        var row = ret.NewRow();
        row[0] = i;

        for (int j = 1; j < ret.Columns.Count; j++)
        {
          var p = double.Parse(ret.Columns[j].ColumnName, CultureInfo.InvariantCulture);
          var t = SpecialFunctions.invstudenttdistribution(i, 1 - p / 2);
          row[j] = t;

          if (i == int.MaxValue)
            continue;

          var back = (1 - SpecialFunctions.studenttdistribution(i, t)) * 2;
          if (i < 1000 && Math.Abs(back - p) / p > 1e-12)
            throw new ArgumentException();
        }
        ret.Rows.Add(row);
      }

      return ret;
    }

    public static DataTable GetFTable(double p)
    {
      DataTable ret = new DataTable();

      ret.Columns.Add("N", typeof(int));

      for (int i = 1; i <= 10; i++)
        ret.Columns.Add(i.ToString(), typeof(double));

      ret.Columns.Add("12", typeof(double));
      ret.Columns.Add("24", typeof(double));
      ret.Columns.Add("50", typeof(double));
      ret.Columns.Add("100", typeof(double));

      var list = new List<int>(32);

      for (int i = 1; i <= 26; i++)
        list.Add(i);

      list.AddRange(new[] { 28, 30, 40, 50, 100, 500, 1000, int.MaxValue });

      foreach (var i in list)
      {
        var row = ret.NewRow();

        foreach (DataColumn col in ret.Columns)
        {
          if (col.ColumnName == "N")
            row[col] = i;
          else
          {
            row[col] = SpecialFunctions.invfdistribution(int.Parse(col.ColumnName), i, p);
            var back = SpecialFunctions.fcdistribution(int.Parse(col.ColumnName), i, (double)row[col]);

            if (i < 100 && Math.Abs(back - p) / p > 1e-14)
              throw new ArgumentException();
          }
        }

        ret.Rows.Add(row);
      }

      return ret;
    }

    public static DataTable GetChiSquare()
    {
      DataTable table = new DataTable();
      table.Columns.Add("N", typeof(int));
      table.Columns.Add("0.99", typeof(double));
      table.Columns.Add("0.95", typeof(double));
      table.Columns.Add("0.75", typeof(double));
      table.Columns.Add("0.50", typeof(double));
      table.Columns.Add("0.25", typeof(double));
      table.Columns.Add("0.10", typeof(double));
      table.Columns.Add(".05", typeof(double));
      table.Columns.Add(".01", typeof(double));

      var list = new List<int>(40);

      for (int i = 1; i <= 30; i++)
        list.Add(i);

      list.AddRange(new[] { 40, 50, 60, 70, 80, 90, 100, 500, 1000 });

      foreach (var i in list)
      {
        var row = table.NewRow();
        row[0] = i;

        for (int j = 1; j < table.Columns.Count; j++)
        {
          var p = double.Parse(table.Columns[j].ColumnName,
            CultureInfo.InvariantCulture);

          var chi = SpecialFunctions.invchisquaredistribution(i, p);

          row[j] = chi;

          var back = SpecialFunctions.chisquarecdistribution(i, chi);
          if (Math.Abs(back - p) / p > 1e-12)
            throw new ArgumentException();
        }

        table.Rows.Add(row);
      }

      return table;
    }
  }
}