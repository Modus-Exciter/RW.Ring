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

      m_type_selector.DataSource = RegressionDependency.GetDependencyTypeNames().Where(kv => 
        this.Metrics.Formula.Dependencies.Any(d => d.GetType() == kv.Key)).ToArray();

      m_type_selector.ValueMember = "Key";
      m_type_selector.DisplayMember = "Value";
      m_type_selector.SelectedValue = typeof(LinearDependency);
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

      var data = this.Metrics.Formula;

      var dependency = data.Dependencies.Single(d => 
        m_type_selector.SelectedValue.Equals(d.GetType()));

      m_chart.Series[1].Points.SuspendUpdates();
      m_chart.Series[1].Points.Clear();

      while (m_chart.Series.Count > 2)
        m_chart.Series.RemoveAt(m_chart.Series.Count - 1);

      ((TextAnnotation)m_chart.Annotations[0]).Text = dependency.ToString();

      double[] points = CorrelationUIUtils.GetKeyPoints(dependency, data.MaxX, data.MinX);
      double max_y = 1.3 * data.MaxY - 0.3 * data.MinY;
      double min_y = 1.3 * data.MinY - 0.3 * data.MaxY;

      for (int i = 1; i < points.Length; i++)
      {
        double min_x = points[i - 1];
        double max_x = points[i];

        CorrelationUIUtils.CorrectBorders(dependency, (max_x - min_x) / 1000, 
          max_y, min_y, ref max_x, ref min_x);

        Series series = GetSeriesForRange(i);
        int pt = 100;

        for (int j = 0; j <= pt; j++)
        {
          double x = min_x + j * (max_x - min_x) / pt;

          if (!dependency.GetGaps().Contains(x))
            series.Points.AddXY(x, dependency.Calculate(x));
        }
      }

      m_chart.Series[1].Points.ResumeUpdates();

      m_label_heteroscedasticity.Text = string.Format("{0}: {1:0.0000}",
        SchicksalResources.HETEROSCEDASTICITY, dependency.Heteroscedasticity);
    }

    private Series GetSeriesForRange(int index)
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

      return series;
    }
  }

  static class CorrelationUIUtils
  {
    public static double[] GetKeyPoints(RegressionDependency dependency, double maxX, double minX)
    {
      double[] points = new double[] { minX }
        .Concat(dependency.GetGaps().Where(g => g < maxX && g > minX))
        .Concat(new double[] { maxX }).ToArray();

      return points;
    }

    public static void CorrectBorders(RegressionDependency dependency, double shift, double maxY, double minY, ref double maxX, ref double minX)
    {
      if (dependency.GetGaps().Contains(minX))
      {
        double y = dependency.Calculate(minX + shift);
        double old_x = minX;

        while (y > maxY || y < minY)
        {
          minX += shift;
          y = dependency.Calculate(minX);

          if (minX > maxX)
          {
            minX = old_x + shift * 10;
            break;
          }
        }
      }

      if (dependency.GetGaps().Contains(maxX))
      {
        double y = dependency.Calculate(maxX - shift);
        double old_x = maxX;
        while (y > maxY || y < minY)
        {
          maxX -= shift;
          y = dependency.Calculate(maxX);

          if (minX > maxX)
          {
            maxX = old_x - shift * 10;
            break;
          }
        }
      }
    }
  }
}