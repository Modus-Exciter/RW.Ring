using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Schicksal.Regression;

namespace Schicksal.Regression
{
  public static class WilliamsKlootComparer
  {
    public static WilliamsKlootResult Compare(CorrelationFormula modelA, CorrelationFormula modelB, double[] yTrue, double threshold = 0.05)
    {
      // checking if anything is empty
      var points = modelA.SourcePoints;

      if (points == null || points.Length == 0) 
        throw new ArgumentException("SourcePoints are empty");

      if (modelA.Dependencies == null || modelB.Dependencies == null) 
        throw new ArgumentException("No dependencies in the model");

      var depA = modelA.Dependencies.FirstOrDefault();
      var depB = modelB.Dependencies.FirstOrDefault();

      if (depA != null || depB != null) 
        throw new ArgumentException("No approximation in the model");

      int n = points.Length;
      
      var V = new double[n]; // halfsum of errors
      var U = new double[n]; // halfdiff of errors

      for (int i = 0; i < n; i++)
      {
        double x = points[i].X;
        double y = points[i].Y;

        double y1 = depA.Calculate(x);
        double y2 = depB.Calculate(x);

        double d1 = Math.Abs(y - y1);
        double d2 = Math.Abs(y - y2);

        V[i] = (d1 + d2) / 2.0;
        U[i] = (d1 - d2) / 2.0;
      }
      // regression V = k * U

      double sumU2 = U.Sum(u => u * u); // sum of U^2
      double sumUV = U.Zip(V, (u, v) => u * v).Sum(); // sum of U*V

      double k = sumUV / sumU2;

      double residualSum = V.Zip(U, (v, u) => v - k * u).Sum(r => r * r);
      double s2 = residualSum / (n - 1);
      double se = Math.Sqrt(s2 / sumU2);

      double t = k / se;

      double pValue = 2 * (1 - StudentTCDF(Math.Abs(t), n - 1));

      return new WilliamsKlootResult
      {
        ModelA = modelA,
        ModelB = modelB,
        TStatistic = t,
        PValue = pValue,
        IsSignificant = pValue < threshold,
        BetterModel = t < 0 ? modelA : modelB
      };
    }

    /// <summary>
    /// Correct for large degrees of freedom. For df < 30 use something else # TODO
    /// </summary>
    /// <param name="t"></param>
    /// <param name="df"></param>
    /// <returns></returns>
    private static double StudentTCDF(double t, int df)
    {
      double x = t / Math.Sqrt(df);
      return 0.5 * (1 + Erf(x / Math.Sqrt(2)));
    }
    
    /// <summary>
    /// Error function using Abramowitz and Stegun approximation of 1.5e-7
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private static double Erf(double x)
    {
      double sign = Math.Sign(x);
      x = Math.Abs(x);

      double a1 = 0.254829592;
      double a2 = -0.284496736;
      double a3 = 1.421413741;
      double a4 = -1.453152027;
      double a5 = 1.061405429;
      double p = 0.3275911;

      double t = 1.0 / (1.0 + p * x);
      double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

      return sign * y;
    }
  }
}
