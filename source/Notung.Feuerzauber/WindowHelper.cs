using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace Notung.Feuerzauber
{
  public class WindowHelper
  {
    public static void HeaderMouseDown(MouseButtonEventArgs e, bool checkDoubleClick)
    {
      DependencyObject dob = e.Source as DependencyObject;

      if (dob == null)
        return;

      var window = Window.GetWindow(dob);

      if (e.ChangedButton != MouseButton.Left)
        return;

      if (checkDoubleClick)
      {
        if (e.ClickCount == 1)
        {
          if (window.WindowState == WindowState.Normal)
            window.DragMove();
        }
        else if (window.WindowState == WindowState.Normal)
          window.WindowState = WindowState.Maximized;
        else
          window.WindowState = WindowState.Normal;
      }
      else if (window.WindowState == WindowState.Normal)
        window.DragMove(); 
    }
  }
}
