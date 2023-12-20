using System;
using Schicksal.Basic;


namespace Schicksal.Regression
{
  public class Dispersion
  {
    private PolylineFit m_variance;
    private IDataGroup m_varValues;

    public Func<double, double> Calculate { get => m_variance.Calculate; }

    public Dispersion(IDataGroup factor, IDataGroup result, Func<double, double> modelFunction)
    {
      IDataGroup residual = Residual.Calculate(factor, result, modelFunction);
      m_variance = new PolylineFit(factor, residual);
      double[] varValues = new double[factor.Count];
      for (int i = 0; i < factor.Count; i++)
        varValues[i] = m_variance.Calculate(factor[i]);
      m_varValues = new ArrayDataGroup(varValues);
    }

    public double this[int index]
    {
      get
      {
        return m_varValues[index];
      }
    }
  }
}
