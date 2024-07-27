using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Schicksal.Anova;
using Schicksal.Basic;

namespace ANOVATest
{
  [TestClass]
  public class CalcMultyCritTest
  {
    /// <summary>
    /// Сопоставление результатов, рассчитаных с помощью Excel (1 эксперимент) и языка R для фактора B "сорт"
    /// </summary>
    [TestMethod]
    public void ExcelAndRComparisonExpNo1FactB()
    {
      ArrayPlainSample groupA1 = new ArrayPlainSample(new double[] { 48, 49, 50 });
      ArrayPlainSample groupA2 = new ArrayPlainSample(new double[] { 53, 54, 55 });
      ArrayDividedSample multiA = new ArrayDividedSample(new IPlainSample[] { groupA1, groupA2 });

      ArrayPlainSample groupB1 = new ArrayPlainSample(new double[] { 18, 19, 20 });
      ArrayPlainSample groupB2 = new ArrayPlainSample(new double[] { 24, 25, 26 });
      ArrayDividedSample multiB = new ArrayDividedSample(new IPlainSample[] { groupB1, groupB2 });

      ArrayComplexSample set = new ArrayComplexSample(new IDividedSample[] { multiA, multiB });

      FisherMetrics f = FisherCriteria.CalculateMultiplyCriteria(set);
      double fExp = 2610.75;
      Assert.AreEqual(fExp, f.F);
    }

    /// <summary>
    /// Сопоставление результатов, рассчитаных с помощью Excel (1 эксперимент) и языка R для фактора A "удобрение"
    /// </summary>
    [TestMethod]
    public void ExcelAndRComparisonExpNo1FactA()
    {
      ArrayPlainSample groupA1 = new ArrayPlainSample(new double[] { 48, 49, 50 });
      ArrayPlainSample groupA2 = new ArrayPlainSample(new double[] { 18, 19, 20 });
      ArrayDividedSample multiA = new ArrayDividedSample(new IPlainSample[] { groupA1, groupA2 });

      ArrayPlainSample groupB1 = new ArrayPlainSample(new double[] { 53, 54, 55 });
      ArrayPlainSample groupB2 = new ArrayPlainSample(new double[] { 24, 25, 26 });
      ArrayDividedSample multiB = new ArrayDividedSample(new IPlainSample[] { groupB1, groupB2 });

      ArrayComplexSample set = new ArrayComplexSample(new IDividedSample[] { multiA, multiB });

      FisherMetrics f = FisherCriteria.CalculateMultiplyCriteria(set);
      double fExp = 90.75;
      Assert.AreEqual(fExp, f.F);
    }

    /// <summary>
    /// Проверка работы с различными градациями разных факторов: 2 вида сорта, 3 вида удобрений для Fb (сорт)
    /// </summary>
    [TestMethod]
    public void TwoSortsThreeNitrogenFacB()
    {
      ArrayPlainSample groupA1 = new ArrayPlainSample(new double[] { 20, 28, 15 });
      ArrayPlainSample groupA2 = new ArrayPlainSample(new double[] { 21, 30, 19 });
      ArrayPlainSample groupA3 = new ArrayPlainSample(new double[] { 32, 33, 35 });
      ArrayDividedSample multiA = new ArrayDividedSample(new IPlainSample[] { groupA1, groupA2, groupA3 });

      ArrayPlainSample groupB1 = new ArrayPlainSample(new double[] { 32, 30, 36 });
      ArrayPlainSample groupB2 = new ArrayPlainSample(new double[] { 34, 31, 36 });
      ArrayPlainSample groupB3 = new ArrayPlainSample(new double[] { 37, 35, 38 });
      ArrayDividedSample multiB = new ArrayDividedSample(new IPlainSample[] { groupB1, groupB2, groupB3 });

      ArrayComplexSample set = new ArrayComplexSample(new IDividedSample[] { multiA, multiB });

      FisherMetrics f = FisherCriteria.CalculateMultiplyCriteria(set);
      double fExp = 19.71;
      Assert.AreEqual(fExp, Math.Round(f.F, 2));
    }

