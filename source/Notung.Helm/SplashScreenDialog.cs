using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Notung.Loader;
using Notung.Threading;

namespace Notung.Helm
{
  public partial class SplashScreenDialog : Form
  {
    private readonly ApplicationLoadingTask m_task;

    public SplashScreenDialog(ApplicationLoadingTask task)
    {
      if (task == null)
        throw new ArgumentNullException("task");

      m_task = task;
      InitializeComponent();
    }

    public Image Picture
    {
      get { return m_splash_screen.Image; }
      set
      {
        m_splash_screen.Image = value;

        this.Width = value.Width + 2;
        this.Height = value.Height + m_label_descrition.Height + m_progress_bar.Height + 2;

        this.Left = (Screen.PrimaryScreen.Bounds.Width - value.Width) / 2 - 1;
        this.Top = (Screen.PrimaryScreen.Bounds.Height - value.Height) / 2 - 1;
      }
    }

    public ApplicationLoadingResult Result { get; private set; }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      m_worker.RunWorkerAsync();
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);
      WinAPIHelper.SetForegroundWindow(this.Handle);
    }

    private void m_worker_DoWork(object sender, DoWorkEventArgs e)
    {
      this.Result = m_task.Run(this, new BackgroundWorkProgressIndicator(m_worker));
    }

    private void m_worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      m_label_descrition.Text = (e.UserState ?? string.Empty).ToString();
      
      var value = e.ProgressPercentage;

      if (value < m_progress_bar.Minimum)
      {
        m_progress_bar.Style = ProgressBarStyle.Marquee;
      }
      else
      {
        m_progress_bar.Style = ProgressBarStyle.Continuous;

        if (value > m_progress_bar.Maximum)
          value = m_progress_bar.Maximum;

        m_progress_bar.Value = value;
      }
    }

    private void m_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      this.Close();
    }
  }
}
