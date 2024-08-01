using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schicksal.Anova;
using Schicksal.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANOVATest
{
  class Utils
  {
    public static void CheckSingleFactor(AnovaParameters pm, double f, double p)
    {
      var calc = new AnovaCalculator(pm);
      calc.Run();

      var comparator = new MultiVariantsComparator(new VariantsComparator(
        calc, FactorInfo.Parse("Factor"),
        new SampleVariance
        {
          SumOfSquares = calc.FisherTestResults[0].SSw,
          DegreesOfFreedom = (int)calc.FisherTestResults[0].Ndf
        }));

      comparator.Run();

      Assert.AreEqual(1, calc.FisherTestResults.Length);
      Assert.AreEqual(1, comparator.Results.Length);

      CheckValue(f, calc.FisherTestResults[0].F);
      CheckValue(p, calc.FisherTestResults[0].P);
      CheckValue(calc.FisherTestResults[0].P, comparator.Results[0].Probability);

      CheckDifferenceInfo(comparator.Results[0], pm.Probability);
    }

    public static void CheckMultipleFactors(AnovaParameters pm, Dictionary<FactorInfo, double> f, Dictionary<FactorInfo, double> p)
    {
      var calc = new AnovaCalculator(pm);
      calc.Run();

      Assert.AreEqual(f.Count, calc.FisherTestResults.Length);

      foreach (var res in calc.FisherTestResults)
      {
        var comparator = new MultiVariantsComparator(new VariantsComparator(
        calc, res.Factor,
        new SampleVariance
        {
          SumOfSquares = res.SSw,
          DegreesOfFreedom = (int)res.Ndf
        }));

        comparator.Run();

        if (res.Factor.Count == 1)
          Assert.AreEqual(1, comparator.Results.Length);

        CheckValue(f[res.Factor], res.F);
        CheckValue(p[res.Factor], res.P);

        if (res.Factor.Count == 1)
          CheckValue(res.P, comparator.Results[0].Probability);

        CheckDifferenceInfo(comparator.Results[0], pm.Probability);
      }
    }

    public static void CheckDifferenceInfo(DifferenceInfo diff, double p)
    {
      Assert.AreEqual(diff.Probability > p, diff.MinimalDifference > Math.Abs(diff.Mean1 - diff.Mean2));
    }

    public static void CheckValue(double expected, double actual)
    {
      if (expected == 0)
        Assert.AreEqual(0, actual);
      else if (expected > 1e-3)
        Assert.AreEqual(expected, actual, 1e-4);
      else
      {
        try
        {
          Assert.AreEqual(Math.Log(expected), Math.Log(actual), 1e-5);
        }
        catch
        {
          Assert.AreEqual(expected, actual);
        }
      }
    }
  }
}
