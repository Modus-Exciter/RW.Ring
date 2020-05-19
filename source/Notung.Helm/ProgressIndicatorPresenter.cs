using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Notung.Helm.Properties;
using Notung.Threading;
using System.Drawing;

namespace Notung.Helm
{
  public sealed class ProgressIndicatorPresenter
  {
    private readonly IProcessIndicatorView m_view;
    private readonly LaunchParameters m_launch_parameters;
    private readonly TaskInfo m_task_info;
    private CancellationTokenSource m_cancel_source;

    public ProgressIndicatorPresenter(LaunchParameters parameters, IAsyncResult work, IProcessIndicatorView view)
    {
      if (parameters == null)
        throw new ArgumentNullException("parameters");

      if (work == null)
        throw new ArgumentNullException("work");

      if (view == null)
        throw new ArgumentNullException("view");

      m_launch_parameters = parameters;
      m_task_info = work.AsyncState as TaskInfo;
      m_view = view;

      if (m_task_info == null)
        throw new ArgumentException();
    }

    public void Initialize()
    {
      if (m_task_info.RunBase is ICancelableRunBase)
      {
        m_cancel_source = new CancellationTokenSource();
        ((ICancelableRunBase)m_task_info.RunBase).CancellationToken = m_cancel_source.Token;
      }
      else
        m_view.ButtonVisible = false;

      m_launch_parameters.PropertyChanged += HanldePropertyChanged;
      m_launch_parameters.ShowCurrentSettings();

      m_task_info.ProgressChanged += HandleProgressChanged;
      m_task_info.ShowCurrentProgress();

      m_task_info.TaskCompleted += HandleTaskCompleted;
    }

    private void HanldePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      m_view.Text = m_launch_parameters.Caption;
      m_view.Image = m_launch_parameters.Bitmap;
      m_view.SupportPercent = m_launch_parameters.SupportsPercentNotification;
      m_view.ButtonEnabled = m_launch_parameters.CanCancel;
    }

    private void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      if (m_launch_parameters.SupportsPercentNotification)
        m_view.ProgressValue = e.ProgressPercentage;

      m_view.StateText = (e.UserState ?? string.Empty).ToString();
    }

    private void HandleTaskCompleted(object sender, EventArgs e)
    {
      m_launch_parameters.PropertyChanged -= HanldePropertyChanged;
      m_task_info.ProgressChanged -= HandleProgressChanged;
      m_task_info.TaskCompleted -= HandleTaskCompleted;

      m_view.ButtonVisible = true;
      m_view.ButtonEnabled = true;
      m_view.SupportPercent = true;

      if (m_cancel_source != null)
        m_cancel_source.Dispose();

      if (m_task_info.Status != TaskStatus.RanToCompletion)
      {
        m_view.DialogResultOK = false;
        m_view.Close();
      }
      else if (m_launch_parameters.CloseOnFinish)
      {
        m_view.DialogResultOK = true;
        m_view.Close();
      }
      else
      {
        m_view.ButtonDialogResultOK = true;
        m_view.ButtonText = Resources.READY;
        m_view.ProgressValue = 100;
      }
    }

    public void ButtonClick()
    {
      if (m_cancel_source != null && m_task_info.Status != TaskStatus.RanToCompletion)
        m_cancel_source.Cancel();
    }
  }

  public interface IProcessIndicatorView
  {
    bool ButtonVisible { get; set; }

    bool ButtonEnabled { get; set; }

    bool? ButtonDialogResultOK { get; set; }

    string ButtonText { get; set; }

    string Text { get; set; }

    Image Image { get; set; }

    bool SupportPercent { get; set; }

    int ProgressValue { get; set; }

    string StateText { get; set; }

    bool? DialogResultOK { get; set; }

    void Close();
  }
}
