using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Notung.Feuerzauber.Dialogs
{
  /// <summary>
  /// Логика взаимодействия для MessageDialog.xaml
  /// </summary>
  public sealed partial class MessageDialog : Window
  {
    private MessageDialog()
    {
      InitializeComponent();
    }

    public static bool? Show(string message, 
      string title = null,
      MessageBoxImage image = MessageBoxImage.None,
      MessageBoxButton button = MessageBoxButton.OK,
      Window parent = null)
    {
      if (parent == null)
        parent = Application.Current.MainWindow;

      if (title == null)
        title = Application.Current.MainWindow.Title;

      MessageDialog dlg = new MessageDialog();
      dlg.Context.Buttons = button;
      dlg.Context.Image = image;
      dlg.Context.Message = message;
      dlg.Context.Title = title;
      dlg.Owner = parent;

      return dlg.ShowDialog();
    }

    private void Header_MouseDown(object sender, MouseButtonEventArgs e)
    {
      this.DragMove();
    }

    private void PositiveButton_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }

    private void NegativeButton_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
    }
  }
}
