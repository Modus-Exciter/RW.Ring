using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Notung;
using Notung.ComponentModel;
using Schicksal.Helm.Dialogs;
using Schicksal.Helm.Properties;

namespace Schicksal.Helm
{
  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();

      var size = Screen.PrimaryScreen.WorkingArea.Size;

      this.Size = new System.Drawing.Size(size.Width * 3 / 4, size.Height * 3 / 4);
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      FillLastFilesMenu();
    }

    private void FillLastFilesMenu()
    {
      var preferences = AppManager.Configurator.GetSection<Program.Preferences>();

      foreach (var file in preferences.LastFiles)
      {
        if (File.Exists(file))
          m_menu_last_files.DropDownItems.Add(file);
      }

      if (m_menu_last_files.DropDownItems.Count == 0)
      {
        m_menu_last_files.Visible = false;
        m_last_files_separator.Visible = false;
      }
    }

    private void m_lang_LanguageChanged(object sender, LanguageEventArgs e)
    {
      m_menu_file.Text = Resources.FILE;
      m_menu_last_files.Text = Resources.LAST_FILES;
      m_menu_windows.Text = Resources.WINDOWS;
      m_cmd_new.Text = Resources.NEW;
      m_cmd_open.Text = Resources.OPEN;
      m_cmd_save.Text = Resources.SAVE;
      m_cmd_save_as.Text = Resources.SAVE_AS;
      m_cmd_settings.Text = Resources.SETTINGS;
      m_menu_standard_tables.Text = Resources.STANDARD_TABLES;
      m_cmd_student.Text = Resources.STUDENT;
      m_cmd_fisher_1.Text = string.Format("{0} (1%)", Resources.FISHER);
      m_cmd_fisher_5.Text = string.Format("{0} (5%)", Resources.FISHER);
      m_menu_import.Text = Resources.IMPORT;
      m_menu_analyze.Text = Resources.ANALYZE;
      m_cmd_basic.Text = Resources.BASIC_STATISTICS;
      m_cmd_anova.Text = Resources.ANOVA;
    }

    private void m_menu_last_files_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
      OpenFile(e.ClickedItem.Text);
    }

    private void m_cmd_new_Click(object sender, EventArgs e)
    {
      using (var dlg = new EditColumnsDialog())
      {
        dlg.Text = Resources.CREATE_NEW_TABLE;

        if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
        {
          var table_form = new TableForm();
          table_form.DataSource = TableColumnInfo.CreateTable(dlg.Columns);
          table_form.Text = Resources.NEW_TABLE;
          table_form.MdiParent = this;
          table_form.Show();
          table_form.WindowState = FormWindowState.Maximized;
        }
      }
    }

    private void m_cmd_open_Click(object sender, EventArgs e)
    {
      using (var dlg = new OpenFileDialog())
      {
        if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
          this.OpenFile(dlg.FileName);
      }
    }

    private void OpenFile(string fileName)
    {
      var table_form = this.MdiChildren.OfType<TableForm>().FirstOrDefault(f => f.FileName == fileName);

      if (table_form == null)
      {
        table_form = new TableForm();

        using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
          var bf = new BinaryFormatter();
          table_form.DataSource = bf.Deserialize(fs);
        }

        table_form.Text = Path.GetFileName(fileName);
        table_form.MdiParent = this;
        table_form.FileName = fileName;
        table_form.Show();
        table_form.WindowState = FormWindowState.Maximized;
      }
      else
        table_form.Activate();
    }

    private void m_cmd_save_Click(object sender, EventArgs e)
    {
      var table_form = this.ActiveMdiChild as TableForm;

      if (table_form == null)
        return;

      table_form.Save();
    }

    private void m_cmd_save_as_Click(object sender, EventArgs e)
    {
      var table_form = this.ActiveMdiChild as TableForm;

      if (table_form == null)
        return;

      table_form.SaveAs();
    }

    private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Process.Start(ApplicationInfo.Instance.GetWorkingPath());
    }

    private void MainForm_MdiChildActivate(object sender, EventArgs e)
    {
      var table_form = this.ActiveMdiChild as TableForm;

      m_cmd_save.Enabled = m_cmd_save_as.Enabled = table_form != null;
      m_menu_analyze.Enabled = table_form != null && !table_form.FileName.StartsWith("<");
    }

    private void OpenReadOnlyTable(string key, string text, Func<DataTable> loader)
    {
      var table_form = this.MdiChildren.OfType<TableForm>().FirstOrDefault(f => f.FileName == key);

      if (table_form == null)
      {
        table_form = new TableForm();
        table_form.DataSource = loader();
        table_form.Text = text;
        table_form.FileName = key;
        table_form.MdiParent = this;
        table_form.MarkAsReadOnly();
        table_form.Show();
        table_form.WindowState = FormWindowState.Maximized;
      }
      else
        table_form.Activate();
    }

    private void studentToolStripMenuItem_Click(object sender, EventArgs e)
    {
      OpenReadOnlyTable("<Student>", Resources.STUDENT, StatisticsTables.GetStudentTable);
    }

    private void fisher5ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      OpenReadOnlyTable("<Fisher5>", string.Format("{0} (5%)", Resources.FISHER), () => StatisticsTables.GetFTable(0.05));
    }

    private void fisher1ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      OpenReadOnlyTable("<Fisher1>", string.Format("{0} (1%)", Resources.FISHER), () => StatisticsTables.GetFTable(0.01));
    }

    private void x2ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      OpenReadOnlyTable("<X^2>", string.Format("X^2 {0}", Resources.DISTRIBUTION), StatisticsTables.GetStudentTable);
    }

    private void m_cmd_anova_Click(object sender, EventArgs e)
    {

    }
  }
}
