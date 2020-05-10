using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
        AcceptStringArgs(MainFormAppInstanceView.GetStringArgs(msg));
      }
    }

    protected virtual void AcceptStringArgs(string[] args) { }
  }
}