    /// <summary>
    /// Проверка работы с различными градациями разных факторов: 2 вида сорта, 3 вида удобрений для Fa (удобрение)
    /// </summary>
    [TestMethod]
    public void TwoSortsThreeNitrogenFacA()
    {
      ArrayPlainSample groupA1 = new ArrayPlainSample(new double[] { 20, 28, 15 });
      ArrayPlainSample groupA2 = new ArrayPlainSample(new double[] { 32, 30, 36 });
      ArrayDividedSample multiA = new ArrayDividedSample(new IPlainSample[] { groupA1, groupA2 });

      ArrayPlainSample groupB1 = new ArrayPlainSample(new double[] { 21, 30, 19 });
      ArrayPlainSample groupB2 = new ArrayPlainSample(new double[] { 34, 31, 36 });
      ArrayDividedSample multiB = new ArrayDividedSample(new IPlainSample[] { groupB1, groupB2 });

      ArrayPlainSample groupC1 = new ArrayPlainSample(new double[] { 32, 33, 35 });
      ArrayPlainSample groupC2 = new ArrayPlainSample(new double[] { 37, 35, 38 });
      ArrayDividedSample multiC = new ArrayDividedSample(new IPlainSample[] { groupC1, groupC2 });

      ArrayComplexSample set = new ArrayComplexSample(new IDividedSample[] { multiA, multiB, multiC });

      FisherMetrics f = FisherCriteria.CalculateMultiplyCriteria(set);
      double fExp = 6.86;
      Assert.AreEqual(fExp, Math.Round(f.F, 2));
    }

    /// <summary>
    /// 3 повторности для Интенсив ~ N0 и Экстенсив ~ N0, и отсутствие повторностей для Интенсив ~ N60 и Экстенсив ~ N60
    /// для Fb (сорт)
    /// </summary>
    [TestMethod]
    public void IgnoreIntensivOneGradation2x2FacB()
    {
      ArrayPlainSample groupA1 = new ArrayPlainSample(new double[] { 32, 31, 30 });
      ArrayPlainSample groupA2 = new ArrayPlainSample(new double[] { 33 });
      ArrayDividedSample multiA = new ArrayDividedSample(new IPlainSample[] { groupA1, groupA2 });

      ArrayPlainSample groupB1 = new ArrayPlainSample(new double[] { 33, 32, 31 });
      ArrayPlainSample groupB2 = new ArrayPlainSample(new double[] { 34 });
      ArrayDividedSample multiB = new ArrayDividedSample(new IPlainSample[] { groupB1, groupB2 });

      ArrayComplexSample set = new ArrayComplexSample(new IDividedSample[] { multiA, multiB });

      FisherMetrics f = FisherCriteria.CalculateMultiplyCriteria(set);
      double fExp = 2;
      Assert.AreEqual(fExp, f.F);
    }

    /// <summary>
    /// 3 повторности для Интенсив ~ N0 и Экстенсив ~ N0, и отсутствие повторностей для Интенсив ~ N60 и Экстенсив ~ N60
    /// для Fa (удобрение)
    /// </summary>
    [TestMethod]
    public void IgnoreIntensivOneGradation2x2FacA()
    {
      ArrayPlainSample groupA1 = new ArrayPlainSample(new double[] { 32, 31, 30 });
      ArrayPlainSample groupA2 = new ArrayPlainSample(new double[] { 33, 32, 31 });
      ArrayDividedSample multiA = new ArrayDividedSample(new IPlainSample[] { groupA1, groupA2 });

      ArrayPlainSample groupB1 = new ArrayPlainSample(new double[] { 33 });
      ArrayPlainSample groupB2 = new ArrayPlainSample(new double[] { 34 });
      ArrayDividedSample multiB = new ArrayDividedSample(new IPlainSample[] { groupB1, groupB2 });

      ArrayComplexSample set = new ArrayComplexSample(new IDividedSample[] { multiA, multiB });

      FisherMetrics f = FisherCriteria.CalculateMultiplyCriteria(set);
      double fExp = 6;
      Assert.AreEqual(fExp, f.F);
    }

    /// <summary>
    /// Интенсив ~ N0 и Экстенсив ~ N0 - одна повторность, а для Интенсив, Экстенсив ~ N60, N120 - три повторности
    /// </summary>
    [TestMethod]
    public void OneRepeatForNitrogenFacB()
    {
      ArrayPlainSample groupA1 = new ArrayPlainSample(new double[] { 20 });
      ArrayPlainSample groupA2 = new ArrayPlainSample(new double[] { 22, 24, 22 });
      ArrayPlainSample groupA3 = new ArrayPlainSample(new double[] { 29, 30, 32 });
      ArrayDividedSample multiA = new ArrayDividedSample(new IPlainSample[] { groupA1, groupA2, groupA3 });

      ArrayPlainSample groupB1 = new ArrayPlainSample(new double[] { 21 });
      ArrayPlainSample groupB2 = new ArrayPlainSample(new double[] { 27, 26, 22 });
      ArrayPlainSample groupB3 = new ArrayPlainSample(new double[] { 32, 33, 34 });
      ArrayDividedSample multiB = new ArrayDividedSample(new IPlainSample[] { groupB1, groupB2, groupB3 });

      ArrayComplexSample set = new ArrayComplexSample(new IDividedSample[] { multiA, multiB });

      FisherMetrics f = FisherCriteria.CalculateMultiplyCriteria(set);
      double fExp = 6.269;
      Assert.AreEqual(fExp, f.F, 1e-3);
    }

