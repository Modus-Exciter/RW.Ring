using System;
using Notung;
using Notung.ComponentModel;
using Notung.Logging;
using Notung.Services;

[assembly: LibraryInitializer(typeof(PluginsTest.Cust))]

namespace PluginsTest
{
  public class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Scanning...");
      AppManager.AssemblyClassifier.PluginsDirectory = @"Plugins";
      AppManager.AssemblyClassifier.LoadPlugins("*.adapter", LoadPluginsMode.DomainPerPlugin);
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
        info.InnerMessages.Add(string.Format("{0}, {1}", plugin.Name, plugin.AssemblyName), InfoLevel.Debug);

      AppManager.Notificator.Show(info);

      AppManager.AssemblyClassifier.Plugins[0].Unload();

      Console.WriteLine("Plugin unloaded. Left {0}", AppManager.AssemblyClassifier.Plugins.Count);

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

      AppManager.AssemblyClassifier.ShareServices(newDomain);

      newDomain.DoCallBack(() =>
        {
          Console.WriteLine(AppManager.Instance.CurrentProcess.MainModule.FileName);
#if APPLICATION_INFO
          Console.WriteLine(ApplicationInfo.Instance);
#endif
          LogManager.GetLogger("").Info("Mesage from new domain");
          LogManager.GetLogger("").Error("Test error", new Exception("Test error!"));
          LogManager.GetLogger("").Alert(new Info("Alert cust", InfoLevel.Info) { Details = new Cust() });
          AppManager.Notificator.Show(new Info("OK!", InfoLevel.Info) { Details = new Cust() });
          Console.WriteLine(LoggingContext.Global["Note"]);
        });

      IServiceProvider source = (IServiceProvider)newDomain.CreateInstanceAndUnwrap(
        typeof(InfoLogSource).Assembly.FullName, typeof(InfoLogSource).FullName);

      Console.WriteLine(source.GetService<Info>());

      AppDomain.Unload(newDomain);
    }
  }


  public class InfoLogSource : MarshalByRefObject, IServiceProvider, IRunBase
  {

    public object GetService(Type serviceType)
    {
      if (serviceType == typeof(Info))
        return new Info("Some message", InfoLevel.Info) { Details = new Cust() };

      else return null;
    }

    void IRunBase.Run()
    {
      
    }

    event System.ComponentModel.ProgressChangedEventHandler IRunBase.ProgressChanged
    {
      add {  }
      remove {  }
    }
  }

  public class Cust
  {
    static Cust()
    {
      Console.WriteLine("Domain {0} initilizing...", AppDomain.CurrentDomain.FriendlyName);
    }
  }
}
