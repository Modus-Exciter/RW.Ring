using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schicksal.Basic;
using System;

namespace BasicStatisticsTest
{
  [TestClass]
  public class GroupsEquality
  {
    [TestMethod]
    public void OneDimensionGroup()
    {
      double[] arr1 = new double[] { 1, 2, 3, 4, 5 };
      double[] arr2 = new double[] { 1, 2, 3, 4, 5 };

      ArrayPlainSample g1 = new ArrayPlainSample(arr1);
      ArrayPlainSample g2 = new ArrayPlainSample(arr2);
      ArrayPlainSample g3 = new ArrayPlainSample(arr2);

      Assert.AreNotEqual(g1, g2);
      Assert.AreEqual(g2, g3);
      Assert.AreEqual(g2.GetHashCode(), g3.GetHashCode());
    }

    [TestMethod]
    public void TwoDimensionGroup()
    {
      double[] arr1 = new double[] { 1, 2, 3, 4, 5 };
      double[] arr2 = new double[] { 1, 2, 3, 4, 5 };
      double[] arr3 = new double[] { 1, 2, 3, 4, 5 };
      ArrayPlainSample g1 = new ArrayPlainSample(arr1);
      ArrayPlainSample g2 = new ArrayPlainSample(arr2);
      ArrayDividedSample g3 = new ArrayDividedSample(new IPlainSample[] { g1, g2 });
      ArrayDividedSample g4 = new ArrayDividedSample(new[] { arr1, arr2 });
      ArrayDividedSample g5 = new ArrayDividedSample(new[] { arr1, arr3 });

      Assert.AreEqual(g3, g4);
      Assert.AreEqual(g3.GetHashCode(), g4.GetHashCode());
      Assert.AreNotEqual(g4, g5);
    }

    [TestMethod]
    public void TwoDimensionGroupWithName()
    {
      double[] arr1 = new double[] { 1, 2, 3, 4, 5 };
      double[] arr2 = new double[] { 1, 2, 3, 4, 5 };
      ArrayDividedSample<string> g1 = new ArrayDividedSample<string>(new double[][] { arr1, arr2 }, new[] { "one", "two" });
      ArrayDividedSample<string> g2 = new ArrayDividedSample<string>(new double[][] { arr1, arr2 }, new[] { "one", "two" });
      ArrayDividedSample<string> g3 = new ArrayDividedSample<string>(new double[][] { arr1, arr2 }, new[] { "ONE", "TWO" });
      Assert.AreEqual(g1, g2);
      Assert.AreEqual(g1.GetHashCode(), g2.GetHashCode());
      Assert.AreNotEqual(g2, g3);
    }
  }
}
