using System;
using Schicksal.Basic;

namespace Schicksal.Regression
{
  public class Dispersion
  {
    private PolylineFit m_variance;
    private IPlainSample m_var_values;

    public Func<double, double> Calculate { get => m_variance.Calculate; }

    public Dispersion(IPlainSample factor, IPlainSample result, Func<double, double> modelFunction)
    {
      IPlainSample residual = Residual.Calculate(factor, result, modelFunction);
      m_variance = new PolylineFit(factor, residual);

      double[] varValues = new double[factor.Count];

      for (int i = 0; i < factor.Count; i++)
        varValues[i] = m_variance.Calculate(factor[i]);
      
      m_var_values = new ArrayPlainSample(varValues);
    }

    public double this[int index] => m_var_values[index];

    public IPlainSample Values { get => m_var_values; }
  }
}