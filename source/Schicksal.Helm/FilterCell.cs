using System;
using System.Drawing;
using System.Windows.Forms;

namespace Schicksal.Helm
{
  public partial class FilterCell : UserControl
  {
    private readonly string m_property;

    public FilterCell(DataGridViewColumn column)
    {
      m_property = column.DataPropertyName;

      this.InitializeComponent();
    }

    public override string Text
    {
      get { return m_text_box.Text; }
      set { m_text_box.Text = value; }
    }

    public string Property
    {
      get { return m_property; }
    }

    private void HandleTextChanged(object sender, EventArgs e)
    {
      this.OnTextChanged(e);
    }

    private void m_text_box_SizeChanged(object sender, EventArgs e)
    {
      this.Height = m_text_box.Height + this.Padding.Top + this.Padding.Bottom
        + m_text_border.Padding.Top + m_text_border.Padding.Bottom;
    }

    private void m_text_box_MouseEnter(object sender, EventArgs e)
    {
      this.BackColor = SystemColors.GrayText;
    }

    private void m_text_box_MouseLeave(object sender, EventArgs e)
    {
      this.BackColor = SystemColors.ControlDark;
    }
  }
}
