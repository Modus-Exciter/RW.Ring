using Notung.Data;
using Schicksal.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Regression
{
  public static class MathFunction
  {
    public static double LogisticFunction(double x, double[] t)
    {
      if (t.Length != 3) throw new ArgumentException("Wrong size of parameter t");
      double power = Math.Pow(t[1], x);
      return t[0] * power / (t[2] + power);
    }

    public static double LinearFunction(double x, double[] t)
    {
      if (t.Length != 2) throw new ArgumentException("Wrong size of parameter t");
      return t[0] * x + t[1];
    }
  }

  public class LikelyhoodFunction
  {
    const double MINIMAL_WEIGHT = 0.01;

    readonly IDataGroup m_x;
    readonly IDataGroup m_y;
    readonly int m_n;

    Func<double, double[], double> m_regr_function;
    Func<double, double> m_var_function;

    public LikelyhoodFunction(IDataGroup x, IDataGroup y, Func<double, double[], double> regrFunction, Func<double, double> varFunction = null)
    {
      if (x.Count != y.Count) throw new ArgumentException("Sizes of selection doesn't match");
      this.m_x = x; this.m_y = y;
      this.m_n = x.Count;
      this.m_regr_function = regrFunction;
      this.m_var_function = varFunction;
    }
    public double Calculate(double[] t)
    {
      return m_var_function == null ? this.Calc(t) : this.Calc(t, m_var_function);
    }
    private double Calc(double[] t)
    {
      double res = 10E100;
      //double res = Math.Pow(2*Math.PI, -n/2);
      double variance = this.StandartVariance(t);
      double variance2 = variance * variance;

      for (int i = 0; i < m_n; i++)
      {
        res /= variance;
        res *= Math.Exp(-Math.Pow((m_y[i] - m_regr_function(m_x[i], t)), 2) / (2 * variance2));
      }

      return res;
    }
    private double Calc(double[] t, Func<double, double> varFunction)
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
        res *= Math.Exp(-Math.Pow((m_y[i] - m_regr_function(m_x[i], t)), 2) / (2 * variance2));
      }

      return res;
    }

    public double StandartVariance(double[] t)
    {
      if (m_x.Count != m_y.Count) throw new ArgumentException("Sizes of selection doesn't match");

      double res = 0;
      for (int i = 0; i < m_x.Count; i++)
      {
        double derivation = m_y[i] - m_regr_function(m_x[i], t);
        res += derivation * derivation;
      }
      res /= (m_x.Count - 1);

      return Math.Sqrt(res); ;
    }

    public IDataGroup CalculateResidual(double[] t)
    {
      double[] res = new double[m_n];
      double variance = this.StandartVariance(t);
      for (int i = 0; i < m_n; i++)
        res[i] = Math.Abs(m_y[i] - m_regr_function(m_x[i], t)) / variance;

      return new ArrayDataGroup(res);
    }
  }
}
