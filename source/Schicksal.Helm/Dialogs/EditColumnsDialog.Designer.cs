namespace Schicksal.Helm.Dialogs
{
  partial class EditColumnsDialog
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditColumnsDialog));
      this.m_bottom_panel = new System.Windows.Forms.FlowLayoutPanel();
      this.m_button_cancel = new System.Windows.Forms.Button();
      this.m_button_ok = new System.Windows.Forms.Button();
      this.m_grid = new System.Windows.Forms.DataGridView();
      this.columnNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.columnTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
      this.m_binding_source = new System.Windows.Forms.BindingSource(this.components);
      this.m_bottom_panel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).BeginInit();
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
      this.m_bottom_panel.Location = new System.Drawing.Point(0, 358);
      this.m_bottom_panel.Name = "m_bottom_panel";
      this.m_bottom_panel.Padding = new System.Windows.Forms.Padding(4);
      this.m_bottom_panel.Size = new System.Drawing.Size(339, 41);
      this.m_bottom_panel.TabIndex = 0;
      // 
      // m_button_cancel
      // 
      this.m_button_cancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.m_button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_button_cancel.Location = new System.Drawing.Point(251, 7);
      this.m_button_cancel.Name = "m_button_cancel";
      this.m_button_cancel.Size = new System.Drawing.Size(75, 25);
      this.m_button_cancel.TabIndex = 0;
      this.m_button_cancel.Text = "Cancel";
      this.m_button_cancel.UseVisualStyleBackColor = true;
      // 
      // m_button_ok
      // 
      this.m_button_ok.AutoSize = true;
      this.m_button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_button_ok.Location = new System.Drawing.Point(170, 7);
      this.m_button_ok.Name = "m_button_ok";
      this.m_button_ok.Size = new System.Drawing.Size(75, 25);
      this.m_button_ok.TabIndex = 1;
      this.m_button_ok.Text = "OK";
      this.m_button_ok.UseVisualStyleBackColor = true;
      // 
      // m_grid
      // 
      this.m_grid.AutoGenerateColumns = false;
      this.m_grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.m_grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnNameDataGridViewTextBoxColumn,
            this.columnTypeDataGridViewTextBoxColumn});
      this.m_grid.DataSource = this.m_binding_source;
      this.m_grid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_grid.Location = new System.Drawing.Point(0, 0);
      this.m_grid.Name = "m_grid";
      this.m_grid.Size = new System.Drawing.Size(339, 358);
      this.m_grid.TabIndex = 1;
      // 
      // columnNameDataGridViewTextBoxColumn
      // 
      this.columnNameDataGridViewTextBoxColumn.DataPropertyName = "ColumnName";
      this.columnNameDataGridViewTextBoxColumn.HeaderText = "ColumnName";
      this.columnNameDataGridViewTextBoxColumn.Name = "columnNameDataGridViewTextBoxColumn";
      // 
      // columnTypeDataGridViewTextBoxColumn
      // 
      this.columnTypeDataGridViewTextBoxColumn.DataPropertyName = "ColumnType";
      this.columnTypeDataGridViewTextBoxColumn.HeaderText = "ColumnType";
      this.columnTypeDataGridViewTextBoxColumn.Name = "columnTypeDataGridViewTextBoxColumn";
      this.columnTypeDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
      this.columnTypeDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
      // 
      // m_binding_source
      // 
      this.m_binding_source.DataSource = typeof(Schicksal.Helm.Dialogs.TableColumnInfo);
      // 
      // EditColumnsDialog
      // 
      this.AcceptButton = this.m_button_ok;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.m_button_cancel;
      this.ClientSize = new System.Drawing.Size(339, 399);
      this.Controls.Add(this.m_grid);
      this.Controls.Add(this.m_bottom_panel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "EditColumnsDialog";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.m_bottom_panel.ResumeLayout(false);
      this.m_bottom_panel.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel m_bottom_panel;
    private System.Windows.Forms.Button m_button_cancel;
    private System.Windows.Forms.Button m_button_ok;
    private System.Windows.Forms.DataGridView m_grid;
    private System.Windows.Forms.BindingSource m_binding_source;
    private System.Windows.Forms.DataGridViewTextBoxColumn columnNameDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewComboBoxColumn columnTypeDataGridViewTextBoxColumn;
  }
}