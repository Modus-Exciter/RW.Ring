using Notung;
using Notung.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FeuerzauberGraphicalTest.Configuration
{
    [XmlRoot]
    [DisplayName("Outer section xml")]
    public class OuterSectionXml : ConfigurationSection
    {

        [DefaultValue(1)]
        public int Number { get; set; }

        [DefaultValue("Valhalla111")]
        public string Text { get; set; }

        public override bool Validate(InfoBuffer buffer)
        {

            bool result = true;

            if (this.Number < 1)
            {
                buffer.Add("Number must be more than 1", InfoLevel.Warning);
                result = false;
            }

            return result;
        }

    }

}
