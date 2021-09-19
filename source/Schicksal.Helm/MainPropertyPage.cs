using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Notung;
using Notung.ComponentModel;
using Notung.Helm.Configuration;
using Schicksal.Helm.Properties;

namespace Schicksal.Helm
{
  public partial class MainPropertyPage : UserControl, IConfigurationPage
  {
    SettingsBindingSourceCollection m_sources = new SettingsBindingSourceCollection();

    public MainPropertyPage()
    {
      this.InitializeComponent();
      m_sources.Add(new SettingsBindingSource<Program.Preferences>(m_binging_source));
    }

    SettingsBindingSourceCollection IConfigurationPage.Sections
    {
      get { return m_sources; }
    }

    Control IConfigurationPage.Page
    {
      get { return this; }
    }

    Image IConfigurationPage.Image
    {
      get { return Resources.Logo_24; }
    }

    bool IConfigurationPage.UIThreadValidation
    {
      get { return true; }
    }

    public event EventHandler Changed;

    public override string ToString()
    {
      return "Schicksal";
    }

    private void Switcher_LanguageChanged(object sender, Notung.ComponentModel.LanguageEventArgs e)
    {
      m_significant_label.Text = Resources.SIGNIFICAT_COLOR;
      m_exclusive_label.Text = Resources.EXCLUSIVE_COLOR;
      m_locale_label.Text = Resources.LANGUAGE;
      m_buton_open.Text = Resources.OPEN_SYSTEM_FOLDER;
    }

    private void Buton_open_Click(object sender, EventArgs e)
    {
      Process.Start(ApplicationInfo.Instance.GetWorkingPath());
    }

    private void Significat_button_Click(object sender, EventArgs e)
    {
      this.SetColor(m_significat_panel);
    }

    private void Exclusive_button_Click(object sender, EventArgs e)
    {
      this.SetColor(m_exclusive_panel);
    }

    private void SetColor(Panel panel)
    {
      using (var dlg = new ColorDialog())
      {
        dlg.Color = panel.BackColor;

        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
          panel.BackColor = dlg.Color;

          this.Changed.InvokeIfSubscribed(this, EventArgs.Empty);
        }
      }
    }

    private void Binging_source_CurrentItemChanged(object sender, EventArgs e)
    {
      this.Changed.InvokeIfSubscribed(this, e);
    }
  }
}
