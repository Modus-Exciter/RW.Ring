using System;
using System.Drawing;
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
      set
      {
        m_summary_label.Text = value;
        AdaptSummary();
      }
    }

    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      AdaptSummary();
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

    private void AdaptSummary()
    {
      using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
      {
        int h = (int)g.MeasureString(" ", m_summary_label.Font).Height;

        foreach (string s in m_summary_label.Text.Split(new char[] { '\n' }))
        {
          SizeF string_size = g.MeasureString(s, m_summary_label.Font);
          double row_counter = Math.Ceiling(string_size.Width / m_summary_label.Width);
          h += (int)((row_counter) * string_size.Height);
        }

        m_top_panel.Height = h;
      }
    }
  }
}
