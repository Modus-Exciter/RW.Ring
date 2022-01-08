using System;
using System.Data;
using System.Drawing;
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

    public DataTable Table { get; set; }

    public string Factor { get; set; }

    public string Effect { get; set; }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      var results = new CorrelationResults(this.Table, this.Factor, this.Effect);
      var data = results.Run((x, y) => m_chart.Series[0].Points.AddXY(x, y));

      ((TextAnnotation)m_chart.Annotations[0]).Text = data.ToString();
      m_chart.Series[0].Name = this.Factor;
      m_chart.Series[1].Points.AddXY(data.MinX, data.MinY);
      m_chart.Series[1].Points.AddXY(data.MaxX, data.MaxY);
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
  }
}