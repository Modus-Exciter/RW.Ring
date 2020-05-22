using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Notung.Data;
using Notung.Logging;
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
      MessageBox.Show(content, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

    public virtual void ShowProgressDialog(LengthyOperation operation, LaunchParameters parameters)
    {
      using (var dlg = new ProgressIndicatorDialog(operation, parameters))
      {
        dlg.ShowDialog(m_main_form);
      }
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
      return this.Alert(new Info(summary + Environment.NewLine
        + string.Join(Environment.NewLine, buffer.Select(i => i.Message)),
        summaryLevel), confirm);
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
