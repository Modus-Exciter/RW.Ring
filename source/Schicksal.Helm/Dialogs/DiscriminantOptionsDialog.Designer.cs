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
      this.m_radio_none = new System.Windows.Forms.RadioButton();
      this.m_radio_entropy = new System.Windows.Forms.RadioButton();
      this.m_radio_gini = new System.Windows.Forms.RadioButton();
      this.m_button_ok = new System.Windows.Forms.Button();
      this.m_button_cancel = new System.Windows.Forms.Button();
      this.m_buttons_panel = new System.Windows.Forms.FlowLayoutPanel();
      this.m_groupbox = new System.Windows.Forms.GroupBox();

      // 
      // m_radio_none
      // 
      this.m_radio_none.AutoSize = true;
      this.m_radio_none.Location = new System.Drawing.Point(20, 20);
      this.m_radio_none.Name = "m_radio_none";
      this.m_radio_none.Size = new System.Drawing.Size(51, 17);
      this.m_radio_none.Text = "None";
      this.m_radio_none.Checked = true;
      this.m_radio_none.UseVisualStyleBackColor = true;

      // 
      // m_radio_entropy
      // 
      this.m_radio_entropy.AutoSize = true;
      this.m_radio_entropy.Location = new System.Drawing.Point(20, 45);
      this.m_radio_entropy.Name = "m_radio_entropy";
      this.m_radio_entropy.Size = new System.Drawing.Size(70, 17);
      this.m_radio_entropy.Text = "Entropy";
      this.m_radio_entropy.UseVisualStyleBackColor = true;

      // 
      // m_radio_gini
      // 
      this.m_radio_gini.AutoSize = true;
      this.m_radio_gini.Location = new System.Drawing.Point(20, 70);
      this.m_radio_gini.Name = "m_radio_gini";
      this.m_radio_gini.Size = new System.Drawing.Size(44, 17);
      this.m_radio_gini.Text = "Gini";
      this.m_radio_gini.UseVisualStyleBackColor = true;

      // 
      // m_button_ok
      // 
      this.m_button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_button_ok.Text = "OK";
      this.m_button_ok.UseVisualStyleBackColor = true;

      // 
      // m_button_cancel
      // 
      this.m_button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_button_cancel.Text = "Cancel";
      this.m_button_cancel.UseVisualStyleBackColor = true;

      // 
      // m_buttons_panel
      // 
      this.m_buttons_panel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
      this.m_buttons_panel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.m_buttons_panel.Controls.Add(this.m_button_ok);
      this.m_buttons_panel.Controls.Add(this.m_button_cancel);
      this.m_buttons_panel.Height = 40;

      // 
      // m_groupbox
      // 
      this.m_groupbox.Text = "Split Criterion";
      this.m_groupbox.Controls.Add(this.m_radio_none);
      this.m_groupbox.Controls.Add(this.m_radio_entropy);
      this.m_groupbox.Controls.Add(this.m_radio_gini);
      this.m_groupbox.Dock = System.Windows.Forms.DockStyle.Fill;

      // 
      // DiscriminantOptionsDialog
      // 
      this.ClientSize = new System.Drawing.Size(300, 150);
      this.Controls.Add(this.m_groupbox);
      this.Controls.Add(this.m_buttons_panel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Location = System.Windows.Forms.Cursor.Position; // появится около курсора
      this.Name = "DiscriminantOptionsDialog";
      this.Text = "Discriminant Options";
    }
  }
}
