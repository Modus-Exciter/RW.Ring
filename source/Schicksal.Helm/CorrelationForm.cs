using System;
using System.Data;
using System.Windows.Forms;

namespace Schicksal.Helm
{
  public partial class CorrelationForm : Form
  {
    public CorrelationForm()
    {
      this.InitializeComponent();
    }

    public DataTable Table { get; set; }

    public string Factor { get; set; }

    public string Effect { get; set; }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      var series = m_chart.Series[0];

      series.Name = this.Factor;

      foreach (DataRow row in this.Table.Rows)
        series.Points.AddXY(Convert.ToDouble(row[this.Factor]), Convert.ToDouble(row[this.Effect]));
    }
  }
}
