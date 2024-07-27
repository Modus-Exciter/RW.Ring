using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schicksal.Basic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    private TestContext m_test_context_instance;

    /// <summary>
    ///Получает или устанавливает контекст теста, в котором предоставляются
    ///сведения о текущем тестовом запуске и обеспечивается его функциональность.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return m_test_context_instance;
      }
      set
      {
        m_test_context_instance = value;
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
    public void OrderGroup()
    {
      var group = new ArrayPlainSample(new double[] { 2, 7, 9.2, 7, 4, 4, 4, 3 });

      var gr2 = OrderedSample.Construct(group);

      for (int i = 1; i < group.Count; i++)
        Assert.IsTrue(gr2[i - 1] <= gr2[i]);
    }

    [TestMethod]
    public void OrderGroupDesc()
    {
      var group = new ArrayPlainSample(new double[] { 2, 7, 9.2, 7, 4, 4, 4, 3 });

      var gr2 = OrderedSample.Construct(group, System.ComponentModel.ListSortDirection.Descending);

      for (int i = 1; i < group.Count; i++)
        Assert.IsTrue(gr2[i - 1] >= gr2[i]);
    }

    [TestMethod]
    public void OrderedEnumeration()
    {
      var group1 = CreateRandomGroup(200);
      var group2 = CreateRandomGroup(300);
      var group3 = CreateRandomGroup(70000);

      var ordered1 = OrderedSample.Construct(group1);
      var ordered2 = OrderedSample.Construct(group2);
      var ordered3 = OrderedSample.Construct(group3);

      var arr1 = group1.OrderBy(a => a).ToArray();
      var arr2 = group2.OrderBy(a => a).ToArray();
      var arr3 = group3.OrderBy(a => a).ToArray();

      CheckEquality(ordered1, arr1);
      CheckEquality(ordered2, arr2);
      CheckEquality(ordered3, arr3);

      ordered1 = OrderedSample.Construct(group1, System.ComponentModel.ListSortDirection.Descending);
      ordered2 = OrderedSample.Construct(group2, System.ComponentModel.ListSortDirection.Descending);
      ordered3 = OrderedSample.Construct(group3, System.ComponentModel.ListSortDirection.Descending);

      Array.Reverse(arr1);
      Array.Reverse(arr2);
      Array.Reverse(arr3);

      CheckEquality(ordered1, arr1);
      CheckEquality(ordered2, arr2);
      CheckEquality(ordered3, arr3);
    }

    [TestMethod]
    public void ResortGroup()
    {
      double[] array = new double[200];
      Random rnd = new Random();

      for (int i = 0; i < 200; i++)
        array[i] = rnd.Next() + rnd.NextDouble();

      var group = new ArrayPlainSample(array);

      var ordered = OrderedSample.Construct(group);

      for (int i = 17; i < 200; i += rnd.Next(20))
        array[i] = -array[i];

      Array.Sort(array);

      bool invert = false;

      for (int i = 1; i < ordered.Count; i++)
      {
        if (ordered[i - 1] > ordered[i])
        {
          invert = true;
          break;
        }
      }

      Assert.IsTrue(invert);

      ordered.Resort();

      for (int i = 0; i < ordered.Count; i++)
        Assert.IsTrue(array[i] == ordered[i]);
    }

    private static void CheckEquality(IPlainSample group, double[] array)
    {
      for (int i = 0; i < array.Length; i++)
        Assert.AreEqual(array[i], group[i]);
    }

    private static IPlainSample CreateRandomGroup(int size)
    {
      double[] array = new double[size];
      Random rnd = new Random();

      for (int i = 0; i < size; i++)
        array[i] = rnd.Next() + rnd.NextDouble();

      return new ArrayPlainSample(array);
    }

    [TestMethod]
    public void OneDimensionGroup()
    {
      var group = new ArrayPlainSample(new double[] { 2, 7, 9.2, 7, 4, 4, 4 });
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
    public void SeveralMaxes()
    {
      var group = new ArrayPlainSample(new double[] { 2, 7, 9.2, 7, 4, 4, 9.2, 9.2 });
      var ranked = GroupNormalizer.NormalizeByRanks(group, 2);
      Assert.AreEqual(8, ranked.Count);
      Assert.AreEqual(1, ranked[0], 0.01);
      Assert.AreEqual(4.5, ranked[1], 0.01);
      Assert.AreEqual(7, ranked[2], 0.01);
      Assert.AreEqual(4.5, ranked[3], 0.01);
      Assert.AreEqual(2.5, ranked[4], 0.01);
      Assert.AreEqual(2.5, ranked[5], 0.01);
      Assert.AreEqual(7, ranked[6], 0.01);
    }

    [TestMethod]
    public void NegativeAndPositive()
    {
      var group = new ArrayPlainSample(new double[] { -3, -1, 2, 5, 8, 12 });
      var delta = GroupNormalizer.CalculateDelta(group);
      var group2 = new ArrayPlainSample(group.Select(a => a + delta).ToArray());
      var group3 = GroupNormalizer.NormalizeByBoxCox(group);
      var lambda = (double)group3.GetType().GetField("Lambda").GetValue(group3);
      Assert.AreEqual(lambda, GroupNormalizer.CalculateLambda(group2), 1e-5);
    }

    [TestMethod]
    public void TwoDimensionsGroup()
    {
      var group1 = new ArrayPlainSample(new double[] { 2, 4, 6 });
      var group2 = new ArrayPlainSample(new double[] { 4, 5, 7});
      var group3 = new ArrayPlainSample(new double[] { 6, 7, 7, 8 });

      var group = new ArrayDividedSample(new IPlainSample[] { group1, group2, group3 });

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

      var group5 = new ArrayDividedSample(new IPlainSample[] { ranked[0], ranked[1], group4 });

      var ranked3 = GroupNormalizer.NormalizeByRanks(group5);
      Assert.AreNotSame(ranked3, group5);

      var ranked4 = GroupNormalizer.NormalizeByRanks(ranked, 2);
      Assert.AreSame(ranked4, ranked);
    }

    [TestMethod]
    public void BoxCoxTestOneDimension()
    {
      var group = new ArrayPlainSample(new double[] { 2, 7, 8, 7, 4, 4, 4 });
      var lambda = GroupNormalizer.CalculateLambda(group);
      Assert.AreEqual(0.705, lambda, 0.00025);
      var group2 = GroupNormalizer.NormalizeByBoxCox(group);
      lambda = (double)group2.GetType().GetField("Lambda").GetValue(group2);
      Assert.AreEqual(0.705, lambda, 0.00025);
    }

    [TestMethod]
    public void BoxCoxTestTwoDimensions()
    {
      var group1 = new ArrayPlainSample(new double[] { 3, 4, 6 });
      var group2 = new ArrayPlainSample(new double[] { 4, 5, 7 });
      var group3 = new ArrayPlainSample(new double[] { 6, 7, 7, 8 });

      var group4 = new ArrayDividedSample(new IPlainSample[] { group1, group2, group3 });

      var group5 = GroupNormalizer.NormalizeByBoxCox(group4);
      var lambda = (double)group5[0].GetType().GetField("Lambda").GetValue(group5[0]);

      Assert.IsTrue(lambda > -10 && lambda < 10);
    }

    [TestMethod]
    public void InverseBoxCox()
    {
      var group = new ArrayPlainSample(new double[] { 2, 7, 8, 7, 4, 4, 4 });
      var group2 = GroupNormalizer.NormalizeByBoxCox(group);
      var handler = GroupNormalizer.CreateInverseHandler(group2);

      for (int i = 0; i < group.Count; i++)
        Assert.AreEqual(group[i], handler(group2[i]), 1e-5);

      group2.GetType().GetField("Lambda").SetValue(group2, 0);
      handler = GroupNormalizer.CreateInverseHandler(group2);

      for (int i = 0; i < group.Count; i++)
        Assert.AreEqual(group[i], handler(group2[i]), 1e-5);
    }

    [TestMethod]
    public void InverseBoxCoxInSet()
    {
      var group1 = new ArrayPlainSample(new double[] { 9, 4, 6 });
      var group2 = new ArrayPlainSample(new double[] { 10, 5, 7 });
      var group3 = new ArrayPlainSample(new double[] { 8, 11, 15 });
      var group4 = new ArrayPlainSample(new double[] { 12, 13, 14 });
      var mg1 = new ArrayDividedSample(new[] { group1, group2 });
      var mg2 = new ArrayDividedSample(new[] { group3, group4 });
      var group = new ArrayComplexSample(new IDividedSample[] { mg1, mg2 });
      var group5 = GroupNormalizer.NormalizeByBoxCox(group);
      var handler = GroupNormalizer.CreateInverseHandler(group5);

      for (int i = 0; i < group.Count; i++)
      {
        Assert.AreEqual(group[i].Count, group5[i].Count);

        for (int j = 0; j < group[i].Count; j++)
        {
          Assert.AreEqual(group[i][j].Count, group5[i][j].Count);

          for (int k = 0; k < group[i][j].Count; k++)
            Assert.AreEqual(group[i][j][k], handler(group5[i][j][k]), 1e-5);
        }
      }
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void NegativeValues()
    {
      ArrayPlainSample group = new ArrayPlainSample(new double[] { -3, -2, -1, 0, 1, 2, 3, 4 });
      GroupNormalizer.CalculateLambda(group);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void NegativeValues2()
    {
      ArrayPlainSample group = new ArrayPlainSample(new double[] { -3, -2, -1, 0, 1, 2, 3, 4 });
      GroupNormalizer.CalculateLambda(group, 3);
    }

    [TestMethod]
    public void Delta()
    {
      double[] values = new double[] { -3, -2, -1, 0, 1, 2, 3, 4 };

      Assert.AreEqual(4, GroupNormalizer.CalculateDelta(values));

      values = new double[] { -5 };

      Assert.AreEqual(6, GroupNormalizer.CalculateDelta(values));

      values = new double[] { 0, 1, 2 };

      Assert.AreEqual(1, GroupNormalizer.CalculateDelta(values));

      values = new double[] { double.Epsilon, 2 , 3};

      Assert.AreEqual(0, GroupNormalizer.CalculateDelta(values));
    }

    [TestMethod]
    public void BoxCoxTestThreeDimensions()
    {
      var group1 = new ArrayPlainSample(new double[] { 9, 4, 6 });
      var group2 = new ArrayPlainSample(new double[] { 10, 5, 7 });
      var group3 = new ArrayPlainSample(new double[] { 8, 11, 15 });
      var group4 = new ArrayPlainSample(new double[] { 12, 13, 14 });

      var mg1 = new ArrayDividedSample(new[] { group1, group2 });
      var mg2 = new ArrayDividedSample(new[] { group3, group4 });
      var mg3 = new ArrayDividedSample(new[] { group1, group2, group3, group4 });

      var group = new ArrayComplexSample(new IDividedSample[] { mg1, mg2 });

      var group5 = GroupNormalizer.NormalizeByBoxCox(mg3);
      var group6 = GroupNormalizer.NormalizeByBoxCox(group);

      var lambda1 = (double)group5[0].GetType().GetField("Lambda").GetValue(group5[0]);

      var lambda2 = (double)group6[0][0].GetType().GetField("Lambda").GetValue(group6[0][0]);

      Assert.AreEqual(lambda1, lambda2);

      Assert.IsTrue(lambda1 > -10 && lambda1 < 10);
    }

    [TestMethod]
    public void ThreeDimensionsGroup()
    {
      var group1 = new ArrayPlainSample(new double[] { 9, 4, 6 });
      var group2 = new ArrayPlainSample(new double[] { 10, 5, 7 });
      var group3 = new ArrayPlainSample(new double[] { 8, 11, 15 });
      var group4 = new ArrayPlainSample(new double[] { 12, 13, 14 });

      var mg1 = new ArrayDividedSample(new[] { group1, group2 });
      var mg2 = new ArrayDividedSample(new[] { group3, group4 });

      var group = new ArrayComplexSample(new IDividedSample[] { mg1, mg2 });
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

      var handler = GroupNormalizer.CreateInverseHandler(ranked2);
      for (int i = 0; i < group.Count; i++)
      {
        Assert.AreEqual(group[i].Count, ranked[i].Count);

        for (int j = 0; j < group[i].Count; j++)
        {
          Assert.AreEqual(group[i][j].Count, ranked[i][j].Count);

          for (int k = 0; k < group[i][j].Count; k++)
            Assert.AreEqual(group[i][j][k], handler(ranked[i][j][k]), 0.01);
        }
      }
    }
  }
  public static class GroupNormalizer
  {
    #region Box-Cox -------------------------------------------------------------------------------

    /// <summary>
    /// Вычисление фиксированного коэффициента смещения для преобразования Бокса-Кокса
    /// </summary>
    /// <param name="group">Числовая последовательность</param>
    /// <returns>Коэффициент для смещения отрицательных значений</returns>
    public static double CalculateDelta(IEnumerable<double> group)
    {
      return BoxCoxNormalizer.CalculateDelta(group);
    }

    /// <summary>
    /// Вычисление коэффициента для преобразования Бокса-Кокса
    /// </summary>
    /// <param name="group">Числовая последовательность</param>
    /// <param name="delta">Коэффициент для смещения отрицательных значений</param>
    /// <returns>Коэффициент для преобразования</returns>
    public static double CalculateLambda(IPlainSample group, double delta = 0)
    {
      return new BoxCoxNormalizer().CalculateLambda(group, delta);
    }

    /// <summary>
    /// Преобразование Бокса-Кокса
    /// </summary>
    /// <param name="value">Преобразуемое значение</param>
    /// <param name="lambda">Коэффициент для преобразования</param>
    /// <param name="delta">Коэффициент для смещения отрицательных значений</param>
    /// <returns>Преобразованное значение</returns>
    public static double BoxCoxTransform(double value, double lambda, double delta = 0)
    {
      return BoxCoxNormalizer.BoxCoxTransform(value, lambda, delta);
    }

    /// <summary>
    /// Нормирование группы значений методом Бокса-Кокса
    /// </summary>
    /// <param name="group">Исходная группа</param>
    /// <returns>Нормированная группа</returns>
    public static IPlainSample NormalizeByBoxCox(IPlainSample group)
    {
      return new BoxCoxNormalizer().Normalize(group);
    }

    /// <summary>
    /// Нормирование группы значений второго порядка методом Бокса-Кокса
    /// </summary>
    /// <param name="group">Исходная группа</param>
    /// <returns>Нормированная группа</returns>
    public static IDividedSample NormalizeByBoxCox(IDividedSample group)
    {
      return new BoxCoxNormalizer().Normalize(group);
    }

    /// <summary>
    /// Нормирование группы значений третьего порядка методом Бокса-Кокса
    /// </summary>
    /// <param name="group">Исходная группа</param>
    /// <returns>Нормированная группа</returns>
    public static IComplexSample NormalizeByBoxCox(IComplexSample group)
    {
      return new BoxCoxNormalizer().Normalize(group);
    }

    #endregion

    #region Kruskal-Wallis ------------------------------------------------------------------------

    /// <summary>
    /// Расчёт рангов чисел в числовой последовательности
    /// </summary>
    /// <param name="data">Числовая последовательность</param>
    /// <param name="round">Количество знаков после запятой для округления</param>
    /// <returns>Каждому значению из числовой последовательности сопоставляется его ранг</returns>
    public static Dictionary<double, float> CalculateRanks(IEnumerable<double> data, int round = -1)
    {
      return RankNormalizer.CalculateRanks(data, round);
    }

    /// <summary>
    /// Преобразование группы значений в группу рангов
    /// </summary>
    /// <param name="group">Исходная группа</param>
    /// <param name="round">Количество знаков после запятой для округления</param>
    /// <returns>Преобразованная группа</returns>
    public static IPlainSample NormalizeByRanks(IPlainSample group, int round = -1)
    {
      return new RankNormalizer(round).Normalize(group);
    }

    /// <summary>
    /// Преобразование группы значений второго порядка в группу рангов
    /// </summary>
    /// <param name="group">Исходная группа</param>
    /// <param name="round">Количество знаков после запятой для округления</param>
    /// <returns>Преобразованная группа</returns>
    public static IDividedSample NormalizeByRanks(IDividedSample multyGroup, int round = -1)
    {
      return new RankNormalizer(round).Normalize(multyGroup);
    }

    /// <summary>
    /// Преобразование группы значений третьего порядка в группу рангов
    /// </summary>
    /// <param name="group">Исходная группа</param>
    /// <param name="round">Количество знаков после запятой для округления</param>
    /// <returns>Преобразованная группа</returns>
    public static IComplexSample NormalizeByRanks(IComplexSample group, int round = -1)
    {
      return new RankNormalizer(round).Normalize(group);
    }

    #endregion

    #region Inverse handlers ----------------------------------------------------------------------

    /// <summary>
    /// Расчёт изначального значения по преобразованному
    /// </summary>
    /// <param name="group">Группа преобразованных значений</param>
    /// <returns>Значение в исходной группе</returns>
    public static Func<double, double> CreateInverseHandler(IPlainSample group)
    {
      Debug.Assert(group != null, "group cannot be null");

      var ranked = new RankNormalizer().GetDenormalizer(group);

      if (ranked != DummyNormalizer.Instance.GetDenormalizer(group))
        return ranked.Denormalize;
      else
        return new BoxCoxNormalizer().GetDenormalizer(group).Denormalize;
    }

    /// <summary>
    /// Расчёт изначального значения по преобразованному
    /// </summary>
    /// <param name="group">Группа преобразованных значений второго порядка</param>
    /// <returns>Значение в исходной группе</returns>
    public static Func<double, double> CreateInverseHandler(IDividedSample group)
    {
      Debug.Assert(group != null, "group cannot be null");

      var ranked = new RankNormalizer().GetDenormalizer(group);

      if (ranked != DummyNormalizer.Instance.GetDenormalizer(group))
        return ranked.Denormalize;
      else
        return new BoxCoxNormalizer().GetDenormalizer(group).Denormalize;
    }

    /// <summary>
    /// Расчёт изначального значения по преобразованному
    /// </summary>
    /// <param name="group">Группа преобразованных значений третьего порядка</param>
    /// <returns>Значение в исходной группе</returns>
    public static Func<double, double> CreateInverseHandler(IComplexSample group)
    {
      Debug.Assert(group != null, "group cannot be null");

      var ranked = new RankNormalizer().GetDenormalizer(group);

      if (ranked != DummyNormalizer.Instance.GetDenormalizer(group))
        return ranked.Denormalize;
      else
        return new BoxCoxNormalizer().GetDenormalizer(group).Denormalize;
    }

    #endregion
  }
}
