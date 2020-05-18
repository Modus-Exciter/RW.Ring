using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Notung.Helm.Properties;
using Notung.Threading;

namespace Notung.Helm
{
  public sealed partial class ProgressIndicatorDialog : Form
  {
    private readonly LaunchParameters m_launch_parameters;
    private readonly TaskInfo m_task_info;
    private readonly CancellationTokenSource m_cancel_source;
    
    public ProgressIndicatorDialog(LaunchParameters parameters, IAsyncResult work)
    {
      if (parameters == null)
        throw new ArgumentNullException("parameters");

      if (work == null)
        throw new ArgumentNullException("work");

      m_launch_parameters = parameters;
      m_task_info = work.AsyncState as TaskInfo;

      if (m_task_info == null)
        throw new ArgumentException();

      InitializeComponent();

      if (m_task_info.RunBase is ICancelableRunBase)
      {
        m_cancel_source = new CancellationTokenSource();
        ((ICancelableRunBase)m_task_info.RunBase).CancellationToken = m_cancel_source.Token;
      }
      else
        m_button.Visible = false;

      m_launch_parameters.PropertyChanged += HanldePropertyChanged;
      m_launch_parameters.ShowCurrentSettings();

      m_task_info.ProgressChanged += HandleProgressChanged;
      m_task_info.ShowCurrentProgress();

      m_task_info.TaskCompleted += HandleTaskCompleted;
    }

    private void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      if (m_launch_parameters.SupportsPercentNotification)
        m_progress_bar.Value = e.ProgressPercentage;

      m_state_label.Text = (e.UserState ?? string.Empty).ToString();
    }

    private void HandleTaskCompleted(object sender, EventArgs e)
    {
      m_launch_parameters.PropertyChanged -= HanldePropertyChanged;
      m_task_info.ProgressChanged -= HandleProgressChanged;
      m_task_info.TaskCompleted -= HandleTaskCompleted;

      m_button.Visible = true;
      m_button.Enabled = true;
      m_progress_bar.Style = ProgressBarStyle.Continuous;

      if (m_cancel_source != null)
        m_cancel_source.Dispose();

      if (m_task_info.Status != TaskStatus.RanToCompletion)
      {
        this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.Close();
      }
      else if (m_launch_parameters.CloseOnFinish)
      {
        this.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.Close();
      }
      else
      {
        m_button.DialogResult = System.Windows.Forms.DialogResult.OK;
        m_button.Text = Resources.READY;
        m_progress_bar.Value = 100;
      }
    }

    private void HanldePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      this.Text = m_launch_parameters.Caption;

      m_picture.Image = m_launch_parameters.Bitmap;
      
      m_progress_bar.Style = m_launch_parameters.SupportsPercentNotification ? 
        ProgressBarStyle.Continuous : ProgressBarStyle.Marquee;

      m_button.Enabled = m_launch_parameters.CanCancel;
    }

    private void m_button_Click(object sender, EventArgs e)
    {
      if (m_cancel_source != null && m_task_info.Status != TaskStatus.RanToCompletion)
        m_cancel_source.Cancel();
    }
  }
}
