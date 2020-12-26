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
  public partial class NSRForm : Form
  {
    public NSRForm(DifferenceInfo difference)
    {
      InitializeComponent();

      this.Text = difference.ToString();
      m_grid.DataSource = difference.ToTuples();
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      m_grid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      m_grid.Columns.Insert(0, new DataGridViewTextBoxColumn());
      m_grid.AutoResizeColumn(2);
      m_grid.ClearSelection();
    }
  }
}
