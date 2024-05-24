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
      InitializeComponent();

      m_biding_source.DataSource = new ClusteringParameters(table);
      m_combo_metrics.DataSource = ((ClusteringParameters)m_biding_source.DataSource).GetAllDistanceMetrics().ToList();
      m_combo_arc_deleter.DataSource = ((ClusteringParameters)m_biding_source.DataSource).GetAllArcDeleters().ToList();
    }

    public ClusteringParameters Parameters
    {
      get { return (ClusteringParameters)m_biding_source.DataSource; }
    }
  }
}
