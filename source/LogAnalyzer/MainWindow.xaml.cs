using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Win32;
using Notung.Feuerzauber;
using Notung.Feuerzauber.Controls;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace LogAnalyzer
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, System.Windows.Forms.IWin32Window
  {
    private readonly WindowInteropHelper m_helper;

    public MainWindow()
    {
      InitializeComponent();
      m_helper = new WindowInteropHelper(this);
    }

    IntPtr System.Windows.Forms.IWin32Window.Handle
    {
      get { return m_helper.Handle; }
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
      using (var dlg = new FolderBrowserDialog())
      {
        dlg.SelectedPath = m_context.GetDirectoryPath();

        if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
          this.DisplayLog(m_context.OpenDirectory(dlg.SelectedPath));
      }
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
          Properties.Resources.document_gear
        ) { Tag = entry.FileName },
        item => object.Equals(item.Tag, entry.FileName));
    }

    private void Context_ExceptionOccured(object sender, ExceptionEventArgs e)
    {
      MessageBox.Show(e.Error, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void OpenSingleLog_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();

      dialog.Filter = "Log files|*.log";

      if (dialog.ShowDialog(this) == true)
        this.DisplayLog(m_context.OpenLog(dialog.FileName));
    }
  }
}
