using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Notung.Configuration;
using Notung.Services;
using Schicksal.Helm.Properties;
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

      Dictionary<Type, string> types = new Dictionary<Type, string>();

      types.Add(typeof(LinearDependency), Resources.LINEAR);
      types.Add(typeof(ParabolicDependency), Resources.PARABOLIC);
      types.Add(typeof(HyperbolicDependency), Resources.HYPERBOLIC);
      types.Add(typeof(MichaelisDependency), Resources.MICHAELIS);
      types.Add(typeof(ExponentialDependency), Resources.EXPONENT);

      m_type_selector.DataSource = types.Where(kv => 
        this.Metrics.Correlations.Dependencies.Any(d => d.GetType() == kv.Key)).ToArray();

      m_type_selector.ValueMember = "Key";
      m_type_selector.DisplayMember = "Value";
      m_type_selector.SelectedValue = typeof(LinearDependency);
      m_chart.Series[0].Name = this.Metrics.Factor;

      foreach (var pt in this.Metrics.Correlations.SourcePoints)
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

      var data = this.Metrics.Correlations;
      var dependency = data.Dependencies.Single(d => 
        m_type_selector.SelectedValue.Equals(d.GetType()));

      m_chart.Series[1].Points.SuspendUpdates();
      m_chart.Series[1].Points.Clear();

      ((TextAnnotation)m_chart.Annotations[0]).Text = dependency.ToString();

      int pt = 100;

      for (int i = 0; i <= pt; i++)
      {
        double x = data.MinX + i * (data.MaxX - data.MinX) / pt;

        if (dependency.CheckPoint(x))
          m_chart.Series[1].Points.AddXY(x, dependency.Calculate(x));
      }

      m_chart.Series[1].Points.ResumeUpdates();

      m_label_heteroscedasticity.Text = string.Format("{0}: {1:0.0000}",
        SchicksalResources.HETEROSCEDASTICITY, dependency.Heteroscedasticity);
    }
  }
}