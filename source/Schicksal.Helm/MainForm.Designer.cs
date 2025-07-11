namespace Schicksal.Helm
{
  partial class MainForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.m_lang = new Notung.ComponentModel.LanguageSwitcher(this.components);
            this.m_main_menu = new System.Windows.Forms.MenuStrip();
            this.m_menu_file = new System.Windows.Forms.ToolStripMenuItem();
            this.m_cmd_new = new System.Windows.Forms.ToolStripMenuItem();
            this.m_cmd_open = new System.Windows.Forms.ToolStripMenuItem();
            this.m_cmd_save = new System.Windows.Forms.ToolStripMenuItem();
            this.m_cmd_save_as = new System.Windows.Forms.ToolStripMenuItem();
            this.m_last_files_separator = new System.Windows.Forms.ToolStripSeparator();
            this.m_menu_last_files = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.m_cmd_settings = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menu_import = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menu_analyze = new System.Windows.Forms.ToolStripMenuItem();
            this.m_cmd_basic = new System.Windows.Forms.ToolStripMenuItem();
            this.m_cmd_anova = new System.Windows.Forms.ToolStripMenuItem();
            this.m_cmd_regression = new System.Windows.Forms.ToolStripMenuItem();
            this.m_cmd_clustering = new System.Windows.Forms.ToolStripMenuItem();
            this.m_cmd_discriminant = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menu_standard_tables = new System.Windows.Forms.ToolStripMenuItem();
            this.m_cmd_student = new System.Windows.Forms.ToolStripMenuItem();
            this.m_cmd_fisher_5 = new System.Windows.Forms.ToolStripMenuItem();
            this.m_cmd_fisher_1 = new System.Windows.Forms.ToolStripMenuItem();
            this.m_cmd_chi2 = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menu_windows = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menu_help = new System.Windows.Forms.ToolStripMenuItem();
            this.m_cmd_about = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.m_lang)).BeginInit();
            this.m_main_menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_lang
            // 
            this.m_lang.LanguageChanged += new System.EventHandler<Notung.ComponentModel.LanguageEventArgs>(this.Lang_LanguageChanged);
            // 
            // m_main_menu
            // 
            this.m_main_menu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_main_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menu_file,
            this.m_menu_import,
            this.m_menu_analyze,
            this.m_menu_standard_tables,
            this.m_menu_windows,
            this.m_menu_help});
            resources.ApplyResources(this.m_main_menu, "m_main_menu");
            this.m_main_menu.MdiWindowListItem = this.m_menu_windows;
            this.m_main_menu.Name = "m_main_menu";
            // 
            // m_menu_file
            // 
            this.m_menu_file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmd_new,
            this.m_cmd_open,
            this.m_cmd_save,
            this.m_cmd_save_as,
            this.m_last_files_separator,
            this.m_menu_last_files,
            this.toolStripSeparator2,
            this.m_cmd_settings});
            this.m_menu_file.MergeIndex = 0;
            this.m_menu_file.Name = "m_menu_file";
            resources.ApplyResources(this.m_menu_file, "m_menu_file");
            // 
            // m_cmd_new
            // 
            this.m_cmd_new.Image = global::Schicksal.Helm.Properties.Resources.document_new;
            this.m_cmd_new.Name = "m_cmd_new";
            resources.ApplyResources(this.m_cmd_new, "m_cmd_new");
            this.m_cmd_new.Click += new System.EventHandler(this.Cmd_new_Click);
            // 
            // m_cmd_open
            // 
            this.m_cmd_open.Image = global::Schicksal.Helm.Properties.Resources.folder;
            this.m_cmd_open.Name = "m_cmd_open";
            resources.ApplyResources(this.m_cmd_open, "m_cmd_open");
            this.m_cmd_open.Click += new System.EventHandler(this.Cmd_open_Click);
            // 
            // m_cmd_save
            // 
            resources.ApplyResources(this.m_cmd_save, "m_cmd_save");
            this.m_cmd_save.Image = global::Schicksal.Helm.Properties.Resources.disk_blue;
            this.m_cmd_save.Name = "m_cmd_save";
            this.m_cmd_save.Click += new System.EventHandler(this.Cmd_save_Click);
            // 
            // m_cmd_save_as
            // 
            resources.ApplyResources(this.m_cmd_save_as, "m_cmd_save_as");
            this.m_cmd_save_as.Image = global::Schicksal.Helm.Properties.Resources.save_as_icon;
            this.m_cmd_save_as.Name = "m_cmd_save_as";
            this.m_cmd_save_as.Click += new System.EventHandler(this.Cmd_save_as_Click);
            // 
            // m_last_files_separator
            // 
            this.m_last_files_separator.Name = "m_last_files_separator";
            resources.ApplyResources(this.m_last_files_separator, "m_last_files_separator");
            // 
            // m_menu_last_files
            // 
            this.m_menu_last_files.Name = "m_menu_last_files";
            resources.ApplyResources(this.m_menu_last_files, "m_menu_last_files");
            this.m_menu_last_files.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.Menu_last_files_DropDownItemClicked);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // m_cmd_settings
            // 
            this.m_cmd_settings.Image = global::Schicksal.Helm.Properties.Resources.gear_preferences;
            this.m_cmd_settings.Name = "m_cmd_settings";
            resources.ApplyResources(this.m_cmd_settings, "m_cmd_settings");
            this.m_cmd_settings.Click += new System.EventHandler(this.SettingsToolStripMenuItem_Click);
            // 
            // m_menu_import
            // 
            this.m_menu_import.Name = "m_menu_import";
            resources.ApplyResources(this.m_menu_import, "m_menu_import");
            this.m_menu_import.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.Menu_import_DropDownItemClicked);
            // 
            // m_menu_analyze
            // 
            this.m_menu_analyze.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmd_basic,
            this.m_cmd_anova,
            this.m_cmd_regression,
            this.m_cmd_clustering,
            this.m_cmd_discriminant});
            resources.ApplyResources(this.m_menu_analyze, "m_menu_analyze");
            this.m_menu_analyze.Name = "m_menu_analyze";
            // 
            // m_cmd_basic
            // 
            this.m_cmd_basic.Image = global::Schicksal.Helm.Properties.Resources.table_sql;
            this.m_cmd_basic.Name = "m_cmd_basic";
            resources.ApplyResources(this.m_cmd_basic, "m_cmd_basic");
            this.m_cmd_basic.Click += new System.EventHandler(this.HandleAnalyzeClick);
            // 
            // m_cmd_anova
            // 
            resources.ApplyResources(this.m_cmd_anova, "m_cmd_anova");
            this.m_cmd_anova.Name = "m_cmd_anova";
            this.m_cmd_anova.Click += new System.EventHandler(this.HandleAnalyzeClick);
            // 
            // m_cmd_regression
            // 
            this.m_cmd_regression.Image = global::Schicksal.Helm.Properties.Resources.line_chart;
            this.m_cmd_regression.Name = "m_cmd_regression";
            resources.ApplyResources(this.m_cmd_regression, "m_cmd_regression");
            this.m_cmd_regression.Click += new System.EventHandler(this.HandleAnalyzeClick);
            // 
            // m_cmd_clustering
            // 
            this.m_cmd_clustering.Image = global::Schicksal.Helm.Properties.Resources.dot_chart;
            this.m_cmd_clustering.Name = "m_cmd_clustering";
            resources.ApplyResources(this.m_cmd_clustering, "m_cmd_clustering");
            this.m_cmd_clustering.Click += new System.EventHandler(this.HandleAnalyzeClick);
            // 
            // m_cmd_discriminant
            // 
            this.m_cmd_discriminant.Image = global::Schicksal.Helm.Properties.Resources.discriminant1;
            resources.ApplyResources(this.m_cmd_discriminant, "m_cmd_discriminant");
            this.m_cmd_discriminant.Name = "m_cmd_discriminant";
            this.m_cmd_discriminant.Click += new System.EventHandler(this.HandleAnalyzeClick);
            // 
            // m_menu_standard_tables
            // 
            this.m_menu_standard_tables.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmd_student,
            this.m_cmd_fisher_5,
            this.m_cmd_fisher_1,
            this.m_cmd_chi2});
            this.m_menu_standard_tables.Name = "m_menu_standard_tables";
            resources.ApplyResources(this.m_menu_standard_tables, "m_menu_standard_tables");
            // 
            // m_cmd_student
            // 
            this.m_cmd_student.Name = "m_cmd_student";
            resources.ApplyResources(this.m_cmd_student, "m_cmd_student");
            this.m_cmd_student.Click += new System.EventHandler(this.StudentToolStripMenuItem_Click);
            // 
            // m_cmd_fisher_5
            // 
            this.m_cmd_fisher_5.Name = "m_cmd_fisher_5";
            resources.ApplyResources(this.m_cmd_fisher_5, "m_cmd_fisher_5");
            this.m_cmd_fisher_5.Click += new System.EventHandler(this.Fisher5ToolStripMenuItem_Click);
            // 
            // m_cmd_fisher_1
            // 
            this.m_cmd_fisher_1.Name = "m_cmd_fisher_1";
            resources.ApplyResources(this.m_cmd_fisher_1, "m_cmd_fisher_1");
            this.m_cmd_fisher_1.Click += new System.EventHandler(this.Fisher1ToolStripMenuItem_Click);
            // 
            // m_cmd_chi2
            // 
            this.m_cmd_chi2.Name = "m_cmd_chi2";
            resources.ApplyResources(this.m_cmd_chi2, "m_cmd_chi2");
            this.m_cmd_chi2.Click += new System.EventHandler(this.X2ToolStripMenuItem_Click);
            // 
            // m_menu_windows
            // 
            this.m_menu_windows.MergeIndex = 10;
            this.m_menu_windows.Name = "m_menu_windows";
            resources.ApplyResources(this.m_menu_windows, "m_menu_windows");
            // 
            // m_menu_help
            // 
            this.m_menu_help.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmd_about});
            this.m_menu_help.MergeIndex = 20;
            this.m_menu_help.Name = "m_menu_help";
            resources.ApplyResources(this.m_menu_help, "m_menu_help");
            // 
            // m_cmd_about
            // 
            this.m_cmd_about.Image = global::Schicksal.Helm.Properties.Resources.About_Picture;
            this.m_cmd_about.Name = "m_cmd_about";
            resources.ApplyResources(this.m_cmd_about, "m_cmd_about");
            this.m_cmd_about.Click += new System.EventHandler(this.Cmd_about_Click);
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_main_menu);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.m_main_menu;
            this.Name = "MainForm";
            this.MdiChildActivate += new System.EventHandler(this.MainForm_MdiChildActivate);
            ((System.ComponentModel.ISupportInitialize)(this.m_lang)).EndInit();
            this.m_main_menu.ResumeLayout(false);
            this.m_main_menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private Notung.ComponentModel.LanguageSwitcher m_lang;
    private System.Windows.Forms.MenuStrip m_main_menu;
    private System.Windows.Forms.ToolStripMenuItem m_menu_file;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_new;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_open;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_save;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_save_as;
    private System.Windows.Forms.ToolStripSeparator m_last_files_separator;
    private System.Windows.Forms.ToolStripMenuItem m_menu_last_files;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_settings;
    private System.Windows.Forms.ToolStripMenuItem m_menu_windows;
    private System.Windows.Forms.ToolStripMenuItem m_menu_standard_tables;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_student;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_fisher_5;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_fisher_1;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_chi2;
    private System.Windows.Forms.ToolStripMenuItem m_menu_import;
    private System.Windows.Forms.ToolStripMenuItem m_menu_analyze;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_anova;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_basic;
    private System.Windows.Forms.ToolStripMenuItem m_menu_help;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_about;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_regression;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_clustering;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_discriminant;
  }
}

