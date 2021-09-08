using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Win32;
using Notung.Feuerzauber;
using Notung.Feuerzauber.Controls;
using Notung.Feuerzauber.Dialogs;

namespace LogAnalyzer
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void buttonClose_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void WindowHeader_MouseDown(object sender, MouseButtonEventArgs e)
    {
      WindowHelper.HeaderMouseDown(e, true);
    }

    private void OpenConfig_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();

      dialog.Filter = "Configuration files|*.config";

      if (dialog.ShowDialog(this) == true)
        m_context.OpenConfig(dialog.FileName);
    }

    private void OpenDirectory_Click(object sender, RoutedEventArgs e)
    {
      SelectFolderDialog dlg = new SelectFolderDialog() { Owner = this };

      if (dlg.ShowDialog() == true)
        this.DisplayLog(m_context.OpenDirectory(dlg.Tree.SelectedValue.ToString()));
    }

    private void DisplayLog(FileEntry entry)
    {
      if (entry == null)
        return;

      m_mdi_manager.Presenter.ActivateWindow(() =>
        new MdiChild
        (
          new LogDisplay { DataContext = entry },
          entry.ToString(),
          Properties.Resources.Document
        ) { Tag = entry.FileName },
        item => object.Equals(item.Tag, entry.FileName));
    }

    private void Context_ExceptionOccured(object sender, ExceptionEventArgs e)
    {
      MessageDialog.Show(e.Error, this.Title, MessageBoxImage.Error, MessageBoxButton.OK, this);
    }

    private void OpenSingleLog_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();

      dialog.Filter = "Log files|*.log";

      if (dialog.ShowDialog(this) == true)
        this.DisplayLog(m_context.OpenLog(dialog.FileName));
    }

    private void buttonMinimize_Click(object sender, RoutedEventArgs e)
    {
      this.WindowState = WindowState.Minimized;
    }

    private void buttonMaximize_Click(object sender, RoutedEventArgs e)
    {
      this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
      new AboutBox { Owner = this }.ShowDialog();
    }
  }
}