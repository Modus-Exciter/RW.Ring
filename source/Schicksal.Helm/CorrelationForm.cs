using System;
using System.Data;
using System.Linq;
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

      double min_x = double.MaxValue;
      double max_x = double.MinValue;
      double min_y = 0;
      double max_y = 0;

      double avg_x = this.Table.Select(string.Format("{0} IS NOT NULL AND {1} IS NOT NULL",
        this.Factor, this.Effect)).Average(row => Convert.ToDouble(row[this.Factor]));
      double avg_y = this.Table.Select(string.Format("{0} IS NOT NULL AND {1} IS NOT NULL",
        this.Factor, this.Effect)).Average(row => Convert.ToDouble(row[this.Effect]));

      double sum_up = 0;
      double sum_dn = 0;

     foreach (DataRow row in this.Table.Select(string.Format("{0} IS NOT NULL AND {1} IS NOT NULL", this.Factor, this.Effect)))
      {
        double x = Convert.ToDouble(row[this.Factor]);
        double y = Convert.ToDouble(row[this.Effect]);
        series.Points.AddXY(x, y);

        sum_up += (x - avg_x) * (y - avg_y);
        sum_dn += (x - avg_x) * (x - avg_x);

        if (min_x > x)
          min_x = x;

        if (max_x < x)
          max_x = x;
      }

      double byx = sum_up / sum_dn;
      min_y = avg_y + byx * (min_x - avg_x);
      max_y = avg_y + byx * (max_x - avg_x);

      m_chart.Series[1].Name = "y = ax + b";
      m_chart.Series[1].Points.AddXY(min_x, min_y);
      m_chart.Series[1].Points.AddXY(max_x, max_y);
    }
  }
}
