using Microsoft.Win32;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
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

    public IntPtr Handle
    {
      get { return m_helper.Handle; }
    }

    private TablePresenter Presenter
    {
      get { return ((TablePresenter)this.DataContext); }
    }

    private void OpenConfig_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();

      dialog.Filter = "Configuration files|*.config";

      if (dialog.ShowDialog(this) == true)
        this.Presenter.OpenConfig(dialog.FileName);
    }

    private void OpenFile_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();

      dialog.Filter = "Log files|*.log";

      if (dialog.ShowDialog(this) == true)
        this.Presenter.OpenLog(dialog.FileName);
    }

    private void OpenDirectory_Click(object sender, RoutedEventArgs e)
    {
      using (var dlg = new FolderBrowserDialog())
      {
        dlg.SelectedPath = this.Presenter.GetDirectoryPath();

        if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
          this.Presenter.OpenDirectory(dlg.SelectedPath);
      }
    }

    private void MenuItemClose_Click(object sender, RoutedEventArgs e)
    {
      var button = sender as Button;
      var obj = button.TemplatedParent as ListBoxItem;

      this.Presenter.ClosePage(obj.Content as FileEntry);
    }

    private void TablePresenter_ExceptionOccured(object sender, ExceptionEventArgs e)
    {
      MessageBox.Show(e.Error, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
    }
  }
}
