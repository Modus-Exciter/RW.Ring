using System;
using System.Drawing;
using System.Windows.Forms;
using Notung.Services;
using Schicksal.Regression;

namespace Schicksal.Helm.Dialogs
{
  public partial class RegressionDetailsDialog : Form
  {
    private Point m_clicked;
    private Point m_clicked_mouse;

    public RegressionDetailsDialog(RegressionDependency dependency)
    {
      this.InitializeComponent();

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
      if (m_clicked_mouse == MousePosition)
        this.Close();
    }

    private void RegressionDetailsDialog_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        m_clicked = e.Location;
        m_clicked_mouse = MousePosition;
      }
    }

    private void RegressionDetailsDialog_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
        return;

      var dx = e.Location.X - m_clicked.X;
      var dy = e.Location.Y - m_clicked.Y;

      this.Location = new Point(this.Location.X + dx, this.Location.Y + dy);
    }

    private void RegressionDetailsDialog_Deactivate(object sender, EventArgs e)
    {
      new Action(() =>
      {
        AppManager.OperationLauncher.Invoker.Invoke(new Action(() =>
        {
          if (!this.IsDisposed)
            this.Close();
        }), null);
      }).BeginInvoke(null, null);
    }
  }
}