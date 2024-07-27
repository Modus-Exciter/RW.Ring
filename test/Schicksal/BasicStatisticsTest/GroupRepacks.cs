using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Schicksal.Basic;

namespace BasicStatisticsTest
{
  [TestClass]
  public class GroupRepacks
  {
    [TestMethod]
    public void SingleDimensions()
    {
      double[] arr1 = new double[] { 1, 2, 3, 4, 5 };
      double[] arr2 = new double[] { 1, 2, 3, 4, 5 };
      double[] arr3 = new double[] { 1, 2, 3, 4, 5 };

      MultiArrayDataGroup g4 = new MultiArrayDataGroup(new[] { arr1, arr2, arr3 });

      Assert.AreEqual(g4, GroupRepack.Wrap(g4));
    }

    [TestMethod]
    public void TwoDimensions()
    {
      double[] arr1 = new double[] { 1, 2, 3, 4, 5 };
      double[] arr2 = new double[] { 7, 8, 93, 6, 9 };
      double[] arr3 = new double[] { 1, 2, 3, 4, 5 };
      ArrayDataGroup g1 = new ArrayDataGroup(arr1);
      ArrayDataGroup g2 = new ArrayDataGroup(arr2);
      var g3 = OrderedGroup.Construct(new ArrayDataGroup(arr3));
      MultiArrayDataGroup g4 = new MultiArrayDataGroup(new IDataGroup[]  { g1, g2, g3 });

      Assert.AreNotEqual(g4, GroupRepack.Wrap(g4));
    }

    [TestMethod]
    public void ThreeDimensions()
    {
      var group1 = new ArrayDataGroup(new double[] { 9, 4, 6 });
      var group2 = new ArrayDataGroup(new double[] { 10, 5, 7 });
      var group3 = new ArrayDataGroup(new double[] { 8, 11, 15 });
      var group4 = new ArrayDataGroup(new double[] { 12, 13, 14, 13 });

      var mg1 = new MultiArrayDataGroup(new[] { group1, group2 });
      var mg2 = new MultiArrayDataGroup(new[] { group3, group4 });

      var group = new SetMultiArrayDataGroup(new IMultyDataGroup[] { mg1, mg2 }); ;
      Assert.AreEqual(group, GroupRepack.Wrap(group));
    }

  }
}
