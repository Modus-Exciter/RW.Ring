using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
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
    public void EqualSubGroups()
    {
      var arr1 = new ArrayDataGroup(new double[] { 1, 2, 3, 4, 5 });
      var arr2 = new ArrayDataGroup(new double[] { 11, 12, 13, 14, 15 });
      var arr3 = new ArrayDataGroup(new double[] { 10, 20, 30, 40, 50 });

      MultiArrayDataGroup g4 = new MultiArrayDataGroup(new IDataGroup[] { arr1, arr2, OrderedGroup.Construct(arr3) });

      var repack = GroupRepack.Wrap(g4);

      JoinedDataGroup joined = new JoinedDataGroup(repack);
      ArrayDataGroup gr_2 = new ArrayDataGroup(g4.SelectMany(g => g).ToArray());

      Assert.IsTrue(repack is IEqualSubGroups);

      Assert.AreEqual(joined.Count, gr_2.Count);

      for (int i = 0; i < joined.Count; i++)
        Assert.AreEqual(joined[i], gr_2[i]);
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
      var group4 = new ArrayDataGroup(new double[] { 12, 13, 14 });

      var mg1 = new MultiArrayDataGroup(new[] { group1, group2 });
      var mg2 = new MultiArrayDataGroup(new IDataGroup[] { group3, OrderedGroup.Construct(group4) });

      var group = new SetMultiArrayDataGroup(new IMultyDataGroup[] { mg1, mg2, mg1, mg2 });

      var repack = GroupRepack.Wrap(group);

      Assert.AreEqual(group.Count, repack.Count);

      for (int i = 0; i < group.Count; i++)
      {
        Assert.AreEqual(group[i].Count, repack[i].Count);

        for (int j = 0; j < group[i].Count; j++)
        {
          Assert.AreEqual(group[i][j].Count, repack[i][j].Count);

          for (int k = 0; k < group[i][j].Count; k++)
            Assert.AreEqual(group[i][j][k], repack[i][j][k]);
        }
      }

      var repack_enum = repack.GetEnumerator();
      foreach (var mg in group)
      {
        repack_enum.MoveNext();
        Assert.AreEqual(mg.Count, repack_enum.Current.Count);

        var mg_enum = repack_enum.Current.GetEnumerator();

        foreach (var g in mg)
        {
          mg_enum.MoveNext();
          Assert.AreEqual(g.Count, mg_enum.Current.Count);

          var group_enum = mg_enum.Current.GetEnumerator();
          foreach (var value in g)
          {
            group_enum.MoveNext();
            Assert.AreEqual(value, group_enum.Current);
          }
        }
      }
    }
  }
}
