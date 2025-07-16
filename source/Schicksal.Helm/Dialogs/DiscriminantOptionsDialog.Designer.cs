namespace Schicksal.Helm.Dialogs
{
  partial class DiscriminantOptionsDialog
  {
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.RadioButton m_radio_none;
    private System.Windows.Forms.RadioButton m_radio_entropy;
    private System.Windows.Forms.RadioButton m_radio_gini;
    private System.Windows.Forms.Button m_button_ok;
    private System.Windows.Forms.Button m_button_cancel;
    private System.Windows.Forms.FlowLayoutPanel m_buttons_panel;
    private System.Windows.Forms.GroupBox m_groupbox;

    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
        components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiscriminantOptionsDialog));
            this.m_radio_none = new System.Windows.Forms.RadioButton();
            this.m_radio_entropy = new System.Windows.Forms.RadioButton();
            this.m_radio_gini = new System.Windows.Forms.RadioButton();
            this.m_button_ok = new System.Windows.Forms.Button();
            this.m_button_cancel = new System.Windows.Forms.Button();
            this.m_buttons_panel = new System.Windows.Forms.FlowLayoutPanel();
            this.m_groupbox = new System.Windows.Forms.GroupBox();
            this.m_buttons_panel.SuspendLayout();
            this.m_groupbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_radio_none
            // 
            resources.ApplyResources(this.m_radio_none, "m_radio_none");
            this.m_radio_none.Checked = true;
            this.m_radio_none.Name = "m_radio_none";
            this.m_radio_none.TabStop = true;
            this.m_radio_none.UseVisualStyleBackColor = true;
            // 
            // m_radio_entropy
            // 
            resources.ApplyResources(this.m_radio_entropy, "m_radio_entropy");
            this.m_radio_entropy.Name = "m_radio_entropy";
            this.m_radio_entropy.UseVisualStyleBackColor = true;
            // 
            // m_radio_gini
            // 
            resources.ApplyResources(this.m_radio_gini, "m_radio_gini");
            this.m_radio_gini.Name = "m_radio_gini";
            this.m_radio_gini.UseVisualStyleBackColor = true;
            // 
            // m_button_ok
            // 
            resources.ApplyResources(this.m_button_ok, "m_button_ok");
            this.m_button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_button_ok.Name = "m_button_ok";
            this.m_button_ok.UseVisualStyleBackColor = true;
            // 
            // m_button_cancel
            // 
            resources.ApplyResources(this.m_button_cancel, "m_button_cancel");
            this.m_button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_button_cancel.Name = "m_button_cancel";
            this.m_button_cancel.UseVisualStyleBackColor = true;
            // 
            // m_buttons_panel
            // 
            resources.ApplyResources(this.m_buttons_panel, "m_buttons_panel");
            this.m_buttons_panel.Controls.Add(this.m_button_ok);
            this.m_buttons_panel.Controls.Add(this.m_button_cancel);
            this.m_buttons_panel.Name = "m_buttons_panel";
            // 
            // m_groupbox
            // 
            resources.ApplyResources(this.m_groupbox, "m_groupbox");
            this.m_groupbox.Controls.Add(this.m_radio_none);
            this.m_groupbox.Controls.Add(this.m_radio_entropy);
            this.m_groupbox.Controls.Add(this.m_radio_gini);
            this.m_groupbox.Name = "m_groupbox";
            this.m_groupbox.TabStop = false;
            // 
            // DiscriminantOptionsDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.m_groupbox);
            this.Controls.Add(this.m_buttons_panel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DiscriminantOptionsDialog";
            this.m_buttons_panel.ResumeLayout(false);
            this.m_groupbox.ResumeLayout(false);
            this.m_groupbox.PerformLayout();
            this.ResumeLayout(false);

    }
  }
}
