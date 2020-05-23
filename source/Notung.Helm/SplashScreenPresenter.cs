using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Notung.Loader;
using Notung.Threading;

namespace Notung.Helm
{
  public sealed class SplashScreenPresenter
  {
    private readonly ApplicationLoadingWork m_work;
    private readonly ISplashScreenView m_view;
    private readonly BackgroundWorker m_worker;

    public SplashScreenPresenter(ApplicationLoadingWork work, ISplashScreenView view)
    {
      if (work == null)
        throw new ArgumentNullException("work");

      if (view == null)
        throw new ArgumentNullException("view");

      m_work = work;
      m_view = view;
      m_view.Load += this.HandleLoad;
      m_view.Shown += this.HandleShown;

      m_worker = m_view.Worker;
      m_worker.WorkerReportsProgress = true;
      m_worker.DoWork += HandleDoWork;
      m_worker.ProgressChanged += HandleProgressChanged;
      m_worker.RunWorkerCompleted += HandleRunWorkerCompleted;
    }

    public Image Picture
    {
      get { return m_view.BackgroundImage; }
      set
      {
        if (value == null)
          return;

        m_view.BackgroundImage = value;

        m_view.Width = value.Width;
        m_view.Height = value.Height + m_view.IndicatorHeight;

        m_view.Left = (Screen.PrimaryScreen.Bounds.Width - value.Width) / 2;
        m_view.Top = (Screen.PrimaryScreen.Bounds.Height - value.Height) / 2;
      }
    }

    private void HandleLoad(object sender, EventArgs e)
    {
      m_worker.RunWorkerAsync();
    }

    public ApplicationLoadingResult Result { get; private set; }

    private void HandleShown(object sender, EventArgs e)
    {
      WinAPIHelper.SetForegroundWindow(m_view.Handle);
    }

    private void HandleDoWork(object sender, DoWorkEventArgs e)
    {
      this.Result = m_work.Run(m_view, new BackgroundWorkProgressIndicator(m_worker));
    }

    private void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      m_view.DescriptionText = (e.UserState ?? string.Empty).ToString();

      var value = e.ProgressPercentage;

      if (value < m_view.MimimumProgress)
      {
        m_view.IsMarquee = true;
      }
      else
      {
        m_view.IsMarquee = false;

        if (value > m_view.MaximumProgress)
          value = m_view.MaximumProgress;

        m_view.ProgressValue = value;
      }
    }

    private void HandleRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      m_view.Close();
    }
  }

  public interface ISplashScreenView : ISynchronizeInvoke, IDisposable
  {
    #region Common Windows Form members -----------------------------------------------------------

    IntPtr Handle { get; }

    int Left { get; set; }

    int Top { get; set; }

    int Width { get; set; }

    int Height { get; set; }

    Image BackgroundImage { get; set; }

    void Close();

    DialogResult ShowDialog(IWin32Window owner);

    event EventHandler Load;

    event EventHandler Shown;

    #endregion

    #region Specific members ----------------------------------------------------------------------

    BackgroundWorker Worker { get; }

    int IndicatorHeight { get; }

    int MimimumProgress { get; }

    int MaximumProgress { get; }

    int ProgressValue { get; set; }

    string DescriptionText { get; set; }

    bool IsMarquee { get; set; }

    #endregion
  }
}
