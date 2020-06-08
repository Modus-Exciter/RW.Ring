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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InfoBufferForm));
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
      resources.ApplyResources(this.m_bottom_panel, "m_bottom_panel");
      this.m_bottom_panel.Controls.Add(this.m_button_cancel);
      this.m_bottom_panel.Controls.Add(this.m_button_ok);
      this.m_bottom_panel.Controls.Add(this.m_button_no);
      this.m_bottom_panel.Controls.Add(this.m_button_yes);
      this.m_bottom_panel.Name = "m_bottom_panel";
      // 
      // m_button_cancel
      // 
      resources.ApplyResources(this.m_button_cancel, "m_button_cancel");
      this.m_button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_button_cancel.Name = "m_button_cancel";
      this.m_button_cancel.UseVisualStyleBackColor = true;
      // 
      // m_button_ok
      // 
      resources.ApplyResources(this.m_button_ok, "m_button_ok");
      this.m_button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_button_ok.Name = "m_button_ok";
      this.m_button_ok.UseVisualStyleBackColor = true;
      // 
      // m_button_no
      // 
      resources.ApplyResources(this.m_button_no, "m_button_no");
      this.m_button_no.DialogResult = System.Windows.Forms.DialogResult.No;
      this.m_button_no.Name = "m_button_no";
      this.m_button_no.UseVisualStyleBackColor = true;
      // 
      // m_button_yes
      // 
      resources.ApplyResources(this.m_button_yes, "m_button_yes");
      this.m_button_yes.DialogResult = System.Windows.Forms.DialogResult.Yes;
      this.m_button_yes.Name = "m_button_yes";
      this.m_button_yes.UseVisualStyleBackColor = true;
      // 
      // m_top_panel
      // 
      resources.ApplyResources(this.m_top_panel, "m_top_panel");
      this.m_top_panel.Controls.Add(this.m_summary_label);
      this.m_top_panel.Name = "m_top_panel";
      // 
      // m_summary_label
      // 
      resources.ApplyResources(this.m_summary_label, "m_summary_label");
      this.m_summary_label.BackColor = System.Drawing.Color.White;
      this.m_summary_label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.m_summary_label.Name = "m_summary_label";
      // 
      // m_buffer_view
      // 
      resources.ApplyResources(this.m_buffer_view, "m_buffer_view");
      this.m_buffer_view.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.m_buffer_view.Name = "m_buffer_view";
      // 
      // InfoBufferForm
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ControlBox = false;
      this.Controls.Add(this.m_buffer_view);
      this.Controls.Add(this.m_top_panel);
      this.Controls.Add(this.m_bottom_panel);
      this.Name = "InfoBufferForm";
      this.ShowInTaskbar = false;
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