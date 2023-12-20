using System;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Notung.Configuration;
using Notung.Services;
using Schicksal.Regression;

namespace Schicksal.Helm
{
  public partial class CorrelationForm : Form
  {
    public CorrelationForm()
    {
      this.InitializeComponent();

      Resolution resolution = AppManager.Configurator.GetSection<Resolution>();

      if (resolution.height != 0)
        this.Height = resolution.height;

      if (resolution.width != 0)
        this.Width = resolution.width;
    }

    [DataContract]
    public class Resolution : ConfigurationSection
    {
      [DataMember(Name = "Height")]
      public int height;
      [DataMember(Name = "Width")]
      public int width;
    }

    public CorrelationMetrics Metrics { get; set; }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      m_type_selector.DataSource = CorrelationGraphUtils.GetDependencySource(this.Metrics.Formula);

      m_type_selector.ValueMember = "Key";
      m_type_selector.DisplayMember = "Value";
      m_type_selector.SelectedValue = CorrelationGraphUtils.GetBestDependency(this.Metrics.Formula).GetType();

      m_chart.Series[0].Name = this.Metrics.Factor;

      foreach (var pt in this.Metrics.Formula.SourcePoints)
        m_chart.Series[0].Points.AddXY(pt.X, pt.Y);
    }

    protected override void OnClosed(EventArgs e)
    {
      AppManager.Configurator.GetSection<Resolution>().height = this.Size.Height;
      AppManager.Configurator.GetSection<Resolution>().width = this.Size.Width;
      AppManager.Configurator.SaveSettings();
    }

    private void m_cmd_copy_Click(object sender, EventArgs e)
    {
      var image = new Bitmap(m_chart.Width, m_chart.Height);

      m_chart.DrawToBitmap(image, m_chart.DisplayRectangle);

      Clipboard.SetImage(image);
    }

    private void m_type_selector_SelectedValueChanged(object sender, EventArgs e)
    {
      if (!(m_type_selector.SelectedValue is Type))
        return;

      m_chart.Series[1].Points.SuspendUpdates();
      m_chart.Series[1].Points.Clear();

      while (m_chart.Series.Count > 2)
        m_chart.Series.RemoveAt(m_chart.Series.Count - 1);

      var dependency = CorrelationGraphUtils.FillPoints(this.Metrics.Formula, 
        (Type)m_type_selector.SelectedValue, GetSeriesForRange);

      ((TextAnnotation)m_chart.Annotations[0]).Text = dependency.ToString();

      m_chart.Series[1].Points.ResumeUpdates();

      m_label_metrics1.Text = string.Format("{0}: {1}; {2}: {3:0.0000}; {4}: {5:0.0000}",
        SchicksalResources.HETEROSCEDASTICITY, dependency.Heteroscedasticity,
        SchicksalResources.CONSISTENCY, dependency.Consistency,
        SchicksalResources.CONSISTENCY_WEIGHTED, dependency.ConsistencyWeighted);

      m_label_metrics2.Text = string.Format("{0}: {1}; {2}: {3};",
        SchicksalResources.RMS_ERROR, dependency.RMSError,
        SchicksalResources.RMS_ERROR_WEIGHTED, dependency.RMSErrorWeighted);

      this.MinimumSize = new Size(m_label_metrics1.Width 
        + m_type_selector.Width + 20, 200);
    }

    private Action<double, double> GetSeriesForRange(int index)
    {
      Series series = index == 1 ? m_chart.Series[1] : new Series("Snip " + index);

      if (index > 1)
      {
        series.BorderWidth = m_chart.Series[1].BorderWidth;
        series.ChartArea = m_chart.Series[1].ChartArea;
        series.ChartType = m_chart.Series[1].ChartType;
        series.Color = System.Drawing.Color.Red;
        series.ShadowOffset = 1;
        m_chart.Series.Add(series);
      }

      return (x,y) => series.Points.AddXY(x,y);
    }
  }
}