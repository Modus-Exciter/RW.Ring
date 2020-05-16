using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notung;
using Notung.Logging;

namespace NotungTest
{
  [TestClass]
  public class LoggingTest
  {
    [TestMethod]
    public void SetContext()
    {
      LoggingContext.Global["RW"] = "Composer";

      LoggingEvent evt = new LoggingEvent("TEST", "MSG", InfoLevel.Info, null);
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
      LoggingEvent data = default(LoggingEvent);

      LoggingContext.Thread["RW"] = "Composer";

      Thread parallel = new Thread(() =>
      {
        data = new LoggingEvent("TEST", "MSG", InfoLevel.Info, null);
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

      LoggingEvent evt = new LoggingEvent("TEST", "MSG", InfoLevel.Info, null);

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

      LoggingEvent evt = new LoggingEvent("TEST", "MSG", InfoLevel.Info, null);

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

      LoggingEvent evt = new LoggingEvent("TEST", "MSG", InfoLevel.Info, null);

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
      LoggingEvent evt = new LoggingEvent("TEST", "MSG", InfoLevel.Info, null);
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
      LoggingEvent evt = new LoggingEvent("TEST", "MSG", InfoLevel.Info, null);
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
      LoggingEvent evt = new LoggingEvent("TEST", "MSG", InfoLevel.Info, null);
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
      LoggingEvent evt = default(LoggingEvent);
      Thread parallel = new Thread(() =>
      {
        evt = new LoggingEvent("TEST", "MSG", InfoLevel.Info, null);
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
