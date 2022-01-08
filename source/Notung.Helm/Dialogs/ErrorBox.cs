using System.Windows.Forms;

namespace Notung.Helm.Dialogs
{
  public partial class ErrorBox : Form
  {
    public ErrorBox()
    {
      this.InitializeComponent();
    }

    public string Content
    {
      get { return m_text_box.Text; }
      set { m_text_box.Text = value; }
    }
  }
}
