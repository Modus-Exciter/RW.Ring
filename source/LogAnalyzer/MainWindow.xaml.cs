using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using Notung.Feuerzauber.Controls;
using Notung.Feuerzauber.Dialogs;

namespace LogAnalyzer
{
  /// <summary>
  /// Main window of the log analyzer
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      this.InitializeComponent();
    }

    private void OpenConfig(object sender, ExecutedRoutedEventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();

      dialog.Filter = "Configuration files|*.config";
      dialog.Title =((RoutedUICommand) e.Command).Text;

      if (dialog.ShowDialog(this) == true)
        m_context.OpenConfig(dialog.FileName);
    }

    private void OpenFolder(object sender, ExecutedRoutedEventArgs e)
    {
      SelectFolderDialog dlg = new SelectFolderDialog() { Owner = this };

      if (dlg.ShowDialog() == true)
        this.DisplayLog(m_context.OpenDirectory(dlg.Tree.SelectedValue.ToString()));
    }

    private void OpenLogFile(object sender, ExecutedRoutedEventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();

      dialog.Filter = "Log files|*.log";
      dialog.Title = ((RoutedUICommand)e.Command).Text;

      if (dialog.ShowDialog(this) == true)
        this.DisplayLog(m_context.OpenLog(dialog.FileName));
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
        )
        { Tag = entry.FileName },
        item => object.Equals(item.Tag, entry.FileName));
    }

    private void Context_MessageRecieved(object sender, MessageEventArgs e)
    {
      MessageDialog.Show(e.Message, image: e.IsError ? MessageBoxImage.Error : MessageBoxImage.Information);
    }
  }
}