using Schicksal.Clustering;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Schicksal.Helm.Dialogs
{
  public partial class ClusteringParametersDialog : Form
  {
    public ClusteringParametersDialog(DataTable table)
    {
      this.InitializeComponent();

      m_binding_source.DataSource = new ClusteringParameters(table);
      m_combo_metrics.DataSource = ((ClusteringParameters)m_binding_source.DataSource).GetAllDistanceMetrics().ToList();
      m_combo_arc_deleter.DataSource = ((ClusteringParameters)m_binding_source.DataSource).GetAllArcDeleters().ToList();
      ((ClusteringParameters)m_binding_source.DataSource).ClusterCount = (uint)m_numeric_cluster_count.Value;
      ((ClusteringParameters)m_binding_source.DataSource).DistanceMetrics = (IDistanceMetrics<double>)m_combo_metrics.SelectedItem;
      ((ClusteringParameters)m_binding_source.DataSource).ArcDeleter = (IArcDeleter<double>)m_combo_arc_deleter.SelectedItem;
    }

    public ClusteringParameters Parameters
    {
      get { return (ClusteringParameters)m_binding_source.DataSource; }
      private set { }
    }

    private void m_numeric_cluster_count_ValueChanged(object sender, System.EventArgs e)
    {
      this.Parameters.ClusterCount = (uint)m_numeric_cluster_count.Value;
      ((ClusteringParameters)m_binding_source.DataSource).ClusterCount = (uint)m_numeric_cluster_count.Value;
    }

    private void m_combo_metrics_SelectedIndexChanged(object sender, System.EventArgs e)
    {
      this.Parameters.DistanceMetrics = (IDistanceMetrics<double>)m_combo_metrics.SelectedItem;
      ((ClusteringParameters)m_binding_source.DataSource).DistanceMetrics = (IDistanceMetrics<double>)m_combo_metrics.SelectedItem;
    }

    private void m_combo_arc_deleter_SelectedIndexChanged(object sender, System.EventArgs e)
    {
      this.Parameters.ArcDeleter = (IArcDeleter<double>)m_combo_arc_deleter.SelectedItem;
      ((ClusteringParameters)m_binding_source.DataSource).ArcDeleter = (IArcDeleter<double>)m_combo_arc_deleter.SelectedItem;
    }

    private void m_grid_selected_columns_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
      this.Parameters.ColumnWeights[e.ColumnIndex].Weight = (double)(m_grid_selected_columns.CurrentCell.Value);
      ((ClusteringParameters)m_binding_source.DataSource).ColumnWeights[e.ColumnIndex].Weight = (double)(m_grid_selected_columns.CurrentCell.Value);
    }
  }
}
