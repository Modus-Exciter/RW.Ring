
namespace CertGenerator
{
  partial class PasswordForm
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
      this.m_ok_button = new System.Windows.Forms.Button();
      this.m_password_edit = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // m_ok_button
      // 
      this.m_ok_button.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_ok_button.Location = new System.Drawing.Point(121, 54);
      this.m_ok_button.Name = "m_ok_button";
      this.m_ok_button.Size = new System.Drawing.Size(112, 35);
      this.m_ok_button.TabIndex = 0;
      this.m_ok_button.Text = "OK";
      this.m_ok_button.UseVisualStyleBackColor = true;
      // 
      // m_password_edit
      // 
      this.m_password_edit.Location = new System.Drawing.Point(12, 25);
      this.m_password_edit.Name = "m_password_edit";
      this.m_password_edit.PasswordChar = '*';
      this.m_password_edit.Size = new System.Drawing.Size(326, 22);
      this.m_password_edit.TabIndex = 1;
      // 
      // PasswordForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(346, 101);
      this.Controls.Add(this.m_password_edit);
      this.Controls.Add(this.m_ok_button);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "PasswordForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Введите пароль";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button m_ok_button;
    private System.Windows.Forms.TextBox m_password_edit;
  }
}