using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notung.Feuerzauber.Configuration
{
   
    public class ConfigurationSectionViewAttribute : Attribute
    {
        public Type Type { get; set; }
    }
}
