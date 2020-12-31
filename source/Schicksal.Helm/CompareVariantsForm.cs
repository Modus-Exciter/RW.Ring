using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Notung;
using Schicksal.Anova;
using Schicksal.Helm.Dialogs;
using Schicksal.Helm.Properties;

namespace Schicksal.Helm
{
  public partial class CompareVariantsForm : Form
  {
    private readonly VariantsComparator m_comparator;
    private readonly float m_probability;
    private DifferenceInfo[] m_all_data;
    private bool m_only_significant;
    private string m_selection = string.Empty;
    private Color m_significat_color;
    private Color m_exclusive_color;

    public CompareVariantsForm(DataTable table, string factor, string result, string filter, float p)
    {
      InitializeComponent();

      this.Text = string.Format("{0}({1}) [{2}]", result, factor.Replace("+", ", "), filter);

      m_comparator = new VariantsComparator(table, factor, result, filter);
      m_probability = p;
      m_significat_color = AppManager.Configurator.GetSection<Program.Preferences>().SignificatColor;
      m_exclusive_color = AppManager.Configurator.GetSection<Program.Preferences>().ExclusiveColor;
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      var mult = new MultiVariantsComparator(m_comparator, m_probability);

      if (AppManager.OperationLauncher.Run(mult) == System.Threading.Tasks.TaskStatus.RanToCompletion)
      {
        DataTable res = mult.Source;
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

        m_all_data = mult.Results;
        m_nsr_grid.DataSource = new DifferenceInfoList(m_all_data);

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
        e.CellStyle.ForeColor = m_only_significant ? m_exclusive_color : m_significat_color;
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

    private void m_cmd_copy_chart_Click(object sender, EventArgs e)
    {
      var image = new Bitmap(m_chart.Width, m_chart.Height);

      m_chart.DrawToBitmap(image, m_chart.DisplayRectangle);

      Clipboard.SetImage(image);
    }

    private void m_cmd_filter_Click(object sender, EventArgs e)
    {
      if (m_all_data == null)
        return;
      
      using (var dlg = new ComparisonFilterDialog())
      {
        var set = new HashSet<string>();
        set.Add(string.Empty);

        foreach (var item in m_all_data)
        {
          set.Add(item.Factor1);
          set.Add(item.Factor2);

          if (set.Count == m_grid.Rows.Count)
            break;
        }

        dlg.SetSelectionList(set);
        dlg.Selection = m_selection;
        dlg.OnlySignificat = m_only_significant;

        if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
        {
          m_selection = dlg.Selection;
          m_only_significant = dlg.OnlySignificat;

          IEnumerable<DifferenceInfo> res = m_all_data;

          if (m_only_significant)
            res = res.Where(d => d.Probability <= m_probability);

          if (!string.IsNullOrEmpty(m_selection))
            res = res.Where(d => d.Factor1 == m_selection || d.Factor2 == m_selection);

          m_nsr_grid.DataSource = new DifferenceInfoList(res.ToArray());
        }
      }
    }
  }
}