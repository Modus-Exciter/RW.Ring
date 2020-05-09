using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Notung.Configuration;

namespace Notung
{
  public static class AppManager
  {
    private static IConfigurator _configurator = new DataContractConfigurator();
    private static IAppInstance _app_instance = new AppInstance(new ProcessAppInstanceView());

    public static IConfigurator Configurator
    {
      get { return _configurator; }
      set
      {
        if (value == null)
          throw new ArgumentNullException();

        _configurator = value;
      }
    }

    public static IAppInstance Instance
    {
      get { return _app_instance; }
      set
      {
        if (value == null)
          throw new ArgumentNullException();

        _app_instance = value;
      }
    }
  }
}
