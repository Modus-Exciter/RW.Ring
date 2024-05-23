using System;
using System.Drawing;
using System.Windows.Forms;
using Notung.Helm.Configuration;

namespace ConfiguratorGraphicalTest
{
  public partial class TestPage2 : UserControl, IConfigurationPage
  {
    private readonly SettingsBindingSourceCollection m_sections = new SettingsBindingSourceCollection();

    public TestPage2()
    {
      this.InitializeComponent();
      m_sections.Add(new SettingsBindingSource<OuterSectionDataContractName>(m_contract_source));
      m_sections.Add(new SettingsBindingSource<OuterSectionXmlName>(m_xml_source));
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
      get { return Properties.Resources.DOS_TRACK; }
    }

    public bool UIThreadValidation
    {
      get { return true; }
    }

    public override string ToString()
    {
      return "Second section with named roots!";
    }

    public event EventHandler Changed { add { } remove { } }
  }
}
