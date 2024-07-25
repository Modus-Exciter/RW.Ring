using Schicksal.Basic;
using Schicksal.VectorField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Schicksal.Optimization
{
  public static partial class MathOptimization
  {
    public class Direct
    {
      const double UNIT_SIZE = 1E10;
      const double START_X = UNIT_SIZE / 2;

      readonly int m_dimension_count;
      readonly double[] m_transform_coeff;

      readonly RectangleProvider m_provider;
      readonly RectangleDivider m_divider;
      readonly Domain m_domain;
      readonly OptimalSet m_optimal_set;
      
      readonly OptimizationOptions m_options;
      
      readonly Func<double[], double> m_function;
      readonly double[] m_low_bound;
      readonly double[] m_high_bound;
      readonly double m_tolerance;

      double[] m_function_buffer;

      public Direct(Func<double[], double> function, double[] lowBound, double[] highBound, OptimizationOptions options = null)
      {
        if (lowBound.Length != highBound.Length)
          throw new ArgumentException();
        if (options == null)
          m_options = OptimizationOptions.Default;

        m_dimension_count = lowBound.Length;
        m_function = function;
        m_low_bound = lowBound;
        m_high_bound = highBound;
        m_transform_coeff = this.TransformCoefficient();
        m_tolerance = this.Tolerance();
        m_function_buffer = new double[m_dimension_count];

        m_provider = new RectangleProvider(m_dimension_count, this.StartPoolSize());
        m_divider = new RectangleDivider(this.Function, m_provider, m_dimension_count);
        m_domain = new Domain(this.DomainSize() + m_dimension_count, this.InitialRectangle());
        m_optimal_set = new OptimalSet(m_domain, this.DomainSize(), m_options.m_tolY);
      }

      public double[] Process()
      {
        int i = 0;
        Rectangle[] children;
        while (i < m_options.m_maxIter && m_domain.Min.Value.Peek().Diag > m_tolerance)
        {
          foreach (Domain.Node node in m_optimal_set)
          {
            children = m_divider.Divide(node.Value.Peek());
            m_provider.ReturnInstance(m_domain.Exchange(node, children));
          }
          i++;
        }

        return this.Transform(m_domain.Min.Value.Peek().X);
      }

      //TODO: Find out actual formula for starting pool size!
      private int StartPoolSize()
      {
        return 40000;
      }

      private int DomainSize()
      {
        double num = Math.Log(Math.Sqrt(m_dimension_count) * UNIT_SIZE / m_tolerance);
        double denum = Math.Log(3);
        int result = (int)Math.Floor(m_dimension_count * (num / denum));
        return result;
      }

      private double[] TransformCoefficient()
      {
        double[] coeff = new double[m_dimension_count];
        for (int i = 0; i < m_dimension_count; i++)
          coeff[i] = (m_high_bound[i] - m_low_bound[i]) / UNIT_SIZE;
        return coeff;
      }

      private double[] Transform(double[] x)
      {
        double[] result = new double[x.Length];
        for (int i = 0; i < m_dimension_count; i++)
          result[i] = m_transform_coeff[i] * x[i] + m_low_bound[i];
        return result;
      }

      private double Tolerance()
      {
        double num = 0;
        double denum = 0; 
        for(int i = 0; i < m_dimension_count; i++)
        {
          num += UNIT_SIZE * UNIT_SIZE;
          denum += (m_high_bound[i] - m_low_bound[i]) * (m_high_bound[i] - m_low_bound[i]);
        }
        return Math.Sqrt(num / denum) * m_options.m_tolX;
      }

      private Rectangle InitialRectangle()
      {
        Rectangle rect = m_provider.GetInstance();
        double[] x = new double[m_dimension_count];
        double[] sizes = new double[m_dimension_count];
        for (int i = 0; i < m_dimension_count; i++)
        {
          x[i] = START_X;
          sizes[i] = UNIT_SIZE;
        }
        rect.Set(x, sizes, this.Function(x), m_dimension_count);
        return rect;
      }

      private double Function(double[] x)
      {
        for (int i = 0; i < m_dimension_count; i++)
          m_function_buffer[i] = m_transform_coeff[i] * x[i] + m_low_bound[i];
        return m_function(m_function_buffer);
      }
    }
  }

}

