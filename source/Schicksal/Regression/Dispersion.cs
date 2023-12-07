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
  public class Dispersion
  {
    private PolylineFit m_variance;

    public Func<double, double> Calculate { get => m_variance.Calculate; }

    public Dispersion(IDataGroup factor, IDataGroup result, Func<double, double> modelFunction)
    {
      IDataGroup residual = Residual.Calculate(factor, result, modelFunction);
      m_variance = new PolylineFit(factor, residual);
    }
  }
}
