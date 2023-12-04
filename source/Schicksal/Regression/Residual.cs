using System;
using System.Collections.Generic;
using System.Globalization;
using Notung.Data;
using Schicksal.Basic;
using Schicksal.VectorField;
using Schicksal.Properties;
using Schicksal.Optimization;
using System.Linq;

namespace Schicksal.Regression
{
  public static class Residual
  {
    public static IDataGroup Calculate(IDataGroup x, IDataGroup y, Func<double, double> model)
    {
      if (x.Count != y.Count) throw new ArgumentException("Different sizes of arrays");
      double[] result = new double[x.Count];
      for (int i = 0; i < x.Count; i++)
        result[i] = Math.Abs(y[i] - model(x[i]));
      return new ArrayDataGroup(result);
    }

    public static IDataGroup Calculate2(IDataGroup x, IDataGroup y, Func<double, double> model)
    {
      if (x.Count != y.Count) throw new ArgumentException("Different sizes of arrays");
      double[] result = new double[x.Count];
      for (int i = 0; i < x.Count; i++)
      {
        result[i] = (y[i] - model(x[i]));
        result[i] *= result[i];
      }
      return new ArrayDataGroup(result);
    }
  }
}
