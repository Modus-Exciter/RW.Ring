
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
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.m_btn_no_norm = new System.Windows.Forms.RadioButton();
      this.m_btn_non_parametric = new System.Windows.Forms.RadioButton();
      this.m_btn_box_cox = new System.Windows.Forms.RadioButton();
      this.label1 = new System.Windows.Forms.Label();
      this.m_bottom_panel.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
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
      // tableLayoutPanel1
      // 
      resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
      this.tableLayoutPanel1.Controls.Add(this.m_btn_no_norm, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.m_btn_non_parametric, 2, 0);
      this.tableLayoutPanel1.Controls.Add(this.m_btn_box_cox, 3, 0);
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      // 
      // m_btn_no_norm
      // 
      resources.ApplyResources(this.m_btn_no_norm, "m_btn_no_norm");
      this.m_btn_no_norm.Checked = true;
      this.m_btn_no_norm.Name = "m_btn_no_norm";
      this.m_btn_no_norm.TabStop = true;
      this.m_btn_no_norm.UseVisualStyleBackColor = true;
      // 
      // m_btn_non_parametric
      // 
      resources.ApplyResources(this.m_btn_non_parametric, "m_btn_non_parametric");
      this.m_btn_non_parametric.Name = "m_btn_non_parametric";
      this.m_btn_non_parametric.TabStop = true;
      this.m_btn_non_parametric.UseVisualStyleBackColor = true;
      // 
      // m_btn_box_cox
      // 
      resources.ApplyResources(this.m_btn_box_cox, "m_btn_box_cox");
      this.m_btn_box_cox.Name = "m_btn_box_cox";
      this.m_btn_box_cox.TabStop = true;
      this.m_btn_box_cox.UseVisualStyleBackColor = true;
      // 
      // label1
      // 
      resources.ApplyResources(this.label1, "label1");
      this.label1.Name = "label1";
      // 
      // AnovaOptionsDialog
      // 
      this.AcceptButton = this.m_button_ok;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.m_button_cancel;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Controls.Add(this.m_bottom_panel);
      this.Name = "AnovaOptionsDialog";
      this.m_bottom_panel.ResumeLayout(false);
      this.m_bottom_panel.PerformLayout();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel m_bottom_panel;
    private System.Windows.Forms.Button m_button_cancel;
    private System.Windows.Forms.Button m_button_ok;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.RadioButton m_btn_no_norm;
    private System.Windows.Forms.RadioButton m_btn_non_parametric;
    private System.Windows.Forms.RadioButton m_btn_box_cox;
    private System.Windows.Forms.Label label1;
  }
}