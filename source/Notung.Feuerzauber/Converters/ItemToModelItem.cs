using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Notung.Feuerzauber.Converters
{
    /// <summary>
    /// Преобразование объекта в структуру описывающий класс исходного объекта
    /// </summary>
    [ValueConversion(typeof(object), typeof(ModelItem))]
    public class ItemToModelItem : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;
            var type = value.GetType();
            return type.GetProperties().ToList().ConvertAll(x => {
               


                return new ModelItem(x, value);


            });
        
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ModelItem mi)
                return mi.Source;
            return value;
        }
    }

    public class ModelItem
    {
        public ModelItem(PropertyInfo property, object source)
        {
            Property = property;
            var attr = property.GetCustomAttribute<DisplayNameAttribute>();
            string dn = string.Empty;
            if (attr != null)
            {
                DisplayName =  attr.DisplayName;
            }
            else
            {
                DisplayName = property.Name;
            }

            Source = source;
        }
             
        public PropertyInfo Property { get; set; }
        public string DisplayName { get; set; }
        public string Path { get => Property.Name; }
        public object Source { get; set; }
    } 
}
