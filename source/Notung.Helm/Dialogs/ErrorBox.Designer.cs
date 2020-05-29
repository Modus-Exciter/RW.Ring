namespace Notung.Helm.Dialogs
{
  partial class ErrorBox
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorBox));
      this.m_element_host = new System.Windows.Forms.Integration.ElementHost();
      this.m_error_box = new Notung.Helm.Dialogs.ErrorBoxControl();
      this.SuspendLayout();
      // 
      // m_element_host
      // 
      this.m_element_host.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_element_host.Location = new System.Drawing.Point(0, 0);
      this.m_element_host.Name = "m_element_host";
      this.m_element_host.Size = new System.Drawing.Size(517, 368);
      this.m_element_host.TabIndex = 0;
      this.m_element_host.Text = "elementHost1";
      this.m_element_host.Child = this.m_error_box;
      // 
      // ErrorBox
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(517, 368);
      this.ControlBox = false;
      this.Controls.Add(this.m_element_host);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(250, 150);
      this.Name = "ErrorBox";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Integration.ElementHost m_element_host;
    private ErrorBoxControl m_error_box;

  }
}