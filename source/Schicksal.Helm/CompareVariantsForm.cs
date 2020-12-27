using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Notung;
using Schicksal.Anova;
using Schicksal.Helm.Properties;

namespace Schicksal.Helm
{
  public partial class CompareVariantsForm : Form
  {
    private readonly VariantsComparator m_comparator;
    private readonly float m_probability;

    public CompareVariantsForm(DataTable table, string factor, string result, string filter, float p)
    {
      InitializeComponent();

      this.Text = string.Format("{0}({1}) [{2}]", result, factor.Replace("+", ", "), filter);

      m_comparator = new VariantsComparator(table, factor, result, filter);
      m_probability = p;
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      DataTable res = m_comparator.CreateDescriptiveTable(m_probability);
      m_grid.DataSource = res;

      m_grid.Columns["Count"].DefaultCellStyle = new DataGridViewCellStyle { Format = "0" };
      m_grid.Columns["Count"].HeaderText = Resources.COUNT;
      m_grid.Columns["Factor"].Visible = false;
      m_grid.Columns["Mean"].HeaderText = Resources.MEAN;
      m_grid.Columns["Std error"].HeaderText = Resources.STD_ERROR;
      m_grid.Columns["Interval"].HeaderText = Resources.INTERVAL;

      m_summary_page.Text = Resources.STATISTICS;
      m_details_page.Text = Resources.COMPARISON;

      m_grid.AutoResizeColumns();

      var series = m_chart.Series["Means"];

      series.Points.Clear();

      foreach (DataRow row in res.Rows)
      {
        var mean = (double)row["Mean"];
        var interval = (double)row["Interval"];
        series.Points.AddXY(row["Factor"], mean - interval, mean + interval, mean, mean);
      }

      var mult = new MultiVariantsComparator(m_comparator,  m_probability, m_grid.DataSource as DataTable);

      if (AppManager.OperationLauncher.Run(mult) == System.Threading.Tasks.TaskStatus.RanToCompletion)
      {
        m_nsr_grid.DataSource = mult.Results;

        using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
        {
          var width1 = (int)graphics.MeasureString(mult.Factor1MaxLength,
            factor1DataGridViewTextBoxColumn.DefaultCellStyle.Font ?? Control.DefaultFont).Width;

          var width2 = (int)graphics.MeasureString(mult.Factor2MaxLength,
            factor2DataGridViewTextBoxColumn.DefaultCellStyle.Font ?? Control.DefaultFont).Width;

          factor1DataGridViewTextBoxColumn.Width = Math.Max(factor1DataGridViewTextBoxColumn.Width, width1);
          factor2DataGridViewTextBoxColumn.Width = Math.Max(factor2DataGridViewTextBoxColumn.Width, width2);
        }
      }
    }

    private void m_nsr_grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
      if (e.RowIndex < 0)
        return;

      var row = m_nsr_grid.Rows[e.RowIndex].DataBoundItem as DifferenceInfo;

      if (row != null && row.Probability < m_probability)
        e.CellStyle.ForeColor = Color.Red;
    }

    private void m_nsr_grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0)
        return;

      var row = m_nsr_grid.Rows[e.RowIndex].DataBoundItem as DifferenceInfo;

      using (var form = new NSRForm(row))
      {
        form.ShowDialog(this);
      }
    }
  }
}