    /// <summary>
    /// Интенсив ~ N0 и Экстенсив ~ N0 - одна повторность, а для Интенсив, Экстенсив ~ N60, N120 - три повторности
    /// </summary>
    [TestMethod]
    public void OneRepeatForNitrogenFacA()
    {
      ArrayPlainSample groupA1 = new ArrayPlainSample(new double[] { 20 });
      ArrayPlainSample groupA2 = new ArrayPlainSample(new double[] { 21 });
      ArrayDividedSample multiA = new ArrayDividedSample(new IPlainSample[] { groupA1, groupA2 });

      ArrayPlainSample groupB1 = new ArrayPlainSample(new double[] { 22, 24, 22 });
      ArrayPlainSample groupB2 = new ArrayPlainSample(new double[] { 27, 26, 22 });
      ArrayDividedSample multiB = new ArrayDividedSample(new IPlainSample[] { groupB1, groupB2 });

      ArrayPlainSample groupC1 = new ArrayPlainSample(new double[] { 29, 30, 32 });
      ArrayPlainSample groupC2 = new ArrayPlainSample(new double[] { 32, 33, 34 });
      ArrayDividedSample multiC = new ArrayDividedSample(new IPlainSample[] { groupC1, groupC2 });

      ArrayComplexSample set = new ArrayComplexSample(new IDividedSample[] { multiA, multiB, multiC });

      FisherMetrics f = FisherCriteria.CalculateMultiplyCriteria(set);
      double fExp = 47.004;
      Assert.AreEqual(fExp, f.F, 1e-3);
    }

    /// <summary>
    /// Анализ на данных без повторностей, сравнение однофакторного и
    /// многофакторного метода (Интенсив~N0 Интенсив~N60 Экстенсив~N0 Экстенсив~N60)
    /// </summary>
    [TestMethod]
    public void OnefactorVsMultifactorWithOneRep()
    {
      ArrayPlainSample groupA = new ArrayPlainSample(new double[] { 32, 29, 31, 35, 28, 26, 31, 36 });
      ArrayPlainSample groupB = new ArrayPlainSample(new double[] { 31, 28, 31, 40, 25, 29, 35, 39 });
      ArrayDividedSample multi = new ArrayDividedSample(new IPlainSample[] { groupA, groupB });

      FisherMetrics multiF = FisherCriteria.CalculateCriteria(multi);

      ArrayDividedSample multiA = new ArrayDividedSample(new[] { groupA });
      ArrayDividedSample multiB = new ArrayDividedSample(new[] { groupB });

      ArrayComplexSample set = new ArrayComplexSample(new IDividedSample[] { multiA, multiB });

      FisherMetrics f = FisherCriteria.CalculateMultiplyCriteria(set);

      Assert.AreEqual(f.F, multiF.F);
    }

    /// <summary>
    /// Разное количество повторностей во всех градациях всех факторов, игнорируемый фактор - удобрение,
    /// и для него выполняется условие в разное число повторностей
    /// </summary>
    [TestMethod]
    public void FactorsWithDifferentGradations()
    {
      ArrayPlainSample groupA1 = new ArrayPlainSample(new double[] { 32, 30, 33 });
      ArrayPlainSample groupA2 = new ArrayPlainSample(new double[] { 29 });
      ArrayPlainSample groupA3 = new ArrayPlainSample(new double[] { 30, 35, 40 });
      ArrayDividedSample multiA = new ArrayDividedSample(new IPlainSample[] { groupA1, groupA2, groupA3 });

      ArrayPlainSample groupB1 = new ArrayPlainSample(new double[] { 31, 32, 29, 28 });
      ArrayPlainSample groupB2 = new ArrayPlainSample(new double[] { 27 });
      ArrayPlainSample groupB3 = new ArrayPlainSample(new double[] { 33, 35 });
      ArrayDividedSample multiB = new ArrayDividedSample(new IPlainSample[] { groupB1, groupB2, groupB3 });

      ArrayComplexSample set = new ArrayComplexSample(new IDividedSample[] { multiA, multiB });

      FisherMetrics f = FisherCriteria.CalculateMultiplyCriteria(set);
      double fExp = 1.680;
      Assert.AreEqual(fExp, f.F, 1e-3);
    }
  }
}
