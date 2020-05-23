using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Notung.ComponentModel;
using Notung.Data;
using Notung.Helm.Dialogs;
using Notung.Helm.Windows;
using Notung.Logging;
using Notung.Services;
using Notung.Threading;

namespace Notung.Helm
{
  public interface IMainFormView : IAppInstanceView, IOperationLauncherView, INotificatorView
  {
    Form MainForm { get; }

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

    public static readonly DataTypeCode StringArgsMessageCode = 1;

    public static TimeSpan SendMessageTimeout = TimeSpan.FromMilliseconds(0x100);

    public Form MainForm
    {
      get { return m_main_form; }
    }

    public virtual ISplashScreenView GetSplashScreenView()
    {
      return new SplashScreenDialog();
    }

    public virtual bool ShowSettingsForm(InfoBuffer infoBuffer)
    {
      return true;
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

    public void Restart(string startPath, IList<string> args)
    {
      System.Windows.Forms.Application.Restart();
    }

    public void ShowProgressDialog(LengthyOperation operation, LaunchParameters parameters)
    {
      using (var dlg = this.GetProgressIndicatorView())
      {
        var presenter = new ProgressIndicatorPresenter(operation, parameters, dlg);
        dlg.ShowDialog(m_main_form);
      }
    }

    protected virtual IProcessIndicatorView GetProgressIndicatorView()
    {
      return new ProgressIndicatorDialog();
    }

    public virtual bool? Alert(Info info, ConfirmationRegime confirm)
    {
      if (confirm == ConfirmationRegime.None)
        MessageBox.Show(info.Message, m_main_form.Text, MessageBoxButtons.OK, GetIconForLevel(info.Level, false));
      else if (confirm == ConfirmationRegime.Confirm)
        return MessageBox.Show(info.Message, m_main_form.Text, MessageBoxButtons.OKCancel,
          GetIconForLevel(info.Level, true)) == DialogResult.OK;
      else
      {
        var res = MessageBox.Show(info.Message, m_main_form.Text,
          MessageBoxButtons.YesNoCancel, GetIconForLevel(info.Level, true));

        if (res == DialogResult.Yes)
          return true;
        else if (res == DialogResult.No)
          return false;
      }

      return null;
    }

    public virtual bool? Alert(string summary, InfoLevel summaryLevel, InfoBuffer buffer, ConfirmationRegime confirm)
    {
      using (var dialog = new InfoBufferForm())
      {
        dialog.Summary = summary;
        dialog.SetInfoBuffer(buffer);
        dialog.Text = summaryLevel.GetLabel();
        if (confirm == ConfirmationRegime.None)
        {
          dialog.Buttons = MessageBoxButtons.OK;
          dialog.ShowDialog(m_main_form);
          return null;
        }
        else if (confirm == ConfirmationRegime.Confirm)
        {
          dialog.Buttons = MessageBoxButtons.OKCancel;
          return dialog.ShowDialog(m_main_form) == DialogResult.OK;
        }
        else
        {
          dialog.Buttons = MessageBoxButtons.YesNoCancel;
          switch (dialog.ShowDialog(m_main_form))
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
    }

    public static string[] GetStringArgs(Message message)
    {
      if (message.Msg == WinAPIHelper.WM_COPYDATA)
      {
        var cd = new CopyData(message.LParam, StringArgsMessageCode);

        _log.DebugFormat("GetStringArgs(): copy data structure ({0}) recieved", cd);
        if (cd.Data != null)
          return Encoding.Unicode.GetString(cd.Data).Split('\n');
      }

      return ArrayExtensions.Empty<string>();
    }

    protected MessageBoxIcon GetIconForLevel(InfoLevel level, bool confirm)
    {
      switch (level)
      {
        case InfoLevel.Debug:
          return MessageBoxIcon.None;
        case InfoLevel.Info:
          return confirm ? MessageBoxIcon.Information : MessageBoxIcon.Question;
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
  }
}
