using System.Windows;

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
  }
}
