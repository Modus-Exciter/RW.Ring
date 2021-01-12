namespace JetExcelOleDbImport
{
  partial class ExcelImportForm
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
      this.m_path_edit = new System.Windows.Forms.TextBox();
      this.m_button_open_excel = new System.Windows.Forms.Button();
      this.m_table_edit = new System.Windows.Forms.ComboBox();
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
      this.m_bottom_panel.Location = new System.Drawing.Point(0, 87);
      this.m_bottom_panel.Name = "m_bottom_panel";
      this.m_bottom_panel.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
      this.m_bottom_panel.Size = new System.Drawing.Size(322, 43);
      this.m_bottom_panel.TabIndex = 3;
      // 
      // m_button_cancel
      // 
      this.m_button_cancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.m_button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_button_cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.m_button_cancel.Location = new System.Drawing.Point(234, 7);
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
      this.m_button_ok.Location = new System.Drawing.Point(153, 7);
      this.m_button_ok.Name = "m_button_ok";
      this.m_button_ok.Size = new System.Drawing.Size(75, 27);
      this.m_button_ok.TabIndex = 1;
      this.m_button_ok.Text = "OK";
      this.m_button_ok.UseVisualStyleBackColor = true;
      // 
      // m_path_edit
      // 
      this.m_path_edit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_path_edit.Location = new System.Drawing.Point(12, 12);
      this.m_path_edit.Name = "m_path_edit";
      this.m_path_edit.ReadOnly = true;
      this.m_path_edit.Size = new System.Drawing.Size(266, 20);
      this.m_path_edit.TabIndex = 4;
      // 
      // m_button_open_excel
      // 
      this.m_button_open_excel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_button_open_excel.AutoSize = true;
      this.m_button_open_excel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.m_button_open_excel.Location = new System.Drawing.Point(280, 11);
      this.m_button_open_excel.Name = "m_button_open_excel";
      this.m_button_open_excel.Size = new System.Drawing.Size(32, 23);
      this.m_button_open_excel.TabIndex = 7;
      this.m_button_open_excel.Text = ".xls";
      this.m_button_open_excel.UseVisualStyleBackColor = true;
      this.m_button_open_excel.Click += new System.EventHandler(this.m_button_open_excel_Click);
      // 
      // m_table_edit
      // 
      this.m_table_edit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_table_edit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_table_edit.FormattingEnabled = true;
      this.m_table_edit.Location = new System.Drawing.Point(12, 41);
      this.m_table_edit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
      this.m_table_edit.Name = "m_table_edit";
      this.m_table_edit.Size = new System.Drawing.Size(266, 21);
      this.m_table_edit.TabIndex = 9;
      // 
      // ExcelImportForm
      // 
      this.AcceptButton = this.m_button_ok;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.CancelButton = this.m_button_cancel;
      this.ClientSize = new System.Drawing.Size(322, 130);
      this.Controls.Add(this.m_table_edit);
      this.Controls.Add(this.m_button_open_excel);
      this.Controls.Add(this.m_path_edit);
      this.Controls.Add(this.m_bottom_panel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(292, 160);
      this.Name = "ExcelImportForm";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Импорт данных из Excel";
      this.m_bottom_panel.ResumeLayout(false);
      this.m_bottom_panel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel m_bottom_panel;
    private System.Windows.Forms.Button m_button_cancel;
    private System.Windows.Forms.Button m_button_ok;
    private System.Windows.Forms.TextBox m_path_edit;
    private System.Windows.Forms.Button m_button_open_excel;
    private System.Windows.Forms.ComboBox m_table_edit;
  }
}