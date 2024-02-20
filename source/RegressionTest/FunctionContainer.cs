using MathWorks.MATLAB.NET.Arrays;
using Schicksal.Basic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegressionTest
{
  public class FunctionContainer
  {
    readonly Func<double, IDataGroup, double> m_function;
    readonly ArrayProvider m_array;

    public FunctionContainer(Func<double, IDataGroup, double> function) 
    {
      m_function = function;
      m_array = new ArrayProvider();
    }

    public double Calculate(double[] coef, double x)
    {
      m_array.SetSource(coef);
      return m_function(x, m_array);
    }

    public double[,] Calculate(double[] coef, double[,] xs)
    {
      double[,] result = new double[xs.GetLength(0), xs.GetLength(1)];
      m_array.SetSource(coef);
      for (int i = 0; i < result.GetLength(0); i++)
        for(int j = 0; j < result.GetLength(1); j++)
          result[i, j] = m_function(xs[i, j], m_array);
      return result;
    }

    private class ArrayProvider : IDataGroup
    {
      double[] m_source;

      public ArrayProvider() { }

      public double this[int index] => m_source[index];

      public int Count => m_source.Length;

      public void SetSource(double[] newSource)
      {
        m_source = newSource;
      }

      public IEnumerator<double> GetEnumerator()
      {
        return (IEnumerator<double>)m_source.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return m_source.GetEnumerator();
      }
    }
  }
}
