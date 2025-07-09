namespace Schicksal.Helm
{
  partial class MultifactorRegressionResultsForm
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
      if (disposing && (this.components != null))
      {
        this.components.Dispose();
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultifactorRegressionResultsForm));
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      this.m_linearBindingSource = new System.Windows.Forms.BindingSource(this.components); // Инициализация BindingSource
      this.m_parabolicBindingSource = new System.Windows.Forms.BindingSource(this.components); // Инициализация BindingSource
      this.m_tabControl = new System.Windows.Forms.TabControl();
      this.m_tabPageLinear = new System.Windows.Forms.TabPage();
      this.m_grid = new System.Windows.Forms.DataGridView();
      this.factorColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.coefficientColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.standardErrorColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.tStatisticColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.pValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.rSquaredColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.adjustedRSquaredColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.fStatisticColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.pValueFStatisticColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.m_tabPageParabolic = new System.Windows.Forms.TabPage();
      this.m_gridParabolic = new System.Windows.Forms.DataGridView();
      this.factorColumnParabolic = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.coefficientColumnParabolic = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.standardErrorColumnParabolic = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.tStatisticColumnParabolic = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.pValueColumnParabolic = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.rSquaredColumnParabolic = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.adjustedRSquaredColumnParabolic = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.fStatisticColumnParabolic = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.pValueFStatisticColumnParabolic = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.m_tabControl.SuspendLayout();
      this.m_tabPageLinear.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_linearBindingSource)).BeginInit(); // BeginInit для BindingSource
      this.m_tabPageParabolic.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_gridParabolic)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_parabolicBindingSource)).BeginInit(); // BeginInit для BindingSource
      this.SuspendLayout();
      //
      // m_linearBindingSource
      //
      this.m_linearBindingSource.DataSource = typeof(Schicksal.Helm.RegressionRowData); // Указываем тип данных для привязки
                                                                                        //
                                                                                        // m_parabolicBindingSource
                                                                                        //
      this.m_parabolicBindingSource.DataSource = typeof(Schicksal.Helm.RegressionRowData); // Указываем тип данных для привязки
                                                                                           //
                                                                                           // tStandardColumn
                                                                                           //
      //
      // m_tabControl
      //
      this.m_tabControl.Controls.Add(this.m_tabPageLinear);
      this.m_tabControl.Controls.Add(this.m_tabPageParabolic);
      resources.ApplyResources(this.m_tabControl, "m_tabControl");
      this.m_tabControl.Name = "m_tabControl";
      this.m_tabControl.SelectedIndex = 0;
      this.m_tabControl.SelectedIndexChanged += new System.EventHandler(this.m_tabControl_SelectedIndexChanged);
      //
      // m_tabPageLinear
      //
      this.m_tabPageLinear.Controls.Add(this.m_grid);
      resources.ApplyResources(this.m_tabPageLinear, "m_tabPageLinear");
      this.m_tabPageLinear.Name = "m_tabPageLinear";
      this.m_tabPageLinear.UseVisualStyleBackColor = true;
      //
      // m_grid
      //
      this.m_grid.AllowUserToAddRows = false;
      this.m_grid.AllowUserToDeleteRows = false;
      this.m_grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.m_grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.factorColumn,
            this.coefficientColumn,
            this.standardErrorColumn,
            this.tStatisticColumn,
            this.pValueColumn,
            this.rSquaredColumn,
            this.adjustedRSquaredColumn,
            this.fStatisticColumn,
            this.pValueFStatisticColumn});
      this.m_grid.DataSource = this.m_linearBindingSource; // Привязка к BindingSource
      this.m_grid.AutoGenerateColumns = false; // Отключаем автогенерацию колонок
      resources.ApplyResources(this.m_grid, "m_grid");
      this.m_grid.MultiSelect = false;
      this.m_grid.Name = "m_grid";
      this.m_grid.ReadOnly = true;
      this.m_grid.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.m_grid_CellPainting);
      //
      // factorColumn
      //
      this.factorColumn.DataPropertyName = "Factor"; // Имя свойства в RegressionRowData
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.factorColumn.DefaultCellStyle = dataGridViewCellStyle1;
      this.factorColumn.Frozen = true;
      resources.ApplyResources(this.factorColumn, "factorColumn");
      this.factorColumn.Name = "factorColumn";
      this.factorColumn.ReadOnly = true;
      this.factorColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      //
      // coefficientColumn
      //
      this.coefficientColumn.DataPropertyName = "Coefficient";
      resources.ApplyResources(this.coefficientColumn, "coefficientColumn");
      this.coefficientColumn.Name = "coefficientColumn";
      this.coefficientColumn.ReadOnly = true;
      this.coefficientColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      //
      // standardErrorColumn
      //
      this.standardErrorColumn.DataPropertyName = "StandardError";
      resources.ApplyResources(this.standardErrorColumn, "standardErrorColumn");
      this.standardErrorColumn.Name = "standardErrorColumn";
      this.standardErrorColumn.ReadOnly = true;
      this.standardErrorColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      //
      // tStatisticColumn
      //
      this.tStatisticColumn.DataPropertyName = "TStatistic";
      resources.ApplyResources(this.tStatisticColumn, "tStatisticColumn");
      this.tStatisticColumn.Name = "tStatisticColumn";
      this.tStatisticColumn.ReadOnly = true;
      this.tStatisticColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      //
      // pValueColumn
      //
      this.pValueColumn.DataPropertyName = "PValue";
      this.pValueColumn.DividerWidth = 2;
      resources.ApplyResources(this.pValueColumn, "pValueColumn");
      this.pValueColumn.Name = "pValueColumn";
      this.pValueColumn.ReadOnly = true;
      this.pValueColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      //
      // rSquaredColumn
      //
      this.rSquaredColumn.DataPropertyName = "RSquared";
      resources.ApplyResources(this.rSquaredColumn, "rSquaredColumn");
      this.rSquaredColumn.Name = "rSquaredColumn";
      this.rSquaredColumn.ReadOnly = true;
      this.rSquaredColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      //
      // adjustedRSquaredColumn
      //
      this.adjustedRSquaredColumn.DataPropertyName = "AdjustedRSquared";
      resources.ApplyResources(this.adjustedRSquaredColumn, "adjustedRSquaredColumn");
      this.adjustedRSquaredColumn.Name = "adjustedRSquaredColumn";
      this.adjustedRSquaredColumn.ReadOnly = true;
      this.adjustedRSquaredColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      //
      // fStatisticColumn
      //
      this.fStatisticColumn.DataPropertyName = "FStatistic";
      resources.ApplyResources(this.fStatisticColumn, "fStatisticColumn");
      this.fStatisticColumn.Name = "fStatisticColumn";
      this.fStatisticColumn.ReadOnly = true;
      this.fStatisticColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      //
      // pValueFStatisticColumn
      //
      this.pValueFStatisticColumn.DataPropertyName = "PValueFStatistic";
      resources.ApplyResources(this.pValueFStatisticColumn, "pValueFStatisticColumn");
      this.pValueFStatisticColumn.Name = "pValueFStatisticColumn";
      this.pValueFStatisticColumn.ReadOnly = true;
      this.pValueFStatisticColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      //
      // m_tabPageParabolic
      //
      this.m_tabPageParabolic.Controls.Add(this.m_gridParabolic);
      resources.ApplyResources(this.m_tabPageParabolic, "m_tabPageParabolic");
      this.m_tabPageParabolic.Name = "m_tabPageParabolic";
      this.m_tabPageParabolic.UseVisualStyleBackColor = true;
      //
      // m_gridParabolic
      //
      this.m_gridParabolic.AllowUserToAddRows = false;
      this.m_gridParabolic.AllowUserToDeleteRows = false;
      this.m_gridParabolic.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.m_gridParabolic.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.factorColumnParabolic,
            this.coefficientColumnParabolic,
            this.standardErrorColumnParabolic,
            this.tStatisticColumnParabolic,
            this.pValueColumnParabolic,
            this.rSquaredColumnParabolic,
            this.adjustedRSquaredColumnParabolic,
            this.fStatisticColumnParabolic,
            this.pValueFStatisticColumnParabolic});
      this.m_gridParabolic.DataSource = this.m_parabolicBindingSource; // Привязка к BindingSource
      this.m_gridParabolic.AutoGenerateColumns = false; // Отключаем автогенерацию колонок
      resources.ApplyResources(this.m_gridParabolic, "m_gridParabolic");
      this.m_gridParabolic.MultiSelect = false;
      this.m_gridParabolic.Name = "m_gridParabolic";
      this.m_gridParabolic.ReadOnly = true;
      this.m_gridParabolic.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.m_gridParabolic_CellPainting);
      //
      // factorColumnParabolic
      //
      this.factorColumnParabolic.DataPropertyName = "Factor";
      dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.factorColumnParabolic.DefaultCellStyle = dataGridViewCellStyle2;
      this.factorColumnParabolic.Frozen = true;
      resources.ApplyResources(this.factorColumnParabolic, "factorColumnParabolic");
      this.factorColumnParabolic.Name = "factorColumnParabolic";
      this.factorColumnParabolic.ReadOnly = true;
      this.factorColumnParabolic.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      //
      // coefficientColumnParabolic
      //
      this.coefficientColumnParabolic.DataPropertyName = "Coefficient";
      resources.ApplyResources(this.coefficientColumnParabolic, "coefficientColumnParabolic");
      this.coefficientColumnParabolic.Name = "coefficientColumnParabolic";
      this.coefficientColumnParabolic.ReadOnly = true;
      this.coefficientColumnParabolic.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      //
      // standardErrorColumnParabolic
      //
      this.standardErrorColumnParabolic.DataPropertyName = "StandardError";
      resources.ApplyResources(this.standardErrorColumnParabolic, "standardErrorColumnParabolic");
      this.standardErrorColumnParabolic.Name = "standardErrorColumnParabolic";
      this.standardErrorColumnParabolic.ReadOnly = true;
      this.standardErrorColumnParabolic.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      //
      // tStatisticColumnParabolic
      //
      this.tStatisticColumnParabolic.DataPropertyName = "TStatistic";
      resources.ApplyResources(this.tStatisticColumnParabolic, "tStatisticColumnParabolic");
      this.tStatisticColumnParabolic.Name = "tStatisticColumnParabolic";
      this.tStatisticColumnParabolic.ReadOnly = true;
      this.tStatisticColumnParabolic.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      //
      // pValueColumnParabolic
      //
      this.pValueColumnParabolic.DataPropertyName = "PValue";
      this.pValueColumnParabolic.DividerWidth = 2;
      resources.ApplyResources(this.pValueColumnParabolic, "pValueColumnParabolic");
      this.pValueColumnParabolic.Name = "pValueColumnParabolic";
      this.pValueColumnParabolic.ReadOnly = true;
      this.pValueColumnParabolic.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      //
      // rSquaredColumnParabolic
      //
      this.rSquaredColumnParabolic.DataPropertyName = "RSquared";
      resources.ApplyResources(this.rSquaredColumnParabolic, "rSquaredColumnParabolic");
      this.rSquaredColumnParabolic.Name = "rSquaredColumnParabolic";
      this.rSquaredColumnParabolic.ReadOnly = true;
      this.rSquaredColumnParabolic.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      //
      // adjustedRSquaredColumnParabolic
      //
      this.adjustedRSquaredColumnParabolic.DataPropertyName = "AdjustedRSquared";
      resources.ApplyResources(this.adjustedRSquaredColumnParabolic, "adjustedRSquaredColumnParabolic");
      this.adjustedRSquaredColumnParabolic.Name = "adjustedRSquaredColumnParabolic";
      this.adjustedRSquaredColumnParabolic.ReadOnly = true;
      this.adjustedRSquaredColumnParabolic.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      //
      // fStatisticColumnParabolic
      //
      this.fStatisticColumnParabolic.DataPropertyName = "FStatistic";
      resources.ApplyResources(this.fStatisticColumnParabolic, "fStatisticColumnParabolic");
      this.fStatisticColumnParabolic.Name = "fStatisticColumnParabolic";
      this.fStatisticColumnParabolic.ReadOnly = true;
      this.fStatisticColumnParabolic.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      //
      // pValueFStatisticColumnParabolic
      //
      this.pValueFStatisticColumnParabolic.DataPropertyName = "PValueFStatistic";
      resources.ApplyResources(this.pValueFStatisticColumnParabolic, "pValueFStatisticColumnParabolic");
      this.pValueFStatisticColumnParabolic.Name = "pValueFStatisticColumnParabolic";
      this.pValueFStatisticColumnParabolic.ReadOnly = true;
      this.pValueFStatisticColumnParabolic.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      //
      // MultifactorRegressionResultsForm
      //
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_tabControl);
      this.Name = "MultifactorRegressionResultsForm";
      this.m_tabControl.ResumeLayout(false);
      this.m_tabPageLinear.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_linearBindingSource)).EndInit(); // EndInit для BindingSource
      this.m_tabPageParabolic.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_gridParabolic)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_parabolicBindingSource)).EndInit(); // EndInit для BindingSource
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl m_tabControl;
    private System.Windows.Forms.TabPage m_tabPageLinear;
    private System.Windows.Forms.DataGridView m_grid;
    private System.Windows.Forms.DataGridViewTextBoxColumn factorColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn coefficientColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn standardErrorColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn tStatisticColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn pValueColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn rSquaredColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn adjustedRSquaredColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn fStatisticColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn pValueFStatisticColumn;
    private System.Windows.Forms.TabPage m_tabPageParabolic;
    private System.Windows.Forms.DataGridView m_gridParabolic;
    private System.Windows.Forms.DataGridViewTextBoxColumn factorColumnParabolic;
    private System.Windows.Forms.DataGridViewTextBoxColumn coefficientColumnParabolic;
    private System.Windows.Forms.DataGridViewTextBoxColumn standardErrorColumnParabolic;
    private System.Windows.Forms.DataGridViewTextBoxColumn tStatisticColumnParabolic;
    private System.Windows.Forms.DataGridViewTextBoxColumn pValueColumnParabolic;
    private System.Windows.Forms.DataGridViewTextBoxColumn rSquaredColumnParabolic;
    private System.Windows.Forms.DataGridViewTextBoxColumn adjustedRSquaredColumnParabolic;
    private System.Windows.Forms.DataGridViewTextBoxColumn fStatisticColumnParabolic;
    private System.Windows.Forms.DataGridViewTextBoxColumn pValueFStatisticColumnParabolic;

    // ДОБАВЛЕННЫЕ BINDINGSOURCE
    private System.Windows.Forms.BindingSource m_linearBindingSource;
    private System.Windows.Forms.BindingSource m_parabolicBindingSource;
  }
}