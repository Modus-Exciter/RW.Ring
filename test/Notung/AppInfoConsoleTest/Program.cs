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
#if APPLICATION_INFO
      Console.WriteLine(ApplicationInfo.Instance);
#endif
      AppManager.OperationLauncher.SyncWaitingTime = new TimeSpan(0, 0, 1);
      CancellationTokenSource src = new CancellationTokenSource();

      var domain = AppDomain.CreateDomain("Parallel");
      AppManager.Share(domain);

#if ANOTHER_DOMAIN
      var wrk = (ICancelableRunBase)domain.CreateInstanceAndUnwrap(
        typeof(Program).Assembly.FullName, typeof(TestWork).FullName);
#else
      var wrk = new TestWork();
#endif
      wrk.CancellationToken = src.Token;

      src.Cancel();
      Console.WriteLine(AppManager.OperationLauncher.Run(wrk, null));

#if ANOTHER_DOMAIN
      wrk = (ICancelableRunBase)domain.CreateInstanceAndUnwrap(
        typeof(Program).Assembly.FullName, typeof(TestWork).FullName);
#endif
      wrk.CancellationToken = CancellationToken.None;

      Console.WriteLine(AppManager.OperationLauncher.Run(wrk, null));

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
