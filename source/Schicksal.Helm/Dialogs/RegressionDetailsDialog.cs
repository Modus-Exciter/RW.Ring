using Schicksal.Regression;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Schicksal.Helm.Dialogs
{
  public partial class RegressionDetailsDialog : Form
  {
    public RegressionDetailsDialog(RegressionDependency dependency)
    {
      InitializeComponent();

      m_label_metrics1.Text = string.Format("{0}: {1}; {2}: {3:0.0000}; {4}: {5:0.0000}",
         SchicksalResources.HETEROSCEDASTICITY, dependency.Heteroscedasticity,
         SchicksalResources.CONSISTENCY, dependency.Consistency,
         SchicksalResources.CONSISTENCY_WEIGHTED, dependency.ConsistencyWeighted);

      m_label_metrics2.Text = string.Format("{0}: {1}; {2}: {3};",
        SchicksalResources.RMS_ERROR, dependency.RMSError,
        SchicksalResources.RMS_ERROR_WEIGHTED, dependency.RMSErrorWeighted);
    }

    private void RegressionDetailsDialog_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
