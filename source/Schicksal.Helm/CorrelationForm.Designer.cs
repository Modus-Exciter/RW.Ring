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
      System.Windows.Forms.DataVisualization.Charting.TextAnnotation textAnnotation2 = new System.Windows.Forms.DataVisualization.Charting.TextAnnotation();
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CorrelationForm));
      System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
      this.m_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
      this.m_context_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.m_cmd_copy = new System.Windows.Forms.ToolStripMenuItem();
      this.m_type_selector = new System.Windows.Forms.ComboBox();
      this.m_label_heteroscedasticity = new System.Windows.Forms.Label();
      this.m_bottom_panel = new System.Windows.Forms.TableLayoutPanel();
      ((System.ComponentModel.ISupportInitialize)(this.m_chart)).BeginInit();
      this.m_context_menu.SuspendLayout();
      this.m_bottom_panel.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_chart
      // 
      textAnnotation2.Alignment = System.Drawing.ContentAlignment.TopLeft;
      textAnnotation2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      textAnnotation2.IsSizeAlwaysRelative = false;
      textAnnotation2.Name = "TextAnnotation1";
      textAnnotation2.Text = "Урожай = -5,541e-2 * Площадь + 2861,304 ";
      textAnnotation2.Width = 100D;
      textAnnotation2.X = 5D;
      textAnnotation2.Y = 2D;
      this.m_chart.Annotations.Add(textAnnotation2);
      chartArea2.Name = "RegressionGraph";
      chartArea2.Position.Auto = false;
      chartArea2.Position.Height = 92F;
      chartArea2.Position.Width = 90F;
      chartArea2.Position.X = 3F;
      chartArea2.Position.Y = 8F;
      this.m_chart.ChartAreas.Add(chartArea2);
      this.m_chart.ContextMenuStrip = this.m_context_menu;
      resources.ApplyResources(this.m_chart, "m_chart");
      this.m_chart.Name = "m_chart";
      series3.ChartArea = "RegressionGraph";
      series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
      series3.Name = "RealValues";
      series4.BorderWidth = 2;
      series4.ChartArea = "RegressionGraph";
      series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
      series4.Color = System.Drawing.Color.Red;
      series4.Name = "Dependency";
      series4.ShadowOffset = 1;
      this.m_chart.Series.Add(series3);
      this.m_chart.Series.Add(series4);
      // 
      // m_context_menu
      // 
      this.m_context_menu.ImageScalingSize = new System.Drawing.Size(20, 20);
      this.m_context_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmd_copy});
      this.m_context_menu.Name = "m_context_menu";
      resources.ApplyResources(this.m_context_menu, "m_context_menu");
      // 
      // m_cmd_copy
      // 
      this.m_cmd_copy.Name = "m_cmd_copy";
      resources.ApplyResources(this.m_cmd_copy, "m_cmd_copy");
      this.m_cmd_copy.Click += new System.EventHandler(this.m_cmd_copy_Click);
      // 
      // m_type_selector
      // 
      resources.ApplyResources(this.m_type_selector, "m_type_selector");
      this.m_type_selector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_type_selector.FormattingEnabled = true;
      this.m_type_selector.Name = "m_type_selector";
      this.m_type_selector.SelectedValueChanged += new System.EventHandler(this.m_type_selector_SelectedValueChanged);
      // 
      // m_label_heteroscedasticity
      // 
      resources.ApplyResources(this.m_label_heteroscedasticity, "m_label_heteroscedasticity");
      this.m_label_heteroscedasticity.BackColor = System.Drawing.Color.White;
      this.m_label_heteroscedasticity.Name = "m_label_heteroscedasticity";
      // 
      // m_bottom_panel
      // 
      this.m_bottom_panel.BackColor = System.Drawing.Color.White;
      resources.ApplyResources(this.m_bottom_panel, "m_bottom_panel");
      this.m_bottom_panel.Controls.Add(this.m_type_selector, 2, 0);
      this.m_bottom_panel.Controls.Add(this.m_label_heteroscedasticity, 0, 0);
      this.m_bottom_panel.Name = "m_bottom_panel";
      // 
      // CorrelationForm
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_chart);
      this.Controls.Add(this.m_bottom_panel);
      this.Name = "CorrelationForm";
      this.ShowInTaskbar = false;
      ((System.ComponentModel.ISupportInitialize)(this.m_chart)).EndInit();
      this.m_context_menu.ResumeLayout(false);
      this.m_bottom_panel.ResumeLayout(false);
      this.m_bottom_panel.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataVisualization.Charting.Chart m_chart;
    private System.Windows.Forms.ContextMenuStrip m_context_menu;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_copy;
    private System.Windows.Forms.ComboBox m_type_selector;
    private System.Windows.Forms.Label m_label_heteroscedasticity;
    private System.Windows.Forms.TableLayoutPanel m_bottom_panel;
  }
}