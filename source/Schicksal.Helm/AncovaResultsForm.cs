using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Notung.Services;
using Schicksal.Regression;

namespace Schicksal.Helm
{
  public partial class AncovaResultsForm : Form
  {
    private Color m_significat_color;
    public AncovaResultsForm()
    {
      InitializeComponent();
      m_significat_color = AppManager.Configurator.GetSection<Program.Preferences>().SignificatColor;
    }

    public object DataSorce
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

    private void m_grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
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

    private void m_grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
      if (e.RowIndex < 0)
        return;
      var row = m_grid.Rows[e.RowIndex].DataBoundItem as CorrelationMetrics;

      if (row == null)
        return;

      if (row.P <= this.Probability)
        e.CellStyle.ForeColor = m_significat_color;
    }
  }
}
