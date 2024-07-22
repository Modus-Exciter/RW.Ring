using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Notung.Services;
using Schicksal.Anova;
using Schicksal.Helm.Properties;

namespace Schicksal.Helm
{
  public partial class AnovaResultsForm : Form
  {
    private readonly Color m_significat_color;

    public AnovaResultsForm()
    {
      this.InitializeComponent();
      m_significat_color = AppManager.Configurator.GetSection<Program.Preferences>().SignificatColor;
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

    public string[] Factors { get; set; }

    public string Conjugate { get; set; }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      fCriticalColumn.HeaderText = string.Format("F {0}%", this.Probability * 100);
      fCriticalColumn.ToolTipText = string.Format(Resources.STANDARD_F_VALUE, this.Probability * 100);
      m_grid.AutoResizeColumns();
    }

    private void Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0)
        return;

      var fisher = m_grid.Rows[e.RowIndex].DataBoundItem as FisherTestResult;

      if (fisher == null)
        return;

      using (var compare = new CompareVariantsForm(this.SourceTable, fisher.Factor,
        fisher.IgnoredFactor, this.ResultColumn, this.Filter, this.Probability, this.Conjugate))
      {
        compare.ShowDialog(this);
      }
    }

    private void Grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
      if (e.RowIndex < 0)
        return;
      var row = m_grid.Rows[e.RowIndex].DataBoundItem as FisherTestResult;

      if (row == null)
        return;

      if (row.P <= this.Probability)
        e.CellStyle.ForeColor = m_significat_color;
    }

    private void Cmd_export_Click(object sender, EventArgs e)
    {
      using (var dlg = new SaveFileDialog())
      {
        dlg.Filter = "Html files|*.html";
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
          var saver = new AnovaHtmlSaver(
            dlg.FileName,
            this.SourceTable,
            this.DataSource,
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