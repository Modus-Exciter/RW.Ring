using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Schicksal.Optimization.MathOptimization;
using static Schicksal.Optimization.MathOptimization.Domain;
using Schicksal.Regression;
using System;
using System.Collections.Generic;
using Notung;

namespace DIRECT
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void DomainBasicTest()
    {
      Func<double[], double> plug = (arg) => 1.0;
      const int MAX_SIZE = 20;

      var rectangles = new Rectangle[MAX_SIZE];
      for (int i = 0; i < MAX_SIZE; i++)
        rectangles[i] = new Rectangle(1);

      var domain = new Domain(MAX_SIZE, rectangles[0]);
      var temp = new Rectangle[] { rectangles[0], rectangles[1], rectangles[2] };

      domain.Exchange(domain[0], temp);
      domain.Exchange(domain[1], temp);
      domain.Exchange(domain[2], temp);
      domain.Exchange(domain[2], temp);
      domain.Exchange(domain[2], temp);

      for (int i = 0; i < 9; i++)
        domain.Exchange(domain[3], temp);

      foreach (Node node in domain)
      {
        Console.WriteLine(node.Index);

      }

      domain.Exchange(domain[1], temp);

      foreach (Node node in domain)
      {
        Console.WriteLine(node.Index);

      }
    }

    [TestMethod]
    public void OptimalSetTest()
    {

    }
    [TestMethod]
    public void DividerTest()
    {
      int dimCount = 2;
      var provider = new RectangleProvider(dimCount, 10);
      var divider = new RectangleDivider(this.SphereFunction, provider, dimCount);
      var rect = new Rectangle(dimCount);
      rect.Set(new double[] { 0, 0 }, new double[] { 10, 10 },
        this.SphereFunction(new double[] { 0, 0 }), dimCount);
      var children = divider.Divide(rect);
      var children1 = divider.Divide(children[0]);
      var children2 = divider.Divide(children1[0]);
    }

    [TestMethod]
    public void DIRECTTest()
    {
      var optimizer = new Direct(this.BealeFunction, new double[] { -3, -3 }, new double[] { 10, 10 });
      var result = optimizer.Process();
    }

    private double SphereFunction(double[] x)
    {
      double result = 0;
      for (int i = 0; i < x.Length; i++)
        result += x[i] * x[i];
      return result;
    }

    private double BealeFunction(double[] x)
    {
      double comp1 = Math.Pow(1.5 - x[0] + x[0] * x[1], 2);
      double comp2 = Math.Pow(2.25 - x[0] + x[0] * x[1]*x[1], 2);
      double comp3 = Math.Pow(2.625 - x[0] + x[0] * x[1]*x[1]*x[1], 2);

      return comp1 + comp2 + comp3;
    }
  }

}
