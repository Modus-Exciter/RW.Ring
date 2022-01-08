using System;
using System.Windows.Forms;
using Schicksal.Anova;

namespace Schicksal.Helm
{
  public partial class NSRForm : Form
  {
    public NSRForm(DifferenceInfo difference)
    {
      this.InitializeComponent();

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
