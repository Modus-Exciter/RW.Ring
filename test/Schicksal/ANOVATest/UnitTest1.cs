using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Schicksal.Anova;
using Schicksal.Basic;

namespace ANOVATest
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void TestMethod1()
    {
      ArrayDataGroup groupA1 = new ArrayDataGroup(new double[] { 48, 49, 50 });
      ArrayDataGroup groupA2 = new ArrayDataGroup(new double[] { 53, 54, 55 });
      MultiArrayDataGroup multiA = new MultiArrayDataGroup(new IDataGroup[] { groupA1, groupA2 });

      ArrayDataGroup groupB1 = new ArrayDataGroup(new double[] { 18, 19, 20 });
      ArrayDataGroup groupB2 = new ArrayDataGroup(new double[] { 24, 25, 26 });
      MultiArrayDataGroup multiB = new MultiArrayDataGroup(new IDataGroup[] { groupB1, groupB2 });

      SetMultiArrayDataGroup set = new SetMultiArrayDataGroup(new IMultyDataGroup[] { multiA, multiB });

      FisherMetrics f = FisherCriteria.CalculateMultiplyCriteria(set);
      double fExp = 0.75;
      Assert.AreEqual(fExp, f.F);
    }
  }
}
