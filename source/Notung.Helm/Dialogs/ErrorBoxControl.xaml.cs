using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Notung.ComponentModel;

namespace Notung.Helm.Dialogs
{
  /// <summary>
  /// Interaction logic for ErrorBoxControl.xaml
  /// </summary>
  public partial class ErrorBoxControl : UserControl
  {
    private Point m_position;
    private bool m_dragging;
    
    public ErrorBoxControl()
    {
      InitializeComponent();
    }

    public event EventHandler ButtonClick;
    public event EventHandler<MoveEventArgs> Moving;

    public string Caption
    {
      get { return m_caption.Text; }
      set { m_caption.Text = value; }
    }

    public string Text
    {
      get { return m_text.Text; }
      set { m_text.Text = value; }
    }

    private void m_ok_button_Click(object sender, RoutedEventArgs e)
    {
      this.ButtonClick.InvokeSynchronized(this, e);
    }

    private void top_rectangle_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
      {
        m_position = e.GetPosition(m_top);
        m_dragging = true;
      }
    }

    private void m_top_MouseUp(object sender, MouseButtonEventArgs e)
    {
      m_dragging = false;
    }

    private void m_top_MouseMove(object sender, MouseEventArgs e)
    {
      if (!m_dragging)
        return;

      var args = new MoveEventArgs();
      args.X = (int)(e.GetPosition(m_top).X - m_position.X);
      args.Y = (int)(e.GetPosition(m_top).Y - m_position.Y);

      Moving.InvokeSynchronized(this, args);
    }
  }

  public class MoveEventArgs : EventArgs
  {
    public int X { get; set; }

    public int Y { get; set; }
  }
}
