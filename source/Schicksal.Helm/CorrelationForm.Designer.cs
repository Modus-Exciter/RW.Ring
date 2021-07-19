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
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
      this.m_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
      ((System.ComponentModel.ISupportInitialize)(this.m_chart)).BeginInit();
      this.SuspendLayout();
      // 
      // m_chart
      // 
      chartArea2.Name = "ChartArea1";
      this.m_chart.ChartAreas.Add(chartArea2);
      this.m_chart.Dock = System.Windows.Forms.DockStyle.Fill;
      legend2.Name = "Legend1";
      this.m_chart.Legends.Add(legend2);
      this.m_chart.Location = new System.Drawing.Point(0, 0);
      this.m_chart.Name = "m_chart";
      series2.ChartArea = "ChartArea1";
      series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
      series2.Legend = "Legend1";
      series2.Name = "Series1";
      this.m_chart.Series.Add(series2);
      this.m_chart.Size = new System.Drawing.Size(709, 562);
      this.m_chart.TabIndex = 0;
      this.m_chart.Text = "chart1";
      // 
      // CorrelationForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(709, 562);
      this.Controls.Add(this.m_chart);
      this.Name = "CorrelationForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "CorrelationForm";
      ((System.ComponentModel.ISupportInitialize)(this.m_chart)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataVisualization.Charting.Chart m_chart;
  }
}