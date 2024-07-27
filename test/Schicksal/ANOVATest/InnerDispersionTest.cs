using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Schicksal.Anova;
using Schicksal.Basic;

namespace ANOVATest
{
  [TestClass]
  public class InnerDispersionTest
  {
    //В наборе данных есть 3 фактора (A-2уровня, B-3уровня, C-2уровня), необходимо выполнить многофакторный дисперсионный анализ для одной, двух и трех факторов и проверить, одинакова ли внутренняя дисперсия в результатах анализа с одним, двумя и тремя факторами.
    [TestMethod]
    public void NdfTestInThreeGroups()
    {
      //Три фактора
      //A1
      ArrayPlainSample groupA1B1C1 = new ArrayPlainSample(new double[] { 2, 5, 4 });
      ArrayPlainSample groupA1B1C2 = new ArrayPlainSample(new double[] { 3, 6, 1 });
      ArrayPlainSample groupA1B2C1 = new ArrayPlainSample(new double[] { 2, 2, 3 });
      ArrayPlainSample groupA1B2C2 = new ArrayPlainSample(new double[] { 4, 7, 2 });
      ArrayPlainSample groupA1B3C1 = new ArrayPlainSample(new double[] { 2, 5, 6 });
      ArrayPlainSample groupA1B3C2 = new ArrayPlainSample(new double[] { 4, 8, 3 });
      ArrayDividedSample threeMultiA1 = new ArrayDividedSample(new IPlainSample[] { groupA1B1C1, groupA1B1C2, groupA1B2C1, groupA1B2C2, groupA1B3C1, groupA1B3C2 });

      //A2
      ArrayPlainSample groupA2B1C1 = new ArrayPlainSample(new double[] { 3, 5, 5 });
      ArrayPlainSample groupA2B1C2 = new ArrayPlainSample(new double[] { 6, 1, 2 });
      ArrayPlainSample groupA2B2C1 = new ArrayPlainSample(new double[] { 2, 3, 5 });
      ArrayPlainSample groupA2B2C2 = new ArrayPlainSample(new double[] { 4, 7, 5 });
      ArrayPlainSample groupA2B3C1 = new ArrayPlainSample(new double[] { 8, 1, 2 });
      ArrayPlainSample groupA2B3C2 = new ArrayPlainSample(new double[] { 4, 3, 4 });
      ArrayDividedSample threeMultiA2 = new ArrayDividedSample(new IPlainSample[] { groupA2B1C1, groupA2B1C2, groupA2B2C1, groupA2B2C2, groupA2B3C1, groupA2B3C2 });

      ArrayComplexSample threeSet = new ArrayComplexSample(new IDividedSample[] { threeMultiA1, threeMultiA2 });
      FisherMetrics f3 = FisherCriteria.CalculateMultiplyCriteria(threeSet);

      ArrayDividedSample twoMultiA1B1 = new ArrayDividedSample(new IPlainSample[] { groupA1B1C1, groupA1B1C2 });
      ArrayDividedSample twoMultiA1B2 = new ArrayDividedSample(new IPlainSample[] { groupA1B2C1, groupA1B2C2 });
      ArrayDividedSample twoMultiA1B3 = new ArrayDividedSample(new IPlainSample[] { groupA1B3C1, groupA1B3C2 });

      ArrayDividedSample twoMultiA2B1 = new ArrayDividedSample(new IPlainSample[] { groupA2B1C1, groupA2B1C2 });
      ArrayDividedSample twoMultiA2B2 = new ArrayDividedSample(new IPlainSample[] { groupA2B2C1, groupA2B2C2 });
      ArrayDividedSample twoMultiA2B3 = new ArrayDividedSample(new IPlainSample[] { groupA2B3C1, groupA2B3C2 });

      ArrayComplexSample twoSet = new ArrayComplexSample(new IDividedSample[] { twoMultiA1B1, twoMultiA1B2, twoMultiA1B3, twoMultiA2B1, twoMultiA2B2, twoMultiA2B3 });
      FisherMetrics f2 = FisherCriteria.CalculateMultiplyCriteria(twoSet);

      ArrayDividedSample oneMultiA1B1C1 = new ArrayDividedSample(new IPlainSample[] { groupA1B1C1 });
      ArrayDividedSample oneMultiA1B1C2 = new ArrayDividedSample(new IPlainSample[] { groupA1B1C2 });
      ArrayDividedSample oneMultiA1B2C1 = new ArrayDividedSample(new IPlainSample[] { groupA1B2C1 });
      ArrayDividedSample oneMultiA1B2C2 = new ArrayDividedSample(new IPlainSample[] { groupA1B2C2 });
      ArrayDividedSample oneMultiA1B3C1 = new ArrayDividedSample(new IPlainSample[] { groupA1B3C1 });
      ArrayDividedSample oneMultiA1B3C2 = new ArrayDividedSample(new IPlainSample[] { groupA1B3C2 });

      ArrayDividedSample oneMultiA2B1C1 = new ArrayDividedSample(new IPlainSample[] { groupA2B1C1 });
      ArrayDividedSample oneMultiA2B1C2 = new ArrayDividedSample(new IPlainSample[] { groupA2B1C2 });
      ArrayDividedSample oneMultiA2B2C1 = new ArrayDividedSample(new IPlainSample[] { groupA2B2C1 });
      ArrayDividedSample oneMultiA2B2C2 = new ArrayDividedSample(new IPlainSample[] { groupA2B2C2 });
      ArrayDividedSample oneMultiA2B3C1 = new ArrayDividedSample(new IPlainSample[] { groupA2B3C1 });
      ArrayDividedSample oneMultiA2B3C2 = new ArrayDividedSample(new IPlainSample[] { groupA2B3C2 });

      ArrayComplexSample oneSet = new ArrayComplexSample(new IDividedSample[] { oneMultiA1B1C1, oneMultiA1B1C2, oneMultiA1B2C1, oneMultiA1B2C2, oneMultiA1B3C1, oneMultiA1B3C2, oneMultiA2B1C1, oneMultiA2B1C2, oneMultiA2B2C1, oneMultiA2B2C2, oneMultiA2B3C1, oneMultiA2B3C2 });
      FisherMetrics f1 = FisherCriteria.CalculateMultiplyCriteria(oneSet);

      Assert.AreEqual(f3.Ndf, f2.Ndf);
      Assert.AreEqual(f3.MSw, f2.MSw);

      Assert.AreEqual(f2.Ndf, f1.Ndf);
      Assert.AreEqual(f2.MSw, f1.MSw);
    }
    [TestMethod]
    public void Dospehov228pageTest()
    {
      ArrayPlainSample groupA0B0 = new ArrayPlainSample(new double[] { 24.1, 25.8, 23, 27 });
      ArrayPlainSample groupA0B1 = new ArrayPlainSample(new double[] { 28.4, 29.7, 30.1, 27.4 });
      ArrayPlainSample groupA0B2 = new ArrayPlainSample(new double[] { 28.7, 30.4, 32, 27 });
      ArrayDividedSample multiA0 = new ArrayDividedSample(new IPlainSample[] { groupA0B0, groupA0B1, groupA0B2 });

      ArrayPlainSample groupA1B0 = new ArrayPlainSample(new double[] { 30.7, 34.4, 34, 31 });
      ArrayPlainSample groupA1B1 = new ArrayPlainSample(new double[] { 46.7, 45.5, 47.1, 46.3 });
      ArrayPlainSample groupA1B2 = new ArrayPlainSample(new double[] { 59.4, 50.7, 64.5, 60.1 });
      ArrayDividedSample multiA1 = new ArrayDividedSample(new IPlainSample[] { groupA1B0, groupA1B1, groupA1B2 });

      ArrayComplexSample set = new ArrayComplexSample(new IDividedSample[] { multiA0, multiA1 });
      FisherMetrics f = FisherCriteria.CalculateMultiplyCriteria(set);
      Assert.AreEqual(Math.Round(249.88), Math.Round(f.F));
    }
  }
}
