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

    //private void MeasureText1(PaintEventArgs e)
    //{
    //  String text1 = m_summary_label.Text;
    //  Font arialBold = new Font("Arial", 12.0F);
    //  Size textSize = TextRenderer.MeasureText(text1, arialBold);
    //  TextRenderer.MeasureText(m_summary_label.ToString(), arialBold);
    //}

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
    void AdaptSummary ()
    {
      using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
      {
        SizeF stringSize = new SizeF();
        string[] paragraph = m_summary_label.Text.Split(new char[] { '\n' });
        int h = 0;

        foreach (string s in paragraph)
        {
          stringSize = g.MeasureString(s, m_summary_label.Font);
          float rowCounter = (float)Math.Ceiling(stringSize.Width / m_summary_label.Width);
          h += (int)((rowCounter) * stringSize.Height);
        }
        m_top_panel.Height = h;
      }
    }
  }
}
