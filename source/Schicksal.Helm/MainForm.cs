using Notung;
using Notung.ComponentModel;
using Notung.Helm;
using Notung.Helm.Dialogs;
using Notung.Loader;
using Notung.Logging;
using Notung.Services;
using Schicksal.Exchange;
using Schicksal.Helm.Analyze;
using Schicksal.Helm.Dialogs;
using Schicksal.Helm.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Schicksal.Helm
{
  public partial class MainForm : Form, ILoadingQueue, IApplicationLoader
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(MainForm));

    public MainForm()
    {
      this.InitializeComponent();

      var size = Screen.PrimaryScreen.WorkingArea.Size;
      this.Size = new System.Drawing.Size(size.Width * 3 / 4, size.Height * 3 / 4);

      LanguageSwitcher.Switch(AppManager.Configurator.GetSection<Program.Preferences>().Language ?? "RU");

      m_cmd_basic.Tag = new DescriptiveAnalyze();
      m_cmd_anova.Tag = new AnovaAnalyze();
      m_cmd_regression.Tag = new RegressionAnalyze();
    }

    internal void FillLastFilesMenu()
    {
      var preferences = AppManager.Configurator.GetSection<Program.Preferences>();

      if (m_menu_last_files.DropDownItems.Count > 0)
        m_menu_last_files.DropDownItems.Clear();

      foreach (var file in preferences.LastFiles.OrderByDescending(kv => kv.Value))
      {
        if (File.Exists(file.Key))
          m_menu_last_files.DropDownItems.Add(file.Key);
        else
          preferences.LastFiles.Remove(file.Key);
      }

      m_menu_last_files.Visible = m_menu_last_files.DropDownItems.Count > 0;
      m_last_files_separator.Visible = m_menu_last_files.DropDownItems.Count > 0;
    }

    protected override void WndProc(ref Message msg)
    {
      base.WndProc(ref msg);

      string[] args;

      if (MainFormView.GetStringArgs(ref msg, out args))
      {
        foreach (var arg in args)
        {
          if (File.Exists(arg) && Path.GetExtension(arg).ToLower() == ".sks")
            this.OpenFile(arg);
        }

        msg.Result = new System.IntPtr(MainFormView.StringArgsMessageCode.Code);
      }
    }

    private void Lang_LanguageChanged(object sender, LanguageEventArgs e)
    {
      m_menu_file.Text = Resources.FILE;
      m_menu_last_files.Text = Resources.LAST_FILES;
      m_menu_windows.Text = Resources.WINDOWS;
      m_menu_help.Text = Resources.HELP;
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
      m_cmd_regression.Text = Resources.REGRESSION_ANALYSIS;
      m_cmd_about.Text = Resources.ABOUT;

      foreach (ToolStripMenuItem item in m_menu_import.DropDownItems)
        item.Text = item.Tag.ToString();
    }

    private void Menu_last_files_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
      this.OpenFile(e.ClickedItem.Text);
    }

    private void Cmd_new_Click(object sender, EventArgs e)
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
          table_form.WindowState = FormWindowState.Maximized;
          table_form.Show();
        }
      }
    }

    private void Cmd_open_Click(object sender, EventArgs e)
    {
      using (var dlg = new OpenFileDialog())
      {
        dlg.Filter = "Schicksal data files|*.sks";
        if (dlg.ShowDialog(this) == DialogResult.OK)
          this.OpenFile(dlg.FileName);
      }
    }

    private void OpenTableForm(string fileName, DataTable table)
    {
      if (table == null)
        return;

      var table_form = new TableForm();

      table_form.DataSource = table;
      table_form.Text = Path.GetFileName(fileName);
      table_form.MdiParent = this;
      table_form.FileName = fileName;
      table_form.WindowState = FormWindowState.Maximized;

      table_form.Show();
    }

    private DataTable ReadFile(string fileName)
    {
      try
      {
        using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
          return DataTableSaver.ReadDataTable(fs);
        }
      }
      catch (Exception ex)
      {
        _log.Error("Serialization exception", ex);
        AppManager.Notificator.Show(ex.Message, InfoLevel.Error);
        return null;
      }
    }

    private void OpenFile(string fileName)
    {
      AppManager.Configurator.GetSection<Program.Preferences>().LastFiles[fileName] = DateTime.Now;
      this.FillLastFilesMenu();
      var table_form = this.MdiChildren.OfType<TableForm>().FirstOrDefault(f => f.FileName == fileName);

      if (table_form == null)
        this.OpenTableForm(fileName, this.ReadFile(fileName));
      else
        table_form.Activate();
    }

    private void Cmd_save_Click(object sender, EventArgs e)
    {
      var table_form = this.ActiveMdiChild as TableForm;

      if (table_form == null)
        return;

      table_form.Save();
      this.FillLastFilesMenu();
    }

    private void Cmd_save_as_Click(object sender, EventArgs e)
    {
      var table_form = this.ActiveMdiChild as TableForm;

      if (table_form == null)
        return;

      table_form.SaveAs();
      this.FillLastFilesMenu();
    }

    private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      using (var settings = new SettingsDialog { DefaultPage = typeof(MainPropertyPage) })
        settings.ShowDialog(this);
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
        table_form.MarkAsReadOnly();
        table_form.DataSource = loader();
        table_form.Text = text;
        table_form.FileName = key;
        table_form.MdiParent = this;
        table_form.WindowState = FormWindowState.Maximized;
        table_form.Show();
      }
      else
        table_form.Activate();
    }

    private void StudentToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.OpenReadOnlyTable("<Student>", Resources.STUDENT, StatisticsTables.GetStudentTable);
    }

    private void Fisher5ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.OpenReadOnlyTable("<Fisher005>", string.Format("{0} (5%)", Resources.FISHER), () => StatisticsTables.GetFTable(0.05));
    }

    private void Fisher1ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.OpenReadOnlyTable("<Fisher001>", string.Format("{0} (1%)", Resources.FISHER), () => StatisticsTables.GetFTable(0.01));
    }

    private void X2ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.OpenReadOnlyTable("<X^2>", string.Format("χ² {0}", Resources.DISTRIBUTION), StatisticsTables.GetChiSquare);
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
        this.CreateImportMenu(imports);

      this.LoadFilesFromCommandArgs();

      return true;
    }

    private void LoadFilesFromCommandArgs()
    {
      foreach (var arg in AppManager.Instance.CommandLineArgs)
      {
        if (File.Exists(arg) && Path.GetExtension(arg).ToLower() == ".sks")
        {
          AppManager.Configurator.GetSection<Program.Preferences>().LastFiles[arg] = DateTime.Now;

          if (this.InvokeRequired)
            this.Invoke(new Action<string, DataTable>(this.OpenTableForm), arg, this.ReadFile(arg));
          else
            this.OpenTableForm(arg, this.ReadFile(arg));
        }
      }

      if (this.InvokeRequired)
        this.Invoke(new Action(this.FillLastFilesMenu));
      else
        this.FillLastFilesMenu();
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
      get { return new[] { typeof(IList<ITableImport>) }; }
    }

    private void Menu_import_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
      var import = e.ClickedItem.Tag as ITableImport;

      if (import != null)
      {
        m_menu_import.HideDropDown();

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

    private void Cmd_about_Click(object sender, EventArgs e)
    {
      using (var box = new AboutBox())
        box.ShowDialog(this);
    }

    private void HandleAnalyzeClick(object sender, EventArgs e)
    {
      var item = sender as ToolStripMenuItem; // Получаем пункт меню, которым был запущен анализ

      if (item == null) 
        return;

      var analyze = item.Tag as IAnalyze; // К пункту меню привязан вид анализа (1)

      if (analyze == null)
        return;

      var table_form = this.ActiveMdiChild as TableForm; // Анализируем данные в открытой таблице

      if (table_form == null)
        return;

      var table = table_form.DataSource; // Берём таблицу для анализа (2)

      if (table == null)
        return;

      using (var dlg = new StatisticsParametersDialog()) // При любом анализе показываем один диалог
      {
        dlg.Text = analyze.ToString(); // Заголовок диалога - название вида анализа
        dlg.DataSource = new StatisticsParameters(table, analyze.GetSettings()); // Настройки анализа
        dlg.OptionsType = analyze.OptionsType; // Тип окна с дополнительными настройками

        if (dlg.ShowDialog(this) == DialogResult.OK) // Если пользователь нажал ОК
        {
          var processor = analyze.GetProcessor(table, dlg.DataSource); // Получаем фоновую задачу, в которой будет проходить анализ

          if (AppManager.OperationLauncher.Run(processor, analyze.GetLaunchParameters()) // Запускаем фоновую задачу и ждём завершения
            == TaskStatus.RanToCompletion)
          {
            dlg.DataSource.Save(analyze.GetSettings()); // При успешном завершении анализа сохраняем настройки для следующего запуска
            analyze.BindTheResultForm(processor, table_form, dlg.DataSource); // Показываем результаты анализа
          }
        }
      }
    }

    private void clusteringToolStripMenuItem_Click(object sender, EventArgs e)
    {

    }
  }
}
