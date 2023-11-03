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
            if (parameter != null)
                dvalue = System.Convert.ToDouble(parameter);

            if ((bool)value == true)
            {
                if(dvalue > 0)
                {
                    return new GridLength(dvalue, GridUnitType.Pixel);

                }
                else if (dvalue < 0)
                {
                    return new GridLength(Math.Abs(dvalue), GridUnitType.Auto);
                }
                else
                {
                    return  new GridLength(1, GridUnitType.Auto);
                }
            }
            return new GridLength(0);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {    // Don't need any convert back
            return null;
        }
    }
}
