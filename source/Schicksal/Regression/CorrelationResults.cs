using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Collections.ObjectModel;
using Notung;
using Notung.Logging;
using Schicksal.Basic;

namespace Schicksal.Regression
{
  public class CorrelationResults
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(CorrelationResults));

    public CorrelationResults(IDataGroup x, IDataGroup y)
    {
      if (x == null)
        throw new ArgumentNullException("x");

      if (y == null)
        throw new ArgumentNullException("y");

      this.Factor = x;
      this.Effect = y;
    }

    public IDataGroup Factor { get; private set; }

    public IDataGroup Effect { get; private set; }

    public CorrelationFormula Run(string factor, string effect)
    {
      double min_x = double.MaxValue;
      double max_x = double.MinValue;

      Point2D[] points = new Point2D[Factor.Count];

      for (int i = 0; i < Factor.Count; i++)
      {
        var point = new Point2D
        {
          X = Convert.ToDouble(Factor[i]),
          Y = Convert.ToDouble(Effect[i])
        };

        points[i] = point;

        if (min_x > point.X)
          min_x = point.X;

        if (max_x < point.X)
          max_x = point.X;
      }

      List<RegressionDependency> dependencies = new List<RegressionDependency>(5);

      foreach (var type in typeof(RegressionDependency).Assembly.GetAvailableTypes())
      {
        if (!type.IsAbstract && typeof(RegressionDependency).IsAssignableFrom(type))
        {
          try
          {
            var dp = (RegressionDependency)Activator.CreateInstance(type, Factor, Effect);
            dp.Factor = factor;
            dp.Effect = effect;
            dependencies.Add(dp);
          }
          catch (Exception ex)
          {
            _log.Error("Run(): exception", ex);
          }
        }
      }

      return new CorrelationFormula
      {
        MinX = min_x,
        MaxX = max_x,
        SourcePoints = points,
        Dependencies = dependencies.ToArray()
      };
    }
  }

  public struct Point2D
  {
    public double X { get; internal set; }

    public double Y { get; internal set; }

    public override string ToString()
    {
      return string.Format("X = {0}, Y = {1}", this.X, this.Y);
    }
  }

  public class CorrelationFormula
  {
    public double MinX { get; internal set; }

    public double MaxX { get; internal set; }

    public Point2D[] SourcePoints { get; internal set; }

    public RegressionDependency[] Dependencies { get; internal set; }
  }
}
