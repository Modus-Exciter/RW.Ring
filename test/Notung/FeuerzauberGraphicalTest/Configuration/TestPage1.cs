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

    [ConfigurationPageView(Type = typeof(TestPageView))]
    
    public class TestPage1 : ConfigurationPage
    {
        public override string Title => "Тестовая страница";

        public override Image Image => null; 
        public override bool UIThreadValidation => true;

        public override void InitializationSections(List<Type> configurationSections)
        {
          //  configurationSections.Add(typeof(OuterSectionDataContract));
          //  configurationSections.Add(typeof(OuterSectionXml));
        }
    }
}
