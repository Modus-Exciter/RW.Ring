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
            System.Windows.Forms.DataVisualization.Charting.TextAnnotation textAnnotation1 = new System.Windows.Forms.DataVisualization.Charting.TextAnnotation();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CorrelationForm));
            this.m_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.m_context_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.m_cmd_copy = new System.Windows.Forms.ToolStripMenuItem();
            this.m_type_selector = new System.Windows.Forms.ComboBox();
            this.m_label_metrics = new System.Windows.Forms.Label();
            this.m_bottom_panel = new System.Windows.Forms.TableLayoutPanel();
            this.m_button_details = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.m_chart)).BeginInit();
            this.m_context_menu.SuspendLayout();
            this.m_bottom_panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_chart
            // 
            textAnnotation1.Alignment = System.Drawing.ContentAlignment.TopLeft;
            textAnnotation1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            textAnnotation1.IsSizeAlwaysRelative = false;
            textAnnotation1.Name = "TextAnnotation1";
            textAnnotation1.Text = "Урожай = -5,541e-2 * Площадь + 2861,304 ";
            textAnnotation1.Width = 100D;
            textAnnotation1.X = 5D;
            textAnnotation1.Y = 2D;
            this.m_chart.Annotations.Add(textAnnotation1);
            chartArea1.Name = "RegressionGraph";
            chartArea1.Position.Auto = false;
            chartArea1.Position.Height = 92F;
            chartArea1.Position.Width = 90F;
            chartArea1.Position.X = 3F;
            chartArea1.Position.Y = 8F;
            this.m_chart.ChartAreas.Add(chartArea1);
            this.m_chart.ContextMenuStrip = this.m_context_menu;
            resources.ApplyResources(this.m_chart, "m_chart");
            this.m_chart.Name = "m_chart";
            series1.ChartArea = "RegressionGraph";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series1.Name = "RealValues";
            series2.BorderWidth = 2;
            series2.ChartArea = "RegressionGraph";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Color = System.Drawing.Color.Red;
            series2.Name = "Dependency";
            series2.ShadowOffset = 1;
            this.m_chart.Series.Add(series1);
            this.m_chart.Series.Add(series2);
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
            // m_label_metrics
            // 
            resources.ApplyResources(this.m_label_metrics, "m_label_metrics");
            this.m_label_metrics.BackColor = System.Drawing.Color.White;
            this.m_label_metrics.Name = "m_label_metrics";
            // 
            // m_bottom_panel
            // 
            this.m_bottom_panel.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.m_bottom_panel, "m_bottom_panel");
            this.m_bottom_panel.Controls.Add(this.m_type_selector, 2, 0);
            this.m_bottom_panel.Controls.Add(this.m_label_metrics, 0, 0);
            this.m_bottom_panel.Controls.Add(this.m_button_details, 1, 0);
            this.m_bottom_panel.Name = "m_bottom_panel";
            // 
            // m_button_details
            // 
            resources.ApplyResources(this.m_button_details, "m_button_details");
            this.m_button_details.Name = "m_button_details";
            this.m_button_details.UseVisualStyleBackColor = true;
            this.m_button_details.Click += new System.EventHandler(this.DetailsClick);
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
    private System.Windows.Forms.Label m_label_metrics;
    private System.Windows.Forms.TableLayoutPanel m_bottom_panel;
    private System.Windows.Forms.Button m_button_details;
  }
}