using Notung.Feuerzauber.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FeuerzauberGraphicalTest.Configuration
{
    
    public class TestPage2 : ConfigurationPage
    {
        public override string Title => "Автоматическая тестовая страница";

        public override Image Image => Properties.Resources.DOS_TRACK;
        public override bool UIThreadValidation => true;

        public override void InitializationSections(List<Type> configurationSections)
        {
            configurationSections.Add(typeof( OuterSectionDataContract));
            configurationSections.Add(typeof(  OuterSectionXml));
        }
    }
}
