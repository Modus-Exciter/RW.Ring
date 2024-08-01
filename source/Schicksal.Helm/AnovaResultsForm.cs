using Schicksal.Helm.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Notung.Services;
using Schicksal.Anova;

namespace Schicksal.Helm
{
  public partial class AnovaResultsForm : Form
  {
    private readonly Color m_significat_color;
    private readonly IPrimaryAnovaResults m_results;

    public AnovaResultsForm(IPrimaryAnovaResults results)
    {
      if (results is null) 
        throw new ArgumentNullException("results");

      this.InitializeComponent();

      m_significat_color = AppManager.Configurator.GetSection<Program.Preferences>().SignificatColor;
      m_results = results;
      m_binding_source.DataSource = results.FisherTestResults;
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      fCriticalColumn.HeaderText = string.Format("F {0}%", m_results.Parameters.Probability * 100);
      fCriticalColumn.ToolTipText = string.Format(Resources.STANDARD_F_VALUE, m_results.Parameters.Probability * 100);

      m_grid.AutoResizeColumns();
    }

    private void Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0)
        return;

      var fisher = m_grid.Rows[e.RowIndex].DataBoundItem as FisherTestResult;

      if (fisher == null)
        return;

      using (var compare = new CompareVariantsForm(m_results, fisher))
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

      if (row.P <= m_results.Parameters.Probability)
        e.CellStyle.ForeColor = m_significat_color;
    }

    private void Cmd_export_Click(object sender, EventArgs e)
    {
      using (var dlg = new SaveFileDialog())
      {
        dlg.Filter = "Html files|*.html";
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
          var saver = new AnovaHtmlSaver(dlg.FileName, m_results,
            string.Format("{0}, {1}", this.Text, m_results.Parameters.Filter).Replace("[", "").Replace("]", ""));

          if (AppManager.OperationLauncher.Run(saver) == TaskStatus.RanToCompletion)
            Process.Start(dlg.FileName);
        }
      }
    }
  }
}