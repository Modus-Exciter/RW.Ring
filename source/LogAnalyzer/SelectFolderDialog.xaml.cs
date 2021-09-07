using Notung.Feuerzauber.Dialogs;
using System.Windows;
using System.Windows.Input;

namespace LogAnalyzer
{
  /// <summary>
  /// Логика взаимодействия для SelectFolderDialog.xaml
  /// </summary>
  public partial class SelectFolderDialog : Window
  {
    public SelectFolderDialog()
    {
      InitializeComponent();
    }

    private void Header_MouseDown(object sender, MouseButtonEventArgs e)
    {
      this.DragMove();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      var error = ((SelectFolderContext)this.DataContext).GetErrorText();

      if (string.IsNullOrEmpty(error))
        this.DialogResult = true;
      else
        MessageDialog.Show(error, image: MessageBoxImage.Warning);
    }
  }
}