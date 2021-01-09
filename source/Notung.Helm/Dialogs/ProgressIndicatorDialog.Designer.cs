namespace Notung.Helm.Dialogs
{
  partial class ProgressIndicatorDialog
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
      this.m_picture = new System.Windows.Forms.PictureBox();
      this.m_progress_bar = new System.Windows.Forms.ProgressBar();
      this.m_button = new System.Windows.Forms.Button();
      this.m_state_label = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.m_picture)).BeginInit();
      this.SuspendLayout();
      // 
      // m_picture
      // 
      this.m_picture.Location = new System.Drawing.Point(12, 12);
      this.m_picture.Name = "m_picture";
      this.m_picture.Size = new System.Drawing.Size(72, 72);
      this.m_picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.m_picture.TabIndex = 0;
      this.m_picture.TabStop = false;
      // 
      // m_progress_bar
      // 
      this.m_progress_bar.Location = new System.Drawing.Point(90, 40);
      this.m_progress_bar.Name = "m_progress_bar";
      this.m_progress_bar.Size = new System.Drawing.Size(342, 23);
      this.m_progress_bar.TabIndex = 1;
      // 
      // m_button
      // 
      this.m_button.Location = new System.Drawing.Point(187, 70);
      this.m_button.Name = "m_button";
      this.m_button.Size = new System.Drawing.Size(75, 23);
      this.m_button.TabIndex = 2;
      this.m_button.Text = "Cancel";
      this.m_button.UseVisualStyleBackColor = true;
      // 
      // m_state_label
      // 
      this.m_state_label.Location = new System.Drawing.Point(85, 12);
      this.m_state_label.Name = "m_state_label";
      this.m_state_label.Size = new System.Drawing.Size(346, 25);
      this.m_state_label.TabIndex = 3;
      this.m_state_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // ProgressIndicatorDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(444, 100);
      this.ControlBox = false;
      this.Controls.Add(this.m_state_label);
      this.Controls.Add(this.m_button);
      this.Controls.Add(this.m_progress_bar);
      this.Controls.Add(this.m_picture);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ProgressIndicatorDialog";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "   ";
      ((System.ComponentModel.ISupportInitialize)(this.m_picture)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PictureBox m_picture;
    private System.Windows.Forms.ProgressBar m_progress_bar;
    private System.Windows.Forms.Button m_button;
    private System.Windows.Forms.Label m_state_label;
  }
}