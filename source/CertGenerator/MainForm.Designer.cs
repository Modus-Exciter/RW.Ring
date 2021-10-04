
namespace CertGenerator
{
  partial class MainForm
  {
    /// <summary>
    /// Обязательная переменная конструктора.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Освободить все используемые ресурсы.
    /// </summary>
    /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Код, автоматически созданный конструктором форм Windows

    /// <summary>
    /// Требуемый метод для поддержки конструктора — не изменяйте 
    /// содержимое этого метода с помощью редактора кода.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.m_label_host = new System.Windows.Forms.Label();
      this.m_label_port = new System.Windows.Forms.Label();
      this.m_host_edit = new System.Windows.Forms.TextBox();
      this.m_port_edit = new System.Windows.Forms.NumericUpDown();
      this.toolStrip1 = new System.Windows.Forms.ToolStrip();
      this.m_button_run = new System.Windows.Forms.ToolStripSplitButton();
      this.uploadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.textBox1 = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.m_port_edit)).BeginInit();
      this.toolStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_label_host
      // 
      this.m_label_host.AutoSize = true;
      this.m_label_host.Location = new System.Drawing.Point(30, 26);
      this.m_label_host.Name = "m_label_host";
      this.m_label_host.Size = new System.Drawing.Size(39, 17);
      this.m_label_host.TabIndex = 0;
      this.m_label_host.Text = "Хост";
      // 
      // m_label_port
      // 
      this.m_label_port.AutoSize = true;
      this.m_label_port.Location = new System.Drawing.Point(30, 72);
      this.m_label_port.Name = "m_label_port";
      this.m_label_port.Size = new System.Drawing.Size(41, 17);
      this.m_label_port.TabIndex = 1;
      this.m_label_port.Text = "Порт";
      // 
      // m_host_edit
      // 
      this.m_host_edit.Location = new System.Drawing.Point(94, 23);
      this.m_host_edit.Name = "m_host_edit";
      this.m_host_edit.Size = new System.Drawing.Size(296, 22);
      this.m_host_edit.TabIndex = 2;
      // 
      // m_port_edit
      // 
      this.m_port_edit.Location = new System.Drawing.Point(94, 70);
      this.m_port_edit.Name = "m_port_edit";
      this.m_port_edit.Size = new System.Drawing.Size(296, 22);
      this.m_port_edit.TabIndex = 3;
      // 
      // toolStrip1
      // 
      this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
      this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_button_run});
      this.toolStrip1.Location = new System.Drawing.Point(0, 237);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Padding = new System.Windows.Forms.Padding(5);
      this.toolStrip1.Size = new System.Drawing.Size(410, 37);
      this.toolStrip1.TabIndex = 5;
      this.toolStrip1.Text = "toolStrip1";
      // 
      // m_button_run
      // 
      this.m_button_run.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uploadToolStripMenuItem,
            this.createToolStripMenuItem});
      this.m_button_run.Image = ((System.Drawing.Image)(resources.GetObject("m_button_run.Image")));
      this.m_button_run.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.m_button_run.Name = "m_button_run";
      this.m_button_run.Size = new System.Drawing.Size(103, 24);
      this.m_button_run.Text = "Создать";
      this.m_button_run.ButtonClick += new System.EventHandler(this.m_button_run_Click);
      // 
      // uploadToolStripMenuItem
      // 
      this.uploadToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("uploadToolStripMenuItem.Image")));
      this.uploadToolStripMenuItem.Name = "uploadToolStripMenuItem";
      this.uploadToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
      this.uploadToolStripMenuItem.Text = "Загрузить";
      this.uploadToolStripMenuItem.Click += new System.EventHandler(this.HandleButtonSelected);
      // 
      // createToolStripMenuItem
      // 
      this.createToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("createToolStripMenuItem.Image")));
      this.createToolStripMenuItem.Name = "createToolStripMenuItem";
      this.createToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
      this.createToolStripMenuItem.Text = "Создать";
      this.createToolStripMenuItem.Click += new System.EventHandler(this.HandleButtonSelected);
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(33, 121);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.textBox1.Size = new System.Drawing.Size(357, 113);
      this.textBox1.TabIndex = 6;
      this.textBox1.Text = resources.GetString("textBox1.Text");
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(410, 274);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.toolStrip1);
      this.Controls.Add(this.m_port_edit);
      this.Controls.Add(this.m_host_edit);
      this.Controls.Add(this.m_label_port);
      this.Controls.Add(this.m_label_host);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "MainForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Настройка сертификатов";
      ((System.ComponentModel.ISupportInitialize)(this.m_port_edit)).EndInit();
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label m_label_host;
    private System.Windows.Forms.Label m_label_port;
    private System.Windows.Forms.TextBox m_host_edit;
    private System.Windows.Forms.NumericUpDown m_port_edit;
    private System.Windows.Forms.ToolStrip toolStrip1;
    private System.Windows.Forms.ToolStripSplitButton m_button_run;
    private System.Windows.Forms.ToolStripMenuItem uploadToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
    private System.Windows.Forms.TextBox textBox1;
  }
}

