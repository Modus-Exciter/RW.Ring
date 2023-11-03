using Notung.Feuerzauber.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Notung.Feuerzauber.Configuration
{
    public class SettingsItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextTemplate { get; set; }
        public DataTemplate CheckBoxTemplate { get; set; }
        public DataTemplate ComboBoxTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {

            if (item is ModelItem mi)
            {
              

                if (mi.Template == TemplatesView.None || mi.Template == TemplatesView.TextBox)
                    return TextTemplate;
               
                switch(mi.Template)
                {
                    case TemplatesView.CheckBox:
                        return CheckBoxTemplate;
                    case TemplatesView.ComboBox:
                        return ComboBoxTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
