
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
      this.m_button_run = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.m_port_edit)).BeginInit();
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
      // m_button_run
      // 
      this.m_button_run.Location = new System.Drawing.Point(33, 126);
      this.m_button_run.Name = "m_button_run";
      this.m_button_run.Size = new System.Drawing.Size(101, 35);
      this.m_button_run.TabIndex = 4;
      this.m_button_run.Text = "Создать";
      this.m_button_run.UseVisualStyleBackColor = true;
      this.m_button_run.Click += new System.EventHandler(this.m_button_run_Click);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(457, 200);
      this.Controls.Add(this.m_button_run);
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
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label m_label_host;
    private System.Windows.Forms.Label m_label_port;
    private System.Windows.Forms.TextBox m_host_edit;
    private System.Windows.Forms.NumericUpDown m_port_edit;
    private System.Windows.Forms.Button m_button_run;
  }
}

