namespace ImportWofostResults
{
  partial class WofostResultsImportForm
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
      this.m_excel_edit = new System.Windows.Forms.TextBox();
      this.m_third_edit = new System.Windows.Forms.TextBox();
      this.m_second_edit = new System.Windows.Forms.TextBox();
      this.m_button_open_excel = new System.Windows.Forms.Button();
      this.m_button_open_third = new System.Windows.Forms.Button();
      this.m_button_open_second = new System.Windows.Forms.Button();
      this.m_bottom_panel.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_bottom_panel
      // 
      this.m_bottom_panel.AutoSize = true;
      this.m_bottom_panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.m_bottom_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.m_bottom_panel.Controls.Add(this.m_button_cancel);
      this.m_bottom_panel.Controls.Add(this.m_button_ok);
      this.m_bottom_panel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.m_bottom_panel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
      this.m_bottom_panel.Location = new System.Drawing.Point(0, 109);
      this.m_bottom_panel.Name = "m_bottom_panel";
      this.m_bottom_panel.Padding = new System.Windows.Forms.Padding(4);
      this.m_bottom_panel.Size = new System.Drawing.Size(315, 41);
      this.m_bottom_panel.TabIndex = 3;
      // 
      // m_button_cancel
      // 
      this.m_button_cancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.m_button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_button_cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.m_button_cancel.Location = new System.Drawing.Point(227, 7);
      this.m_button_cancel.Name = "m_button_cancel";
      this.m_button_cancel.Size = new System.Drawing.Size(75, 25);
      this.m_button_cancel.TabIndex = 0;
      this.m_button_cancel.Text = "Отмена";
      this.m_button_cancel.UseVisualStyleBackColor = true;
      // 
      // m_button_ok
      // 
      this.m_button_ok.AutoSize = true;
      this.m_button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_button_ok.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.m_button_ok.Location = new System.Drawing.Point(146, 7);
      this.m_button_ok.Name = "m_button_ok";
      this.m_button_ok.Size = new System.Drawing.Size(75, 25);
      this.m_button_ok.TabIndex = 1;
      this.m_button_ok.Text = "OK";
      this.m_button_ok.UseVisualStyleBackColor = true;
      // 
      // m_excel_edit
      // 
      this.m_excel_edit.Location = new System.Drawing.Point(12, 12);
      this.m_excel_edit.Name = "m_excel_edit";
      this.m_excel_edit.ReadOnly = true;
      this.m_excel_edit.Size = new System.Drawing.Size(253, 20);
      this.m_excel_edit.TabIndex = 4;
      // 
      // m_third_edit
      // 
      this.m_third_edit.Location = new System.Drawing.Point(12, 41);
      this.m_third_edit.Name = "m_third_edit";
      this.m_third_edit.ReadOnly = true;
      this.m_third_edit.Size = new System.Drawing.Size(253, 20);
      this.m_third_edit.TabIndex = 5;
      // 
      // m_second_edit
      // 
      this.m_second_edit.Location = new System.Drawing.Point(12, 72);
      this.m_second_edit.Name = "m_second_edit";
      this.m_second_edit.ReadOnly = true;
      this.m_second_edit.Size = new System.Drawing.Size(253, 20);
      this.m_second_edit.TabIndex = 6;
      // 
      // m_button_open_excel
      // 
      this.m_button_open_excel.AutoSize = true;
      this.m_button_open_excel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.m_button_open_excel.Location = new System.Drawing.Point(271, 12);
      this.m_button_open_excel.Name = "m_button_open_excel";
      this.m_button_open_excel.Size = new System.Drawing.Size(32, 23);
      this.m_button_open_excel.TabIndex = 7;
      this.m_button_open_excel.Text = ".xls";
      this.m_button_open_excel.UseVisualStyleBackColor = true;
      this.m_button_open_excel.Click += new System.EventHandler(this.m_button_open_excel_Click);
      // 
      // m_button_open_third
      // 
      this.m_button_open_third.AutoSize = true;
      this.m_button_open_third.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.m_button_open_third.Location = new System.Drawing.Point(271, 39);
      this.m_button_open_third.Name = "m_button_open_third";
      this.m_button_open_third.Size = new System.Drawing.Size(23, 23);
      this.m_button_open_third.TabIndex = 8;
      this.m_button_open_third.Text = "3";
      this.m_button_open_third.UseVisualStyleBackColor = true;
      this.m_button_open_third.Click += new System.EventHandler(this.m_button_open_third_Click);
      // 
      // m_button_open_second
      // 
      this.m_button_open_second.AutoSize = true;
      this.m_button_open_second.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.m_button_open_second.Location = new System.Drawing.Point(271, 70);
      this.m_button_open_second.Name = "m_button_open_second";
      this.m_button_open_second.Size = new System.Drawing.Size(23, 23);
      this.m_button_open_second.TabIndex = 9;
      this.m_button_open_second.Text = "2";
      this.m_button_open_second.UseVisualStyleBackColor = true;
      this.m_button_open_second.Click += new System.EventHandler(this.m_button_open_second_Click);
      // 
      // WofostResultsImportForm
      // 
      this.AcceptButton = this.m_button_ok;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.CancelButton = this.m_button_cancel;
      this.ClientSize = new System.Drawing.Size(315, 150);
      this.Controls.Add(this.m_button_open_second);
      this.Controls.Add(this.m_button_open_third);
      this.Controls.Add(this.m_button_open_excel);
      this.Controls.Add(this.m_second_edit);
      this.Controls.Add(this.m_third_edit);
      this.Controls.Add(this.m_excel_edit);
      this.Controls.Add(this.m_bottom_panel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(295, 180);
      this.Name = "WofostResultsImportForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Импорт результатов расчёта по WOFOST";
      this.m_bottom_panel.ResumeLayout(false);
      this.m_bottom_panel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel m_bottom_panel;
    private System.Windows.Forms.Button m_button_cancel;
    private System.Windows.Forms.Button m_button_ok;
    private System.Windows.Forms.TextBox m_excel_edit;
    private System.Windows.Forms.TextBox m_third_edit;
    private System.Windows.Forms.TextBox m_second_edit;
    private System.Windows.Forms.Button m_button_open_excel;
    private System.Windows.Forms.Button m_button_open_third;
    private System.Windows.Forms.Button m_button_open_second;
  }
}