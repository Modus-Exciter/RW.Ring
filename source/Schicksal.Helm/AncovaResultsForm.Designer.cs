namespace Schicksal.Helm
{
  partial class AncovaResultsForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AncovaResultsForm));
      this.m_grid = new System.Windows.Forms.DataGridView();
      this.factorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.nDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.rDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.zDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.tDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.r005DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.r001DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.pDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.m_binding_source = new System.Windows.Forms.BindingSource(this.components);
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
            this.nDataGridViewTextBoxColumn,
            this.rDataGridViewTextBoxColumn,
            this.zDataGridViewTextBoxColumn,
            this.tDataGridViewTextBoxColumn,
            this.r005DataGridViewTextBoxColumn,
            this.r001DataGridViewTextBoxColumn,
            this.pDataGridViewTextBoxColumn});
      this.m_grid.DataSource = this.m_binding_source;
      this.m_grid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_grid.Location = new System.Drawing.Point(0, 0);
      this.m_grid.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
      this.m_grid.Name = "m_grid";
      this.m_grid.RowHeadersWidth = 51;
      this.m_grid.Size = new System.Drawing.Size(1012, 594);
      this.m_grid.TabIndex = 0;
      this.m_grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Grid_CellDoubleClick);
      this.m_grid.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.Grid_CellPainting);
      // 
      // factorDataGridViewTextBoxColumn
      // 
      this.factorDataGridViewTextBoxColumn.DataPropertyName = "Factor";
      this.factorDataGridViewTextBoxColumn.HeaderText = "Factor";
      this.factorDataGridViewTextBoxColumn.MinimumWidth = 6;
      this.factorDataGridViewTextBoxColumn.Name = "factorDataGridViewTextBoxColumn";
      this.factorDataGridViewTextBoxColumn.ReadOnly = true;
      this.factorDataGridViewTextBoxColumn.Width = 125;
      // 
      // nDataGridViewTextBoxColumn
      // 
      this.nDataGridViewTextBoxColumn.DataPropertyName = "N";
      this.nDataGridViewTextBoxColumn.HeaderText = "N";
      this.nDataGridViewTextBoxColumn.MinimumWidth = 6;
      this.nDataGridViewTextBoxColumn.Name = "nDataGridViewTextBoxColumn";
      this.nDataGridViewTextBoxColumn.ReadOnly = true;
      this.nDataGridViewTextBoxColumn.Width = 125;
      // 
      // rDataGridViewTextBoxColumn
      // 
      this.rDataGridViewTextBoxColumn.DataPropertyName = "R";
      this.rDataGridViewTextBoxColumn.HeaderText = "r";
      this.rDataGridViewTextBoxColumn.MinimumWidth = 6;
      this.rDataGridViewTextBoxColumn.Name = "rDataGridViewTextBoxColumn";
      this.rDataGridViewTextBoxColumn.ReadOnly = true;
      this.rDataGridViewTextBoxColumn.Width = 125;
      // 
      // zDataGridViewTextBoxColumn
      // 
      this.zDataGridViewTextBoxColumn.DataPropertyName = "Z";
      this.zDataGridViewTextBoxColumn.HeaderText = "z";
      this.zDataGridViewTextBoxColumn.MinimumWidth = 6;
      this.zDataGridViewTextBoxColumn.Name = "zDataGridViewTextBoxColumn";
      this.zDataGridViewTextBoxColumn.ReadOnly = true;
      this.zDataGridViewTextBoxColumn.Width = 125;
      // 
      // tDataGridViewTextBoxColumn
      // 
      this.tDataGridViewTextBoxColumn.DataPropertyName = "T";
      this.tDataGridViewTextBoxColumn.HeaderText = "T";
      this.tDataGridViewTextBoxColumn.MinimumWidth = 6;
      this.tDataGridViewTextBoxColumn.Name = "tDataGridViewTextBoxColumn";
      this.tDataGridViewTextBoxColumn.ReadOnly = true;
      this.tDataGridViewTextBoxColumn.Width = 125;
      // 
      // r005DataGridViewTextBoxColumn
      // 
      this.r005DataGridViewTextBoxColumn.DataPropertyName = "R005";
      this.r005DataGridViewTextBoxColumn.HeaderText = "R 5%";
      this.r005DataGridViewTextBoxColumn.MinimumWidth = 6;
      this.r005DataGridViewTextBoxColumn.Name = "r005DataGridViewTextBoxColumn";
      this.r005DataGridViewTextBoxColumn.ReadOnly = true;
      this.r005DataGridViewTextBoxColumn.Width = 125;
      // 
      // r001DataGridViewTextBoxColumn
      // 
      this.r001DataGridViewTextBoxColumn.DataPropertyName = "R001";
      this.r001DataGridViewTextBoxColumn.HeaderText = "R 1%";
      this.r001DataGridViewTextBoxColumn.MinimumWidth = 6;
      this.r001DataGridViewTextBoxColumn.Name = "r001DataGridViewTextBoxColumn";
      this.r001DataGridViewTextBoxColumn.ReadOnly = true;
      this.r001DataGridViewTextBoxColumn.Width = 125;
      // 
      // pDataGridViewTextBoxColumn
      // 
      this.pDataGridViewTextBoxColumn.DataPropertyName = "P";
      this.pDataGridViewTextBoxColumn.HeaderText = "p";
      this.pDataGridViewTextBoxColumn.MinimumWidth = 6;
      this.pDataGridViewTextBoxColumn.Name = "pDataGridViewTextBoxColumn";
      this.pDataGridViewTextBoxColumn.ReadOnly = true;
      this.pDataGridViewTextBoxColumn.Width = 125;
      // 
      // m_binding_source
      // 
      this.m_binding_source.DataSource = typeof(Schicksal.Regression.CorrelationMetrics);
      // 
      // AncovaResultsForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1012, 594);
      this.Controls.Add(this.m_grid);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
      this.Name = "AncovaResultsForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "AncovaResultsForm";
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView m_grid;
    private System.Windows.Forms.DataGridViewTextBoxColumn factorDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn nDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn rDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn zDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn tDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn r005DataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn r001DataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn pDataGridViewTextBoxColumn;
    private System.Windows.Forms.BindingSource m_binding_source;
  }
}