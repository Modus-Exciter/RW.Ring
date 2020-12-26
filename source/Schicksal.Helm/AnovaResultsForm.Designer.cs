namespace Schicksal.Helm
{
  partial class AnovaResultsForm
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
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnovaResultsForm));
      this.m_grid = new System.Windows.Forms.DataGridView();
      this.m_binding_source = new System.Windows.Forms.BindingSource(this.components);
      this.factorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.kdfDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.ndfDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.fDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.f005DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.f001DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.pDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).BeginInit();
      this.SuspendLayout();
      // 
      // m_grid
      // 
      this.m_grid.AutoGenerateColumns = false;
      this.m_grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.m_grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.factorDataGridViewTextBoxColumn,
            this.kdfDataGridViewTextBoxColumn,
            this.ndfDataGridViewTextBoxColumn,
            this.fDataGridViewTextBoxColumn,
            this.f005DataGridViewTextBoxColumn,
            this.f001DataGridViewTextBoxColumn,
            this.pDataGridViewTextBoxColumn});
      this.m_grid.DataSource = this.m_binding_source;
      dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle4.Format = "0.0000";
      dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.m_grid.DefaultCellStyle = dataGridViewCellStyle4;
      this.m_grid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_grid.Location = new System.Drawing.Point(0, 0);
      this.m_grid.Name = "m_grid";
      this.m_grid.Size = new System.Drawing.Size(609, 482);
      this.m_grid.TabIndex = 0;
      this.m_grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_grid_CellDoubleClick);
      this.m_grid.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.m_grid_CellPainting);
      // 
      // m_binding_source
      // 
      this.m_binding_source.DataSource = typeof(Schicksal.Anova.FisherTestResult);
      // 
      // factorDataGridViewTextBoxColumn
      // 
      this.factorDataGridViewTextBoxColumn.DataPropertyName = "Factor";
      this.factorDataGridViewTextBoxColumn.HeaderText = "Factor";
      this.factorDataGridViewTextBoxColumn.Name = "factorDataGridViewTextBoxColumn";
      this.factorDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // kdfDataGridViewTextBoxColumn
      // 
      this.kdfDataGridViewTextBoxColumn.DataPropertyName = "Kdf";
      dataGridViewCellStyle1.Format = "0";
      this.kdfDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
      this.kdfDataGridViewTextBoxColumn.HeaderText = "Kdf";
      this.kdfDataGridViewTextBoxColumn.Name = "kdfDataGridViewTextBoxColumn";
      this.kdfDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // ndfDataGridViewTextBoxColumn
      // 
      this.ndfDataGridViewTextBoxColumn.DataPropertyName = "Ndf";
      dataGridViewCellStyle2.Format = "0";
      this.ndfDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
      this.ndfDataGridViewTextBoxColumn.HeaderText = "Ndf";
      this.ndfDataGridViewTextBoxColumn.Name = "ndfDataGridViewTextBoxColumn";
      this.ndfDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // fDataGridViewTextBoxColumn
      // 
      this.fDataGridViewTextBoxColumn.DataPropertyName = "F";
      this.fDataGridViewTextBoxColumn.HeaderText = "F";
      this.fDataGridViewTextBoxColumn.Name = "fDataGridViewTextBoxColumn";
      this.fDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // f005DataGridViewTextBoxColumn
      // 
      this.f005DataGridViewTextBoxColumn.DataPropertyName = "F005";
      this.f005DataGridViewTextBoxColumn.HeaderText = "F 5%";
      this.f005DataGridViewTextBoxColumn.Name = "f005DataGridViewTextBoxColumn";
      this.f005DataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // f001DataGridViewTextBoxColumn
      // 
      this.f001DataGridViewTextBoxColumn.DataPropertyName = "F001";
      this.f001DataGridViewTextBoxColumn.HeaderText = "F 1%";
      this.f001DataGridViewTextBoxColumn.Name = "f001DataGridViewTextBoxColumn";
      this.f001DataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // pDataGridViewTextBoxColumn
      // 
      this.pDataGridViewTextBoxColumn.DataPropertyName = "P";
      dataGridViewCellStyle3.Format = "g";
      this.pDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle3;
      this.pDataGridViewTextBoxColumn.HeaderText = "p";
      this.pDataGridViewTextBoxColumn.Name = "pDataGridViewTextBoxColumn";
      this.pDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // AnovaResultsForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(609, 482);
      this.Controls.Add(this.m_grid);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "AnovaResultsForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "AnovaResultsForm";
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView m_grid;
    private System.Windows.Forms.BindingSource m_binding_source;
    private System.Windows.Forms.DataGridViewTextBoxColumn factorDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn kdfDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn ndfDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn fDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn f005DataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn f001DataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn pDataGridViewTextBoxColumn;
  }
}