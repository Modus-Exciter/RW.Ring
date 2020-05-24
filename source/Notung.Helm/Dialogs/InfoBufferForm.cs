using System.Windows.Forms;

namespace Notung.Helm.Dialogs
{
  public partial class InfoBufferForm : Form
  {
    private MessageBoxButtons m_buttons;

    public InfoBufferForm()
    {
      InitializeComponent();
    }

    public void SetInfoBuffer(InfoBuffer buffer)
    {
      m_buffer_view.SetInfoBuffer(buffer);
    }

    public string Summary
    {
      get { return m_summary_label.Text; }
      set { m_summary_label.Text = value; }
    }

    public MessageBoxButtons Buttons
    {
      get { return m_buttons; }
      set
      {
        if (value == m_buttons)
          return;

        foreach (Control control in m_bottom_panel.Controls)
          control.Visible = false;

        m_buttons = value;

        switch (value)
        {
          case MessageBoxButtons.OK:
            m_button_ok.Visible = true;
            break;

          case MessageBoxButtons.OKCancel:
            m_button_ok.Visible = true;
            m_button_cancel.Visible = true;
            break;

          case MessageBoxButtons.YesNoCancel:
            m_button_yes.Visible = true;
            m_button_no.Visible = true;
            m_button_cancel.Visible = true;
            break;
        }
      }
    }
  }
}
