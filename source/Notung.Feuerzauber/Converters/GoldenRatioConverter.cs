using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Con = System.Convert;

namespace Notung.Feuerzauber.Converters
{
  public class GoldenRatioConverter : IValueConverter
  {
    public static readonly double Ratio = (Math.Sqrt(5) - 1) / 2;
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (true.Equals(parameter)) // increase
        return Con.ChangeType(Con.ToDouble(value, culture) / Ratio, targetType);
      else if (parameter == null || false.Equals(parameter))
        return Con.ChangeType(Con.ToDouble(value, culture) * Ratio, targetType);
      else
        return Con.ChangeType(Con.ToDouble(value, culture) * Con.ToDouble(parameter, culture), targetType);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (true.Equals(parameter)) // increase
        return Con.ChangeType(Con.ToDouble(value, culture) * Ratio, targetType);
      else if (parameter == null || false.Equals(parameter))
        return Con.ChangeType(Con.ToDouble(value, culture) / Ratio, targetType);
      else
        return Con.ChangeType(Con.ToDouble(value, culture) / Con.ToDouble(parameter, culture), targetType);
    }
  }
}
