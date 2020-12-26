using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Schicksal.Anova;

namespace Schicksal.Helm
{
  public partial class AnovaResultsForm : Form
  {
    public AnovaResultsForm()
    {
      InitializeComponent();
    }

    public FisherTestResult[] DataSource
    {
      get { return m_binding_source.DataSource as FisherTestResult[]; }
      set { m_binding_source.DataSource = (object)value ?? typeof(FisherTestResult); }
    }

    public float Probability { get; set; }

    public DataTable SourceTable { get; set; }

    public string ResultColumn { get; set; }

    public string Filter { get; set; }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      m_grid.AutoResizeColumns();
    }

    private void m_grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0)
        return;

      var fisher = m_grid.Rows[e.RowIndex].DataBoundItem as FisherTestResult;

      if (fisher == null)
        return;

      using (var compare = new CompareVariantsForm(this.SourceTable, fisher.Factor, this.ResultColumn, this.Filter, this.Probability))
      {
        compare.ShowDialog(this);
      }
    }

    private void m_grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
      if (e.RowIndex < 0)
        return;
      var row = m_grid.Rows[e.RowIndex].DataBoundItem as FisherTestResult;

      if (row == null)
        return;

      if (row.P <= this.Probability)
        e.CellStyle.ForeColor = Color.Red;
    }
  }
}
