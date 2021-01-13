namespace JetExcelOleDbImport
{
  partial class ExcelImportAsMatrixForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExcelImportAsMatrixForm));
      this.m_bottom_panel = new System.Windows.Forms.FlowLayoutPanel();
      this.m_button_cancel = new System.Windows.Forms.Button();
      this.m_button_ok = new System.Windows.Forms.Button();
      this.m_path_edit = new System.Windows.Forms.TextBox();
      this.m_button_open_excel = new System.Windows.Forms.Button();
      this.m_table_edit = new System.Windows.Forms.ComboBox();
      this.m_cell_edit = new System.Windows.Forms.TextBox();
      this.m_row_edit = new System.Windows.Forms.TextBox();
      this.m_column_edit = new System.Windows.Forms.TextBox();
      this.m_column_label = new System.Windows.Forms.Label();
      this.m_row_label = new System.Windows.Forms.Label();
      this.m_cell_label = new System.Windows.Forms.Label();
      this.m_file_label = new System.Windows.Forms.Label();
      this.m_table_label = new System.Windows.Forms.Label();
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
      // m_path_edit
      // 
      resources.ApplyResources(this.m_path_edit, "m_path_edit");
      this.m_path_edit.Name = "m_path_edit";
      this.m_path_edit.ReadOnly = true;
      // 
      // m_button_open_excel
      // 
      resources.ApplyResources(this.m_button_open_excel, "m_button_open_excel");
      this.m_button_open_excel.Name = "m_button_open_excel";
      this.m_button_open_excel.UseVisualStyleBackColor = true;
      this.m_button_open_excel.Click += new System.EventHandler(this.m_button_open_excel_Click);
      // 
      // m_table_edit
      // 
      resources.ApplyResources(this.m_table_edit, "m_table_edit");
      this.m_table_edit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_table_edit.FormattingEnabled = true;
      this.m_table_edit.Name = "m_table_edit";
      // 
      // m_cell_edit
      // 
      resources.ApplyResources(this.m_cell_edit, "m_cell_edit");
      this.m_cell_edit.Name = "m_cell_edit";
      // 
      // m_row_edit
      // 
      resources.ApplyResources(this.m_row_edit, "m_row_edit");
      this.m_row_edit.Name = "m_row_edit";
      // 
      // m_column_edit
      // 
      resources.ApplyResources(this.m_column_edit, "m_column_edit");
      this.m_column_edit.Name = "m_column_edit";
      // 
      // m_column_label
      // 
      resources.ApplyResources(this.m_column_label, "m_column_label");
      this.m_column_label.Name = "m_column_label";
      // 
      // m_row_label
      // 
      resources.ApplyResources(this.m_row_label, "m_row_label");
      this.m_row_label.Name = "m_row_label";
      // 
      // m_cell_label
      // 
      resources.ApplyResources(this.m_cell_label, "m_cell_label");
      this.m_cell_label.Name = "m_cell_label";
      // 
      // m_file_label
      // 
      resources.ApplyResources(this.m_file_label, "m_file_label");
      this.m_file_label.Name = "m_file_label";
      // 
      // m_table_label
      // 
      resources.ApplyResources(this.m_table_label, "m_table_label");
      this.m_table_label.Name = "m_table_label";
      // 
      // ExcelImportAsMatrixForm
      // 
      this.AcceptButton = this.m_button_ok;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.m_button_cancel;
      this.Controls.Add(this.m_table_label);
      this.Controls.Add(this.m_file_label);
      this.Controls.Add(this.m_cell_label);
      this.Controls.Add(this.m_row_label);
      this.Controls.Add(this.m_column_label);
      this.Controls.Add(this.m_column_edit);
      this.Controls.Add(this.m_row_edit);
      this.Controls.Add(this.m_cell_edit);
      this.Controls.Add(this.m_table_edit);
      this.Controls.Add(this.m_button_open_excel);
      this.Controls.Add(this.m_path_edit);
      this.Controls.Add(this.m_bottom_panel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ExcelImportAsMatrixForm";
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
    private System.Windows.Forms.TextBox m_path_edit;
    private System.Windows.Forms.Button m_button_open_excel;
    private System.Windows.Forms.ComboBox m_table_edit;
    private System.Windows.Forms.TextBox m_cell_edit;
    private System.Windows.Forms.TextBox m_row_edit;
    private System.Windows.Forms.TextBox m_column_edit;
    private System.Windows.Forms.Label m_column_label;
    private System.Windows.Forms.Label m_row_label;
    private System.Windows.Forms.Label m_cell_label;
    private System.Windows.Forms.Label m_file_label;
    private System.Windows.Forms.Label m_table_label;
  }
}