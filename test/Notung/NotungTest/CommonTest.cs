using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notung.Threading;
using Notung;
using Notung.Log;

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


    [TestMethod]
    public void SetContext()
    {
      LoggingContext.Global["RW"] = "Composer";

      LoggingData evt = new LoggingData("TEST", "MSG", InfoLevel.Info, null);
      Assert.AreEqual("Composer", evt["RW"]);

      LoggingContext.Thread["RW"] = "Stream";
      Assert.AreEqual("Stream", evt["RW"]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CheckReserved()
    {
      LoggingContext.Global["Source"] = "TEST";
    }

    [TestMethod]
    public void SaveContext()
    {
      LoggingData data = default(LoggingData);

      LoggingContext.Thread["RW"] = "Composer";

      Thread parallel = new Thread(() =>
        {
          data = new LoggingData("TEST", "MSG", InfoLevel.Info, null);
          LoggingContext.Thread["RW"] = "Stream";
        });

      parallel.Start();
      parallel.Join();

      Assert.AreEqual("Stream", data["RW"]);
    }

    [TestMethod]
    public void LogStringBuilderTest()
    {
      var bldr = new LogStringBuilder("Summa \\{1} and {SUKA} and {RUKA: 67612} RRR");
      var bldr2 = new LogStringBuilder("Summa \\{1} and {SUKA} and {RUKA: 67612} RRR{Message}");
    }
  }
}
