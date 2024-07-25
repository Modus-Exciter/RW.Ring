using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schicksal.Basic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BasicStatisticsTest
{
  /// <summary>
  /// Сводное описание для NormalizationTest
  /// </summary>
  [TestClass]
  public class NormalizationTest
  {
    public NormalizationTest()
    {
      //
      // TODO: добавьте здесь логику конструктора
      //
    }

    private TestContext testContextInstance;

    /// <summary>
    ///Получает или устанавливает контекст теста, в котором предоставляются
    ///сведения о текущем тестовом запуске и обеспечивается его функциональность.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }

    #region Дополнительные атрибуты тестирования
    //
    // При написании тестов можно использовать следующие дополнительные атрибуты:
    //
    // ClassInitialize используется для выполнения кода до запуска первого теста в классе
    // [ClassInitialize()]
    // public static void MyClassInitialize(TestContext testContext) { }
    //
    // ClassCleanup используется для выполнения кода после завершения работы всех тестов в классе
    // [ClassCleanup()]
    // public static void MyClassCleanup() { }
    //
    // TestInitialize используется для выполнения кода перед запуском каждого теста 
    // [TestInitialize()]
    // public void MyTestInitialize() { }
    //
    // TestCleanup используется для выполнения кода после завершения каждого теста
    // [TestCleanup()]
    // public void MyTestCleanup() { }
    //
    #endregion

    [TestMethod]
    public void OneDimensionGroup()
    {
      var group = new ArrayDataGroup(new double[] { 2, 7, 9.2, 7, 4, 4, 4 });
      var ranked = GroupNormalizer.NormalizeByRanks(group, 2);

      Assert.AreEqual(7, ranked.Count);
      Assert.AreEqual(1, ranked[0], 0.01);
      Assert.AreEqual(5.5, ranked[1], 0.01);
      Assert.AreEqual(7, ranked[2], 0.01);
      Assert.AreEqual(5.5, ranked[3], 0.01);
      Assert.AreEqual(3, ranked[4], 0.01);
      Assert.AreEqual(3, ranked[5], 0.01);
      Assert.AreEqual(3, ranked[6], 0.01);

      var ranked2 = GroupNormalizer.NormalizeByRanks(ranked, 2);

      Assert.AreSame(ranked, ranked2);

      var ranked3 = GroupNormalizer.NormalizeByRanks(ranked);

      Assert.AreNotSame(ranked, ranked3);

      var inverse = GroupNormalizer.CreateInverseHandler(ranked);

      Assert.AreEqual(9.2, inverse(7));
      Assert.AreEqual(4, inverse(3));
      Assert.AreEqual(11.257, inverse(9), 0.01);
      Assert.AreEqual(12.3, inverse(10), 0.1);
      Assert.AreEqual(5, inverse(4), 0.5);
      Assert.AreEqual(0.95, inverse(0), 0.1);
    }

    [TestMethod]
    public void TwoDimensionsGroup()
    {
      var group1 = new ArrayDataGroup(new double[] { 2, 4, 6 });
      var group2 = new ArrayDataGroup(new double[] { 4, 5, 7});
      var group3 = new ArrayDataGroup(new double[] { 6, 7, 7, 8 });

      var group = new MultiArrayDataGroup(new IDataGroup[] { group1, group2, group3 });

      var ranked = GroupNormalizer.NormalizeByRanks(group);

      Assert.AreEqual(3, ranked.Count);
      Assert.AreEqual(3, ranked[0].Count);
      Assert.AreEqual(3, ranked[1].Count);
      Assert.AreEqual(4, ranked[2].Count);

      Assert.AreEqual(1, ranked[0][0], 0.01);
      Assert.AreEqual(2.5, ranked[0][1], 0.01);
      Assert.AreEqual(5.5, ranked[0][2], 0.01);
      Assert.AreEqual(2.5, ranked[1][0], 0.01);
      Assert.AreEqual(4, ranked[1][1], 0.01);
      Assert.AreEqual(8, ranked[1][2], 0.01);
      Assert.AreEqual(5.5, ranked[2][0], 0.01);
      Assert.AreEqual(8, ranked[2][1], 0.01);
      Assert.AreEqual(8, ranked[2][2], 0.01);
      Assert.AreEqual(10, ranked[2][3], 0.01);

      var group4 = GroupNormalizer.NormalizeByRanks(ranked[2]);

      var ranked2 = GroupNormalizer.NormalizeByRanks(ranked);
      Assert.AreSame(ranked2, ranked);

      var group5 = new MultiArrayDataGroup(new IDataGroup[] { ranked[0], ranked[1], group4 });

      var ranked3 = GroupNormalizer.NormalizeByRanks(group5);
      Assert.AreNotSame(ranked3, group5);

      var ranked4 = GroupNormalizer.NormalizeByRanks(ranked, 2);
      Assert.AreNotSame(ranked4, ranked);
    }

    [TestMethod]
    public void ThreeDimensionsGroup()
    {
      var group1 = new ArrayDataGroup(new double[] { 9, 4, 6 });
      var group2 = new ArrayDataGroup(new double[] { 10, 5, 7 });
      var group3 = new ArrayDataGroup(new double[] { 8, 11, 15 });
      var group4 = new ArrayDataGroup(new double[] { 12, 13, 14 });

      var mg1 = new MultiArrayDataGroup(new[] { group1, group2 });
      var mg2 = new MultiArrayDataGroup(new[] { group3, group4 });

      var group = new SetMultiArrayDataGroup(new IMultyDataGroup[] { mg1, mg2 });
      var ranked = GroupNormalizer.NormalizeByRanks(group);

      Assert.AreEqual(group.Count, ranked.Count);

      for (int i = 0; i < group.Count; i++)
      {
        Assert.AreEqual(group[i].Count, ranked[i].Count);

        for (int j = 0; j < group[i].Count; j++)
        {
          Assert.AreEqual(group[i][j].Count, ranked[i][j].Count);

          for (int k = 0; k < group[i][j].Count; k++)
            Assert.AreEqual(group[i][j][k] - 3, ranked[i][j][k], 0.01);
        }
      }

      var ranked2 = GroupNormalizer.NormalizeByRanks(ranked);
      Assert.AreSame(ranked2, ranked);
      var ranked3 = GroupNormalizer.NormalizeByRanks(ranked, 2);
      Assert.AreNotSame(ranked3, ranked);
    }
  }
}
