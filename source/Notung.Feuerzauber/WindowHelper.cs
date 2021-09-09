using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Windows.Navigation;
using Notung.Feuerzauber.Dialogs;

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

      if (e.ClickCount == 1 || !checkDoubleClick)
      {
        if (window.WindowState == WindowState.Normal)
          window.DragMove();
      }
      else if (window.WindowState == WindowState.Normal)
        window.WindowState = WindowState.Maximized;
      else
        window.WindowState = WindowState.Normal;
    }

    public static readonly ICommand Minimize = new MinimizeWindowCommand();
    public static readonly ICommand Maximize = new MaximizeWindowCommand();
    public static readonly ICommand Close = new CloseWindowCommand();
    public static readonly ICommand About = new ShowAboutCommand();

    private class WindowCommandBase 
    {
      public virtual event EventHandler CanExecuteChanged
      {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
      }

      public virtual bool CanExecute(object parameter)
      {
        return parameter is Window;
      }

      public override string ToString()
      {
        var ret = this.GetType().Name;
        return ret.Substring(0, ret.IndexOf("WindowCommand"));
      }
    }

    private class MinimizeWindowCommand : WindowCommandBase, ICommand
    {
      public void Execute(object parameter)
      {
        ((Window)parameter).WindowState = WindowState.Minimized;
      }
    }

    private class MaximizeWindowCommand : WindowCommandBase, ICommand
    {
      public void Execute(object parameter)
      {
        var window = ((Window)parameter);

        window.WindowState = window.WindowState == WindowState.Maximized ? 
          WindowState.Normal : WindowState.Maximized;
      }
    }

    private class CloseWindowCommand : WindowCommandBase, ICommand
    {
      public void Execute(object parameter)
      {
        ((Window)parameter).Close();
      }
    }

    private class ShowAboutCommand : WindowCommandBase, ICommand
    {
      public override bool CanExecute(object parameter)
      {
        return parameter == null || base.CanExecute(parameter);
      }

      public void Execute(object parameter)
      {
        var window = parameter as Window ?? Application.Current.MainWindow;

        new AboutBox { Owner = window }.ShowDialog();
      }
    }
  }
}
