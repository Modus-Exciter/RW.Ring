using System;
using System.Windows.Forms;

namespace Notung.Helm
{
  public partial class MainFormBase : Form
  {
    public MainFormBase()
    {
      InitializeComponent();
    }

    protected override void WndProc(ref Message msg)
    {
      base.WndProc(ref msg);

      if (msg.Msg == MainFormAppInstanceView.StringArgsMessageCode)
      {
        if (AcceptStringArgs(MainFormAppInstanceView.GetStringArgs(msg)))
          msg.Result = new IntPtr(1);
      }
    }

    protected virtual bool AcceptStringArgs(string[] args)
    {
      return false;
    }
  }
}
