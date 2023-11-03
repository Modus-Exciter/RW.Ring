using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Optimization
{
  public class OptimizationOptions
  {
    public static readonly OptimizationOptions Default = new OptimizationOptions();

    public readonly double m_tolX;
    public readonly double m_tolY;
    public readonly int m_maxIter;
    public readonly int m_maxFunEval;
    public OptimizationOptions(double tolX = 1E-12, double tolY = 1E-12, int maxIter = 10000, int maxFunEval = 10000)
    {
      this.m_tolX = tolX;
      this.m_tolY = tolY;
      this.m_maxIter = maxIter;
      this.m_maxFunEval = maxFunEval;
    }
  }
}
