using System;
using System.Collections.Generic;
using System.Linq;
using Notung;

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
      AppDomain newDomain = AppDomain.CreateDomain("Plugin domain");

      AppManager.SetupNewDomain(newDomain);

      ApplicationInfo.Instance.CurrentProcess.ToString();

      newDomain.DoCallBack(() =>
        {
          Console.WriteLine(AppManager.Instance.StartupPath);
          Console.WriteLine(ApplicationInfo.Instance);
        });

      AppDomain.Unload(newDomain);
    }
  }
}
