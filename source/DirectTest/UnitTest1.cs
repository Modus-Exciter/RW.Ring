using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Schicksal.Optimization;
using System.Linq;
using Schicksal.Basic;
using Schicksal.VectorField;

namespace DirectTest
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void TestMethod1()
    {
      VectorDataGroup low = new VectorDataGroup(new double[] { -3, -3 });
      VectorDataGroup high = new VectorDataGroup(new double[] { 10, 10 });
      MathOptimization.DIRECTSearch(this.SphereFunction, low, high);
    }

    private double SphereFunction(IDataGroup x)
    {
      return x.Aggregate((x1, x2) => x1*x1 + x2*x2);
    }
  }
}
