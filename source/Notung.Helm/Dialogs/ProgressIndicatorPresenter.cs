﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Notung.Helm.Properties;
using Notung.Services;

namespace Notung.Helm.Dialogs
{
  /// <summary>
  /// Основная логика работы диалога с индикатором прогресса задачи
  /// </summary>
  public sealed class ProgressIndicatorPresenter
  {
    private readonly IProcessIndicatorView m_view;
    private readonly bool m_close_on_finish;
    private readonly LengthyOperation m_operation;
    private CancellationTokenSource m_cancel_source;

    public ProgressIndicatorPresenter(LengthyOperation operation, IProcessIndicatorView view, bool closeOnFinish)
    {
      if (operation == null)
        throw new ArgumentNullException("operation");

      if (view == null)
        throw new ArgumentNullException("view");

      m_close_on_finish = closeOnFinish;
      m_operation = operation;
      m_view = view;

      m_cancel_source = m_operation.GetCancellationTokenSource();

      if (m_cancel_source != null)
      {
        m_operation.CanCancelChanged += this.HandleCanCancelChanged;
        m_view.ButtonText = WinResources.CANCEL;
      }
      else
        m_view.ButtonVisible = false;

      m_operation.ProgressChanged += this.HandleProgressChanged;
      m_operation.Completed += this.HandleOperationCompleted;

      m_view.ButtonEnabled = m_operation.CanCancel;
      m_view.Text = m_operation.GetWorkCaption();
      m_view.Image = m_operation.GetWorkImage();
      m_view.ButtonClick += this.ButtonClick;
      m_view.Load += this.HandleLoad;
    }

    private void HandleLoad(object sender, EventArgs e)
    {
      if (m_operation.Status == TaskStatus.RanToCompletion || m_operation.Status == TaskStatus.Faulted)
      {
        this.Unsubscribe();
        m_view.Close();
      }
      else
        m_operation.ShowCurrentProgress();
    }

    private void HandleCanCancelChanged(object sender, EventArgs e)
    {
      m_view.ButtonEnabled = m_operation.CanCancel;
    }

    private void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      if (e.ProgressPercentage != ProgressPercentage.Unknown)
      {
        m_view.IsMarquee = false;
        m_view.ProgressValue = e.ProgressPercentage;
      }
      else
        m_view.IsMarquee = true;

      if (e.UserState is LaunchParametersChange)
      {
        if (((LaunchParametersChange)e.UserState & LaunchParametersChange.Caption) != 0)
          m_view.Text = m_operation.GetWorkCaption();

        if (((LaunchParametersChange)e.UserState & LaunchParametersChange.Image) != 0)
          m_view.Image = m_operation.GetWorkImage();
      }
      else
        m_view.StateText = (e.UserState ?? string.Empty).ToString();
    }

    private void HandleOperationCompleted(object sender, EventArgs e)
    {
      this.Unsubscribe();

      m_view.ButtonVisible = true;
      m_view.ButtonEnabled = true;
      m_view.IsMarquee = false;

      if (m_operation.Status != TaskStatus.RanToCompletion)
      {
        m_view.DialogResult = DialogResult.Cancel;
        m_view.Close();
      }
      else if (m_close_on_finish)
      {
        m_view.DialogResult = DialogResult.OK;
        m_view.Close();
      }
      else
      {
        m_view.ButtonDialogResult = DialogResult.OK;
        m_view.ButtonText = Resources.READY;
        m_view.ProgressValue = ProgressPercentage.Completed;
      }
    }

    private void Unsubscribe()
    {
      m_operation.ProgressChanged -= this.HandleProgressChanged;
      m_operation.Completed -= this.HandleOperationCompleted;

      if (m_cancel_source != null)
        m_operation.CanCancelChanged -= this.HandleCanCancelChanged;

      m_view.ButtonClick -= this.ButtonClick;
      m_view.Load -= this.HandleLoad;
    }

    private void ButtonClick(object sender, EventArgs e)
    {
      if (m_cancel_source != null && m_operation.Status != TaskStatus.RanToCompletion)
        m_cancel_source.Cancel();
    }
  }

  public interface IProcessIndicatorView : IDisposable
  {
    bool ButtonVisible { get; set; }

    bool ButtonEnabled { get; set; }

    DialogResult ButtonDialogResult { get; set; }

    string ButtonText { get; set; }

    string Text { get; set; }

    Image Image { get; set; }

    bool IsMarquee { get; set; }

    int ProgressValue { get; set; }

    string StateText { get; set; }

    DialogResult DialogResult { get; set; }

    DialogResult ShowDialog(IWin32Window owner);

    void Close();

    event EventHandler ButtonClick;

    event EventHandler Load;
  }
}