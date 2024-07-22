
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnovaOptionsDialog));
      this.m_bottom_panel = new System.Windows.Forms.FlowLayoutPanel();
      this.m_button_cancel = new System.Windows.Forms.Button();
      this.m_button_ok = new System.Windows.Forms.Button();
      this.m_layout_panel = new System.Windows.Forms.TableLayoutPanel();
      this.m_label_normalization = new System.Windows.Forms.Label();
      this.m_btn_no_norm = new System.Windows.Forms.RadioButton();
      this.m_btn_kruskal_wallis = new System.Windows.Forms.RadioButton();
      this.m_btn_box_cox = new System.Windows.Forms.RadioButton();
      this.m_label_conjuate = new System.Windows.Forms.Label();
      this.m_cb_conjugate = new System.Windows.Forms.ComboBox();
      this.m_bottom_panel.SuspendLayout();
      this.m_layout_panel.SuspendLayout();
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
      // m_layout_panel
      // 
      resources.ApplyResources(this.m_layout_panel, "m_layout_panel");
      this.m_layout_panel.Controls.Add(this.m_label_normalization, 0, 0);
      this.m_layout_panel.Controls.Add(this.m_btn_no_norm, 0, 1);
      this.m_layout_panel.Controls.Add(this.m_btn_kruskal_wallis, 0, 2);
      this.m_layout_panel.Controls.Add(this.m_btn_box_cox, 0, 3);
      this.m_layout_panel.Controls.Add(this.m_label_conjuate, 1, 0);
      this.m_layout_panel.Controls.Add(this.m_cb_conjugate, 1, 1);
      this.m_layout_panel.Name = "m_layout_panel";
      // 
      // m_label_normalization
      // 
      resources.ApplyResources(this.m_label_normalization, "m_label_normalization");
      this.m_label_normalization.Name = "m_label_normalization";
      // 
      // m_btn_no_norm
      // 
      resources.ApplyResources(this.m_btn_no_norm, "m_btn_no_norm");
      this.m_btn_no_norm.Checked = true;
      this.m_btn_no_norm.Name = "m_btn_no_norm";
      this.m_btn_no_norm.TabStop = true;
      this.m_btn_no_norm.UseVisualStyleBackColor = true;
      // 
      // m_btn_kruskal_wallis
      // 
      resources.ApplyResources(this.m_btn_kruskal_wallis, "m_btn_kruskal_wallis");
      this.m_btn_kruskal_wallis.Name = "m_btn_kruskal_wallis";
      this.m_btn_kruskal_wallis.TabStop = true;
      this.m_btn_kruskal_wallis.UseVisualStyleBackColor = true;
      // 
      // m_btn_box_cox
      // 
      resources.ApplyResources(this.m_btn_box_cox, "m_btn_box_cox");
      this.m_btn_box_cox.Name = "m_btn_box_cox";
      this.m_btn_box_cox.TabStop = true;
      this.m_btn_box_cox.UseVisualStyleBackColor = true;
      // 
      // m_label_conjuate
      // 
      resources.ApplyResources(this.m_label_conjuate, "m_label_conjuate");
      this.m_label_conjuate.Name = "m_label_conjuate";
      // 
      // m_cb_conjugate
      // 
      resources.ApplyResources(this.m_cb_conjugate, "m_cb_conjugate");
      this.m_cb_conjugate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_cb_conjugate.FormattingEnabled = true;
      this.m_cb_conjugate.Name = "m_cb_conjugate";
      // 
      // AnovaOptionsDialog
      // 
      this.AcceptButton = this.m_button_ok;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.m_button_cancel;
      this.Controls.Add(this.m_layout_panel);
      this.Controls.Add(this.m_bottom_panel);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AnovaOptionsDialog";
      this.ShowInTaskbar = false;
      this.m_bottom_panel.ResumeLayout(false);
      this.m_bottom_panel.PerformLayout();
      this.m_layout_panel.ResumeLayout(false);
      this.m_layout_panel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel m_bottom_panel;
    private System.Windows.Forms.Button m_button_cancel;
    private System.Windows.Forms.Button m_button_ok;
    private System.Windows.Forms.TableLayoutPanel m_layout_panel;
    private System.Windows.Forms.RadioButton m_btn_no_norm;
    private System.Windows.Forms.RadioButton m_btn_kruskal_wallis;
    private System.Windows.Forms.RadioButton m_btn_box_cox;
    private System.Windows.Forms.Label m_label_normalization;
    private System.Windows.Forms.Label m_label_conjuate;
    private System.Windows.Forms.ComboBox m_cb_conjugate;
  }
}