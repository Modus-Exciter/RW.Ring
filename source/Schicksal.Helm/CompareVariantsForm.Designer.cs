namespace Schicksal.Helm
{
  partial class CompareVariantsForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CompareVariantsForm));
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      this.m_splitter = new System.Windows.Forms.SplitContainer();
      this.m_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
      this.m_graph_context_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.m_cmd_copy_chart = new System.Windows.Forms.ToolStripMenuItem();
      this.m_tab_control = new System.Windows.Forms.TabControl();
      this.m_summary_page = new System.Windows.Forms.TabPage();
      this.m_grid = new System.Windows.Forms.DataGridView();
      this.m_details_page = new System.Windows.Forms.TabPage();
      this.m_nsr_grid = new System.Windows.Forms.DataGridView();
      this.factor1DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.mean1DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.factor2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.mean2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.actualDifferenceDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.minimalDifferenceDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.probabilityDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.m_nsr_context_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.m_cmd_filter = new System.Windows.Forms.ToolStripMenuItem();
      this.differenceInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.m_splitter)).BeginInit();
      this.m_splitter.Panel1.SuspendLayout();
      this.m_splitter.Panel2.SuspendLayout();
      this.m_splitter.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_chart)).BeginInit();
      this.m_graph_context_menu.SuspendLayout();
      this.m_tab_control.SuspendLayout();
      this.m_summary_page.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).BeginInit();
      this.m_details_page.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_nsr_grid)).BeginInit();
      this.m_nsr_context_menu.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.differenceInfoBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // m_splitter
      // 
      resources.ApplyResources(this.m_splitter, "m_splitter");
      this.m_splitter.Name = "m_splitter";
      // 
      // m_splitter.Panel1
      // 
      resources.ApplyResources(this.m_splitter.Panel1, "m_splitter.Panel1");
      this.m_splitter.Panel1.Controls.Add(this.m_chart);
      // 
      // m_splitter.Panel2
      // 
      resources.ApplyResources(this.m_splitter.Panel2, "m_splitter.Panel2");
      this.m_splitter.Panel2.Controls.Add(this.m_tab_control);
      // 
      // m_chart
      // 
      resources.ApplyResources(this.m_chart, "m_chart");
      chartArea1.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
      chartArea1.AxisX.IsLabelAutoFit = false;
      chartArea1.AxisX.LabelStyle.Angle = 90;
      chartArea1.Name = "MeansArea";
      this.m_chart.ChartAreas.Add(chartArea1);
      this.m_chart.ContextMenuStrip = this.m_graph_context_menu;
      this.m_chart.Name = "m_chart";
      series1.BorderWidth = 2;
      series1.ChartArea = "MeansArea";
      series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.BoxPlot;
      series1.Color = System.Drawing.Color.DarkRed;
      series1.CustomProperties = "PointWidth=0.4";
      series1.MarkerBorderWidth = 2;
      series1.Name = "Means";
      series1.ShadowOffset = 1;
      series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
      series1.YValuesPerPoint = 6;
      series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
      this.m_chart.Series.Add(series1);
      // 
      // m_graph_context_menu
      // 
      resources.ApplyResources(this.m_graph_context_menu, "m_graph_context_menu");
      this.m_graph_context_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmd_copy_chart});
      this.m_graph_context_menu.Name = "m_graph_context_menu";
      // 
      // m_cmd_copy_chart
      // 
      resources.ApplyResources(this.m_cmd_copy_chart, "m_cmd_copy_chart");
      this.m_cmd_copy_chart.Name = "m_cmd_copy_chart";
      this.m_cmd_copy_chart.Click += new System.EventHandler(this.m_cmd_copy_chart_Click);
      // 
      // m_tab_control
      // 
      resources.ApplyResources(this.m_tab_control, "m_tab_control");
      this.m_tab_control.Controls.Add(this.m_summary_page);
      this.m_tab_control.Controls.Add(this.m_details_page);
      this.m_tab_control.Name = "m_tab_control";
      this.m_tab_control.SelectedIndex = 0;
      // 
      // m_summary_page
      // 
      resources.ApplyResources(this.m_summary_page, "m_summary_page");
      this.m_summary_page.Controls.Add(this.m_grid);
      this.m_summary_page.ImageKey = global::Schicksal.Helm.Properties.Resources.SCHICKSAL_DATA_FILES;
      this.m_summary_page.Name = "m_summary_page";
      this.m_summary_page.ToolTipText = global::Schicksal.Helm.Properties.Resources.SCHICKSAL_DATA_FILES;
      this.m_summary_page.UseVisualStyleBackColor = true;
      // 
      // m_grid
      // 
      resources.ApplyResources(this.m_grid, "m_grid");
      this.m_grid.AllowUserToAddRows = false;
      this.m_grid.AllowUserToDeleteRows = false;
      this.m_grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle1.Format = "0.0000";
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.m_grid.DefaultCellStyle = dataGridViewCellStyle1;
      this.m_grid.Name = "m_grid";
      this.m_grid.ReadOnly = true;
      // 
      // m_details_page
      // 
      resources.ApplyResources(this.m_details_page, "m_details_page");
      this.m_details_page.Controls.Add(this.m_nsr_grid);
      this.m_details_page.ImageKey = global::Schicksal.Helm.Properties.Resources.SCHICKSAL_DATA_FILES;
      this.m_details_page.Name = "m_details_page";
      this.m_details_page.ToolTipText = global::Schicksal.Helm.Properties.Resources.SCHICKSAL_DATA_FILES;
      this.m_details_page.UseVisualStyleBackColor = true;
      // 
      // m_nsr_grid
      // 
      resources.ApplyResources(this.m_nsr_grid, "m_nsr_grid");
      this.m_nsr_grid.AutoGenerateColumns = false;
      this.m_nsr_grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.m_nsr_grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.factor1DataGridViewTextBoxColumn,
            this.mean1DataGridViewTextBoxColumn,
            this.factor2DataGridViewTextBoxColumn,
            this.mean2DataGridViewTextBoxColumn,
            this.actualDifferenceDataGridViewTextBoxColumn,
            this.minimalDifferenceDataGridViewTextBoxColumn,
            this.probabilityDataGridViewTextBoxColumn});
      this.m_nsr_grid.ContextMenuStrip = this.m_nsr_context_menu;
      this.m_nsr_grid.DataSource = this.differenceInfoBindingSource;
      dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle2.Format = "0.0000";
      dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.m_nsr_grid.DefaultCellStyle = dataGridViewCellStyle2;
      this.m_nsr_grid.Name = "m_nsr_grid";
      this.m_nsr_grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_nsr_grid_CellDoubleClick);
      this.m_nsr_grid.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.m_nsr_grid_CellPainting);
      // 
      // factor1DataGridViewTextBoxColumn
      // 
      this.factor1DataGridViewTextBoxColumn.DataPropertyName = "Factor1";
      resources.ApplyResources(this.factor1DataGridViewTextBoxColumn, "factor1DataGridViewTextBoxColumn");
      this.factor1DataGridViewTextBoxColumn.Name = "factor1DataGridViewTextBoxColumn";
      this.factor1DataGridViewTextBoxColumn.ReadOnly = true;
      this.factor1DataGridViewTextBoxColumn.ToolTipText = global::Schicksal.Helm.Properties.Resources.SCHICKSAL_DATA_FILES;
      // 
      // mean1DataGridViewTextBoxColumn
      // 
      this.mean1DataGridViewTextBoxColumn.DataPropertyName = "Mean1";
      resources.ApplyResources(this.mean1DataGridViewTextBoxColumn, "mean1DataGridViewTextBoxColumn");
      this.mean1DataGridViewTextBoxColumn.Name = "mean1DataGridViewTextBoxColumn";
      this.mean1DataGridViewTextBoxColumn.ReadOnly = true;
      this.mean1DataGridViewTextBoxColumn.ToolTipText = global::Schicksal.Helm.Properties.Resources.SCHICKSAL_DATA_FILES;
      // 
      // factor2DataGridViewTextBoxColumn
      // 
      this.factor2DataGridViewTextBoxColumn.DataPropertyName = "Factor2";
      resources.ApplyResources(this.factor2DataGridViewTextBoxColumn, "factor2DataGridViewTextBoxColumn");
      this.factor2DataGridViewTextBoxColumn.Name = "factor2DataGridViewTextBoxColumn";
      this.factor2DataGridViewTextBoxColumn.ReadOnly = true;
      this.factor2DataGridViewTextBoxColumn.ToolTipText = global::Schicksal.Helm.Properties.Resources.SCHICKSAL_DATA_FILES;
      // 
      // mean2DataGridViewTextBoxColumn
      // 
      this.mean2DataGridViewTextBoxColumn.DataPropertyName = "Mean2";
      resources.ApplyResources(this.mean2DataGridViewTextBoxColumn, "mean2DataGridViewTextBoxColumn");
      this.mean2DataGridViewTextBoxColumn.Name = "mean2DataGridViewTextBoxColumn";
      this.mean2DataGridViewTextBoxColumn.ReadOnly = true;
      this.mean2DataGridViewTextBoxColumn.ToolTipText = global::Schicksal.Helm.Properties.Resources.SCHICKSAL_DATA_FILES;
      // 
      // actualDifferenceDataGridViewTextBoxColumn
      // 
      this.actualDifferenceDataGridViewTextBoxColumn.DataPropertyName = "ActualDifference";
      resources.ApplyResources(this.actualDifferenceDataGridViewTextBoxColumn, "actualDifferenceDataGridViewTextBoxColumn");
      this.actualDifferenceDataGridViewTextBoxColumn.Name = "actualDifferenceDataGridViewTextBoxColumn";
      this.actualDifferenceDataGridViewTextBoxColumn.ReadOnly = true;
      this.actualDifferenceDataGridViewTextBoxColumn.ToolTipText = global::Schicksal.Helm.Properties.Resources.SCHICKSAL_DATA_FILES;
      // 
      // minimalDifferenceDataGridViewTextBoxColumn
      // 
      this.minimalDifferenceDataGridViewTextBoxColumn.DataPropertyName = "MinimalDifference";
      resources.ApplyResources(this.minimalDifferenceDataGridViewTextBoxColumn, "minimalDifferenceDataGridViewTextBoxColumn");
      this.minimalDifferenceDataGridViewTextBoxColumn.Name = "minimalDifferenceDataGridViewTextBoxColumn";
      this.minimalDifferenceDataGridViewTextBoxColumn.ReadOnly = true;
      this.minimalDifferenceDataGridViewTextBoxColumn.ToolTipText = global::Schicksal.Helm.Properties.Resources.SCHICKSAL_DATA_FILES;
      // 
      // probabilityDataGridViewTextBoxColumn
      // 
      this.probabilityDataGridViewTextBoxColumn.DataPropertyName = "Probability";
      resources.ApplyResources(this.probabilityDataGridViewTextBoxColumn, "probabilityDataGridViewTextBoxColumn");
      this.probabilityDataGridViewTextBoxColumn.Name = "probabilityDataGridViewTextBoxColumn";
      this.probabilityDataGridViewTextBoxColumn.ReadOnly = true;
      this.probabilityDataGridViewTextBoxColumn.ToolTipText = global::Schicksal.Helm.Properties.Resources.SCHICKSAL_DATA_FILES;
      // 
      // m_nsr_context_menu
      // 
      resources.ApplyResources(this.m_nsr_context_menu, "m_nsr_context_menu");
      this.m_nsr_context_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmd_filter});
      this.m_nsr_context_menu.Name = "m_nsr_context_menu";
      // 
      // m_cmd_filter
      // 
      resources.ApplyResources(this.m_cmd_filter, "m_cmd_filter");
      this.m_cmd_filter.Name = "m_cmd_filter";
      this.m_cmd_filter.Click += new System.EventHandler(this.m_cmd_filter_Click);
      // 
      // differenceInfoBindingSource
      // 
      this.differenceInfoBindingSource.DataSource = typeof(Schicksal.Anova.DifferenceInfo);
      // 
      // CompareVariantsForm
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_splitter);
      this.Name = "CompareVariantsForm";
      this.ShowInTaskbar = false;
      this.m_splitter.Panel1.ResumeLayout(false);
      this.m_splitter.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_splitter)).EndInit();
      this.m_splitter.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_chart)).EndInit();
      this.m_graph_context_menu.ResumeLayout(false);
      this.m_tab_control.ResumeLayout(false);
      this.m_summary_page.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).EndInit();
      this.m_details_page.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_nsr_grid)).EndInit();
      this.m_nsr_context_menu.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.differenceInfoBindingSource)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer m_splitter;
    private System.Windows.Forms.DataVisualization.Charting.Chart m_chart;
    private System.Windows.Forms.TabControl m_tab_control;
    private System.Windows.Forms.TabPage m_summary_page;
    private System.Windows.Forms.DataGridView m_grid;
    private System.Windows.Forms.TabPage m_details_page;
    private System.Windows.Forms.DataGridView m_nsr_grid;
    private System.Windows.Forms.BindingSource differenceInfoBindingSource;
    private System.Windows.Forms.DataGridViewTextBoxColumn factor1DataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn mean1DataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn factor2DataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn mean2DataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn actualDifferenceDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn minimalDifferenceDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn probabilityDataGridViewTextBoxColumn;
    private System.Windows.Forms.ContextMenuStrip m_graph_context_menu;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_copy_chart;
    private System.Windows.Forms.ContextMenuStrip m_nsr_context_menu;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_filter;
  }
}