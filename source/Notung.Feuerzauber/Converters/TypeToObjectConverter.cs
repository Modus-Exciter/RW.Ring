using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Notung.Feuerzauber.Converters
{
    [ValueConversion(typeof(Type), typeof(object))]
    /// <summary>
    /// Создание  обьекта на основании обьекта Type 
    /// </summary>
    public class TypeToObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null && value is Type tp)
            {
                return Activator.CreateInstance(tp);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return value.GetType();
            return null;
        }
    }
}
