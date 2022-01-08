using System;
using System.Windows.Forms;
using Schicksal.Helm.Properties;
using System.Diagnostics;
using Schicksal.Basic;
using System.Threading.Tasks;
using Notung.Services;

namespace Schicksal.Helm
{
  public partial class BasicStatisticsForm : Form
  {
    public BasicStatisticsForm()
    {
      this.InitializeComponent();
    }

    public object DataSorce
    {
      get { return m_binding_source.DataSource; }
      set
      {
        m_binding_source.DataSource = value;
      }
    }

    public string[] Factors { get; set; }

    public string ResultColumn { get; set; }

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

    private void Lang_LanguageChanged(object sender, Notung.ComponentModel.LanguageEventArgs e)
    {
      m_col_description.HeaderText = SchicksalResources.DESCRIPTION;
      m_col_mean.HeaderText = SchicksalResources.MEAN;
      m_col_median.HeaderText = SchicksalResources.MEDIAN;
      m_col_min.HeaderText = SchicksalResources.MIN;
      m_col_max.HeaderText = SchicksalResources.MAX;
      m_col_count.HeaderText = SchicksalResources.COUNT;
      m_col_error.HeaderText = SchicksalResources.STD_ERROR;
      m_col_interval.HeaderText = SchicksalResources.INTERVAL;
      m_cmd_export.Text = Resources.EXPORT;
    }

    private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
    {
      using (var dlg = new SaveFileDialog())
      {
        dlg.Filter = "Html files|*.html";

        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
          if (AppManager.OperationLauncher.Run(new DescriptionHtmlSaver(dlg.FileName,
            m_binding_source.DataSource as DescriptionStatisticsEntry[])
          {
            Caption = this.Text,
            Factors = this.Factors,
            Result = this.ResultColumn
          }) == TaskStatus.RanToCompletion)
            Process.Start(dlg.FileName);
        }
      }
    }
  }
}
