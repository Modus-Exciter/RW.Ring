using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Notung.Feuerzauber.Converters
{
    /// <summary>
    /// Преобразование Bool в GridLength (Скрытие GridRow) 
    /// </summary>
    [ValueConversion(typeof(bool), typeof(GridLength))]
    public class BoolToGridRowHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Double dvalue = 0;
            if(parameter != null)
                dvalue = System.Convert.ToDouble(parameter);
            return ((bool)value == true) ? dvalue > 0 ? new GridLength(dvalue, GridUnitType.Pixel) : dvalue < 0 ? new GridLength(Math.Abs(dvalue), GridUnitType.Auto) : new GridLength(1, GridUnitType.Auto)  : new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {    // Don't need any convert back
            return null;
        }
    }
}
