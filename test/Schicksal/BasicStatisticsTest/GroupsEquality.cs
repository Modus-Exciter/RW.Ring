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

      ArrayDataGroup g1 = new ArrayDataGroup(arr1);
      ArrayDataGroup g2 = new ArrayDataGroup(arr2);
      ArrayDataGroup g3 = new ArrayDataGroup(arr2);

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
      ArrayDataGroup g1 = new ArrayDataGroup(arr1);
      ArrayDataGroup g2 = new ArrayDataGroup(arr2);
      MultiArrayDataGroup g3 = new MultiArrayDataGroup(new IDataGroup[] { g1, g2 });
      MultiArrayDataGroup g4 = new MultiArrayDataGroup(new[] { arr1, arr2 });
      MultiArrayDataGroup g5 = new MultiArrayDataGroup(new[] { arr1, arr3 });

      Assert.AreEqual(g3, g4);
      Assert.AreEqual(g3.GetHashCode(), g4.GetHashCode());
      Assert.AreNotEqual(g4, g5);
    }

    [TestMethod]
    public void TwoDimensionGroupWithName()
    {
      double[] arr1 = new double[] { 1, 2, 3, 4, 5 };
      double[] arr2 = new double[] { 1, 2, 3, 4, 5 };
      MultiArrayDataGroup<string> g1 = new MultiArrayDataGroup<string>(new double[][] { arr1, arr2 }, new[] { "one", "two" });
      MultiArrayDataGroup<string> g2 = new MultiArrayDataGroup<string>(new double[][] { arr1, arr2 }, new[] { "one", "two" });
      MultiArrayDataGroup<string> g3 = new MultiArrayDataGroup<string>(new double[][] { arr1, arr2 }, new[] { "ONE", "TWO" });
      Assert.AreEqual(g1, g2);
      Assert.AreEqual(g1.GetHashCode(), g2.GetHashCode());
      Assert.AreNotEqual(g2, g3);
    }
  }
}
