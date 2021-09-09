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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Notung.Feuerzauber.Controls
{
  /// <summary>
  /// Логика взаимодействия для DialogButtonPanel.xaml
  /// </summary>
  public partial class DialogButtonPanel : UserControl
  {
    public DialogButtonPanel()
    {
      InitializeComponent();
    }

    private void PositiveButton_Click(object sender, RoutedEventArgs e)
    {
      Window.GetWindow(this).DialogResult = true;
    }

    private void NegativeButton_Click(object sender, RoutedEventArgs e)
    {
      Window.GetWindow(this).DialogResult = false;
    }
  }
}
