namespace Schicksal.Helm
{
  partial class RegressionResultsForm
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
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegressionResultsForm));
      this.m_grid = new System.Windows.Forms.DataGridView();
      this.factorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.nDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.tStandardColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.rDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.tRDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.pRDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.etaDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.tHDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.pHDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.m_context_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.m_cmd_export = new System.Windows.Forms.ToolStripMenuItem();
      this.m_binding_source = new System.Windows.Forms.BindingSource(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).BeginInit();
      this.m_context_menu.SuspendLayout();
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
            this.tStandardColumn,
            this.rDataGridViewTextBoxColumn,
            this.tRDataGridViewTextBoxColumn,
            this.pRDataGridViewTextBoxColumn,
            this.etaDataGridViewTextBoxColumn,
            this.tHDataGridViewTextBoxColumn,
            this.pHDataGridViewTextBoxColumn});
      this.m_grid.ContextMenuStrip = this.m_context_menu;
      this.m_grid.DataSource = this.m_binding_source;
      resources.ApplyResources(this.m_grid, "m_grid");
      this.m_grid.Name = "m_grid";
      this.m_grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Grid_CellDoubleClick);
      this.m_grid.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.Grid_CellPainting);
      // 
      // factorDataGridViewTextBoxColumn
      // 
      this.factorDataGridViewTextBoxColumn.DataPropertyName = "Factor";
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.factorDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
      this.factorDataGridViewTextBoxColumn.Frozen = true;
      resources.ApplyResources(this.factorDataGridViewTextBoxColumn, "factorDataGridViewTextBoxColumn");
      this.factorDataGridViewTextBoxColumn.Name = "factorDataGridViewTextBoxColumn";
      this.factorDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // nDataGridViewTextBoxColumn
      // 
      this.nDataGridViewTextBoxColumn.DataPropertyName = "N";
      resources.ApplyResources(this.nDataGridViewTextBoxColumn, "nDataGridViewTextBoxColumn");
      this.nDataGridViewTextBoxColumn.Name = "nDataGridViewTextBoxColumn";
      this.nDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // tStandardColumn
      // 
      this.tStandardColumn.DataPropertyName = "TStandard";
      this.tStandardColumn.DividerWidth = 2;
      resources.ApplyResources(this.tStandardColumn, "tStandardColumn");
      this.tStandardColumn.Name = "tStandardColumn";
      this.tStandardColumn.ReadOnly = true;
      // 
      // rDataGridViewTextBoxColumn
      // 
      this.rDataGridViewTextBoxColumn.DataPropertyName = "R";
      resources.ApplyResources(this.rDataGridViewTextBoxColumn, "rDataGridViewTextBoxColumn");
      this.rDataGridViewTextBoxColumn.Name = "rDataGridViewTextBoxColumn";
      this.rDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // tRDataGridViewTextBoxColumn
      // 
      this.tRDataGridViewTextBoxColumn.DataPropertyName = "TR";
      resources.ApplyResources(this.tRDataGridViewTextBoxColumn, "tRDataGridViewTextBoxColumn");
      this.tRDataGridViewTextBoxColumn.Name = "tRDataGridViewTextBoxColumn";
      this.tRDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // pRDataGridViewTextBoxColumn
      // 
      this.pRDataGridViewTextBoxColumn.DataPropertyName = "PR";
      this.pRDataGridViewTextBoxColumn.DividerWidth = 2;
      resources.ApplyResources(this.pRDataGridViewTextBoxColumn, "pRDataGridViewTextBoxColumn");
      this.pRDataGridViewTextBoxColumn.Name = "pRDataGridViewTextBoxColumn";
      this.pRDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // etaDataGridViewTextBoxColumn
      // 
      this.etaDataGridViewTextBoxColumn.DataPropertyName = "Eta";
      resources.ApplyResources(this.etaDataGridViewTextBoxColumn, "etaDataGridViewTextBoxColumn");
      this.etaDataGridViewTextBoxColumn.Name = "etaDataGridViewTextBoxColumn";
      this.etaDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // tHDataGridViewTextBoxColumn
      // 
      this.tHDataGridViewTextBoxColumn.DataPropertyName = "TH";
      resources.ApplyResources(this.tHDataGridViewTextBoxColumn, "tHDataGridViewTextBoxColumn");
      this.tHDataGridViewTextBoxColumn.Name = "tHDataGridViewTextBoxColumn";
      this.tHDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // pHDataGridViewTextBoxColumn
      // 
      this.pHDataGridViewTextBoxColumn.DataPropertyName = "PH";
      resources.ApplyResources(this.pHDataGridViewTextBoxColumn, "pHDataGridViewTextBoxColumn");
      this.pHDataGridViewTextBoxColumn.Name = "pHDataGridViewTextBoxColumn";
      this.pHDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // m_context_menu
      // 
      this.m_context_menu.ImageScalingSize = new System.Drawing.Size(20, 20);
      this.m_context_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmd_export});
      this.m_context_menu.Name = "m_context_menu";
      resources.ApplyResources(this.m_context_menu, "m_context_menu");
      // 
      // m_cmd_export
      // 
      this.m_cmd_export.Name = "m_cmd_export";
      resources.ApplyResources(this.m_cmd_export, "m_cmd_export");
      this.m_cmd_export.Click += new System.EventHandler(this.m_cmd_export_Click);
      // 
      // m_binding_source
      // 
      this.m_binding_source.DataSource = typeof(Schicksal.Regression.CorrelationMetrics);
      // 
      // RegressionResultsForm
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_grid);
      this.Name = "RegressionResultsForm";
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).EndInit();
      this.m_context_menu.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView m_grid;
    private System.Windows.Forms.BindingSource m_binding_source;
    private System.Windows.Forms.ContextMenuStrip m_context_menu;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_export;
    private System.Windows.Forms.DataGridViewTextBoxColumn factorDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn nDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn tStandardColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn rDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn tRDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn pRDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn etaDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn tHDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn pHDataGridViewTextBoxColumn;
  }
}