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

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {

            if (item is ModelItem mi)
            {
                var attr = mi.Property.GetCustomAttribute<TemplateViewAttribute>();

                if (attr == null || attr.Template == TemplatesView.None || attr.Template == TemplatesView.TextBox)
                    return TextTemplate;
               
                switch(attr.Template)
                {
                    case TemplatesView.CheckBox:
                        return CheckBoxTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
