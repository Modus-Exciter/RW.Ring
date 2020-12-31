using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
      get { return m_grid.DataSource; }
      set
      {
        m_grid.DataSource = value;
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
  }
}
