using System;
using System.Windows.Controls;

namespace Notung.Feuerzauber.Controls
{
  /// <summary>
  /// Interaction logic for MdiManager.xaml
  /// </summary>
  public partial class MdiManager : UserControl
  {
    private readonly Action<object> m_scroll;

    public MdiManager()
    {
      InitializeComponent();

      m_scroll = listBox.ScrollIntoView;
    }

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      Dispatcher.BeginInvoke(m_scroll, listBox.SelectedItem);
    }
  }
}
