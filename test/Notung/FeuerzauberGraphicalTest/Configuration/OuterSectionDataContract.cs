using Notung;
using Notung.Configuration;
using Notung.Feuerzauber.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FeuerzauberGraphicalTest.Configuration
{
    [DisplayName("Outer section data contract")]
    public class OuterSectionDataContract : ConfigurationSection
    {

        [DataMember]
        [DefaultValue(42)]
        [DisplayName("Номер")]
        [TemplateView (Template = TemplatesView.TextBox)]
        public int Number { get; set; }

        [DefaultValue("Text unserializable!")]
        [TemplateView(Template = TemplatesView.TextBox)]
        public string Text { get; set; }

        [DefaultValue(true)]
        [TemplateView(Template = TemplatesView.CheckBox)]
        public bool BoolValue { get; set; }

        [DisplayName("Язык")]
        [TemplateView(Template = TemplatesView.ComboBox)]
        [ListItem(TypeList = typeof(ListLanguages))]
        public string ListValue { get;
            set; }
        public override bool Validate(InfoBuffer buffer)
        {
            bool result = true;
          //  System.Threading.Thread.Sleep(7000);
            if (string.IsNullOrWhiteSpace(Text) ||( Text != null &&Text.Length < 3))
            {
                buffer.Add("Text cannot be shorter than 3 characters + Text cannot be shorter than 3 characters+ Text cannot be shorter than 3 characters+ Text cannot be shorter than 3 characters", InfoLevel.Warning);
                result =  false;

            }

            if (BoolValue == false)
            {
                buffer.Add("Bool value is false", InfoLevel.Warning);
                result = false;
            }
            if (this.Number < 1)
            {
                buffer.Add("Number must be more than 1", InfoLevel.Warning);
                result =  false;
            }
            if (string.IsNullOrWhiteSpace(ListValue))
            {
                buffer.Add("ListValue Null", InfoLevel.Warning);
                result = false;
            }

            return result;
        }
    }

}
