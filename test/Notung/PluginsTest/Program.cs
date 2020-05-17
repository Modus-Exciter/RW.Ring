using System;
using Notung;
using Notung.Logging;

namespace PluginsTest
{
  public class Program
  {
    static void Main(string[] args)
    {
      AppManager.AssemblyClassifier.PluginsDirectory = @"Plugins";
      AppManager.AssemblyClassifier.LoadPlugins("*.adapter");
      AppManager.AssemblyClassifier.ExcludePrefixes.Add("vshost");
      AppManager.AssemblyClassifier.ExcludePrefixes.Remove("System");
      // AppManager.AssemblyClassifier.LoadDependencies(AppManager.AssemblyClassifier.Plugins[0].Assembly);
      InfoBuffer buffer = new InfoBuffer();

      LogManager.SetMainThreadInfo(new CurrentMainThreadInfo());

      foreach (var asm in AppManager.AssemblyClassifier.TrackingAssemblies)
        buffer.Add(asm.FullName, InfoLevel.Info);

      AppManager.Notificator.Show(buffer, "Assemblies:");

      Console.WriteLine();

      var info = new Info("Plugins:", InfoLevel.Fatal);

      foreach (var plugin in AppManager.AssemblyClassifier.Plugins)
        info.InnerMessages.Add(string.Format("{0}, {1}", plugin.Name, plugin.Assembly), InfoLevel.Debug);

      AppManager.Notificator.Show(info);

      if (AppManager.Notificator.Confirm(new Info("Show unmanaged?", InfoLevel.Warning)))
      {
        buffer = new InfoBuffer();

        foreach (var unm in AppManager.AssemblyClassifier.UnmanagedAsemblies)
          buffer.Add(new Info(unm, InfoLevel.Error));
        AppManager.Notificator.Show(buffer, null);

        DomainTest();

        Console.ReadKey();
      }
    }

    static void DomainTest()
    {
      LoggingContext.Global["Note"] = "Thread of fortune";

      AppDomain newDomain = AppDomain.CreateDomain("Plugin domain");

      AppManager.Share(newDomain);

      newDomain.DoCallBack(() =>
        {
          Console.WriteLine(AppManager.Instance.StartupPath);
          Console.WriteLine(ApplicationInfo.Instance);
          LogManager.GetLogger("").Info("Mesage from new domain");
          LogManager.GetLogger("").Error("Test error", new Exception("Test error!"));
          LogManager.GetLogger("").Alert(new Info("Alert cust", InfoLevel.Info) { Details = new Cust() });
          AppManager.Notificator.Show(new Info("OK!", InfoLevel.Info) { Details = new Cust() });
          Console.WriteLine(LoggingContext.Global["Note"]);
        });

      AppDomain.Unload(newDomain);
    }
  }

  public class Cust { }
}
