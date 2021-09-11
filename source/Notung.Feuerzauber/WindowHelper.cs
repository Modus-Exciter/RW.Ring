using System;
using System.Windows;
using System.Windows.Input;
using Notung.Feuerzauber.Dialogs;

namespace Notung.Feuerzauber
{
  public static class WindowHelper
  {
    public static readonly ICommand Minimize = new MinimizeWindowCommand();
    public static readonly ICommand Maximize = new MaximizeWindowCommand();
    public static readonly ICommand DragMove = new DragMoveCommand();
    public static readonly ICommand Close = new CloseWindowCommand();
    public static readonly ICommand CloseYes = new CloseDialogCommand(true);
    public static readonly ICommand CloseNo = new CloseDialogCommand(false);
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
        return parameter is DependencyObject && 
          Window.GetWindow((DependencyObject)parameter) != null;
      }

      public override string ToString()
      {
        var name = this.GetType().Name;
        return name.Substring(0, name.IndexOf("Command"));
      }
    }

    private class DragMoveCommand : WindowCommandBase, ICommand
    {
      public void Execute(object parameter)
      {
        Window.GetWindow((DependencyObject)parameter).DragMove();
      }
    }

    private class MinimizeWindowCommand : WindowCommandBase, ICommand
    {
      public void Execute(object parameter)
      {
        Window.GetWindow((DependencyObject)parameter).WindowState = WindowState.Minimized;
      }
    }

    private class MaximizeWindowCommand : WindowCommandBase, ICommand
    {
      public void Execute(object parameter)
      {
        var window = Window.GetWindow((DependencyObject)parameter);

        window.WindowState = window.WindowState == WindowState.Maximized ? 
          WindowState.Normal : WindowState.Maximized;
      }
    }

    private class CloseWindowCommand : WindowCommandBase, ICommand
    {
      public void Execute(object parameter)
      {
        Window.GetWindow((DependencyObject)parameter).Close();
      }
    }

    private class CloseDialogCommand : WindowCommandBase, ICommand
    {
      private readonly bool m_result;

      public CloseDialogCommand(bool dialogResult)
      {
        m_result = dialogResult;
      }

      public void Execute(object parameter)
      {
        Window.GetWindow((DependencyObject)parameter).DialogResult = m_result;
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
        Window window = null;

        if (parameter is DependencyObject)
          window = Window.GetWindow((DependencyObject)parameter);

        if (window == null)
          window = Application.Current.MainWindow;

        new AboutBox { Owner = window }.ShowDialog();
      }
    }
  }
}