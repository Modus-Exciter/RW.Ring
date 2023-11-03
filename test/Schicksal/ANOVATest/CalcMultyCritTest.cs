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
      ArrayDataGroup groupA1 = new ArrayDataGroup(new double[] { 48, 49, 50 });
      ArrayDataGroup groupA2 = new ArrayDataGroup(new double[] { 53, 54, 55 });
      MultiArrayDataGroup multiA = new MultiArrayDataGroup(new IDataGroup[] { groupA1, groupA2 });

      ArrayDataGroup groupB1 = new ArrayDataGroup(new double[] { 18, 19, 20 });
      ArrayDataGroup groupB2 = new ArrayDataGroup(new double[] { 24, 25, 26 });
      MultiArrayDataGroup multiB = new MultiArrayDataGroup(new IDataGroup[] { groupB1, groupB2 });

      SetMultiArrayDataGroup set = new SetMultiArrayDataGroup(new IMultyDataGroup[] { multiA, multiB });

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
      ArrayDataGroup groupA1 = new ArrayDataGroup(new double[] { 48, 49, 50 });
      ArrayDataGroup groupA2 = new ArrayDataGroup(new double[] { 18, 19, 20 });
      MultiArrayDataGroup multiA = new MultiArrayDataGroup(new IDataGroup[] { groupA1, groupA2 });

      ArrayDataGroup groupB1 = new ArrayDataGroup(new double[] { 53, 54, 55 });
      ArrayDataGroup groupB2 = new ArrayDataGroup(new double[] { 24, 25, 26 });
      MultiArrayDataGroup multiB = new MultiArrayDataGroup(new IDataGroup[] { groupB1, groupB2 });

      SetMultiArrayDataGroup set = new SetMultiArrayDataGroup(new IMultyDataGroup[] { multiA, multiB });

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
      ArrayDataGroup groupA1 = new ArrayDataGroup(new double[] { 20, 28, 15 });
      ArrayDataGroup groupA2 = new ArrayDataGroup(new double[] { 21, 30, 19 });
      ArrayDataGroup groupA3 = new ArrayDataGroup(new double[] { 32, 33, 35 });
      MultiArrayDataGroup multiA = new MultiArrayDataGroup(new IDataGroup[] { groupA1, groupA2, groupA3 });

      ArrayDataGroup groupB1 = new ArrayDataGroup(new double[] { 32, 30, 36 });
      ArrayDataGroup groupB2 = new ArrayDataGroup(new double[] { 34, 31, 36 });
      ArrayDataGroup groupB3 = new ArrayDataGroup(new double[] { 37, 35, 38 });
      MultiArrayDataGroup multiB = new MultiArrayDataGroup(new IDataGroup[] { groupB1, groupB2, groupB3 });

      SetMultiArrayDataGroup set = new SetMultiArrayDataGroup(new IMultyDataGroup[] { multiA, multiB });

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
      ArrayDataGroup groupA1 = new ArrayDataGroup(new double[] { 20, 28, 15 });
      ArrayDataGroup groupA2 = new ArrayDataGroup(new double[] { 32, 30, 36 });
      MultiArrayDataGroup multiA = new MultiArrayDataGroup(new IDataGroup[] { groupA1, groupA2 });

      ArrayDataGroup groupB1 = new ArrayDataGroup(new double[] { 21, 30, 19 });
      ArrayDataGroup groupB2 = new ArrayDataGroup(new double[] { 34, 31, 36 });
      MultiArrayDataGroup multiB = new MultiArrayDataGroup(new IDataGroup[] { groupB1, groupB2 });

      ArrayDataGroup groupC1 = new ArrayDataGroup(new double[] { 32, 33, 35 });
      ArrayDataGroup groupC2 = new ArrayDataGroup(new double[] { 37, 35, 38 });
      MultiArrayDataGroup multiC = new MultiArrayDataGroup(new IDataGroup[] { groupC1, groupC2 });

      SetMultiArrayDataGroup set = new SetMultiArrayDataGroup(new IMultyDataGroup[] { multiA, multiB, multiC });

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
      ArrayDataGroup groupA1 = new ArrayDataGroup(new double[] { 32, 31, 30 });
      ArrayDataGroup groupA2 = new ArrayDataGroup(new double[] { 33 });
      MultiArrayDataGroup multiA = new MultiArrayDataGroup(new IDataGroup[] { groupA1, groupA2 });

      ArrayDataGroup groupB1 = new ArrayDataGroup(new double[] { 33, 32, 31 });
      ArrayDataGroup groupB2 = new ArrayDataGroup(new double[] { 34 });
      MultiArrayDataGroup multiB = new MultiArrayDataGroup(new IDataGroup[] { groupB1, groupB2 });

      SetMultiArrayDataGroup set = new SetMultiArrayDataGroup(new IMultyDataGroup[] { multiA, multiB });

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
      ArrayDataGroup groupA1 = new ArrayDataGroup(new double[] { 32, 31, 30 });
      ArrayDataGroup groupA2 = new ArrayDataGroup(new double[] { 33, 32, 31 });
      MultiArrayDataGroup multiA = new MultiArrayDataGroup(new IDataGroup[] { groupA1, groupA2 });

      ArrayDataGroup groupB1 = new ArrayDataGroup(new double[] { 33 });
      ArrayDataGroup groupB2 = new ArrayDataGroup(new double[] { 34 });
      MultiArrayDataGroup multiB = new MultiArrayDataGroup(new IDataGroup[] { groupB1, groupB2 });

      SetMultiArrayDataGroup set = new SetMultiArrayDataGroup(new IMultyDataGroup[] { multiA, multiB });

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
      ArrayDataGroup groupA1 = new ArrayDataGroup(new double[] { 20 });
      ArrayDataGroup groupA2 = new ArrayDataGroup(new double[] { 22, 24, 22 });
      ArrayDataGroup groupA3 = new ArrayDataGroup(new double[] { 29, 30, 32 });
      MultiArrayDataGroup multiA = new MultiArrayDataGroup(new IDataGroup[] { groupA1, groupA2, groupA3 });

      ArrayDataGroup groupB1 = new ArrayDataGroup(new double[] { 21 });
      ArrayDataGroup groupB2 = new ArrayDataGroup(new double[] { 27, 26, 22 });
      ArrayDataGroup groupB3 = new ArrayDataGroup(new double[] { 32, 33, 34 });
      MultiArrayDataGroup multiB = new MultiArrayDataGroup(new IDataGroup[] { groupB1, groupB2, groupB3 });

      SetMultiArrayDataGroup set = new SetMultiArrayDataGroup(new IMultyDataGroup[] { multiA, multiB });

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
      ArrayDataGroup groupA1 = new ArrayDataGroup(new double[] { 20 });
      ArrayDataGroup groupA2 = new ArrayDataGroup(new double[] { 21 });
      MultiArrayDataGroup multiA = new MultiArrayDataGroup(new IDataGroup[] { groupA1, groupA2 });

      ArrayDataGroup groupB1 = new ArrayDataGroup(new double[] { 22, 24, 22 });
      ArrayDataGroup groupB2 = new ArrayDataGroup(new double[] { 27, 26, 22 });
      MultiArrayDataGroup multiB = new MultiArrayDataGroup(new IDataGroup[] { groupB1, groupB2 });

      ArrayDataGroup groupC1 = new ArrayDataGroup(new double[] { 29, 30, 32 });
      ArrayDataGroup groupC2 = new ArrayDataGroup(new double[] { 32, 33, 34 });
      MultiArrayDataGroup multiC = new MultiArrayDataGroup(new IDataGroup[] { groupC1, groupC2 });

      SetMultiArrayDataGroup set = new SetMultiArrayDataGroup(new IMultyDataGroup[] { multiA, multiB, multiC });

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
      ArrayDataGroup groupA = new ArrayDataGroup(new double[] { 32, 29, 31, 35, 28, 26, 31, 36 });
      ArrayDataGroup groupB = new ArrayDataGroup(new double[] { 31, 28, 31, 40, 25, 29, 35, 39 });
      MultiArrayDataGroup multi = new MultiArrayDataGroup(new IDataGroup[] { groupA, groupB });

      FisherMetrics multiF = FisherCriteria.CalculateCriteria(multi);

      ArrayDataGroup groupA1 = new ArrayDataGroup(new double[] { 32, 29, 31, 35 });
      ArrayDataGroup groupA2 = new ArrayDataGroup(new double[] { 28, 26, 31, 36 });
      MultiArrayDataGroup multiA = new MultiArrayDataGroup(new IDataGroup[] { groupA1, groupA2 });

      ArrayDataGroup groupB1 = new ArrayDataGroup(new double[] { 31, 28, 31, 40 });
      ArrayDataGroup groupB2 = new ArrayDataGroup(new double[] { 25, 29, 35, 39 });
      MultiArrayDataGroup multiB = new MultiArrayDataGroup(new IDataGroup[] { groupB1, groupB2 });

      SetMultiArrayDataGroup set = new SetMultiArrayDataGroup(new IMultyDataGroup[] { multiA, multiB });

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
      ArrayDataGroup groupA1 = new ArrayDataGroup(new double[] { 32, 30, 33 });
      ArrayDataGroup groupA2 = new ArrayDataGroup(new double[] { 29 });
      ArrayDataGroup groupA3 = new ArrayDataGroup(new double[] { 30, 35, 40 });
      MultiArrayDataGroup multiA = new MultiArrayDataGroup(new IDataGroup[] { groupA1, groupA2, groupA3 });

      ArrayDataGroup groupB1 = new ArrayDataGroup(new double[] { 31, 32, 29, 28 });
      ArrayDataGroup groupB2 = new ArrayDataGroup(new double[] { 27 });
      ArrayDataGroup groupB3 = new ArrayDataGroup(new double[] { 33, 35 });
      MultiArrayDataGroup multiB = new MultiArrayDataGroup(new IDataGroup[] { groupB1, groupB2, groupB3 });

      SetMultiArrayDataGroup set = new SetMultiArrayDataGroup(new IMultyDataGroup[] { multiA, multiB });

      FisherMetrics f = FisherCriteria.CalculateMultiplyCriteria(set);
      double fExp = 1.680;
      Assert.AreEqual(fExp, f.F, 1e-3);
    }
  }
}
