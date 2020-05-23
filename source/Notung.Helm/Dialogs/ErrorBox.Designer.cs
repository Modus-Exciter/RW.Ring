namespace Notung.Helm.Dialogs
{
  partial class ErrorBox
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorBox));
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.m_button_ok = new System.Windows.Forms.Button();
      this.m_text_box = new System.Windows.Forms.TextBox();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 3;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Controls.Add(this.m_button_ok, 1, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 326);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
      this.tableLayoutPanel1.RowCount = 1;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(507, 37);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // m_button_ok
      // 
      this.m_button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_button_ok.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
      this.m_button_ok.Location = new System.Drawing.Point(211, 8);
      this.m_button_ok.Name = "m_button_ok";
      this.m_button_ok.Size = new System.Drawing.Size(84, 26);
      this.m_button_ok.TabIndex = 0;
      this.m_button_ok.Text = "OK";
      this.m_button_ok.UseVisualStyleBackColor = true;
      // 
      // m_text_box
      // 
      this.m_text_box.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_text_box.Location = new System.Drawing.Point(5, 5);
      this.m_text_box.Multiline = true;
      this.m_text_box.Name = "m_text_box";
      this.m_text_box.ReadOnly = true;
      this.m_text_box.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.m_text_box.Size = new System.Drawing.Size(507, 321);
      this.m_text_box.TabIndex = 1;
      // 
      // ErrorBox
      // 
      this.AcceptButton = this.m_button_ok;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(517, 368);
      this.Controls.Add(this.m_text_box);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(250, 150);
      this.Name = "ErrorBox";
      this.Padding = new System.Windows.Forms.Padding(5);
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Error";
      this.tableLayoutPanel1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Button m_button_ok;
    private System.Windows.Forms.TextBox m_text_box;
  }
}