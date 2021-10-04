using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CertGenerator
{
  public partial class PasswordForm : Form
  {
    public PasswordForm()
    {
      this.InitializeComponent();
    }

    public string Password => m_password_edit.Text;
  }
}
