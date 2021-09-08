using System;
using System.Windows.Controls;

namespace Notung.Feuerzauber.Controls
{
  /// <summary>
  /// Interaction logic for MdiManager.xaml
  /// </summary>
  public partial class MdiManager : UserControl
  {
    public MdiManager()
    {
      InitializeComponent();
    }

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      Dispatcher.BeginInvoke(new Action<object>(listBox.ScrollIntoView), listBox.SelectedItem);
    }
  }
}
