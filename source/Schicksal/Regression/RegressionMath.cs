using Notung.Data;
using Schicksal.Basic;
using Schicksal.VectorField;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Security.AccessControl;
using System.Runtime.InteropServices.ComTypes;
#if DEBUG
using Schicksal.Optimization;
#endif
namespace Schicksal.Regression
{
  public static class MathFunction
  {
    public static Func<VectorDataGroup, double> ReverseOnF(Func<VectorDataGroup, double> func)
    {
      return (VectorDataGroup x) => -func(x);
    }

    public static double Logistic(double x, IDataGroup t)
    {
      if (t.Dim != 3) throw new ArgumentException("Wrong size of parameter t");
      double power = Math.Pow(t[1], x);
      return t[0] * power / (t[2] + power);
    }

    public static double Michaelis(double x, IDataGroup t)
    {
      if (t.Dim != 2) throw new ArgumentException("Wrong size of parameter t");
      return t[0] * x / (t[1] + x);
    }

    public static double Linear(double x, IDataGroup t)
    {
      if (t.Dim != 2) throw new ArgumentException("Wrong size of parameter t");
      return t[0] * x + t[1];
    }
  }

  public class LikelyhoodFunction
  {
    const double MIN_VAR = 10E-3;
    const double HET_THRESHOLD = 0.5;
    const double SAMPLE_COUNT_THRESHOLD = 20;

    private readonly IDataGroup x;
    private readonly IDataGroup y;
    private double midVar;
    private readonly Func<double, IDataGroup, double> dependencyFunction;
    private readonly Func<IDataGroup, double> calculate;
    private PolylineFit variance;

    public Func<IDataGroup, double> Calculate { get { return calculate; } }

    public LikelyhoodFunction(IDataGroup x, IDataGroup y, Func<double, IDataGroup, double> dependencyFunction)
    {
      if (x.Dim != y.Dim) throw new ArgumentOutOfRangeException();
      this.x = x; 
      this.y = y;
      this.dependencyFunction = dependencyFunction;
      
      variance = new PolylineFit(x, new PolylineFit(x, y).CalculateResidual());

      if (this.IsHeteroscedascity()) calculate = this.CalculateHet;
      else calculate = this.CalculateDef;
    }

    private bool IsHeteroscedascity()
    {
      double[] varVals = variance.Points.Select(point => point.y)
        .Skip(1).Take(variance.Points.Length - 2).ToArray();

      if (x.Dim >= SAMPLE_COUNT_THRESHOLD)
      {
        midVar = varVals.Sum() / varVals.Length;
        double maxDiff = varVals.Max() - varVals.Min();

        if (maxDiff / midVar <= HET_THRESHOLD) return false;
        else return true;
      }
      else
      {
        midVar = varVals.Max();
        return false;
      }
    }

    private double CalculateHet(IDataGroup t)
    {
      double res = 0;
      double a, b;

      for (int i = 0; i < x.Dim; i++)
      {
        a = y[i] - dependencyFunction(x[i], t);
        b = variance.Calculate(x[i]);
        if (b == 0) b = MIN_VAR;
        res += (a * a) / (2 * b * b);
      }
      if (double.IsNaN(res)) res = double.MaxValue;
      return res;
    }

    private double CalculateDef(IDataGroup t)
    {
      double res = 0;
      double b = 2 * midVar * midVar;
      double a;
      
      for (int i = 0; i < x.Dim; i++)
      {
        a = y[i] - dependencyFunction(x[i], t);
        res += (a * a) / b;
      }

      return res;
    }
  }
}
