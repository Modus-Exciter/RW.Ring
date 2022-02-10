using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Schicksal.Regression
{
  public class CorrelationResults
  {
    public CorrelationResults(DataTable table, string factor, string effect, Type dependencyType)
    {
      if (table == null)
        throw new ArgumentNullException("table");

      if (!table.Columns.Contains(factor) || !table.Columns.Contains(effect))
        throw new KeyNotFoundException();

      if (dependencyType == null)
        throw new ArgumentNullException("dependencyType");

      this.Table = table;
      this.Factor = factor;
      this.Effect = effect;
      this.DependencyType = dependencyType;
    }

    public DataTable Table { get; private set; }

    public string Factor { get; private set; }

    public string Effect { get; private set; }

    public Type DependencyType { get; private set; }

    public CorrelationFormula Run(Action<double, double> addXY = null)
    {
      double min_x = double.MaxValue;
      double max_x = double.MinValue;
      double min_y = 0;
      double max_y = 0;
      string filter = string.Format("{0} IS NOT NULL AND {1} IS NOT NULL", this.Factor, this.Effect);

      DataColumnGroup x_group = new DataColumnGroup(this.Table.Columns[this.Factor], filter);
      DataColumnGroup y_group = new DataColumnGroup(this.Table.Columns[this.Effect], filter);

      for (int i = 0; i < x_group.Count; i++)
      {
        double x = Convert.ToDouble(x_group[i]);
        double y = Convert.ToDouble(y_group[i]);

        if (addXY != null)
          addXY(x, y);

        if (min_x > x)
          min_x = x;

        if (max_x < x)
          max_x = x;
      }

       var dependency = (RegressionDependency)Activator.CreateInstance(this.DependencyType, x_group, y_group);

      min_y = dependency.Calculate(min_x);
      max_y = dependency.Calculate(max_x);

      return new CorrelationFormula
      {
        MinX = min_x,
        MaxX = max_x,
        Dependency = dependency
      };
    }
  }

  public class CorrelationFormula
  {
    public double MinX { get; internal set; }

    public double MaxX { get; internal set; }

    public RegressionDependency Dependency { get; internal set; }
  }
}
