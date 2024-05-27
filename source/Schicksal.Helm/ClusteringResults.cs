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
    public ClusteringResults(DataTable tableClusters,DataTable[]ClusterPoints)
    {
      this.InitializeComponent();
      
      m_clusters = tableClusters;
      m_points = ClusterPoints;
      m_data_grid_view_clusters.DataSource = tableClusters;
      m_data_grid_view_points.DataSource = ClusterPoints[0];
    }

    private void m_data_grid_view_clusters_RowEnter(object sender, DataGridViewCellEventArgs e)
    {
      m_data_grid_view_points.DataSource = m_points[e.RowIndex];
      m_data_grid_view_points.Update();
    }
  }
}
