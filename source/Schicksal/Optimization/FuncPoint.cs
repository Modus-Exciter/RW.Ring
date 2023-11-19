using Schicksal.VectorField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Optimization
{
  public static partial class MathOptimization
  {
    /// <summary>
    /// Точка, расчитывающаяся самостаятельно с помощью функции
    /// </summary>
    private struct FuncPoint : IComparable<FuncPoint>
    {
      /// <summary>
      /// Аргумент функции
      /// </summary>
      public readonly VectorDataGroup x;
      
      /// <summary>
      /// Значение функции
      /// </summary>
      public readonly double y;
      /// <summary>
      /// Конструктор с уже расчитанной точкой
      /// </summary>
      /// <param name="x">Аргумент</param>
      /// <param name="y">Значение функции</param>
      /// <exception cref="NotFiniteNumberException">Значение функции выходит за границы определения double</exception>
      public FuncPoint(VectorDataGroup x, double y)
      {
        if (double.IsNaN(y)) throw new NotFiniteNumberException();
        this.x = x;
        this.y = y;
      }
      /// <summary>
      /// Конструктор с расчитываемой функцией
      /// </summary>
      /// <param name="x">Аргумент</param>
      /// <param name="function">Расчитываемая функция</param>
      /// <exception cref="NotFiniteNumberException"></exception>
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
