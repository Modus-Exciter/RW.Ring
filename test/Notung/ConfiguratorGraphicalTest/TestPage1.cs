using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Notung.Helm.Configuration;

namespace ConfiguratorGraphicalTest
{
  public partial class TestPage1 : UserControl, IConfigurationPage
  {
    private readonly SettingsBindingSourceCollection m_sections = new SettingsBindingSourceCollection();

    public TestPage1()
    {
      InitializeComponent();
      m_sections.Add(new SettingsBindingSource<OuterSectionDataContract>(m_contract_source));
      m_sections.Add(new SettingsBindingSource<OuterSectionXml>(m_xml_source));
    }

    public SettingsBindingSourceCollection Sections
    {
      get { return m_sections; }
    }

    public Control Page
    {
      get { return this; }
    }

    public Image Image
    {
      get { return null; }
    }

    public bool UIThreadValidation
    {
      get { return false; }
    }

    public override string ToString()
    {
      return "First section!";
    }

    public event EventHandler Changed;

    private void HandleChanged(object sender, EventArgs e)
    {
      if (this.Changed != null)
        this.Changed(this, e);
    }
  }
}
