namespace ConfiguratorGraphicalTest
{
  partial class SelectFunctionDialog
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
      this.m_button_ok = new System.Windows.Forms.Button();
      this.m_button_cancel = new System.Windows.Forms.Button();
      this.m_list_box = new System.Windows.Forms.ListBox();
      this.m_bottom_panel.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_bottom_panel
      // 
      this.m_bottom_panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.m_bottom_panel.Controls.Add(this.m_button_cancel);
      this.m_bottom_panel.Controls.Add(this.m_button_ok);
      this.m_bottom_panel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.m_bottom_panel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
      this.m_bottom_panel.Location = new System.Drawing.Point(2, 187);
      this.m_bottom_panel.Name = "m_bottom_panel";
      this.m_bottom_panel.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
      this.m_bottom_panel.Size = new System.Drawing.Size(293, 38);
      this.m_bottom_panel.TabIndex = 0;
      // 
      // m_button_ok
      // 
      this.m_button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_button_ok.Location = new System.Drawing.Point(130, 5);
      this.m_button_ok.Name = "m_button_ok";
      this.m_button_ok.Size = new System.Drawing.Size(75, 26);
      this.m_button_ok.TabIndex = 0;
      this.m_button_ok.Text = "OK";
      this.m_button_ok.UseVisualStyleBackColor = true;
      // 
      // m_button_cancel
      // 
      this.m_button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_button_cancel.Location = new System.Drawing.Point(211, 5);
      this.m_button_cancel.Name = "m_button_cancel";
      this.m_button_cancel.Size = new System.Drawing.Size(75, 26);
      this.m_button_cancel.TabIndex = 1;
      this.m_button_cancel.Text = "Cancel";
      this.m_button_cancel.UseVisualStyleBackColor = true;
      // 
      // m_list_box
      // 
      this.m_list_box.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_list_box.FormattingEnabled = true;
      this.m_list_box.Location = new System.Drawing.Point(2, 2);
      this.m_list_box.Name = "m_list_box";
      this.m_list_box.Size = new System.Drawing.Size(293, 185);
      this.m_list_box.TabIndex = 1;
      // 
      // SelectFunctionDialog
      // 
      this.AcceptButton = this.m_button_ok;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.m_button_cancel;
      this.ClientSize = new System.Drawing.Size(297, 227);
      this.ControlBox = false;
      this.Controls.Add(this.m_list_box);
      this.Controls.Add(this.m_bottom_panel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "SelectFunctionDialog";
      this.Padding = new System.Windows.Forms.Padding(2);
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Select dll function";
      this.m_bottom_panel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel m_bottom_panel;
    private System.Windows.Forms.Button m_button_ok;
    private System.Windows.Forms.Button m_button_cancel;
    private System.Windows.Forms.ListBox m_list_box;
  }
}