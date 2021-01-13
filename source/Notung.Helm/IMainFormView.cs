using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Notung.ComponentModel;
using Notung.Data;
using Notung.Helm.Dialogs;
using Notung.Helm.Windows;
using Notung.Logging;
using Notung.Services;

namespace Notung.Helm
{
  public interface IMainFormView : IAppInstanceView, IOperationLauncherView, INotificatorView
  {
    Form MainForm { get; }

    INotificator Notificator { get; set; }

    bool RestartOnCriticalError { get; }

    ISplashScreenView GetSplashScreenView();

    bool ShowSettingsForm(InfoBuffer infoBuffer);

    void ShowErrorBox(string title, string content);
  }

  public class MainFormView : IMainFormView
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(MainFormView));
    private readonly Form m_main_form;

    static MainFormView()
    {
      DataTypeCode.Set(StringArgsMessageCode, "Command line arguments");
    }

    public MainFormView(Form mainForm)
    {
      if (mainForm == null)
        throw new ArgumentNullException("mainForm");

      m_main_form = mainForm;
    }

    public ISynchronizeInvoke Invoker
    {
      get { return m_main_form; }
    }

    public bool ReliableThreading
    {
      get { return true; }
    }

    public bool SupportSendingArgs
    {
      get
      {
        return m_main_form.GetType().GetMethod("WndProc", 
          BindingFlags.Instance | BindingFlags.NonPublic).DeclaringType != typeof(Form);
      }
    }

    public Form MainForm
    {
      get { return m_main_form; }
    }

    public virtual bool RestartOnCriticalError
    {
      get { return false; }
    }

    public void Restart(string startPath, IList<string> args)
    {
      System.Windows.Forms.Application.Restart();
    }

    #region Dialog factory ------------------------------------------------------------------------

    public virtual ISplashScreenView GetSplashScreenView()
    {
      return new SplashScreenDialog();
    }

    public virtual bool ShowSettingsForm(InfoBuffer infoBuffer)
    {
      using (var dlg = new SettingsDialog())
      {
        return dlg.ShowDialog(m_main_form) == DialogResult.OK;
      }
    }

    public virtual void ShowErrorBox(string title, string content)
    {
      using (var dlg = new ErrorBox())
      {
        dlg.Text = title;
        dlg.Content = content;
        dlg.ShowDialog(m_main_form);
      }
    }

    public void ShowProgressDialog(LengthyOperation operation, bool closeOnFinish)
    {
      using (var dlg = this.GetProgressIndicatorView())
      {
        var presenter = new ProgressIndicatorPresenter(operation, dlg, closeOnFinish);
        dlg.ShowDialog(m_main_form);
      }
    }

    protected virtual IProcessIndicatorView GetProgressIndicatorView()
    {
      return new ProgressIndicatorDialog();
    }

    #endregion

    #region Alerts --------------------------------------------------------------------------------

    public INotificator Notificator { get; set; }

    public void ShowError(Exception error)
    {
      this.Alert(new Info(error), ConfirmationRegime.None);
    }

    public void ShowMessages(InfoBuffer messages) 
    {
      var notificator = this.Notificator;

      if (notificator != null)
        notificator.Show(messages);
    }

    public virtual bool? Alert(Info info, ConfirmationRegime confirm)
    {
      return PerformAlert(buttons => MessageBox.Show(m_main_form, 
        info.Message, m_main_form.Text, buttons, 
        GetIconForLevel(info.Level, confirm != ConfirmationRegime.None)), confirm);
    }

    public virtual bool? Alert(string summary, InfoLevel summaryLevel, InfoBuffer buffer, ConfirmationRegime confirm)
    {
      using (var dialog = new InfoBufferForm())
      {
        dialog.Summary = summary;
        dialog.SetInfoBuffer(buffer);
        dialog.Text = summaryLevel.GetLabel();

        return PerformAlert(delegate(MessageBoxButtons buttons)
        {
          dialog.Buttons = buttons;
          return dialog.ShowDialog(m_main_form);
        }, confirm);
      }
    }

    protected bool? PerformAlert(Func<MessageBoxButtons, DialogResult> dialogFunction, ConfirmationRegime confirm)
    {
      if (confirm == ConfirmationRegime.None)
      {
        dialogFunction(MessageBoxButtons.OK);
        return null;
      }
      else if (confirm == ConfirmationRegime.Confirm)
        return dialogFunction(MessageBoxButtons.OKCancel) == DialogResult.OK;
      else
      {
        switch (dialogFunction(MessageBoxButtons.YesNoCancel))
        {
          case DialogResult.Yes:
            return true;
          case DialogResult.No:
            return false;
          default:
            return null;
        }
      }
    }

    protected MessageBoxIcon GetIconForLevel(InfoLevel level, bool confirm)
    {
      switch (level)
      {
        case InfoLevel.Debug:
          return MessageBoxIcon.None;
        case InfoLevel.Info:
          return confirm ? MessageBoxIcon.Question : MessageBoxIcon.Information;
        case InfoLevel.Warning:
          return MessageBoxIcon.Warning;
        case InfoLevel.Error:
          return MessageBoxIcon.Error;
        case InfoLevel.Fatal:
          return MessageBoxIcon.Stop;
        default:
          return MessageBoxIcon.None;
      }
    }

    #endregion

    #region Command line arguments sending --------------------------------------------------------

    public static readonly DataTypeCode StringArgsMessageCode = 1;

    public static TimeSpan SendMessageTimeout = TimeSpan.FromMilliseconds(0x100);

    public bool SendArgsToProcess(Process previous, IList<string> args)
    {
      var text_to_send = string.Join("\n", args);
      var cd = new CopyData(Encoding.Unicode.GetBytes(text_to_send), StringArgsMessageCode);

      if (cd.Send(previous.MainWindowHandle, SendMessageTimeout) != IntPtr.Zero)
      {
        WinAPIHelper.SetForegroundWindow(previous.MainWindowHandle);
        return true;
      }

      return false;
    }

    public static bool GetStringArgs(ref Message message, out string[] args)
    {
      if (message.Msg == WinAPIHelper.WM_COPYDATA)
      {
        var cd = new CopyData(message.LParam, StringArgsMessageCode);

        _log.DebugFormat("GetStringArgs(): copy data structure ({0}) recieved", cd);

        if (cd.Data != null)
        {
          args = Encoding.Unicode.GetString(cd.Data).Split('\n');
          return true;
        }
      }

      args = ArrayExtensions.Empty<string>();
      return false;
    }

    #endregion
  }
}