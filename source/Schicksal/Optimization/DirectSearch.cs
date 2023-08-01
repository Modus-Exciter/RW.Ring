using Schicksal.Basic;
using Schicksal.VectorField;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Schicksal.Optimization
{
  public static partial class MathOptimization
  {
    private class DividableRectangle
    {
      private class Sample : IComparable<Sample>
      {
        private readonly FuncPoint left;
        private readonly FuncPoint right;
        private readonly int dimIndex;

        public Sample(FuncPoint left, FuncPoint right, int dimIndex)
        {
          this.left = left;
          this.right = right;
          this.dimIndex = dimIndex;
        }

        public int Dim
        {
          get { return dimIndex; }
        }

        public FuncPoint Left
        {
          get { return left; }
        }

        public FuncPoint Right
        {
          get { return right; }
        }

        public FuncPoint Min
        {
          get { return left.y < right.y ? left : right; }
        }

        public int CompareTo(Sample point)
        {
          return this.Min.CompareTo(point.Min);
        }
      }

      const int DIVIDE_COUNT = 3;

      private readonly FuncPoint center;
      private readonly IDataGroup sizes;
      private readonly double diag;
      private readonly Func<VectorDataGroup, double> func;

      public int Dim { get { return sizes.Dim; } }

      public double Diag { get { return diag; } }

      public double F { get { return center.y; } }

      public VectorDataGroup X { get { return center.x; } }

      public override string ToString()
      {
        return String.Format("x:{0}   f:{1}     d:{3}   sizes:{2}", this.X.ToString(), this.F.ToString(), this.sizes.ToString(), this.Diag.ToString());
      }

      public DividableRectangle(FuncPoint center, IDataGroup sizes, Func<VectorDataGroup, double> func)
      {
        this.center = center;
        this.sizes = sizes;
        this.func = func;

        diag = 0;
        sizes.Select(size => diag + size*size);
        diag = Math.Sqrt(diag);
      }

      public DividableRectangle[] Divide()
      {
        Sample[] samples = this.Sampling(this.GetSplitDimensions());
        DividableRectangle[] rectangles = new DividableRectangle[2 * samples.Length + 1];
        DividableRectangle centerRect = this;

        Array.Sort(samples);
        for (int i = 0; i < samples.Length; i++)
        {
          DividableRectangle[] temp = centerRect.Split(samples[i]);
          rectangles[2 * i] = temp[0];
          rectangles[2 * i + 1] = temp[1];
          centerRect = temp[2];
        }
        rectangles[rectangles.Length - 1] = centerRect;
      
        return rectangles;
      }

      private DividableRectangle[] Split(Sample sample)
      {
        DividableRectangle[] result = new DividableRectangle[DIVIDE_COUNT];

        IDataGroup sizes = this.SplitSize(sample.Dim);

        result[0] = new DividableRectangle(sample.Left, sizes, func);
        result[1] = new DividableRectangle(sample.Right, sizes, func);
        result[2] = new DividableRectangle(center, sizes, func);

        return result;
      }

      private Sample[] Sampling(int[] dimIndex)
      {
        Sample[] samples = new Sample[dimIndex.Length];
        FuncPoint left, right;
        double delta;

        for (int i = 0; i < dimIndex.Length; i++)
        {
          delta = sizes[dimIndex[i]] / DIVIDE_COUNT;
          left = new FuncPoint(center.x - this.UnitVector(dimIndex[i]) * delta, func);
          right = new FuncPoint(center.x + this.UnitVector(dimIndex[i]) * delta, func);
          samples[i] = new Sample(left, right, dimIndex[i]);
        }
       
        return samples;
      }

      private int[] GetSplitDimensions()
      {
        List<int> res = new List<int>();
        double max = sizes.Max();

        for (int i = 0; i < this.Dim; i++)
        {
          if (sizes[i] == max)
            res.Add(i); 
        }

        return res.ToArray();
      }

      private IDataGroup SplitSize(int dimIndex)
      {
        double[] newSizes = sizes.ToArray();
        newSizes[dimIndex] /= DIVIDE_COUNT;
        return new ArrayDataGroup(newSizes);
      }

      private VectorDataGroup UnitVector(int dimIndex)
      {
        return VectorDataGroup.Unit(this.Dim, dimIndex);
      }
    }

    private static List<DividableRectangle> DIRECTInitizalization(Func<VectorDataGroup, double> optFunc, VectorDataGroup lowBoundary, VectorDataGroup highBoundary)
    {
      VectorDataGroup center = (lowBoundary + highBoundary) / 2;
      VectorDataGroup sizes = (highBoundary - lowBoundary).Abs();
      DividableRectangle startRect = new DividableRectangle(new FuncPoint(center, optFunc), sizes, optFunc);
      List<DividableRectangle> domain = new List<DividableRectangle>();
      domain.Add(startRect);
      return domain;
    }
    
    public static VectorDataGroup DIRECTSearch(Func<VectorDataGroup, double> optFunc, VectorDataGroup lowBoundary, VectorDataGroup highBoundary, OptimizationOptions options = null)
    {
      options = options ?? OptimizationOptions.Default;
      List<DividableRectangle> domain = DIRECTInitizalization(optFunc, lowBoundary, highBoundary);

      domain.AddRange(domain[0].Divide());

      return domain[0].X;
    }
    
  }
}
