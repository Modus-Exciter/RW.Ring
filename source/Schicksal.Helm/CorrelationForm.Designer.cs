namespace Schicksal.Helm
{
  partial class CorrelationForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CorrelationForm));
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
      this.m_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
      this.m_context_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.m_cmd_copy = new System.Windows.Forms.ToolStripMenuItem();
      ((System.ComponentModel.ISupportInitialize)(this.m_chart)).BeginInit();
      this.m_context_menu.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_chart
      // 
      resources.ApplyResources(this.m_chart, "m_chart");
      chartArea1.Name = "ChartArea1";
      this.m_chart.ChartAreas.Add(chartArea1);
      this.m_chart.ContextMenuStrip = this.m_context_menu;
      legend1.Name = "Legend1";
      this.m_chart.Legends.Add(legend1);
      this.m_chart.Name = "m_chart";
      series1.ChartArea = "ChartArea1";
      series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
      series1.Legend = "Legend1";
      series1.Name = "Series1";
      series2.BorderWidth = 2;
      series2.ChartArea = "ChartArea1";
      series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
      series2.Color = System.Drawing.Color.Red;
      series2.Legend = "Legend1";
      series2.Name = "Series2";
      series2.ShadowOffset = 1;
      this.m_chart.Series.Add(series1);
      this.m_chart.Series.Add(series2);
      // 
      // m_context_menu
      // 
      resources.ApplyResources(this.m_context_menu, "m_context_menu");
      this.m_context_menu.ImageScalingSize = new System.Drawing.Size(20, 20);
      this.m_context_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmd_copy});
      this.m_context_menu.Name = "m_context_menu";
      // 
      // m_cmd_copy
      // 
      resources.ApplyResources(this.m_cmd_copy, "m_cmd_copy");
      this.m_cmd_copy.Name = "m_cmd_copy";
      this.m_cmd_copy.Click += new System.EventHandler(this.m_cmd_copy_Click);
      // 
      // CorrelationForm
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_chart);
      this.Name = "CorrelationForm";
      this.ShowInTaskbar = false;
      ((System.ComponentModel.ISupportInitialize)(this.m_chart)).EndInit();
      this.m_context_menu.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataVisualization.Charting.Chart m_chart;
    private System.Windows.Forms.ContextMenuStrip m_context_menu;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_copy;
  }
}