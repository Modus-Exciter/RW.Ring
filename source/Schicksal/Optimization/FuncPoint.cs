using Schicksal.VectorField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Optimization
{
  public static partial class MathOptimization
  {
    private class FuncPoint : IComparable<FuncPoint>
    {
      public readonly VectorDataGroup x;
      public readonly double y;

      public FuncPoint(VectorDataGroup x, double y)
      {
        if (double.IsNaN(y)) throw new NotFiniteNumberException();
        this.x = x;
        this.y = y;
      }

      public FuncPoint(VectorDataGroup x, Func<VectorDataGroup, double> function)
      {
        this.x = x;
        this.y = function(x);
        if (double.IsNaN(this.y)) throw new NotFiniteNumberException("Result is NaN");
      }

      public int CompareTo(FuncPoint a)
      {
        if (this.y < a.y) return -1;
        if (this.y > a.y) return 1;
        return 0;
      }
    }
  }
}
