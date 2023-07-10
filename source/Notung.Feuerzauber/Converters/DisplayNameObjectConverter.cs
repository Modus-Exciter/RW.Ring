using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Notung.Feuerzauber.Converters
{
   [ValueConversion(typeof(object), typeof(string))]
    /// <summary>
    /// Создание  обьекта на основании обьекта Type 
    /// </summary>
    public class DisplayNameObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value.GetType();
            DisplayNameAttribute ana = type.GetCustomAttribute<DisplayNameAttribute>();
            if (ana != null)
            {
                return ana.DisplayName; 
            }
            return type.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
           
            return null;
        }
    }
}
