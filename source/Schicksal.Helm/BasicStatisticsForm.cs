using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Schicksal.Helm.Properties;
using System.Diagnostics;

namespace Schicksal.Helm
{
  public partial class BasicStatisticsForm : Form
  {
    public BasicStatisticsForm()
    {
      InitializeComponent();
    }

    public object DataSorce
    {
      get { return m_binding_source.DataSource; }
      set
      {
        m_binding_source.DataSource = value;
      }
    }

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

    private void m_lang_LanguageChanged(object sender, Notung.ComponentModel.LanguageEventArgs e)
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

    private Dictionary<string, string> GetColumnNames()
    {
      var ret = new Dictionary<string, string>();

      foreach (DataGridViewColumn col in m_grid.Columns)
        ret[col.DataPropertyName] = col.HeaderText;

      return ret;
    }

    private void exportToolStripMenuItem_Click(object sender, EventArgs e)
    {
      using (var dlg = new SaveFileDialog())
      {
        dlg.Filter = "Html files|*.html";

        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
          HtmlSaver.Save(dlg.FileName,
            m_binding_source.DataSource as System.Collections.IList, GetColumnNames());
          Process.Start(dlg.FileName);
        }
      }
    }
  }
}
