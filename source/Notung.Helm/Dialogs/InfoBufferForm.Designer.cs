namespace Notung.Helm.Dialogs
{
  partial class InfoBufferForm
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
      this.m_button_no = new System.Windows.Forms.Button();
      this.m_button_yes = new System.Windows.Forms.Button();
      this.m_top_panel = new System.Windows.Forms.Panel();
      this.m_summary_label = new System.Windows.Forms.Label();
      this.m_buffer_view = new Notung.Helm.Controls.InfoBufferView();
      this.m_bottom_panel.SuspendLayout();
      this.m_top_panel.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_bottom_panel
      // 
      this.m_bottom_panel.Controls.Add(this.m_button_cancel);
      this.m_bottom_panel.Controls.Add(this.m_button_ok);
      this.m_bottom_panel.Controls.Add(this.m_button_no);
      this.m_bottom_panel.Controls.Add(this.m_button_yes);
      this.m_bottom_panel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.m_bottom_panel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
      this.m_bottom_panel.Location = new System.Drawing.Point(5, 262);
      this.m_bottom_panel.Name = "m_bottom_panel";
      this.m_bottom_panel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
      this.m_bottom_panel.Size = new System.Drawing.Size(466, 32);
      this.m_bottom_panel.TabIndex = 1;
      // 
      // m_button_cancel
      // 
      this.m_button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_button_cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.m_button_cancel.Location = new System.Drawing.Point(388, 8);
      this.m_button_cancel.Name = "m_button_cancel";
      this.m_button_cancel.Size = new System.Drawing.Size(75, 23);
      this.m_button_cancel.TabIndex = 0;
      this.m_button_cancel.Text = "Cancel";
      this.m_button_cancel.UseVisualStyleBackColor = true;
      this.m_button_cancel.Visible = false;
      // 
      // m_button_ok
      // 
      this.m_button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_button_ok.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.m_button_ok.Location = new System.Drawing.Point(307, 8);
      this.m_button_ok.Name = "m_button_ok";
      this.m_button_ok.Size = new System.Drawing.Size(75, 23);
      this.m_button_ok.TabIndex = 1;
      this.m_button_ok.Text = "OK";
      this.m_button_ok.UseVisualStyleBackColor = true;
      // 
      // m_button_no
      // 
      this.m_button_no.DialogResult = System.Windows.Forms.DialogResult.No;
      this.m_button_no.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.m_button_no.Location = new System.Drawing.Point(226, 8);
      this.m_button_no.Name = "m_button_no";
      this.m_button_no.Size = new System.Drawing.Size(75, 23);
      this.m_button_no.TabIndex = 2;
      this.m_button_no.Text = "No";
      this.m_button_no.UseVisualStyleBackColor = true;
      this.m_button_no.Visible = false;
      // 
      // m_button_yes
      // 
      this.m_button_yes.DialogResult = System.Windows.Forms.DialogResult.Yes;
      this.m_button_yes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.m_button_yes.Location = new System.Drawing.Point(145, 8);
      this.m_button_yes.Name = "m_button_yes";
      this.m_button_yes.Size = new System.Drawing.Size(75, 23);
      this.m_button_yes.TabIndex = 3;
      this.m_button_yes.Text = "Yes";
      this.m_button_yes.UseVisualStyleBackColor = true;
      this.m_button_yes.Visible = false;
      // 
      // m_top_panel
      // 
      this.m_top_panel.Controls.Add(this.m_summary_label);
      this.m_top_panel.Dock = System.Windows.Forms.DockStyle.Top;
      this.m_top_panel.Location = new System.Drawing.Point(5, 5);
      this.m_top_panel.Name = "m_top_panel";
      this.m_top_panel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
      this.m_top_panel.Size = new System.Drawing.Size(466, 37);
      this.m_top_panel.TabIndex = 2;
      // 
      // m_summary_label
      // 
      this.m_summary_label.BackColor = System.Drawing.Color.White;
      this.m_summary_label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.m_summary_label.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_summary_label.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.m_summary_label.Location = new System.Drawing.Point(0, 0);
      this.m_summary_label.Name = "m_summary_label";
      this.m_summary_label.Size = new System.Drawing.Size(466, 32);
      this.m_summary_label.TabIndex = 0;
      this.m_summary_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // m_buffer_view
      // 
      this.m_buffer_view.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.m_buffer_view.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_buffer_view.Location = new System.Drawing.Point(5, 42);
      this.m_buffer_view.Name = "m_buffer_view";
      this.m_buffer_view.Size = new System.Drawing.Size(466, 220);
      this.m_buffer_view.TabIndex = 0;
      // 
      // InfoBufferForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(476, 299);
      this.ControlBox = false;
      this.Controls.Add(this.m_buffer_view);
      this.Controls.Add(this.m_top_panel);
      this.Controls.Add(this.m_bottom_panel);
      this.Name = "InfoBufferForm";
      this.Padding = new System.Windows.Forms.Padding(5);
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.m_bottom_panel.ResumeLayout(false);
      this.m_top_panel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private Controls.InfoBufferView m_buffer_view;
    private System.Windows.Forms.FlowLayoutPanel m_bottom_panel;
    private System.Windows.Forms.Panel m_top_panel;
    private System.Windows.Forms.Button m_button_cancel;
    private System.Windows.Forms.Button m_button_ok;
    private System.Windows.Forms.Button m_button_no;
    private System.Windows.Forms.Button m_button_yes;
    private System.Windows.Forms.Label m_summary_label;
  }
}