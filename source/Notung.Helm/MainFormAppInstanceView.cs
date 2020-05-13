using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using Notung.Helm.Properties;

namespace Notung.Helm
{
  public class MainFormAppInstanceView : IAppInstanceView
  {
    private readonly Form m_main_form;

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

    public const int StringArgsMessageCode = 0xA146;

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

    public bool SendArgsToProcess(Process previous, IList<string> args)
    {
      using (var atom = new Atom(string.Join("\n", args)))
      {
        if (atom.IsValid)
        {
          if (WinAPIHelper.SendMessage(previous.MainWindowHandle, StringArgsMessageCode,
              new IntPtr(atom.BufferSize), atom.Handle) != IntPtr.Zero)
          {
            WinAPIHelper.SetForegroundWindow(previous.MainWindowHandle);
            return true;
          }
        }
      } // TODO: Если строку не удалось уместить в атом (Windows сама об этом скажет через IsValid), используем WM_COPYDATA

      return false;

      /*
      Random rd = new Random();

      string fileName = Path.Combine(AppManager.Instance.WorkingPath, string.Format("{0}.link", handle));
      File.WriteAllLines(fileName, args.ToArray(), Encoding.UTF8);

      try
      {
        var result = WinAPIHelper.SendMessage(proc.MainWindowHandle,
          StringArgsMessageCode, new IntPtr(sendee.Length + 1), new IntPtr(handle));

        if (result != IntPtr.Zero)
        {
          WinAPIHelper.SetForegroundWindow(proc.MainWindowHandle);
          return true;
        }
      }
      finally
      {
        if (File.Exists(fileName))
          File.Delete(fileName);
      }
      */
    }

    public void Restart(string startPath, IList<string> args)
    {
      System.Windows.Forms.Application.Restart();
    }

    public static string[] GetStringArgs(Message message)
    {
      if (message.Msg != StringArgsMessageCode)
        throw new ArgumentException(string.Format(Resources.NO_LINK_MESSAGE_CODE, message.Msg, MethodInfo.GetCurrentMethod().DeclaringType));

      /*string linkFile = Path.Combine(AppManager.Instance.WorkingPath,
        message.LParam.ToInt32().ToString() + ".link");

      if (File.Exists(linkFile))
        return File.ReadAllLines(linkFile, Encoding.UTF8);*/

      using (var atom = new Atom(message.LParam, message.WParam.ToInt32()))
      {
        if (atom.Text != null)
          return atom.Text.Split('\n');
        else 
          return ArrayExtensions.Empty<string>();
      }
    }
  }
}
