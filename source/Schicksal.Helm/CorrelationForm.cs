using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Notung.Configuration;
using Notung.Services;
using Schicksal.Regression;
using Schicksal.Helm.Properties;

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

    public DataTable Table { get; set; }

    public string Factor { get; set; }

    public string Effect { get; set; }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      Dictionary<Type, string> types = new Dictionary<Type, string>();

      types.Add(typeof(LinearDependency), Resources.LINEAR);
      types.Add(typeof(ParabolicDependency), Resources.PARABOLIC);
      types.Add(typeof(HyperbolicDependency), Resources.HYPERBOLIC);
      types.Add(typeof(MichaelisDependency), Resources.MICHAELIS);
      types.Add(typeof(ExponentialDependency), Resources.EXPONENT);

      m_type_selector.DataSource = types.ToArray();
      m_type_selector.ValueMember = "Key";
      m_type_selector.DisplayMember = "Value";

      m_type_selector.SelectedValue = typeof(LinearDependency);
      m_chart.Series[0].Name = this.Factor;
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

      m_chart.Series[0].Points.Clear();
      m_chart.Series[1].Points.Clear();

      var results = new CorrelationResults(this.Table, this.Factor, this.Effect, 
        (Type)m_type_selector.SelectedValue);

      var data = results.Run((x, y) => m_chart.Series[0].Points.AddXY(x, y));

      ((TextAnnotation)m_chart.Annotations[0]).Text = data.Dependency.ToString();

      int pt = 100;
      for (int i = 0; i <= pt; i++)
      {
        double x = data.MinX + i * (data.MaxX - data.MinX) / pt;

        if (data.Dependency.CheckPoint(x))
          m_chart.Series[1].Points.AddXY(x, data.Dependency.Calculate(x));
      }
    }
  }
}