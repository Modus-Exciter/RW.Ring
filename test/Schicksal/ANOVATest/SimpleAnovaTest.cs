using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schicksal.Anova;
using Schicksal.Basic;
using System;
using System.Data;

namespace ANOVATest
{
  [TestClass]
  public class SimpleAnovaTest
  {
    [TestMethod]
    public void TestGeneration()
    {
      var table = GenerateTable();

      Assert.AreEqual(10, table.Rows.Count);

      Assert.AreEqual(table.Rows[0]["Factor"], "a");
      Assert.AreEqual(table.Rows[1]["Factor"], "a");
      Assert.AreEqual(table.Rows[2]["Factor"], "a");
      Assert.AreEqual(table.Rows[3]["Factor"], "a");
      Assert.AreEqual(table.Rows[4]["Factor"], "a");
      Assert.AreEqual(table.Rows[5]["Factor"], "b");
      Assert.AreEqual(table.Rows[6]["Factor"], "b");
      Assert.AreEqual(table.Rows[7]["Factor"], "b");
      Assert.AreEqual(table.Rows[8]["Factor"], "b");
      Assert.AreEqual(table.Rows[9]["Factor"], "b");

      Assert.AreEqual(table.Rows[0]["Repeat"], 1);
      Assert.AreEqual(table.Rows[1]["Repeat"], 2);
      Assert.AreEqual(table.Rows[2]["Repeat"], 3);
      Assert.AreEqual(table.Rows[3]["Repeat"], 4);
      Assert.AreEqual(table.Rows[4]["Repeat"], 5);
      Assert.AreEqual(table.Rows[5]["Repeat"], 1);
      Assert.AreEqual(table.Rows[6]["Repeat"], 2);
      Assert.AreEqual(table.Rows[7]["Repeat"], 3);
      Assert.AreEqual(table.Rows[8]["Repeat"], 4);
      Assert.AreEqual(table.Rows[9]["Repeat"], 5);


      Assert.AreEqual((double)table.Rows[0]["Response"], 35.0, 1e-5);
      Assert.AreEqual((double)table.Rows[1]["Response"], 44.0, 1e-5);
      Assert.AreEqual((double)table.Rows[2]["Response"], 37.0, 1e-5);
      Assert.AreEqual((double)table.Rows[3]["Response"], 31.0, 1e-5);
      Assert.AreEqual((double)table.Rows[4]["Response"], 38.0, 1e-5);
      Assert.AreEqual((double)table.Rows[5]["Response"], 32.0, 1e-5);
      Assert.AreEqual((double)table.Rows[6]["Response"], 31.0, 1e-5);
      Assert.AreEqual((double)table.Rows[7]["Response"], 35.0, 1e-5);
      Assert.AreEqual((double)table.Rows[8]["Response"], 30.0, 1e-5);
      Assert.AreEqual((double)table.Rows[9]["Response"], 28.0, 1e-5);
    }

    [TestMethod]
    public void NoNormalizationIndependentCommon()
    {
      CheckSingleFactor(new AnovaParameters
      (
        GenerateTable(),
        null,
        FactorInfo.Parse("Factor"),
        "Response",
        0.05f,
        DummyNormalizer.Instance,
        null,
        false
      ), 5.7603, 0.0431728822081336);
    }

    [TestMethod]
    public void NoNormalizationConjugationCommon()
    {
      CheckSingleFactor(new AnovaParameters
      (
        GenerateTable(),
        null,
        FactorInfo.Parse("Factor"),
        "Response",
        0.05f,
        DummyNormalizer.Instance,
        "Repeat",
        false
      ), 5.8606, 0.072694376);
    }

    [TestMethod]
    public void NoNormalizationIndependentIndividual ()
    {
      CheckSingleFactor(new AnovaParameters
      (
        GenerateTable(),
        null,
        FactorInfo.Parse("Factor"),
        "Response",
        0.05f,
        DummyNormalizer.Instance,
        null,
        true
      ), 5.7603, 0.0431728822081336);
    }

    [TestMethod]
    public void NoNormalizationConjugationIndividual()
    {
      CheckSingleFactor(new AnovaParameters
      (
        GenerateTable(),
        null,
        FactorInfo.Parse("Factor"),
        "Response",
        0.05f,
        DummyNormalizer.Instance,
        "Repeat",
        true
      ), 5.8606, 0.072694376);
    }

    private static void CheckSingleFactor(AnovaParameters pm, double f, double p)
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

    private static void CheckDifferenceInfo(DifferenceInfo diff, double p)
    {
      Assert.AreEqual(diff.Probability > p, diff.MinimalDifference > Math.Abs(diff.Mean1 - diff.Mean2));
    }

    private static void CheckValue(double expected, double actual)
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

    private static DataTable GenerateTable()
    {
      DataTable dt = new DataTable();

      dt.Columns.Add("Factor", typeof(string));
      dt.Columns.Add("Response", typeof(double));
      dt.Columns.Add("Repeat", typeof(int));

      dt.BeginLoadData();

      dt.Rows.Add("a", 35, 1);
      dt.Rows.Add("a", 44, 2);
      dt.Rows.Add("a", 37, 3);
      dt.Rows.Add("a", 31, 4);
      dt.Rows.Add("a", 38, 5);
      dt.Rows.Add("b", 32, 1);
      dt.Rows.Add("b", 31, 2);
      dt.Rows.Add("b", 35, 3);
      dt.Rows.Add("b", 30, 4);
      dt.Rows.Add("b", 28, 5);

      dt.EndInit();

      dt.AcceptChanges();

      return dt;
    }
  }
}
