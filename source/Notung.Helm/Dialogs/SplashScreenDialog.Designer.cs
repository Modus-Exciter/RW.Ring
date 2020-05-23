namespace Notung.Helm.Dialogs
{
  partial class SplashScreenDialog
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
      this.m_worker = new System.ComponentModel.BackgroundWorker();
      this.m_progress_bar = new System.Windows.Forms.ProgressBar();
      this.m_label_descrition = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_progress_bar
      // 
      this.m_progress_bar.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_progress_bar.Location = new System.Drawing.Point(30, 0);
      this.m_progress_bar.MarqueeAnimationSpeed = 20;
      this.m_progress_bar.Name = "m_progress_bar";
      this.m_progress_bar.Size = new System.Drawing.Size(416, 19);
      this.m_progress_bar.TabIndex = 1;
      // 
      // m_label_descrition
      // 
      this.m_label_descrition.BackColor = System.Drawing.Color.Transparent;
      this.m_label_descrition.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.m_label_descrition.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.m_label_descrition.ForeColor = System.Drawing.Color.White;
      this.m_label_descrition.Location = new System.Drawing.Point(0, 377);
      this.m_label_descrition.Name = "m_label_descrition";
      this.m_label_descrition.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
      this.m_label_descrition.Size = new System.Drawing.Size(476, 22);
      this.m_label_descrition.TabIndex = 2;
      this.m_label_descrition.Text = "...";
      this.m_label_descrition.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      // 
      // panel1
      // 
      this.panel1.BackColor = System.Drawing.Color.Transparent;
      this.panel1.Controls.Add(this.m_progress_bar);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 358);
      this.panel1.Name = "panel1";
      this.panel1.Padding = new System.Windows.Forms.Padding(30, 0, 30, 0);
      this.panel1.Size = new System.Drawing.Size(476, 19);
      this.panel1.TabIndex = 3;
      // 
      // SplashScreenDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
      this.ClientSize = new System.Drawing.Size(476, 399);
      this.ControlBox = false;
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.m_label_descrition);
      this.DoubleBuffered = true;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "SplashScreenDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.ComponentModel.BackgroundWorker m_worker;
    private System.Windows.Forms.ProgressBar m_progress_bar;
    private System.Windows.Forms.Label m_label_descrition;
    private System.Windows.Forms.Panel panel1;
  }
}