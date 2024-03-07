using Notung.Data;
using Schicksal.Basic;
using Schicksal.VectorField;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Xml.Schema;

namespace Schicksal.Optimization
{
  public static partial class MathOptimization
  {
    public class RectangleDivider
    {
      const int SPLIT_COUNT = 3;
      private readonly RectangleProvider m_provider;
      private readonly Func<double[], double> m_function;
      private readonly SampleComparer m_comparer;

      private readonly double[] m_x;
      private readonly Sample[] m_samples;
      private readonly double[] m_size;
      private int m_dimension_count;

      public RectangleDivider(Func<double[], double> function, RectangleProvider provider, int dimensionCount)
      {
        m_function = function;
        m_provider = provider;
        m_comparer = new SampleComparer();

        m_x = new double[dimensionCount];
        m_size = new double[dimensionCount];
        m_dimension_count = dimensionCount;
        m_samples = new Sample[dimensionCount];
        for (int i = 0; i < m_samples.Length; i++)
        {
          m_samples[i] = new Sample(dimensionCount);
        }
      }

      public Rectangle[] Divide(Rectangle rect)
      {
        Array.Copy(rect.X, m_x, m_dimension_count);
        Array.Copy(rect.Size, m_size, m_dimension_count);

        (double newSize, int count) = this.GetMaxDimensions();
        newSize /= SPLIT_COUNT;
        this.Sampling(newSize, count);
        Array.Sort<Sample>(m_samples, 0, count, m_comparer);

        Rectangle[] result = m_provider.GetInstances((SPLIT_COUNT - 1) * count + 1);
        int resIndex = 0;
        for (int i = 0; i < count; i++)
        {

          m_size[m_samples[i].index] = newSize;
          result[resIndex].Set(m_samples[i].left.x, m_size, m_samples[i].left.f, m_dimension_count);
          resIndex++;

          result[resIndex].Set(m_samples[i].right.x, m_size, m_samples[i].right.f, m_dimension_count);
          resIndex++;
        }

        result[(SPLIT_COUNT - 1) * count].Set(m_x, m_size, rect.F, m_dimension_count);
        return result;
      }

      private (double maxSize, int count) GetMaxDimensions()
      {
        int count = 0;
        double maxSize = double.MinValue;
        
        for (int i = 0; i < m_dimension_count; i++)
          if (m_size[i] > maxSize)
          {
            maxSize = m_size[i];
            m_samples[0].index = i;
            count = 1;
          }
          else if (m_size[i] == maxSize)
          {
            m_samples[count].index = i;
            count++;
          }

        return (maxSize, count);
      }

      private void Sampling(double delta, int count)
      {
        for (int i = 0; i < count; i++)
        {
          int index = m_samples[i].index;

          Array.Copy(m_x, m_samples[i].left.x, m_dimension_count);
          m_samples[i].left.x[index] = m_x[index] - delta;
          m_samples[i].left.f = m_function(m_samples[i].left.x);

          Array.Copy(m_x, m_samples[i].right.x, m_dimension_count);
          m_samples[i].right.x[index] = m_x[index] + delta;
          m_samples[i].right.f = m_function(m_samples[i].right.x);
        }
      }

      struct Sample
      {
        public (double[] x, double f) left;
        public (double[] x, double f) right;
        public int index;

        public Sample(int dimensionCount)
        {
          left.f = 0;
          right.f = 0;
          index = 0;
          left.x = new double[dimensionCount];
          right.x = new double[dimensionCount];
        }
      }

      private class SampleComparer : IComparer<Sample>
      {
        double m_x_min;
        double m_y_min;

        public SampleComparer() { }

        public int Compare(Sample x, Sample y)
        {
          m_x_min = x.left.f < x.right.f ? x.left.f : x.right.f;
          m_y_min = y.left.f < y.right.f ? y.left.f : y.right.f;
          if (m_x_min < m_y_min) return -1;
          if (m_x_min > m_y_min) return 1;
          return 0;
        }
      }
    }

    [DebuggerDisplay("{ToString()}")]
    public class Rectangle
    {
      protected readonly double[] m_x;
      protected readonly double[] m_size;
      protected double m_f;

      public Rectangle(int dimensionCount)
      {
        m_x = new double[dimensionCount];
        m_size = new double[dimensionCount];
        m_f = 0;
      }

      public double F { get { return m_f; } }
      
      public double[] X { get { return m_x; } }

      public double[] Size { get { return m_size; } }

      public double Diag { get { return Math.Sqrt(m_size.Select(s => s*s).Sum()); } }
#if DEBUG
      public override string ToString()
      {
        string xStr = this.X.Select(xi => xi.ToString("0.0")).
          Aggregate((x1, x2) => x1 + " "+ x2 + " ");
        string sStr = this.Size.Select(xi => xi.ToString("0.0")).
          Aggregate((x1, x2) => x1 + " " + x2 + " ");
        return String.Format("F:{0}; X:{1}; S:{2}; D:{3};", this.F.ToString("0.0"), xStr, sStr, this.Diag.ToString("0.0") );
      }
#endif
      /// <summary>
      /// Осуществляет полное изменение состояния объекта. Все массивы копируются.
      /// </summary>
      /// <param name="x">Центр прямоугольника</param>
      /// <param name="sizes">Размеры прямоугольника</param>
      /// <param name="f">Значение функции в центре прямоугольника</param>
      /// <param name="dimensionCount">Размерность прямоугольника</param>
      public void Set(double[] x, double[] sizes, double f, int dimensionCount)
      {
        Array.Copy(x, m_x, dimensionCount);
        Array.Copy(sizes, m_size, dimensionCount);
        m_f = f;
      }
    }

    public class RectangleProvider
    {
      private readonly Queue<Rectangle> m_free;

      readonly int m_dimension_count;

      public RectangleProvider(int dimensionCount, int capacity) 
      {
        m_dimension_count = dimensionCount;
        m_free = new Queue<Rectangle>(capacity);
        for (int i = 0; i < capacity; i++)
          m_free.Enqueue(new Rectangle(m_dimension_count));
      }

      public Rectangle GetInstance()
      {
        if (m_free.Count == 0)
        {
          int count = m_free.Count;
          for (int i = 0; i < count; i++)
            m_free.Enqueue(new Rectangle(m_dimension_count));
        }
        return m_free.Dequeue();
      }

      public Rectangle[] GetInstances(int amount)
      {
        Rectangle[] result = new Rectangle[amount];
        int count = m_free.Count;
        if(m_free.Count - amount <= 0)
        {
          count = count > amount ? count : amount;
          for (int i = 0; i < count; i++)
            m_free.Enqueue(new Rectangle(m_dimension_count));
        }
        for (int i = 0; i < amount; i++)
          result[i] = m_free.Dequeue();
        return result;
      }

      public void ReturnInstance(Rectangle rectangle)
      {
        m_free.Enqueue(rectangle);
      }
    }
  }
}
