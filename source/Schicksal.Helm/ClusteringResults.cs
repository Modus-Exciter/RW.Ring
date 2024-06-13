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
  public partial class ClusteringResults : Form
  {
    private DataTable m_clusters;
    private DataTable[] m_points;
    private string m_abscissa;
    private string m_ordinate;
    private Bitmap m_bitmap;
    Pen pen;
    SolidBrush brush;
    Color[] colors = { Color.Yellow,Color.Tomato,Color.SkyBlue,Color.SeaGreen,Color.Salmon,Color.RosyBrown};
    public ClusteringResults(DataTable tableClusters, DataTable[] ClusterPoints)
    {
      this.InitializeComponent();
      brush = new SolidBrush(colors[0]);
      pen = new Pen(brush, 3);
      pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
      pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
      m_bitmap = new Bitmap(this.Width,this.Height);
      m_clusters = tableClusters;
      m_points = ClusterPoints;
      m_data_grid_view_clusters.DataSource = tableClusters;
      m_data_grid_view_points.DataSource = ClusterPoints[0];

      foreach (DataColumn dataColumn in m_points[0].Columns)
      {

        m_ComboBox_abscissa.Items.Add(dataColumn.ColumnName);
        m_ComboBox_ordinate.Items.Add(dataColumn.ColumnName);

      }
    }
    private void reinit_combobox_abscissa() {
      foreach (DataColumn dataColumn in m_points[0].Columns)
      {
        if ((dataColumn.DataType.IsPrimitive && dataColumn.DataType != typeof(bool)) || dataColumn.DataType == typeof(decimal))
        {
          if (m_ordinate!=dataColumn.ColumnName && !m_ComboBox_abscissa.Items.Contains(dataColumn.ColumnName)) {
          m_ComboBox_abscissa.Items.Add(dataColumn.ColumnName);
          }
        }
      }
    }

    private void reinit_combobox_ordinate()
    {
      foreach (DataColumn dataColumn in m_points[0].Columns)
      {
        if ((dataColumn.DataType.IsPrimitive && dataColumn.DataType != typeof(bool)) || dataColumn.DataType == typeof(decimal))
        {
          if (m_abscissa != dataColumn.ColumnName && !m_ComboBox_ordinate.Items.Contains(dataColumn.ColumnName))
          {
            m_ComboBox_ordinate.Items.Add(dataColumn.ColumnName);
          }
        }
      }
    }

    private void redraw_picturebox() {
      m_bitmap = new Bitmap(this.Width,this.Height);
      Graphics g = Graphics.FromImage(m_bitmap);
      for (int i = 0; i < m_points.Length; i++) {
        pen.Color = colors[i];
        brush.Color = colors[i];
      foreach (DataRow row in m_points[i].Rows) {
        g.DrawEllipse(pen, (int)double.Parse(row[m_abscissa].ToString()) - 3 + (m_pictureBox.Width / 2), (m_pictureBox.Height / 2) + (int)double.Parse(row[m_ordinate].ToString()) - 3, 6, 6);
        g.FillEllipse(brush, (int)double.Parse(row[m_abscissa].ToString()) - 3 + (m_pictureBox.Width / 2), (m_pictureBox.Height / 2) + (int)double.Parse(row[m_ordinate].ToString()) - 3, 6, 6);
      }
      }

        m_pictureBox.Image = m_bitmap;


    }
    private void m_data_grid_view_clusters_RowEnter(object sender, DataGridViewCellEventArgs e)
    {
      m_data_grid_view_points.DataSource = m_points[e.RowIndex];
      m_data_grid_view_points.Update();
    }

    private void m_ComboBox_abscissa_SelectedIndexChanged(object sender, EventArgs e)
    {
      m_abscissa = (string)m_ComboBox_abscissa.SelectedItem;
      m_ComboBox_ordinate.Items.Remove(m_abscissa);
      this.reinit_combobox_ordinate();
    }

    private void m_ComboBox_ordinate_SelectedIndexChanged(object sender, EventArgs e)
    {
      m_ordinate = (string)m_ComboBox_ordinate.SelectedItem;
      m_ComboBox_abscissa.Items.Remove(m_ordinate);
      this.reinit_combobox_abscissa();
    }

    private void ClusteringResults_Resize(object sender, EventArgs e)
    {
      m_bitmap = new Bitmap(this.Width,this.Height);
      this.redraw_picturebox();
    }

    private void m_toolStripButton_Click(object sender, EventArgs e)
    {
      this.redraw_picturebox();
    }
  }
}
