using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
    
    public System.ComponentModel.ISynchronizeInvoke Invoker
    {
      get { return m_main_form; }
    }

    public string StartupPath
    {
      get { return System.Windows.Forms.Application.StartupPath; }
    }

    public void Restart(IList<string> args)
    {
      System.Windows.Forms.Application.Restart();
    }
  }
}
