using Notung.Feuerzauber.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeuerzauberGraphicalTest.Configuration
{
    public class ListLanguages : ListItemBase
    {
        public override IEnumerable<object> LoadingItems()
        {
            yield return "RU";
            yield return "EN";
        }
    }
}
