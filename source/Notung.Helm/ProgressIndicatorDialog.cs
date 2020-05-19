using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Notung.Helm.Properties;
using Notung.Threading;

namespace Notung.Helm
{
  public sealed partial class ProgressIndicatorDialog : Form, IProcessIndicatorView
  {
    private readonly ProgressIndicatorPresenter m_presenter;

    public ProgressIndicatorDialog(LengthyOperation work, LaunchParameters parameters)
    {
      m_presenter = new ProgressIndicatorPresenter(work, parameters, this);
      InitializeComponent();
      m_presenter.Initialize();
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

    private void m_button_Click(object sender, EventArgs e)
    {
      m_presenter.ButtonClick();
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

    bool? IProcessIndicatorView.ButtonDialogResultOK
    {
      get { return ToNeitralResult(m_button.DialogResult); }
      set { m_button.DialogResult = FromNeitralResult(value); }
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

    bool IProcessIndicatorView.SupportPercent
    {
      get { return m_progress_bar.Style != ProgressBarStyle.Marquee; }
      set { m_progress_bar.Style = value ? ProgressBarStyle.Continuous : ProgressBarStyle.Marquee; }
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

    bool? IProcessIndicatorView.DialogResultOK
    {
      get { return ToNeitralResult(this.DialogResult); }
      set { this.DialogResult = FromNeitralResult(value); }
    }
  }
}
