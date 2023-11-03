using Schicksal.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Regression
{
  public class PolylineFit
  {
    public struct Point
    {
      public readonly double x;
      public readonly double y;

      public Point(double x, double y)
      {
        this.x = x; this.y = y;
      }

      public override string ToString()
      {
        return x.ToString() + ' ' + y.ToString();
      }
    }
    public struct Line
    {
      public readonly Point left;
      public readonly Point right;
      public readonly double slope;

      public Line(Point left, Point right)
      {
        this.left = left;
        this.right = right;
        this.slope = (right.y - left.y) / (right.x - left.x);
      }

      public bool IsXBelong(double x)
      {
        if (x >= left.x && x <= right.x)
          return true;
        return false;
      }

      public double Calculate(double x)
      {
        return (x - left.x) * slope + left.y;
      }
    }

    const int TOL = 3;
    const double DEFAULT_SECTION_COUNT_COEF = 1;

    private readonly Line[] lines;
    private readonly Point[] points;
    private readonly List<List<Point>> dataPoints;

    public Line[] Lines { get { return (Line[])lines.Clone(); } }
    public Point[] Points { get { return (Point[])points.Clone(); } }

    public PolylineFit(IDataGroup x, IDataGroup y, double sectionCountCoef = DEFAULT_SECTION_COUNT_COEF)
    {
      if (x.Count != y.Count) throw new ArgumentOutOfRangeException();
      dataPoints = this.GetPointsByUniqeX(x, y);
      points = this.FitPoints(sectionCountCoef);
      lines = this.CreateLines();
    }

    private List<List<Point>> GetPointsByUniqeX(IDataGroup x, IDataGroup y)
    {
      List<List<Point>> points = new List<List<Point>> { new List<Point> { new Point(x[0], y[0]) } };
      for (int i = 1; i < x.Count; i++)
      {
        if (Math.Round(x[i], TOL) == Math.Round(points[points.Count - 1][0].x, TOL))
          points[points.Count - 1].Add(new Point(x[i], y[i]));
        else
          points.Add(new List<Point> { new Point(x[i], y[i]) });
      }
      return points;
    }

    private Point[] FitPoints(double sectionCountCoef)
    {
      int sectionCount = (int)(sectionCountCoef * Math.Sqrt(dataPoints.Count));
      int sectionSize = (int)((double)dataPoints.Count / sectionCount);
      int modulo = dataPoints.Count - sectionSize * sectionCount;
      Point[] linePoints = new Point[sectionCount + 2];

      linePoints[0] = new Point
        (dataPoints[0].Select(point => point.x).Average(),
        dataPoints[0].Select(point => point.y).Average());

      int index = 0;
      for (int i = 0; i < sectionCount; i++)
      {
        double midY = 0;
        double midX = 0;
        int pointsCount = 0;
        for (int j = 0; j < sectionSize || (i < modulo && j < (sectionSize + 1)); j++)
        {
          midY += dataPoints[index].Select(point => point.y).Sum();
          midX += dataPoints[index].Select(point => point.x).Sum();
          pointsCount += dataPoints[index].Count;
          index++;
        }
        midY /= pointsCount;
        midX /= pointsCount;
        linePoints[i + 1] = new Point(midX, midY);
      }

      linePoints[linePoints.Length - 1] = new Point
        (dataPoints.Last().Select(point => point.x).Average(),
        dataPoints.Last().Select(point => point.y).Average());

      return linePoints;
    }

    private Line[] CreateLines()
    {
      Line[] lines = new Line[points.Length - 1];
      for (int i = 0; i < lines.Length; i++)
        lines[i] = new Line(points[i], points[i + 1]);
      return lines;
    }

    public double Calculate(double x)
    {
      int i = 0;
      while (!lines[i].IsXBelong(x)) i++;
      return lines[i].Calculate(x);
    }

    public IDataGroup CalculateResidual()
    {
      List<double> res = new List<double>();

      for (int i = 0; i < dataPoints.Count; i++)
        for (int j = 0; j < dataPoints[i].Count; j++)
          res.Add(Math.Abs(this.Calculate(dataPoints[i][j].x) - dataPoints[i][j].y));

      return new ArrayDataGroup(res.ToArray());
    }
  }
}
