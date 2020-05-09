using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Notung.Threading;
using System.Threading;
using System.Runtime.InteropServices;

namespace Notung
{
  public interface IAppInstance : ISynchronizeProvider
  {
    Thread MainThread { get; }
  }

  public class AppInstance : IAppInstance
  {
    private readonly IAppInstanceView m_view;
    private Thread m_main_thread;
    private Thread m_start_thread;

    public AppInstance(IAppInstanceView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      m_view = view;
      m_start_thread = Thread.CurrentThread;
    }
    
    public System.ComponentModel.ISynchronizeInvoke Invoker
    {
      get { return m_view.Invoker; }
    }

    public Thread MainThread
    {
      get
      {
        if (m_main_thread != null)
          return m_main_thread;

        if (m_view is ProcessAppInstanceView)
          return m_start_thread;

        if (m_view.Invoker.InvokeRequired)
          return m_main_thread = (Thread)m_view.Invoker.Invoke(
            new Func<Thread>(APIHelper.GetCurrentThread), new object[0]);
        else
          return Thread.CurrentThread;
      }
    }

    static private class APIHelper
    {
      [DllImport("shell32.dll")]
      public static extern bool IsUserAnAdmin();

      public static Thread GetCurrentThread()
      {
        return Thread.CurrentThread;
      }
    }
  }

  public interface IAppInstanceView : ISynchronizeProvider
  {
  }

  public sealed class ProcessAppInstanceView : SynchronizeProviderStub, IAppInstanceView
  {
  }
}
