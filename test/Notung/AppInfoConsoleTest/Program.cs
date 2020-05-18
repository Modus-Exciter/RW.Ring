using System;
using Notung;
using Notung.Threading;
using System.Threading;

namespace AppInfoConsoleTest
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine(ApplicationInfo.Instance);

      AppManager.TaskManager.SyncWaitingTime = new TimeSpan(0, 0, 1);
      CancellationTokenSource src = new CancellationTokenSource();

      var domain = AppDomain.CreateDomain("Parallel");
      AppManager.Share(domain);

      var wrk = (ICancelableRunBase)domain.CreateInstanceAndUnwrap(
        typeof(Program).Assembly.FullName, typeof(TestWork).FullName);

      wrk.CancellationToken = src.Token;

      src.Cancel();
      Console.WriteLine(AppManager.TaskManager.Run(wrk, null));

      wrk = (ICancelableRunBase)domain.CreateInstanceAndUnwrap(
        typeof(Program).Assembly.FullName, typeof(TestWork).FullName);

      wrk.CancellationToken = null;

      Console.WriteLine(AppManager.TaskManager.Run(wrk, null));

      Console.ReadKey();
    }

    [PercentNotification]
    private class TestWork : CancelableRunBase
    {
      public override void Run()
      {
        this.ReportProgress("Some state");
        for (int i = 1; i <= 100; i++)
        {
          this.ReportProgress(i);
          Thread.Sleep(50);

          if (i == 33)
            this.CancellationToken.ThrowIfCancellationRequested();
        }
      }
    }
  }
}
