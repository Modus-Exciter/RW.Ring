using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schicksal.Basic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ANOVATest
{
  [TestClass]
  public class FactorInfoTest
  {
    [TestMethod]
    public void SplitFactors()
    {
      var fi = FactorInfo.Parse("A+B+C");
      var fia = new HashSet<FactorInfo>(fi.Split());

      Assert.AreEqual(7, fia.Count);
      Assert.IsTrue(fia.Contains(FactorInfo.Parse("A")));
      Assert.IsTrue(fia.Contains(FactorInfo.Parse("B")));
      Assert.IsTrue(fia.Contains(FactorInfo.Parse("C")));
      Assert.IsTrue(fia.Contains(FactorInfo.Parse("A+B")));
      Assert.IsTrue(fia.Contains(FactorInfo.Parse("A+C")));
      Assert.IsTrue(fia.Contains(FactorInfo.Parse("B+C")));
      Assert.IsTrue(fia.Contains(fi));
      Assert.IsFalse(fia.Contains(new FactorInfo(Enumerable.Empty<string>())));
    }

    [TestMethod]
    public void PlusMinus()
    {
      var fi = FactorInfo.Parse("A") + FactorInfo.Parse("B") + FactorInfo.Parse("C");

      Assert.AreEqual(FactorInfo.Parse("B + C + A"), fi);

      var fi2 = fi - FactorInfo.Parse("C+ D");

      Assert.AreEqual(FactorInfo.Parse("A+B"), fi2);
      Assert.AreEqual(FactorInfo.Empty, fi2 - FactorInfo.Parse("B+A "));
    }
  }
}
