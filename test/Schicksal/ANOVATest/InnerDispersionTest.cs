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
      ArrayDataGroup groupA1B1C1 = new ArrayDataGroup(new double[] { 2, 5, 4 });
      ArrayDataGroup groupA1B1C2 = new ArrayDataGroup(new double[] { 3, 6, 1 });
      ArrayDataGroup groupA1B2C1 = new ArrayDataGroup(new double[] { 2, 2, 3 });
      ArrayDataGroup groupA1B2C2 = new ArrayDataGroup(new double[] { 4, 7, 2 });
      ArrayDataGroup groupA1B3C1 = new ArrayDataGroup(new double[] { 2, 5, 6 });
      ArrayDataGroup groupA1B3C2 = new ArrayDataGroup(new double[] { 4, 8, 3 });
      MultiArrayDataGroup threeMultiA1 = new MultiArrayDataGroup(new IDataGroup[] { groupA1B1C1, groupA1B1C2, groupA1B2C1, groupA1B2C2, groupA1B3C1, groupA1B3C2 });

      //A2
      ArrayDataGroup groupA2B1C1 = new ArrayDataGroup(new double[] { 3, 5, 5 });
      ArrayDataGroup groupA2B1C2 = new ArrayDataGroup(new double[] { 6, 1, 2 });
      ArrayDataGroup groupA2B2C1 = new ArrayDataGroup(new double[] { 2, 3, 5 });
      ArrayDataGroup groupA2B2C2 = new ArrayDataGroup(new double[] { 4, 7, 5 });
      ArrayDataGroup groupA2B3C1 = new ArrayDataGroup(new double[] { 8, 1, 2 });
      ArrayDataGroup groupA2B3C2 = new ArrayDataGroup(new double[] { 4, 3, 4 });
      MultiArrayDataGroup threeMultiA2 = new MultiArrayDataGroup(new IDataGroup[] { groupA2B1C1, groupA2B1C2, groupA2B2C1, groupA2B2C2, groupA2B3C1, groupA2B3C2 });

      SetMultiArrayDataGroup threeSet = new SetMultiArrayDataGroup(new IMultyDataGroup[] { threeMultiA1, threeMultiA2 });
      FisherMetrics f3 = FisherCriteria.CalculateMultiplyCriteria(threeSet);

      MultiArrayDataGroup twoMultiA1B1 = new MultiArrayDataGroup(new IDataGroup[] { groupA1B1C1, groupA1B1C2 });
      MultiArrayDataGroup twoMultiA1B2 = new MultiArrayDataGroup(new IDataGroup[] { groupA1B2C1, groupA1B2C2 });
      MultiArrayDataGroup twoMultiA1B3 = new MultiArrayDataGroup(new IDataGroup[] { groupA1B3C1, groupA1B3C2 });

      MultiArrayDataGroup twoMultiA2B1 = new MultiArrayDataGroup(new IDataGroup[] { groupA2B1C1, groupA2B1C2 });
      MultiArrayDataGroup twoMultiA2B2 = new MultiArrayDataGroup(new IDataGroup[] { groupA2B2C1, groupA2B2C2 });
      MultiArrayDataGroup twoMultiA2B3 = new MultiArrayDataGroup(new IDataGroup[] { groupA2B3C1, groupA2B3C2 });

      SetMultiArrayDataGroup twoSet = new SetMultiArrayDataGroup(new IMultyDataGroup[] { twoMultiA1B1, twoMultiA1B2, twoMultiA1B3, twoMultiA2B1, twoMultiA2B2, twoMultiA2B3 });
      FisherMetrics f2 = FisherCriteria.CalculateMultiplyCriteria(twoSet);

      MultiArrayDataGroup oneMultiA1B1C1 = new MultiArrayDataGroup(new IDataGroup[] { groupA1B1C1 });
      MultiArrayDataGroup oneMultiA1B1C2 = new MultiArrayDataGroup(new IDataGroup[] { groupA1B1C2 });
      MultiArrayDataGroup oneMultiA1B2C1 = new MultiArrayDataGroup(new IDataGroup[] { groupA1B2C1 });
      MultiArrayDataGroup oneMultiA1B2C2 = new MultiArrayDataGroup(new IDataGroup[] { groupA1B2C2 });
      MultiArrayDataGroup oneMultiA1B3C1 = new MultiArrayDataGroup(new IDataGroup[] { groupA1B3C1 });
      MultiArrayDataGroup oneMultiA1B3C2 = new MultiArrayDataGroup(new IDataGroup[] { groupA1B3C2 });

      MultiArrayDataGroup oneMultiA2B1C1 = new MultiArrayDataGroup(new IDataGroup[] { groupA2B1C1 });
      MultiArrayDataGroup oneMultiA2B1C2 = new MultiArrayDataGroup(new IDataGroup[] { groupA2B1C2 });
      MultiArrayDataGroup oneMultiA2B2C1 = new MultiArrayDataGroup(new IDataGroup[] { groupA2B2C1 });
      MultiArrayDataGroup oneMultiA2B2C2 = new MultiArrayDataGroup(new IDataGroup[] { groupA2B2C2 });
      MultiArrayDataGroup oneMultiA2B3C1 = new MultiArrayDataGroup(new IDataGroup[] { groupA2B3C1 });
      MultiArrayDataGroup oneMultiA2B3C2 = new MultiArrayDataGroup(new IDataGroup[] { groupA2B3C2 });

      SetMultiArrayDataGroup oneSet = new SetMultiArrayDataGroup(new IMultyDataGroup[] { oneMultiA1B1C1, oneMultiA1B1C2, oneMultiA1B2C1, oneMultiA1B2C2, oneMultiA1B3C1, oneMultiA1B3C2, oneMultiA2B1C1, oneMultiA2B1C2, oneMultiA2B2C1, oneMultiA2B2C2, oneMultiA2B3C1, oneMultiA2B3C2 });
      FisherMetrics f1 = FisherCriteria.CalculateMultiplyCriteria(oneSet);

      Assert.AreEqual(f3.Ndf, f2.Ndf);
      Assert.AreEqual(f3.MSw, f2.MSw);

      Assert.AreEqual(f2.Ndf, f1.Ndf);
      Assert.AreEqual(f2.MSw, f1.MSw);
    }
  }
}
