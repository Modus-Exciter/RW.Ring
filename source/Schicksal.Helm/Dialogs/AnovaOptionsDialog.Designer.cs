
namespace Schicksal.Helm.Dialogs
{
  partial class AnovaOptionsDialog
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
      this.m_bottom_panel = new System.Windows.Forms.FlowLayoutPanel();
      this.m_button_cancel = new System.Windows.Forms.Button();
      this.m_button_ok = new System.Windows.Forms.Button();
      this.m_bottom_panel.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_bottom_panel
      // 
      this.m_bottom_panel.AutoSize = true;
      this.m_bottom_panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.m_bottom_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.m_bottom_panel.Controls.Add(this.m_button_cancel);
      this.m_bottom_panel.Controls.Add(this.m_button_ok);
      this.m_bottom_panel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.m_bottom_panel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
      this.m_bottom_panel.Location = new System.Drawing.Point(0, 398);
      this.m_bottom_panel.Margin = new System.Windows.Forms.Padding(4);
      this.m_bottom_panel.Name = "m_bottom_panel";
      this.m_bottom_panel.Padding = new System.Windows.Forms.Padding(5);
      this.m_bottom_panel.Size = new System.Drawing.Size(800, 52);
      this.m_bottom_panel.TabIndex = 2;
      // 
      // m_button_cancel
      // 
      this.m_button_cancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.m_button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_button_cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.m_button_cancel.Location = new System.Drawing.Point(684, 9);
      this.m_button_cancel.Margin = new System.Windows.Forms.Padding(4);
      this.m_button_cancel.Name = "m_button_cancel";
      this.m_button_cancel.Size = new System.Drawing.Size(100, 31);
      this.m_button_cancel.TabIndex = 0;
      this.m_button_cancel.Text = "Cancel";
      this.m_button_cancel.UseVisualStyleBackColor = true;
      // 
      // m_button_ok
      // 
      this.m_button_ok.AutoSize = true;
      this.m_button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_button_ok.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.m_button_ok.Location = new System.Drawing.Point(575, 9);
      this.m_button_ok.Margin = new System.Windows.Forms.Padding(4);
      this.m_button_ok.Name = "m_button_ok";
      this.m_button_ok.Size = new System.Drawing.Size(101, 32);
      this.m_button_ok.TabIndex = 1;
      this.m_button_ok.Text = "OK";
      this.m_button_ok.UseVisualStyleBackColor = true;
      // 
      // AnovaOptionsDialog
      // 
      this.AcceptButton = this.m_button_ok;
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.m_button_cancel;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.m_bottom_panel);
      this.Name = "AnovaOptionsDialog";
      this.Text = "AnovaOptionsDialog";
      this.m_bottom_panel.ResumeLayout(false);
      this.m_bottom_panel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel m_bottom_panel;
    private System.Windows.Forms.Button m_button_cancel;
    private System.Windows.Forms.Button m_button_ok;
  }
}