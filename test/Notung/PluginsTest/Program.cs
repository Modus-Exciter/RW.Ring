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

      InfoBuffer buffer = new InfoBuffer();

      foreach (var asm in AppManager.AssemblyClassifier.TrackingAssemblies)
        buffer.Add(asm.FullName, InfoLevel.Info);

      AppManager.Notificator.Show("Assemblies:", buffer);

      Console.WriteLine();

      buffer = new InfoBuffer();

      foreach (var plugin in AppManager.AssemblyClassifier.Plugins)
        buffer.Add(string.Format("{0}, {1}", plugin.Name, plugin.Assembly), InfoLevel.Debug);

      AppManager.Notificator.Show("Plugins:", buffer);

      if (AppManager.Notificator.Confirm(new Info("Show unmanaged?", InfoLevel.Warning)))
      {
        buffer = new InfoBuffer();

        foreach (var unm in AppManager.AssemblyClassifier.GetUnmanagedAsemblies())
          buffer.Add(new Info(unm, InfoLevel.Error));
        AppManager.Notificator.Show(null, buffer);

        Console.ReadKey();
      }
    }
  }
}
