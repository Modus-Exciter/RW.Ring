using System;
using Notung.Configuration;
using Notung.Loader;
using Notung.Log;

namespace Notung
{
  public static class AppManager
  {
    private static IConfigurator _configurator;
    private static IAppInstance _app_instance;
    private static IAssemblyClassifier _asm_classifier;

    private static readonly object _lock = new object();

    private static T InitService<T>(ref T field, Func<T> creator)
    {
      lock (_lock)
      {
        if (field == null)
          field = creator();

        return field;
      }
    }

    public static IConfigurator Configurator
    {
      get
      {
        return _configurator ?? InitService(ref _configurator, () => new DataContractConfigurator());
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();

        _configurator = value;
      }
    }

    public static IAppInstance Instance
    {
      get
      {
        return _app_instance ?? InitService(ref _app_instance, () => new AppInstance());
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();

        _app_instance = value;
      }
    }

    public static IAssemblyClassifier AssemblyClassifier
    {
      get
      {
        return _asm_classifier ?? InitService(ref _asm_classifier, () => new AssemblyClassifier());
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();

        lock (_lock)
        {
          _asm_classifier.Dispose();
          _asm_classifier = value;
        }
      }
    }
  }
}
