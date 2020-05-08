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
  }
}
