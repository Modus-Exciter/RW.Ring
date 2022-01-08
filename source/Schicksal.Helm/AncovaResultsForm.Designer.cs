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
      this.m_context_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.m_cmd_export = new System.Windows.Forms.ToolStripMenuItem();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).BeginInit();
      this.m_context_menu.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_grid
      // 
      resources.ApplyResources(this.m_grid, "m_grid");
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
      this.m_grid.ContextMenuStrip = this.m_context_menu;
      this.m_grid.DataSource = this.m_binding_source;
      this.m_grid.Name = "m_grid";
      this.m_grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Grid_CellDoubleClick);
      this.m_grid.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.Grid_CellPainting);
      // 
      // factorDataGridViewTextBoxColumn
      // 
      this.factorDataGridViewTextBoxColumn.DataPropertyName = "Factor";
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
      // rDataGridViewTextBoxColumn
      // 
      this.rDataGridViewTextBoxColumn.DataPropertyName = "R";
      resources.ApplyResources(this.rDataGridViewTextBoxColumn, "rDataGridViewTextBoxColumn");
      this.rDataGridViewTextBoxColumn.Name = "rDataGridViewTextBoxColumn";
      this.rDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // zDataGridViewTextBoxColumn
      // 
      this.zDataGridViewTextBoxColumn.DataPropertyName = "Z";
      resources.ApplyResources(this.zDataGridViewTextBoxColumn, "zDataGridViewTextBoxColumn");
      this.zDataGridViewTextBoxColumn.Name = "zDataGridViewTextBoxColumn";
      this.zDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // tDataGridViewTextBoxColumn
      // 
      this.tDataGridViewTextBoxColumn.DataPropertyName = "T";
      resources.ApplyResources(this.tDataGridViewTextBoxColumn, "tDataGridViewTextBoxColumn");
      this.tDataGridViewTextBoxColumn.Name = "tDataGridViewTextBoxColumn";
      this.tDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // r005DataGridViewTextBoxColumn
      // 
      this.r005DataGridViewTextBoxColumn.DataPropertyName = "R005";
      resources.ApplyResources(this.r005DataGridViewTextBoxColumn, "r005DataGridViewTextBoxColumn");
      this.r005DataGridViewTextBoxColumn.Name = "r005DataGridViewTextBoxColumn";
      this.r005DataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // r001DataGridViewTextBoxColumn
      // 
      this.r001DataGridViewTextBoxColumn.DataPropertyName = "R001";
      resources.ApplyResources(this.r001DataGridViewTextBoxColumn, "r001DataGridViewTextBoxColumn");
      this.r001DataGridViewTextBoxColumn.Name = "r001DataGridViewTextBoxColumn";
      this.r001DataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // pDataGridViewTextBoxColumn
      // 
      this.pDataGridViewTextBoxColumn.DataPropertyName = "P";
      resources.ApplyResources(this.pDataGridViewTextBoxColumn, "pDataGridViewTextBoxColumn");
      this.pDataGridViewTextBoxColumn.Name = "pDataGridViewTextBoxColumn";
      this.pDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // m_binding_source
      // 
      this.m_binding_source.DataSource = typeof(Schicksal.Regression.CorrelationMetrics);
      // 
      // m_context_menu
      // 
      resources.ApplyResources(this.m_context_menu, "m_context_menu");
      this.m_context_menu.ImageScalingSize = new System.Drawing.Size(20, 20);
      this.m_context_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmd_export});
      this.m_context_menu.Name = "m_context_menu";
      // 
      // m_cmd_export
      // 
      resources.ApplyResources(this.m_cmd_export, "m_cmd_export");
      this.m_cmd_export.Name = "m_cmd_export";
      this.m_cmd_export.Click += new System.EventHandler(this.m_cmd_export_Click);
      // 
      // AncovaResultsForm
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_grid);
      this.Name = "AncovaResultsForm";
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).EndInit();
      this.m_context_menu.ResumeLayout(false);
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
    private System.Windows.Forms.ContextMenuStrip m_context_menu;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_export;
  }
}