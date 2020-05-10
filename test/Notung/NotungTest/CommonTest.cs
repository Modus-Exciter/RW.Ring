using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notung.Threading;
using Notung;

namespace NotungTest
{
  [TestClass]
  public class CommonTest
  {
    [TestMethod]
    public void ApartmentState()
    {
      Assert.AreEqual("hello, world!", AppManager.Instance.ApartmentWrapper.Invoke(() => "Hello, World!".ToLower()));
    }
   
  }
}
