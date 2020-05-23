using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Notung.Helm.Dialogs
{
  public partial class ErrorBox : Form
  {
    public ErrorBox()
    {
      InitializeComponent();
    }

    public string Content
    {
      get { return m_text_box.Text; }
      set { m_text_box.Text = value; }
    }
  }
}
