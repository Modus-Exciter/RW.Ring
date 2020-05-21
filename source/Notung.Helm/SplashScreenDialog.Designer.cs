namespace Notung.Helm
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
      this.m_splash_screen = new System.Windows.Forms.PictureBox();
      this.m_progress_bar = new System.Windows.Forms.ProgressBar();
      this.m_label_descrition = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.m_splash_screen)).BeginInit();
      this.SuspendLayout();
      // 
      // m_worker
      // 
      this.m_worker.WorkerReportsProgress = true;
      this.m_worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.m_worker_DoWork);
      this.m_worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.m_worker_ProgressChanged);
      this.m_worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.m_worker_RunWorkerCompleted);
      // 
      // m_splash_screen
      // 
      this.m_splash_screen.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_splash_screen.Location = new System.Drawing.Point(0, 0);
      this.m_splash_screen.Name = "m_splash_screen";
      this.m_splash_screen.Size = new System.Drawing.Size(480, 361);
      this.m_splash_screen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
      this.m_splash_screen.TabIndex = 0;
      this.m_splash_screen.TabStop = false;
      // 
      // m_progress_bar
      // 
      this.m_progress_bar.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.m_progress_bar.Location = new System.Drawing.Point(0, 380);
      this.m_progress_bar.Name = "m_progress_bar";
      this.m_progress_bar.Size = new System.Drawing.Size(480, 23);
      this.m_progress_bar.TabIndex = 1;
      // 
      // m_label_descrition
      // 
      this.m_label_descrition.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.m_label_descrition.Location = new System.Drawing.Point(0, 361);
      this.m_label_descrition.Name = "m_label_descrition";
      this.m_label_descrition.Size = new System.Drawing.Size(480, 19);
      this.m_label_descrition.TabIndex = 2;
      this.m_label_descrition.Text = "...";
      this.m_label_descrition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // SplashScreenDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(480, 403);
      this.ControlBox = false;
      this.Controls.Add(this.m_splash_screen);
      this.Controls.Add(this.m_label_descrition);
      this.Controls.Add(this.m_progress_bar);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Name = "SplashScreenDialog";
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      ((System.ComponentModel.ISupportInitialize)(this.m_splash_screen)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.ComponentModel.BackgroundWorker m_worker;
    private System.Windows.Forms.PictureBox m_splash_screen;
    private System.Windows.Forms.ProgressBar m_progress_bar;
    private System.Windows.Forms.Label m_label_descrition;
  }
}