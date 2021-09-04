using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace LogAnalyzer
{
  public class GridConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is DataGrid && targetType == typeof(Thickness))
      {
        var presenter = FindVisualChild<DataGridColumnHeadersPresenter>((DataGrid)value);
        return new Thickness(0, -presenter.ActualHeight , 0, 0);
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
