using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schicksal.Basic;
using Schicksal.Regression;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DIRECT
{
  [TestClass]
  public class WilliamsKlootTest
  {
    [TestMethod]
    public void CompareDependencies_ReturnExpectedResult()
    {
      var x = new double[] { 1, 2, 3, 4, 5 };
      var yLinear = new double[] { 2, 4, 6, 8, 10 };
      var yParabola = new double[] { 1, 4, 9, 16, 25 };

      IPlainSample sampleX = new ArrayPlainSample(x);
      IPlainSample sampleYLinear = new ArrayPlainSample(yLinear); 
      IPlainSample sampleYParabola = new ArrayPlainSample(yParabola);

      var linear = new LinearDependency(sampleX, sampleYLinear);
      var parabolic = new ParabolicDependency(sampleX, sampleYParabola);

      var sourcePoints = new Point2D[x.Length];

      for (int i = 0; i < x.Length; i++)
      {
        var point = (Point2D)Activator.CreateInstance(typeof(Point2D), true);

        var xProp = typeof(Point2D).GetProperty("X");
        xProp.SetValue(point, x[i]);

        var yProp = typeof(Point2D).GetProperty("Y");
        yProp.SetValue(point, yLinear[i]);

        sourcePoints[i] = point;
      }


      var formula = (CorrelationFormula)Activator.CreateInstance(typeof(CorrelationFormula), true);
      typeof(CorrelationFormula).GetProperty("SourcePoints").SetValue(formula, sourcePoints);
      typeof(CorrelationFormula).GetProperty("Dependencies").SetValue(formula, new RegressionDependency[] { linear, parabolic});

      var results = WilliamsKlootComparer.CompareDependencies(formula);

      // assert
      Assert.AreEqual(1, results.Count);
      var result = results[0];
      Assert.IsTrue(result.IsSignificant);
      Assert.AreEqual(linear, result.DependencyA);  // Порядок
      Assert.AreEqual(parabolic, result.DependencyB);

      Console.WriteLine($"T = {result.TStatistic:0.000}, P = {result.PValue:0.000}, Better = {result.BetterModel.GetType().Name}");
    }
  }

}

