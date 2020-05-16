using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Notung.Data;
using Notung.Logging;

namespace Notung.Helm
{
  public class MainFormAppInstanceView : IAppInstanceView
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(MainFormAppInstanceView));
    private readonly Form m_main_form;
    private static readonly uint _args_code = 1;

    static MainFormAppInstanceView()
    {
      DataTypeCode.Set(1, "Command line arguments");
    }

    public MainFormAppInstanceView(Form mainForm)
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

    public static uint StringArgsMessageCode
    {
      get { return _args_code; }
    }

    public static TimeSpan SendMessageTimeout = TimeSpan.FromMilliseconds(0x100);

    public bool SendArgsToProcess(Process previous, IList<string> args)
    {
      var text_to_send = string.Join("\n", args);

      var cd = new CopyData(Encoding.Unicode.GetBytes(text_to_send), (DataTypeCode)StringArgsMessageCode);

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

    public static string[] GetStringArgs(Message message)
    {
      if (message.Msg == WinAPIHelper.WM_COPYDATA)
      {
        var cd = new CopyData(message.LParam, (DataTypeCode)StringArgsMessageCode);

        _log.DebugFormat("GetStringArgs(): copy data structure ({0}) recieved", cd);
        if (cd.Data != null)
          return Encoding.Unicode.GetString(cd.Data).Split('\n');
      }

      return ArrayExtensions.Empty<string>();
    }
  }
}
