using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Notung.Configuration
{
  public abstract class ConfigurationSection
  {
    [OnDeserializing]
    private void OnDeserializing(StreamingContext context)
    {
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
    }
  }
}
