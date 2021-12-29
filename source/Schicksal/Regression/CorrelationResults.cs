using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Schicksal.Regression
{
  public class CorrelationResults
  {
    public CorrelationResults(DataTable table, string factor, string effect)
    {
      if (table == null)
        throw new ArgumentNullException("table");

      if (!table.Columns.Contains(factor) || !table.Columns.Contains(effect))
        throw new KeyNotFoundException();

      this.Table = table;
      this.Factor = factor;
      this.Effect = effect;
    }

    public DataTable Table { get; private set; }

    public string Factor { get; private set; }

    public string Effect { get; private set; }

    public CorrelationFormula Run(Action<double, double> addXY)
    {
      double min_x = double.MaxValue;
      double max_x = double.MinValue;
      double min_y = 0;
      double max_y = 0;

      double avg_x = this.Table.Select(string.Format("{0} IS NOT NULL AND {1} IS NOT NULL",
        this.Factor, this.Effect)).Average(row => Convert.ToDouble(row[this.Factor]));
      double avg_y = this.Table.Select(string.Format("{0} IS NOT NULL AND {1} IS NOT NULL",
        this.Factor, this.Effect)).Average(row => Convert.ToDouble(row[this.Effect]));

      double sum_up = 0;
      double sum_dn = 0;

      foreach (DataRow row in this.Table.Select(string.Format("{0} IS NOT NULL AND {1} IS NOT NULL", this.Factor, this.Effect)))
      {
        double x = Convert.ToDouble(row[this.Factor]);
        double y = Convert.ToDouble(row[this.Effect]);
        addXY(x, y);

        sum_up += (x - avg_x) * (y - avg_y);
        sum_dn += (x - avg_x) * (x - avg_x);

        if (min_x > x)
          min_x = x;

        if (max_x < x)
          max_x = x;
      }

      double byx = sum_up / sum_dn;
      min_y = avg_y + byx * (min_x - avg_x);
      max_y = avg_y + byx * (max_x - avg_x);

      return new CorrelationFormula
      {
        MinX = min_x,
        MaxX = max_x,
        MinY = min_y,
        MaxY = max_y,
        Factor = this.Factor,
        Effect = this.Effect,
        A = byx,
        B = avg_y - byx * avg_x
      };
    }
  }

  public class CorrelationFormula
  {
    public double MinX { get; internal set; }

    public double MaxX { get; internal set; }

    public double MinY { get; internal set; }

    public double MaxY { get; internal set; }

    public double A { get; internal set; }

    public double B { get; internal set; }

    public string Factor { get; internal set; }

    public string Effect { get; internal set; }

    public override string ToString()
    {
      return string.Format("{0} = {1} * {2} {3} {4}",
        (this.Effect ?? "y"), ConvertNumber(this.A), (this.Factor ?? "x"), 
        this.B >= 0 ? '+' : '-', ConvertNumber(Math.Abs(this.B)));
    }

    private static string ConvertNumber(double number)
    {
      if (Math.Abs(number) < 0.1 || Math.Abs(number) > 999999)
        return number.ToString("0.000e+0");
      else
        return number.ToString("0.000");
    }
  }
}
