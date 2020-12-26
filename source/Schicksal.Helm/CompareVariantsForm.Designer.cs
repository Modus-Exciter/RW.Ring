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
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CompareVariantsForm));
      this.m_splitter = new System.Windows.Forms.SplitContainer();
      this.m_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
      this.m_tab_control = new System.Windows.Forms.TabControl();
      this.m_summary_page = new System.Windows.Forms.TabPage();
      this.m_grid = new System.Windows.Forms.DataGridView();
      this.m_details_page = new System.Windows.Forms.TabPage();
      this.m_nsr_grid = new System.Windows.Forms.DataGridView();
      ((System.ComponentModel.ISupportInitialize)(this.m_splitter)).BeginInit();
      this.m_splitter.Panel1.SuspendLayout();
      this.m_splitter.Panel2.SuspendLayout();
      this.m_splitter.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_chart)).BeginInit();
      this.m_tab_control.SuspendLayout();
      this.m_summary_page.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).BeginInit();
      this.m_details_page.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_nsr_grid)).BeginInit();
      this.SuspendLayout();
      // 
      // m_splitter
      // 
      this.m_splitter.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_splitter.Location = new System.Drawing.Point(0, 0);
      this.m_splitter.Name = "m_splitter";
      this.m_splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // m_splitter.Panel1
      // 
      this.m_splitter.Panel1.Controls.Add(this.m_chart);
      // 
      // m_splitter.Panel2
      // 
      this.m_splitter.Panel2.Controls.Add(this.m_tab_control);
      this.m_splitter.Size = new System.Drawing.Size(747, 619);
      this.m_splitter.SplitterDistance = 346;
      this.m_splitter.TabIndex = 0;
      // 
      // m_chart
      // 
      chartArea1.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
      chartArea1.AxisX.IsLabelAutoFit = false;
      chartArea1.AxisX.LabelStyle.Angle = 90;
      chartArea1.Name = "MeansArea";
      this.m_chart.ChartAreas.Add(chartArea1);
      this.m_chart.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_chart.Location = new System.Drawing.Point(0, 0);
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
      this.m_chart.Size = new System.Drawing.Size(747, 346);
      this.m_chart.TabIndex = 0;
      this.m_chart.Text = "chart1";
      // 
      // m_tab_control
      // 
      this.m_tab_control.Controls.Add(this.m_summary_page);
      this.m_tab_control.Controls.Add(this.m_details_page);
      this.m_tab_control.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_tab_control.Location = new System.Drawing.Point(0, 0);
      this.m_tab_control.Name = "m_tab_control";
      this.m_tab_control.SelectedIndex = 0;
      this.m_tab_control.Size = new System.Drawing.Size(747, 269);
      this.m_tab_control.TabIndex = 0;
      // 
      // m_summary_page
      // 
      this.m_summary_page.Controls.Add(this.m_grid);
      this.m_summary_page.Location = new System.Drawing.Point(4, 22);
      this.m_summary_page.Name = "m_summary_page";
      this.m_summary_page.Padding = new System.Windows.Forms.Padding(3);
      this.m_summary_page.Size = new System.Drawing.Size(739, 243);
      this.m_summary_page.TabIndex = 0;
      this.m_summary_page.Text = "Summary";
      this.m_summary_page.UseVisualStyleBackColor = true;
      // 
      // m_grid
      // 
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
      this.m_grid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_grid.Location = new System.Drawing.Point(3, 3);
      this.m_grid.Name = "m_grid";
      this.m_grid.ReadOnly = true;
      this.m_grid.Size = new System.Drawing.Size(733, 237);
      this.m_grid.TabIndex = 0;
      // 
      // m_details_page
      // 
      this.m_details_page.Controls.Add(this.m_nsr_grid);
      this.m_details_page.Location = new System.Drawing.Point(4, 22);
      this.m_details_page.Name = "m_details_page";
      this.m_details_page.Padding = new System.Windows.Forms.Padding(3);
      this.m_details_page.Size = new System.Drawing.Size(739, 243);
      this.m_details_page.TabIndex = 1;
      this.m_details_page.Text = "Details";
      this.m_details_page.UseVisualStyleBackColor = true;
      // 
      // m_nsr_grid
      // 
      this.m_nsr_grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle2.Format = "0.0000";
      dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.m_nsr_grid.DefaultCellStyle = dataGridViewCellStyle2;
      this.m_nsr_grid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_nsr_grid.Location = new System.Drawing.Point(3, 3);
      this.m_nsr_grid.Name = "m_nsr_grid";
      this.m_nsr_grid.Size = new System.Drawing.Size(733, 237);
      this.m_nsr_grid.TabIndex = 0;
      this.m_nsr_grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_nsr_grid_CellDoubleClick);
      this.m_nsr_grid.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.m_nsr_grid_CellPainting);
      // 
      // CompareVariantsForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(747, 619);
      this.Controls.Add(this.m_splitter);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "CompareVariantsForm";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "AnovaDetailsForm";
      this.m_splitter.Panel1.ResumeLayout(false);
      this.m_splitter.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_splitter)).EndInit();
      this.m_splitter.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_chart)).EndInit();
      this.m_tab_control.ResumeLayout(false);
      this.m_summary_page.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).EndInit();
      this.m_details_page.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_nsr_grid)).EndInit();
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
  }
}