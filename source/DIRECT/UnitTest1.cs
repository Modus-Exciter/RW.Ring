using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Schicksal.Optimization.MathOptimization;
using static Schicksal.Optimization.MathOptimization.Domain;
using Schicksal.Regression;
using System;
using System.Collections.Generic;

namespace DIRECT
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void DomainTest()
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

      for(int i = 0; i < 9; i++)
        domain.Exchange(domain[3], temp);

      foreach (Node node in domain)
        Console.WriteLine(node.Value.Count);

      domain.Exchange(domain[1], temp);

      foreach (Node node in domain)
        Console.WriteLine(node.Value.Count);
    }

    public void MainTest()
    {

    }
  }
}
