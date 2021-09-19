using System.Windows;
using System.Windows.Controls;

namespace Notung.Feuerzauber.Controls
{
  /// <summary>
  /// Логика взаимодействия для DialogHeader.xaml
  /// </summary>
  public partial class DialogHeader : UserControl
  {
    public DialogHeader()
    {
      this.InitializeComponent();
    }

    public bool ShowCloseButton
    {
      get { return closeButton.Visibility == Visibility.Visible; }
      set
      {
        if (value)
          closeButton.Visibility = Visibility.Visible;
        else
          closeButton.Visibility = Visibility.Collapsed;
      }
    }
  }
}
