using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace Notung.Feuerzauber.Converters
{
  public class GridConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (targetType == typeof(Thickness))
      {
        if (value is DataGrid)
        {
          var grid = (DataGrid)value;

          var presenter = FindVisualChild<DataGridColumnHeadersPresenter>(grid);

          return new Thickness(0, -presenter.ActualHeight, 0, 0);
        }
        else
          return new Thickness();
      }

      return null;
    }

    public static T FindVisualChild<T>(DependencyObject current) where T : DependencyObject
    {
      if (current == null)
        return null;

      int childrenCount = VisualTreeHelper.GetChildrenCount(current);

      for (int i = 0; i < childrenCount; i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(current, i);

        if (child is T)
          return (T)child;

        T result = FindVisualChild<T>(child);

        if (result != null)
          return result;
      }

      return null;
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
