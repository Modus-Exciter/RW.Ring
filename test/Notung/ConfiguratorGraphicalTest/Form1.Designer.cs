namespace ConfiguratorGraphicalTest
{
  partial class Form1
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.buttonRestart = new System.Windows.Forms.Button();
      this.buttonDLL = new System.Windows.Forms.Button();
      this.buttonOpenFolder = new System.Windows.Forms.Button();
      this.buttonWork = new System.Windows.Forms.Button();
      this.m_settings_button = new System.Windows.Forms.Button();
      this.comboBoxLang = new System.Windows.Forms.ComboBox();
      this.buttonInfoBufferView = new System.Windows.Forms.Button();
      this.languageSwitcher = new Notung.ComponentModel.LanguageSwitcher(this.components);
      this.m_placeholder = new Notung.Helm.Controls.ControlPlaceholder();
      this.flowLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.languageSwitcher)).BeginInit();
      this.SuspendLayout();
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.Controls.Add(this.buttonRestart);
      this.flowLayoutPanel1.Controls.Add(this.buttonDLL);
      this.flowLayoutPanel1.Controls.Add(this.buttonOpenFolder);
      this.flowLayoutPanel1.Controls.Add(this.buttonWork);
      this.flowLayoutPanel1.Controls.Add(this.m_settings_button);
      this.flowLayoutPanel1.Controls.Add(this.comboBoxLang);
      this.flowLayoutPanel1.Controls.Add(this.buttonInfoBufferView);
      resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      // 
      // buttonRestart
      // 
      resources.ApplyResources(this.buttonRestart, "buttonRestart");
      this.buttonRestart.Name = "buttonRestart";
      this.buttonRestart.UseVisualStyleBackColor = true;
      this.buttonRestart.Click += new System.EventHandler(this.button1_Click);
      // 
      // buttonDLL
      // 
      resources.ApplyResources(this.buttonDLL, "buttonDLL");
      this.buttonDLL.Name = "buttonDLL";
      this.buttonDLL.UseVisualStyleBackColor = true;
      this.buttonDLL.Click += new System.EventHandler(this.buttonDLL_Click);
      // 
      // buttonOpenFolder
      // 
      resources.ApplyResources(this.buttonOpenFolder, "buttonOpenFolder");
      this.buttonOpenFolder.Name = "buttonOpenFolder";
      this.buttonOpenFolder.UseVisualStyleBackColor = true;
      this.buttonOpenFolder.Click += new System.EventHandler(this.buttonOpenFolder_Click);
      // 
      // buttonWork
      // 
      resources.ApplyResources(this.buttonWork, "buttonWork");
      this.buttonWork.Name = "buttonWork";
      this.buttonWork.UseVisualStyleBackColor = true;
      this.buttonWork.Click += new System.EventHandler(this.button2_Click);
      // 
      // m_settings_button
      // 
      resources.ApplyResources(this.m_settings_button, "m_settings_button");
      this.m_settings_button.Name = "m_settings_button";
      this.m_settings_button.UseVisualStyleBackColor = true;
      this.m_settings_button.Click += new System.EventHandler(this.m_settings_button_Click);
      // 
      // comboBoxLang
      // 
      this.comboBoxLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxLang.FormattingEnabled = true;
      this.comboBoxLang.Items.AddRange(new object[] {
            resources.GetString("comboBoxLang.Items"),
            resources.GetString("comboBoxLang.Items1")});
      resources.ApplyResources(this.comboBoxLang, "comboBoxLang");
      this.comboBoxLang.Name = "comboBoxLang";
      this.comboBoxLang.SelectedIndexChanged += new System.EventHandler(this.comboBoxLang_SelectedIndexChanged);
      // 
      // buttonInfoBufferView
      // 
      resources.ApplyResources(this.buttonInfoBufferView, "buttonInfoBufferView");
      this.buttonInfoBufferView.Name = "buttonInfoBufferView";
      this.buttonInfoBufferView.UseVisualStyleBackColor = true;
      this.buttonInfoBufferView.Click += new System.EventHandler(this.buttonInfoBufferView_Click);
      // 
      // languageSwitcher
      // 
      this.languageSwitcher.LanguageChanged += new System.EventHandler<Notung.ComponentModel.LanguageEventArgs>(this.languageSwitcher_LanguageChanged);
      // 
      // m_placeholder
      // 
      this.m_placeholder.ControlDescription = "Гриды конфигурации";
      resources.ApplyResources(this.m_placeholder, "m_placeholder");
      this.m_placeholder.Name = "m_placeholder";
      this.m_placeholder.ReplacingType = "ConfiguratorGraphicalTest.ConfigurationGrids, ConfiguratorGraphicalTest";
      this.m_placeholder.LoadingCompleted += new System.EventHandler<Notung.Helm.Controls.LoadingCompletedEventArgs>(this.m_placeholder_LoadingCompleted);
      // 
      // Form1
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_placeholder);
      this.Controls.Add(this.flowLayoutPanel1);
      this.Name = "Form1";
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.languageSwitcher)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.Button buttonRestart;
    private System.Windows.Forms.Button buttonDLL;
    private System.Windows.Forms.Button buttonOpenFolder;
    private System.Windows.Forms.Button buttonWork;
    private Notung.ComponentModel.LanguageSwitcher languageSwitcher;
    private System.Windows.Forms.ComboBox comboBoxLang;
    private System.Windows.Forms.Button m_settings_button;
    private System.Windows.Forms.Button buttonInfoBufferView;
    private Notung.Helm.Controls.ControlPlaceholder m_placeholder;
  }
}

