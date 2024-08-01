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

      ArrayDividedSample g4 = new ArrayDividedSample(new[] { arr1, arr2, arr3 });

      Assert.AreEqual(g4, SampleRepack.Wrap(g4));
    }

    [TestMethod]
    public void EqualSubGroups()
    {
      var arr1 = new ArrayPlainSample(new double[] { 1, 2, 3, 4, 5 });
      var arr2 = new ArrayPlainSample(new double[] { 11, 12, 13, 14, 15 });
      var arr3 = new ArrayPlainSample(new double[] { 10, 20, 30, 40, 50 });

      ArrayDividedSample g4 = new ArrayDividedSample(new IPlainSample[] { arr1, arr2, OrderedSample.Construct(arr3) });

      var repack = SampleRepack.Wrap(g4);

      JoinedSample joined = new JoinedSample(repack);
      ArrayPlainSample gr_2 = new ArrayPlainSample(g4.SelectMany(g => g).ToArray());

      Assert.IsTrue(repack is IEqualSubSamples);

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
      ArrayPlainSample g1 = new ArrayPlainSample(arr1);
      ArrayPlainSample g2 = new ArrayPlainSample(arr2);
      var g3 = OrderedSample.Construct(new ArrayPlainSample(arr3));
      ArrayDividedSample g4 = new ArrayDividedSample(new IPlainSample[]  { g1, g2, g3 });

      Assert.AreNotEqual(g4, SampleRepack.Wrap(g4));
    }

    [TestMethod]
    public void ThreeDimensions()
    {
      var group1 = new ArrayPlainSample(new double[] { 9, 4, 6 });
      var group2 = new ArrayPlainSample(new double[] { 10, 5, 7 });
      var group3 = new ArrayPlainSample(new double[] { 8, 11, 15 });
      var group4 = new ArrayPlainSample(new double[] { 12, 13, 14 });

      var mg1 = new ArrayDividedSample(new[] { group1, group2 });
      var mg2 = new ArrayDividedSample(new IPlainSample[] { group3, OrderedSample.Construct(group4) });

      var group = new ArrayComplexSample(new IDividedSample[] { mg1, mg2, mg1, mg2 });

      var repack = SampleRepack.Wrap(group);

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
