using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Notung.Services;
using Schicksal.Regression;

namespace Schicksal.Helm
{
  public partial class AncovaResultsForm : Form
  {
    private readonly Color m_significat_color;
    public AncovaResultsForm()
    {
      this.InitializeComponent();
      m_significat_color = AppManager.Configurator.GetSection<Program.Preferences>().SignificatColor;
    }

    public object DataSource
    {
      get { return m_binding_source.DataSource; }
      set
      {
        m_binding_source.DataSource = value;
      }
    }

    public float Probability { get; set; }

    public DataTable SourceTable { get; set; }

    public string ResultColumn { get; set; }

    public string Filter { get; set; }

    public string[] Factors { get; set; }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      if (m_grid.DataSource == null)
        return;

      foreach (DataGridViewColumn col in m_grid.Columns)
      {
        if (col.ValueType == typeof(double) || col.ValueType == typeof(float))
          col.DefaultCellStyle.Format = "0.0000";
      }

      m_grid.AutoResizeColumns();
    }

    private void Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0)
        return;
      var row = m_grid.Rows[e.RowIndex].DataBoundItem as CorrelationMetrics;

      if (row == null)
        return;

      using (var form = new CorrelationForm())
      {
        form.Factor = row.Factor;
        form.Effect = this.ResultColumn;
        form.Table = this.SourceTable;
        form.Text = this.Text;

        form.ShowDialog(this);
      }
    }

    private void Grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
      if (e.RowIndex < 0)
        return;
      var row = m_grid.Rows[e.RowIndex].DataBoundItem as CorrelationMetrics;

      if (row == null)
        return;

      if (row.P <= this.Probability)
        e.CellStyle.ForeColor = m_significat_color;
    }

    private void m_cmd_export_Click(object sender, EventArgs e)
    {
      using (var dlg = new SaveFileDialog())
      {
        dlg.Filter = "Html files|*.html";
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
          var saver = new RegressionHtmlSaver(
           dlg.FileName,
           this.SourceTable,
           this.DataSource as CorrelationMetrics[],
           this.Probability,
           string.Format("{0}, {1}", this.Text, this.Filter).Replace("[", "").Replace("]", ""))
          {
            Factors = this.Factors,
            Result = this.ResultColumn,
            Filter = this.Filter
          };

          if (AppManager.OperationLauncher.Run(saver) == TaskStatus.RanToCompletion)
          {
            Process.Start(dlg.FileName);
          }
        }
      }
    }
  }
}