using System.Windows.Forms;

namespace Notung.Helm.Dialogs
{
  public partial class ErrorBox : Form
  {
    public ErrorBox()
    {
      InitializeComponent();

      m_error_box.ButtonClick += (sender, e) => this.DialogResult = System.Windows.Forms.DialogResult.OK;
      m_error_box.Moving += (sender, e) =>
        {
          this.Left += e.X;
          this.Top += e.Y;
        };
    }

    public string Content
    {
      get { return m_error_box.Text; }
      set { m_error_box.Text = value; }
    }

    public new string Text
    {
      get
      {
        return m_error_box.Caption;
      }
      set
      {
        m_error_box.Caption = value;
      }
    }
  }
}
