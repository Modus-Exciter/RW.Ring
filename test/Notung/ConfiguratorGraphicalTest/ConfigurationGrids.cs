using System.Windows.Forms;
using Notung.Helm.Controls;
using Notung.Services;

namespace ConfiguratorGraphicalTest
{
  [ContractPlaceholder(typeof(IConfigurationGrids))]
  public partial class ConfigurationGrids : UserControl, IConfigurationGrids
  {
    public ConfigurationGrids(HelpItem item)
    {
      InitializeComponent();
    }

    public void HandleFormShown()
    {
      innerDefault.SelectedObject = AppManager.Configurator.GetSection<Form1.InnerSectionDefault>();
      innerContract.SelectedObject = AppManager.Configurator.GetSection<Form1.InnerSectionDataContract>();
      innerContractName.SelectedObject = AppManager.Configurator.GetSection<Form1.InnerSectionDataContractName>();
      innerXml.SelectedObject = AppManager.Configurator.GetSection<Form1.InnerSectionXml>();
      innerXmlName.SelectedObject = AppManager.Configurator.GetSection<Form1.InnerSectionXmlName>();

      outerDefault.SelectedObject = AppManager.Configurator.GetSection<OuterSectionDefault>();
      outerContract.SelectedObject = AppManager.Configurator.GetSection<OuterSectionDataContract>();
      outerContractName.SelectedObject = AppManager.Configurator.GetSection<OuterSectionDataContractName>();
      outerXml.SelectedObject = AppManager.Configurator.GetSection<OuterSectionXml>();
      outerXmlName.SelectedObject = AppManager.Configurator.GetSection<OuterSectionXmlName>();
    }
  }

  public interface IConfigurationGrids { }
}
