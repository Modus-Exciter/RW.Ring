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
using System.Diagnostics;

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

    [TestMethod]
    public void InfoString()
    {
      InfoLevel l1 = InfoLevel.Info;
      InfoLevel l2 = InfoLevel.Info;

      Assert.IsTrue(ReferenceEquals(l1.ToString(), l2.ToString()));
    }

    [TestMethod]
    public void DefaultDateFormat()
    {
      LoggingData evt = new LoggingData("TEST", "MSG", InfoLevel.Info, null);
      var builder = new LogStringBuilder("{Date}");

      var sb = new StringBuilder();
      using (var sw = new StringWriter(sb))
      {
        builder.BuildString(sw, evt);
      }

      Assert.AreEqual(evt.LoggingDate.ToString("dd.MM.yyyy HH:mm:ss.fff"), sb.ToString());
    }

    [TestMethod]
    public void CustomDateFormat()
    {
      LoggingData evt = new LoggingData("TEST", "MSG", InfoLevel.Info, null);
      var builder = new LogStringBuilder("{Date:dd.MM.yyyy HH:mm:ss}");

      var sb = new StringBuilder();
      using (var sw = new StringWriter(sb))
      {
        builder.BuildString(sw, evt);
      }

      Assert.AreEqual(evt.LoggingDate.ToString("dd.MM.yyyy HH:mm:ss"), sb.ToString());
    }

    [TestMethod]
    public void ProcessAndThread()
    {
      LoggingData evt = new LoggingData("TEST", "MSG", InfoLevel.Info, null);
      var builder = new LogStringBuilder("P:{Process}, T:{Thread}");

      var sb = new StringBuilder();
      using (var sw = new StringWriter(sb))
      {
        builder.BuildString(sw, evt);
      }

      Assert.AreEqual(string.Format("P:{0}, T:{1} {2}", 
        Process.GetCurrentProcess().Id, 
        Thread.CurrentThread.ManagedThreadId,
        Thread.CurrentThread.Name), sb.ToString());
    }

    [TestMethod]
    public void ProcessAndThreadWithoutName()
    {
      LoggingData evt = default(LoggingData);
      Thread parallel = new Thread(() =>
      {
        evt = new LoggingData("TEST", "MSG", InfoLevel.Info, null);
      });

      parallel.Start();
      parallel.Join();

      var builder = new LogStringBuilder("P:{Process}, T:{Thread}");

      var sb = new StringBuilder();
      using (var sw = new StringWriter(sb))
      {
        builder.BuildString(sw, evt);
      }

      Assert.AreEqual(string.Format("P:{0}, T:{1}",
        Process.GetCurrentProcess().Id,
        parallel.ManagedThreadId), sb.ToString());
    }
  }
}
