using Notung.Logging;
using Schicksal.Basic;
using Schicksal.VectorField;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using static Schicksal.Optimization.MathOptimization;

namespace Schicksal.Optimization
{
  public static partial class MathOptimization
  {
    public static VectorDataGroup DIRECTSearch(Func<VectorDataGroup, double> optFunc, VectorDataGroup lowBoundary, VectorDataGroup highBoundary, OptimizationOptions options = null)
    {
      options = options ?? OptimizationOptions.Default;
      int iterCount = 0;
      int dim = lowBoundary.Count;
      double deltaY = double.MaxValue;
      double deltaX = double.MaxValue;
      Domain domain = new Domain(optFunc, lowBoundary, highBoundary);
      FuncPoint minPoint = domain.MinPoint; 
      FuncPoint newMinPoint = null;

      while (( (deltaY > options.m_tolY && deltaX > options.m_tolX) || deltaX == 0) && iterCount < options.m_maxIter)
      {
        DividableRectangle[] potentialRect = domain.GetPotentialRectangles();
        foreach (DividableRectangle rectangle in potentialRect)
        {
          domain.Distribute(rectangle.Divide());
          domain.Delete(rectangle);
        }
        newMinPoint = domain.MinPoint;
        deltaX = Math.Abs((newMinPoint.x - minPoint.x).Length()/dim);
        deltaY = Math.Abs((newMinPoint.y - minPoint.y)/newMinPoint.y);
        minPoint = newMinPoint;
        iterCount++;
      }

      return newMinPoint.x;
    }
  }
}

