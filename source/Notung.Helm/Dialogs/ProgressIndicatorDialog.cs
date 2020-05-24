using System;
using System.Windows.Forms;

namespace Notung.Helm.Dialogs
{
  public sealed partial class ProgressIndicatorDialog : Form, IProcessIndicatorView
  {
    public ProgressIndicatorDialog()
    {
      InitializeComponent();
    }

    public static bool? ToNeitralResult(DialogResult resutl)
    {
      if (resutl == DialogResult.OK)
        return true;
      else if (resutl == DialogResult.Cancel)
        return false;
      else
        return null;
    }

    public static DialogResult FromNeitralResult(bool? value)
    {
      if (value == true)
        return System.Windows.Forms.DialogResult.OK;
      else if (value == false)
        return System.Windows.Forms.DialogResult.Cancel;
      else
        return System.Windows.Forms.DialogResult.None;
    }

    bool IProcessIndicatorView.ButtonVisible
    {
      get { return m_button.Visible; }
      set { m_button.Visible = value; }
    }

    bool IProcessIndicatorView.ButtonEnabled
    {
      get { return m_button.Enabled; }
      set { m_button.Enabled = value; }
    }

    DialogResult IProcessIndicatorView.ButtonDialogResult
    {
      get { return m_button.DialogResult; }
      set { m_button.DialogResult = value; }
    }

    string IProcessIndicatorView.ButtonText
    {
      get { return m_button.Text; }
      set { m_button.Text = value; }
    }

    System.Drawing.Image IProcessIndicatorView.Image
    {
      get { return m_picture.Image; }
      set { m_picture.Image = value; }
    }

    bool IProcessIndicatorView.IsMarquee
    {
      get { return m_progress_bar.Style == ProgressBarStyle.Marquee; }
      set { m_progress_bar.Style = value ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous; }
    }

    int IProcessIndicatorView.ProgressValue
    {
      get { return m_progress_bar.Value; }
      set { m_progress_bar.Value = value; }
    }

    string IProcessIndicatorView.StateText
    {
      get { return m_state_label.Text ; }
      set { m_state_label.Text = value; }
    }

    event EventHandler IProcessIndicatorView.ButtonClick
    {
      add { m_button.Click += value; }
      remove { m_button.Click -= value; }
    }
  }
}