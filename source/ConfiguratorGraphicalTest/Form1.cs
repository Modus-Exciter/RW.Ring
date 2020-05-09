﻿using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml.Serialization;
using Notung;
using Notung.Configuration;

namespace ConfiguratorGraphicalTest
{
  public partial class Form1 : Form
  {
    public enum InnerEnum
    {
      One, Two, Three
    }
    
    public class InnerSectionDefault : ConfigurationSection
    {
      public int Number { get; set; }

      public string Text { get; set; }

      [DefaultValue(typeof(InnerEnum), "Three")]
      public InnerEnum Nom { get; set; }
    }

    [DataContract]
    public class InnerSectionDataContract : ConfigurationSection
    {
      [DataMember]
      [DefaultValue(42)]
      public int Number { get; set; }

      [DefaultValue("Text unserializable!")]
      public string Text { get; set; }

      [DataMember]
      public InnerEnum Nom { get; set; }
    }

    [DataContract(Name="INNER_CONTRACT")]
    public class InnerSectionDataContractName : ConfigurationSection
    {
      [DataMember]
      public int Number { get; set; }

      [DataMember(Name="TEXT")]
      public string Text { get; set; }

      [DataMember]
      public InnerEnum Nom { get; set; }
    }

    [XmlRoot]
    public class InnerSectionXml : ConfigurationSection
    {
      public int Number { get; set; }

      [DefaultValue("Valhalla")]
      public string Text { get; set; }

      public InnerEnum Nom { get; set; }
    }

    [XmlRoot(ElementName="XML_NAME")]
    public class InnerSectionXmlName : ConfigurationSection
    {
      public int Number { get; set; }

      [DefaultValue("Valhalla")]
      public string Text { get; set; }

      public InnerEnum Nom { get; set; }
    }    
    public Form1()
    {
      InitializeComponent();
      
      innerDefault.SelectedObject = AppManager.Configurator.GetSection<InnerSectionDefault>();
      innerContract.SelectedObject = AppManager.Configurator.GetSection<InnerSectionDataContract>();
      innerContractName.SelectedObject = AppManager.Configurator.GetSection<InnerSectionDataContractName>();
      innerXml.SelectedObject = AppManager.Configurator.GetSection<InnerSectionXml>();
      innerXmlName.SelectedObject = AppManager.Configurator.GetSection<InnerSectionXmlName>();
      
      outerDefault.SelectedObject = AppManager.Configurator.GetSection<OuterSectionDefault>();
      outerContract.SelectedObject = AppManager.Configurator.GetSection<OuterSectionDataContract>();
      outerContractName.SelectedObject = AppManager.Configurator.GetSection<OuterSectionDataContractName>();
      outerXml.SelectedObject = AppManager.Configurator.GetSection<OuterSectionXml>();
      outerXmlName.SelectedObject = AppManager.Configurator.GetSection<OuterSectionXmlName>();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
      base.OnFormClosed(e);
      AppManager.Configurator.SaveSettings();
    }
  }
}
