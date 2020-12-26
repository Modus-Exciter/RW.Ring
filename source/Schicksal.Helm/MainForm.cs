using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows.Forms;
using Notung;
using Notung.ComponentModel;
using Notung.Helm;
using Notung.Helm.Windows;
using Notung.Loader;
using Notung.Services;
using Schicksal.Anova;
using Schicksal.Exchange;
using Schicksal.Helm.Dialogs;
using Schicksal.Helm.Properties;

namespace Schicksal.Helm
{
  public partial class MainForm : Form, ILoadingQueue, IApplicationLoader
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

      foreach (var arg in AppManager.Instance.CommandLineArgs)
      {
        if (File.Exists(arg) && Path.GetExtension(arg).ToLower() == ".sks")
          this.OpenFile(arg);
      }

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

    protected override void WndProc(ref Message msg)
    {
      base.WndProc(ref msg);

      if (msg.Msg == WinAPIHelper.WM_COPYDATA)
      {
        foreach (var arg in MainFormView.GetStringArgs(msg))
        {
          if (File.Exists(arg) && Path.GetExtension(arg).ToLower() == ".sks")
            this.OpenFile(arg);
        }
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
        dlg.Filter = "Schicksal data files|*.sks";
        if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
          this.OpenFile(dlg.FileName);
      }
    }

    private void OpenFile(string fileName)
    {
      AppManager.Configurator.GetSection<Program.Preferences>().LastFiles.Add(fileName);
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
      m_menu_analyze.Enabled = table_form != null && !(table_form.FileName ?? "").StartsWith("<");
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
      var table_form = this.ActiveMdiChild as TableForm;

      if (table_form == null)
        return;

      var table = table_form.DataSource as DataTable;

      if (table == null)
        return;

      using (var dlg = new AnovaDialog())
      {
        dlg.Text = Resources.ANOVA;
        dlg.DataSource = new AnovaDialogData(table);
        if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
        {
          var processor = new FisherTableProcessor(table, dlg.DataSource.Predictors.ToArray(), dlg.DataSource.Result);

          if (!string.IsNullOrEmpty(dlg.DataSource.Filter))
            processor.Filter = dlg.DataSource.Filter;

          processor.RunInParrallel = true;

          if (AppManager.OperationLauncher.Run(processor, 
            new LaunchParameters
            {
              Caption = Resources.ANOVA,
              Bitmap = Resources.column_chart
            }) == TaskStatus.RanToCompletion)
          {
            dlg.DataSource.Save();
            var results_form = new AnovaResultsForm();
            results_form.Text = string.Format("{0}: {1}, p={2}", 
              Resources.ANOVA, table_form.Text, dlg.DataSource.Probability);
            results_form.DataSource = processor.Result;
            results_form.SourceTable = table;
            results_form.ResultColumn = dlg.DataSource.Result;
            results_form.Filter = dlg.DataSource.Filter;
            results_form.Probability = dlg.DataSource.Probability;
            results_form.Show(this);
          }
        }
      }
    }

    public IApplicationLoader[] GetLoaders()
    {
      return new IApplicationLoader[] { this };
    }

    bool IApplicationLoader.Load(LoadingContext context)
    {
      var imports = context.Container.GetService<IList<ITableImport>>();

      if (this.InvokeRequired)
        this.Invoke(new Action<IList<ITableImport>>(this.CreateImportMenu), imports);
      else
        CreateImportMenu(imports);

      return true;
    }

    private void CreateImportMenu(IList<ITableImport> imports)
    {
      m_menu_import.DropDownItems.Clear();
      
      foreach (var import in imports)
        m_menu_import.DropDownItems.Add(import.ToString()).Tag = import;

      if (m_menu_import.DropDownItems.Count == 0)
        m_menu_import.Visible = false;
    }

    void IApplicationLoader.Setup(LoadingContext context) { }

    Type IDependencyItem<Type>.Key
    {
      get { return typeof(Form); }
    }

    ICollection<Type> IDependencyItem<Type>.Dependencies
    {
      get { return new [] {typeof(IList<ITableImport>)}; }
    }

    private void m_menu_import_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
      var import = e.ClickedItem.Tag as ITableImport;

      if (import != null)
      {
        try
        {
          var result = import.Import(this);

          if (result == null)
            return;

          var table_form = new TableForm();
          table_form.DataSource = result.Table;
          table_form.Text = result.Description;
          table_form.MdiParent = this;
          table_form.Show();
          table_form.WindowState = FormWindowState.Maximized;
        }
        catch (Exception ex)
        {
          AppManager.Notificator.Show(new Info(ex));
        }
      }
    }
  }
}
