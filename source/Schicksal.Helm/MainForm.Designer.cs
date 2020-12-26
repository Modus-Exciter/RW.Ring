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
      this.m_cmd_anova = new System.Windows.Forms.ToolStripMenuItem();
      this.m_cmd_basic = new System.Windows.Forms.ToolStripMenuItem();
      this.m_menu_standard_tables = new System.Windows.Forms.ToolStripMenuItem();
      this.m_cmd_student = new System.Windows.Forms.ToolStripMenuItem();
      this.m_cmd_fisher_5 = new System.Windows.Forms.ToolStripMenuItem();
      this.m_cmd_fisher_1 = new System.Windows.Forms.ToolStripMenuItem();
      this.m_cmd_chi2 = new System.Windows.Forms.ToolStripMenuItem();
      this.m_menu_windows = new System.Windows.Forms.ToolStripMenuItem();
      ((System.ComponentModel.ISupportInitialize)(this.m_lang)).BeginInit();
      this.m_main_menu.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_lang
      // 
      this.m_lang.LanguageChanged += new System.EventHandler<Notung.ComponentModel.LanguageEventArgs>(this.m_lang_LanguageChanged);
      // 
      // m_main_menu
      // 
      this.m_main_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menu_file,
            this.m_menu_import,
            this.m_menu_analyze,
            this.m_menu_standard_tables,
            this.m_menu_windows});
      this.m_main_menu.Location = new System.Drawing.Point(0, 0);
      this.m_main_menu.MdiWindowListItem = this.m_menu_windows;
      this.m_main_menu.Name = "m_main_menu";
      this.m_main_menu.Size = new System.Drawing.Size(684, 24);
      this.m_main_menu.TabIndex = 1;
      this.m_main_menu.Text = "menuStrip1";
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
      this.m_menu_file.Size = new System.Drawing.Size(35, 20);
      this.m_menu_file.Text = "File";
      // 
      // m_cmd_new
      // 
      this.m_cmd_new.Name = "m_cmd_new";
      this.m_cmd_new.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
      this.m_cmd_new.Size = new System.Drawing.Size(151, 22);
      this.m_cmd_new.Text = "New";
      this.m_cmd_new.Click += new System.EventHandler(this.m_cmd_new_Click);
      // 
      // m_cmd_open
      // 
      this.m_cmd_open.Name = "m_cmd_open";
      this.m_cmd_open.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
      this.m_cmd_open.Size = new System.Drawing.Size(151, 22);
      this.m_cmd_open.Text = "Open";
      this.m_cmd_open.Click += new System.EventHandler(this.m_cmd_open_Click);
      // 
      // m_cmd_save
      // 
      this.m_cmd_save.Enabled = false;
      this.m_cmd_save.Name = "m_cmd_save";
      this.m_cmd_save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
      this.m_cmd_save.Size = new System.Drawing.Size(151, 22);
      this.m_cmd_save.Text = "Save";
      this.m_cmd_save.Click += new System.EventHandler(this.m_cmd_save_Click);
      // 
      // m_cmd_save_as
      // 
      this.m_cmd_save_as.Enabled = false;
      this.m_cmd_save_as.Name = "m_cmd_save_as";
      this.m_cmd_save_as.Size = new System.Drawing.Size(151, 22);
      this.m_cmd_save_as.Text = "Save As";
      this.m_cmd_save_as.Click += new System.EventHandler(this.m_cmd_save_as_Click);
      // 
      // m_last_files_separator
      // 
      this.m_last_files_separator.Name = "m_last_files_separator";
      this.m_last_files_separator.Size = new System.Drawing.Size(148, 6);
      // 
      // m_menu_last_files
      // 
      this.m_menu_last_files.Name = "m_menu_last_files";
      this.m_menu_last_files.Size = new System.Drawing.Size(151, 22);
      this.m_menu_last_files.Text = "Last files";
      this.m_menu_last_files.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.m_menu_last_files_DropDownItemClicked);
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(148, 6);
      // 
      // m_cmd_settings
      // 
      this.m_cmd_settings.Name = "m_cmd_settings";
      this.m_cmd_settings.Size = new System.Drawing.Size(151, 22);
      this.m_cmd_settings.Text = "Settings";
      this.m_cmd_settings.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
      // 
      // m_menu_import
      // 
      this.m_menu_import.Name = "m_menu_import";
      this.m_menu_import.Size = new System.Drawing.Size(51, 20);
      this.m_menu_import.Text = "Import";
      this.m_menu_import.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.m_menu_import_DropDownItemClicked);
      // 
      // m_menu_analyze
      // 
      this.m_menu_analyze.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmd_anova,
            this.m_cmd_basic});
      this.m_menu_analyze.Enabled = false;
      this.m_menu_analyze.Name = "m_menu_analyze";
      this.m_menu_analyze.Size = new System.Drawing.Size(57, 20);
      this.m_menu_analyze.Text = "Analyze";
      // 
      // m_cmd_anova
      // 
      this.m_cmd_anova.Name = "m_cmd_anova";
      this.m_cmd_anova.Size = new System.Drawing.Size(154, 22);
      this.m_cmd_anova.Text = "ANOVA";
      this.m_cmd_anova.Click += new System.EventHandler(this.m_cmd_anova_Click);
      // 
      // m_cmd_basic
      // 
      this.m_cmd_basic.Name = "m_cmd_basic";
      this.m_cmd_basic.Size = new System.Drawing.Size(154, 22);
      this.m_cmd_basic.Text = "Basic statistics";
      this.m_cmd_basic.Visible = false;
      // 
      // m_menu_standard_tables
      // 
      this.m_menu_standard_tables.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmd_student,
            this.m_cmd_fisher_5,
            this.m_cmd_fisher_1,
            this.m_cmd_chi2});
      this.m_menu_standard_tables.Name = "m_menu_standard_tables";
      this.m_menu_standard_tables.Size = new System.Drawing.Size(94, 20);
      this.m_menu_standard_tables.Text = "StandardTables";
      // 
      // m_cmd_student
      // 
      this.m_cmd_student.Name = "m_cmd_student";
      this.m_cmd_student.Size = new System.Drawing.Size(138, 22);
      this.m_cmd_student.Text = "Student";
      this.m_cmd_student.Click += new System.EventHandler(this.studentToolStripMenuItem_Click);
      // 
      // m_cmd_fisher_5
      // 
      this.m_cmd_fisher_5.Name = "m_cmd_fisher_5";
      this.m_cmd_fisher_5.Size = new System.Drawing.Size(138, 22);
      this.m_cmd_fisher_5.Text = "Fisher, 5%";
      this.m_cmd_fisher_5.Click += new System.EventHandler(this.fisher5ToolStripMenuItem_Click);
      // 
      // m_cmd_fisher_1
      // 
      this.m_cmd_fisher_1.Name = "m_cmd_fisher_1";
      this.m_cmd_fisher_1.Size = new System.Drawing.Size(138, 22);
      this.m_cmd_fisher_1.Text = "Fisher, 1%";
      this.m_cmd_fisher_1.Click += new System.EventHandler(this.fisher1ToolStripMenuItem_Click);
      // 
      // m_cmd_chi2
      // 
      this.m_cmd_chi2.Name = "m_cmd_chi2";
      this.m_cmd_chi2.Size = new System.Drawing.Size(138, 22);
      this.m_cmd_chi2.Text = "X^2";
      this.m_cmd_chi2.Click += new System.EventHandler(this.x2ToolStripMenuItem_Click);
      // 
      // m_menu_windows
      // 
      this.m_menu_windows.MergeIndex = 10;
      this.m_menu_windows.Name = "m_menu_windows";
      this.m_menu_windows.Size = new System.Drawing.Size(62, 20);
      this.m_menu_windows.Text = "Windows";
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(684, 487);
      this.Controls.Add(this.m_main_menu);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.IsMdiContainer = true;
      this.MainMenuStrip = this.m_main_menu;
      this.Name = "MainForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Shicksal";
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
  }
}

