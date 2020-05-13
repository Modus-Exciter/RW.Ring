using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notung.Threading;
using Notung;
using Notung.Log;
using System.Text;
using System.IO;

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

      Assert.AreEqual("Summa \\{1} and {SUKA} and {RUKA: 67612} RRR{Message}", bldr2.ToString());
    }

    [TestMethod]
    public void EmptyFormat()
    {
      var builder = new LogStringBuilder("RW = {RW:}.");
      LoggingContext.Global["RW"] = 123;

      LoggingData evt = new LoggingData("TEST", "MSG", InfoLevel.Info, null);

      var sb = new StringBuilder();

      using (var sw = new StringWriter(sb))
      {
        builder.BuildString(sw, evt);
      }

      Assert.AreEqual("RW = 123.", sb.ToString());
    }

    [TestMethod]
    public void EscapeFormat()
    {
      var builder = new LogStringBuilder("RW = {RW:}.\\{MH}");
      LoggingContext.Global["RW"] = 123;
      LoggingContext.Global["MH"] = "BERRO";

      LoggingData evt = new LoggingData("TEST", "MSG", InfoLevel.Info, null);

      var sb = new StringBuilder();

      using (var sw = new StringWriter(sb))
      {
        builder.BuildString(sw, evt);
      }

      Assert.AreEqual("RW = 123.{MH}", sb.ToString());
    }

    [TestMethod]
    public void UnEscapeFormat()
    {
      var builder = new LogStringBuilder("RW = {RW:}.\\{{MH}}");
      LoggingContext.Global["RW"] = 123;
      LoggingContext.Global["MH"] = "BERRO";

      LoggingData evt = new LoggingData("TEST", "MSG", InfoLevel.Info, null);

      var sb = new StringBuilder();

      using (var sw = new StringWriter(sb))
      {
        builder.BuildString(sw, evt);
      }

      Assert.AreEqual("RW = 123.{BERRO}", sb.ToString());
    }
  }
}
