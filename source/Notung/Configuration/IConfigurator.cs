using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notung.Configuration
{
  public interface IConfigurator
  {
    TSection GetSection<TSection>() where TSection : ConfigurationSection, new();

    void SaveSettings();
  }
}
