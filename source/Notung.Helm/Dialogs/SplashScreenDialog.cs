using System.ComponentModel;
using System.Windows.Forms;

namespace Notung.Helm.Dialogs
{
  public sealed partial class SplashScreenDialog : Form, ISplashScreenView
  {
    public SplashScreenDialog()
    {
      this.InitializeComponent();
    }

    BackgroundWorker ISplashScreenView.Worker
    {
      get { return m_worker; }
    }

    int ISplashScreenView.IndicatorHeight
    {
      get { return 0; }
    }

    int ISplashScreenView.MimimumProgress
    {
      get { return m_progress_bar.Minimum; }
    }

    int ISplashScreenView.MaximumProgress
    {
      get { return m_progress_bar.Maximum; }
    }

    int ISplashScreenView.ProgressValue
    {
      get { return m_progress_bar.Value; }
      set { m_progress_bar.Value = value; }
    }

    string ISplashScreenView.DescriptionText
    {
      get { return m_label_descrition.Text; }
      set { m_label_descrition.Text = value; }
    }

    bool ISplashScreenView.IsMarquee
    {
      get { return m_progress_bar.Style == ProgressBarStyle.Marquee; }
      set { m_progress_bar.Style = value ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous; }
    }
  }
}
