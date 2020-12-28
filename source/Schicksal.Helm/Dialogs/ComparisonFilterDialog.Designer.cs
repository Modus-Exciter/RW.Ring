namespace Schicksal.Helm.Dialogs
{
  partial class ComparisonFilterDialog
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComparisonFilterDialog));
      this.m_bottom_panel = new System.Windows.Forms.FlowLayoutPanel();
      this.m_button_cancel = new System.Windows.Forms.Button();
      this.m_button_ok = new System.Windows.Forms.Button();
      this.m_check_only_significat = new System.Windows.Forms.CheckBox();
      this.m_label_box = new System.Windows.Forms.Label();
      this.m_select_edit = new System.Windows.Forms.ComboBox();
      this.m_bottom_panel.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_bottom_panel
      // 
      resources.ApplyResources(this.m_bottom_panel, "m_bottom_panel");
      this.m_bottom_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.m_bottom_panel.Controls.Add(this.m_button_cancel);
      this.m_bottom_panel.Controls.Add(this.m_button_ok);
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
      // m_check_only_significat
      // 
      resources.ApplyResources(this.m_check_only_significat, "m_check_only_significat");
      this.m_check_only_significat.Name = "m_check_only_significat";
      this.m_check_only_significat.UseVisualStyleBackColor = true;
      // 
      // m_label_box
      // 
      resources.ApplyResources(this.m_label_box, "m_label_box");
      this.m_label_box.Name = "m_label_box";
      // 
      // m_select_edit
      // 
      this.m_select_edit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_select_edit.FormattingEnabled = true;
      resources.ApplyResources(this.m_select_edit, "m_select_edit");
      this.m_select_edit.Name = "m_select_edit";
      // 
      // ComparisonFilterDialog
      // 
      this.AcceptButton = this.m_button_ok;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.m_button_cancel;
      this.Controls.Add(this.m_select_edit);
      this.Controls.Add(this.m_label_box);
      this.Controls.Add(this.m_check_only_significat);
      this.Controls.Add(this.m_bottom_panel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ComparisonFilterDialog";
      this.ShowInTaskbar = false;
      this.m_bottom_panel.ResumeLayout(false);
      this.m_bottom_panel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel m_bottom_panel;
    private System.Windows.Forms.Button m_button_cancel;
    private System.Windows.Forms.Button m_button_ok;
    private System.Windows.Forms.CheckBox m_check_only_significat;
    private System.Windows.Forms.Label m_label_box;
    private System.Windows.Forms.ComboBox m_select_edit;
  }
}