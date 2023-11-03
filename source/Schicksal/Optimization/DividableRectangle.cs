using Schicksal.Basic;
using Schicksal.VectorField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Optimization
{
  public static partial class MathOptimization
  {
    private struct DividableRectangle : IComparable<DividableRectangle>
    {
      private struct Sample : IComparable<Sample>
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
      private readonly VectorDataGroup sizes;
      private readonly double diag;
      private readonly Func<VectorDataGroup, double> func;

      public int Dim { get { return sizes.Count; } }

      public double Diag { get { return diag; } }

      public double F { get { return center.y; } }

      public VectorDataGroup X { get { return center.x; } }

      public FuncPoint Center { get { return center; } }

      public override string ToString()
      {
        return String.Format("x:{0}   f:{1}     d:{3}   sizes:{2}", this.X.ToString(), NumberConvert.Do(this.F), this.sizes.ToString(), NumberConvert.Do(this.Diag));
      }

      public DividableRectangle(FuncPoint center, VectorDataGroup sizes, Func<VectorDataGroup, double> func)
      {
        this.center = center;
        this.sizes = sizes;
        this.func = func;
        diag = sizes.Length();
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

        VectorDataGroup sizes = this.SplitSize(sample.Dim);

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

      private VectorDataGroup SplitSize(int dimIndex)
      {
        double[] newSizes = sizes.ToArray();
        newSizes[dimIndex] /= DIVIDE_COUNT;
        return new VectorDataGroup(newSizes);
      }

      private VectorDataGroup UnitVector(int dimIndex)
      {
        return VectorDataGroup.Unit(this.Dim, dimIndex);
      }

      public int CompareTo(DividableRectangle other)
      {
        if (this.F < other.F) return -1;
        if (this.F > other.F) return 1;
        return 0;
      }
    }

    private class Domain
    {
      public const double UNIT_SIZE = 10E6;

      public readonly List<List<DividableRectangle>> domain;
      public FuncPoint MinPoint { get { return this.GetMinPoint(); } }
      public readonly VectorDataGroup highBound;
      public readonly VectorDataGroup lowBound;

      public Domain(Func<IDataGroup, double> optFunc, VectorDataGroup lowBound, VectorDataGroup highBound)
      {
        VectorDataGroup center = VectorDataGroup.Ones(lowBound.Count) * UNIT_SIZE / 2;
        VectorDataGroup sizes = VectorDataGroup.Ones(lowBound.Count) * UNIT_SIZE;
        this.highBound = highBound; this.lowBound = lowBound;
        Func<VectorDataGroup, double> unitOptFunc = x => optFunc(this.UnitCubeTransfer(x));
        DividableRectangle startRect = new DividableRectangle(new FuncPoint(center, unitOptFunc), sizes, unitOptFunc);
        domain = new List<List<DividableRectangle>>
          { new List<DividableRectangle> { startRect } };
      }

      public VectorDataGroup UnitCubeTransfer(VectorDataGroup x)
      {
        return (highBound - lowBound) / UNIT_SIZE * x + lowBound;
      }

      public void Distribute(DividableRectangle[] rectangles)
      {
        foreach (DividableRectangle rectangle in rectangles)
        {
          int i = 0;
          while (i < domain.Count && rectangle.Diag > domain[i][0].Diag) i++;
          if (rectangle.Diag == domain[i][0].Diag)
          {
            int j = 0;
            while (j < domain[i].Count && rectangle.F > domain[i][j].F) j++;
            domain[i].Insert(j, rectangle);
          }
          else
            domain.Insert(i, new List<DividableRectangle>(new DividableRectangle[] { rectangle }));
        }
      }

      public void Delete(DividableRectangle rectangle)
      {
        int i = 0;
        while (i < domain.Count && rectangle.Diag != domain[i][0].Diag) i++;
        domain[i].Remove(rectangle);
        if (domain[i].Count == 0) domain.RemoveAt(i);
      }

      public DividableRectangle[] GetPotentialRectangles()
      {
        List<DividableRectangle> result;
        double minF = double.MaxValue;
        int indexMin = -1;
        for (int i = 0; i < domain.Count; i++)
          if (minF >= domain[i][0].F)
          {
            minF = domain[i][0].F;
            indexMin = i;
          }
        result = new List<DividableRectangle>(domain.Count - indexMin + 1);
        for (int i = indexMin; i < domain.Count; i++)
          result.Add(domain[i][0]);

        return result.ToArray();
      }

      private FuncPoint GetMinPoint()
      {
        FuncPoint temp = domain.Select(rectList => rectList[0]).Min().Center;
        return new FuncPoint(this.UnitCubeTransfer(temp.x), temp.y);
      }
    }
  }
}
