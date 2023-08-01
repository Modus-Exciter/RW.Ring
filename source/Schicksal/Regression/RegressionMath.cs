using Notung.Data;
using Schicksal.Basic;
using Schicksal.VectorField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Regression
{
  public static class MathFunction
  {

    public static double Quadratic(VectorDataGroup x)
    {
      return Math.Pow(x.Length(), 2);
    }

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

    public static double Linear(double x, IDataGroup t)
    {
      if (t.Dim != 2) throw new ArgumentException("Wrong size of parameter t");
      return t[0] * x + t[1];
    }
  }

  public class LikelyhoodFunction
  {
    const double MINIMAL_WEIGHT = 0.01;

    readonly IDataGroup m_x;
    readonly IDataGroup m_y;
    readonly int m_n;

    Func<double, IDataGroup, double> m_regr_function;
    Func<double, double> m_var_function;

    public LikelyhoodFunction(IDataGroup x, IDataGroup y, Func<double, IDataGroup, double> regrFunction, Func<double, double> varFunction = null)
    {
      if (x.Dim != y.Dim) throw new ArgumentException("Sizes of selection doesn't match");
      this.m_x = x; this.m_y = y;
      this.m_n = x.Dim;
      this.m_regr_function = regrFunction;
      this.m_var_function = varFunction;
    }

    public double Calculate(IDataGroup t)
    {
      return m_var_function == null ? this.Calc(t) : this.Calc(t, m_var_function);
    }

    private double Calc(IDataGroup t)
    {
      double res = 10E100;
      //double res = Math.Pow(2*Math.PI, -n/2);
      double variance = this.StandartVariance(t);
      double variance2 = variance * variance;

      for (int i = 0; i < m_n; i++)
      {
        res /= variance;
        res *= Math.Exp(-Math.Pow(m_y[i] - m_regr_function(m_x[i], t), 2) / (2 * variance2));
      }

      return res;
    }

    private double Calc(IDataGroup t, Func<double, double> varFunction)
    {
      double res = 10E100;
      //double res = Math.Pow(2 * Math.PI, -n / 2);
      double standartVariance = this.StandartVariance(t);
      double variance = 0; double variance2 = 0;

      for (int i = 0; i < m_n; i++)
      {
        variance = varFunction(m_x[i]) * standartVariance;
        if (variance <= 0) variance = MINIMAL_WEIGHT * standartVariance;
        variance2 = variance * variance;

        res /= variance;
        res *= Math.Exp(-Math.Pow(m_y[i] - m_regr_function(m_x[i], t), 2) / (2 * variance2));
      }

      return res;
    }

    public double StandartVariance(IDataGroup t)
    {
      if (m_x.Dim != m_y.Dim) throw new ArgumentException("Sizes of selection doesn't match");

      double res = 0;
      for (int i = 0; i < m_x.Dim; i++)
      {
        double derivation = m_y[i] - m_regr_function(m_x[i], t);
        res += derivation * derivation;
      }
      res /= (m_x.Dim - 1);

      return Math.Sqrt(res);
    }

    public IDataGroup CalculateResidual(IDataGroup t)
    {
      VectorDataGroup res = new VectorDataGroup(new double[m_n]);
      double variance = this.StandartVariance(t);
      for (int i = 0; i < m_n; i++)
        res[i] = Math.Abs(m_y[i] - m_regr_function(m_x[i], t)) / variance;

      return new ArrayDataGroup(res);
    }
  }
}
