using System;
using System.Collections.Generic;
using System.Linq;

namespace Schicksal.Regression
{
  public static class CorrelationGraphUtils
  {
    public static System.Collections.Generic.KeyValuePair<Type, string>[] GetDependencySource(CorrelationFormula data)
    {
      return RegressionDependency.GetDependencyTypeNames().Where(kv =>
              data.Dependencies.Any(d => d.GetType() == kv.Key)).ToArray();
    }

    public static RegressionDependency GetBestDependency(CorrelationFormula data)
    {
      return data.Dependencies.OrderByDescending(d => d.Consistency).First();
    }

    public static RegressionDependency FillPoints(CorrelationFormula data, Type dependencyType, Func<int, Action<double, double>> createDestinaion)
    {
      var dependency = data.Dependencies.Single(d => dependencyType.Equals(d.GetType()));

      double[] points = CorrelationGraphUtils.GetKeyPoints(dependency, data.MaxX, data.MinX);
      double[] gaps = dependency.GetGaps();

      double max_y = 1.3 * data.MaxY - 0.3 * data.MinY;
      double min_y = 1.3 * data.MinY - 0.3 * data.MaxY;

      for (int i = 1; i < points.Length; i++)
      {
        double min_x = points[i - 1];
        double max_x = points[i];

        CorrectBorders(dependency, (max_x - min_x) / 1000, max_y, min_y, ref max_x, ref min_x);

        var destination = createDestinaion(i);
        int pt = 100;

        for (int j = 0; j <= pt; j++)
        {
          double x = min_x + j * (max_x - min_x) / pt;

          if (Array.IndexOf(gaps, x) < 0)
            destination(x, dependency.Calculate(x));
        }
      }

      return dependency;
    }

    private static double[] GetKeyPoints(RegressionDependency dependency, double maxX, double minX)
    {
      double[] points = new double[] { minX }
        .Concat(dependency.GetGaps().Where(g => g < maxX && g > minX))
        .Concat(new double[] { maxX }).ToArray();

      return points;
    }

    private static void CorrectBorders(RegressionDependency dependency, double shift, double maxY, double minY, ref double maxX, ref double minX)
    {
      if (Array.IndexOf(dependency.GetGaps(), minX) >= 0)
      {
        double y = dependency.Calculate(minX + shift);
        double old_x = minX;

        while (y > maxY || y < minY)
        {
          minX += shift;
          y = dependency.Calculate(minX);

          if (minX > maxX)
          {
            minX = old_x + shift * 10;
            break;
          }
        }
      }

      if (Array.IndexOf(dependency.GetGaps(), maxX) >= 0)
      {
        double y = dependency.Calculate(maxX - shift);
        double old_x = maxX;

        while (y > maxY || y < minY)
        {
          maxX -= shift;
          y = dependency.Calculate(maxX);

          if (minX > maxX)
          {
            maxX = old_x - shift * 10;
            break;
          }
        }
      }
    }
  }
}