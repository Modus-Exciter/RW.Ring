using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml.Serialization;
using Notung;
using Notung.Configuration;
using Notung.Data;
using Notung.Helm;
using Notung.Loader;

namespace ConfiguratorGraphicalTest
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static int Main()
    {
      ApplicationStarter.ShowSplashScreen("splashscreen.jpg");

      ApplicationStarter starter = new ApplicationStarter(
        new DeferredFactory<Form>("ConfiguratorGraphicalTest", "ConfiguratorGraphicalTest.Form1"),
        Factory.Default<ILoadingQueue, TestLoadingQueue>()) { AllowOnlyOneInstance = true };



      return starter.RunApplication();
    }
  }

  class A
  {
    public B Bp { get; set; }
  }

  class B
  {
    public A AP { get; set; }
  }


  public class TestLoadingQueue : ILoadingQueue
  {
    public IApplicationLoader[] GetLoaders()
    {
      return new IApplicationLoader[] { new LongApplicationLoader(),
      new ApplicationLoader<A, A>(),
      new ApplicationLoader<B, B>()
      };
    }

    private class LongApplicationLoader : IApplicationLoader
    {
      public Type Key
      {
        get { return typeof(HelpItem); }
      }

      public ICollection<Type> Dependencies
      {
        get { return ArrayExtensions.Empty<Type>(); }
      }

      public void Setup(LoadingContext context) { }

      public bool Load(LoadingContext context)
      {
        Report(context, 0, "Form1.InnerSectionDefault");
        AppManager.Configurator.GetSection<Form1.InnerSectionDefault>();

        Report(context, 10, "Form1.InnerSectionDataContract");
        AppManager.Configurator.GetSection<Form1.InnerSectionDataContract>();

        Report(context, 20, "Form1.InnerSectionDataContractName");
        AppManager.Configurator.GetSection<Form1.InnerSectionDataContractName>();

        Report(context, 30, "Form1.InnerSectionXml");
        AppManager.Configurator.GetSection<Form1.InnerSectionXml>();

        Report(context, 40, "Form1.InnerSectionXmlName");
        AppManager.Configurator.GetSection<Form1.InnerSectionXmlName>();

        Report(context, 50, "OuterSectionDefault");
        AppManager.Configurator.GetSection<OuterSectionDefault>();

        Report(context, 60, "OuterSectionDataContract");
        AppManager.Configurator.GetSection<OuterSectionDataContract>();
        
        Report(context, 70, "OuterSectionDataContractName");
        AppManager.Configurator.GetSection<OuterSectionDataContractName>();
        
        Report(context, 80, "OuterSectionXml");
        AppManager.Configurator.GetSection<OuterSectionXml>();
        
        Report(context, 90, "OuterSectionXmlName");
        AppManager.Configurator.GetSection<OuterSectionXmlName>();

        return true;
      }

      private DateTime m_current;

      private void Report(LoadingContext context, int start, string section)
      {
        if (start == 0)
          m_current = DateTime.Now;
        
        context.Indicator.ReportProgress(start, string.Format("Loading section {0}...", section));
        System.Threading.Thread.Sleep(DateTime.Now - m_current);
        context.Indicator.ReportProgress(start + 5, string.Format("Loading section {0}...", section));
        m_current = DateTime.Now;
      }
    }
  }

  public enum OuterEnum
  {
    One1, Two2, Three3
  }

  public class OuterSectionDefault : ConfigurationSection
  {
    public string[] Lines{get; set;}
        
    [XmlAttribute]
    public int Number { get; set; }

    [XmlAttribute]
    public string Text { get; set; }

    [DefaultValue(typeof(OuterEnum), "Two2")]
    [XmlAttribute]
    public OuterEnum Nom { get; set; }
  }

  public class HelpItem
  {
    public string Link { get; set; }

    public DateTime Date { get; set; }
  }

  [DataContract]
  public class OuterSectionDataContract : ConfigurationSection
  {
    [DataMember]
    private readonly List<HelpItem> m_items = new List<HelpItem>();

    public List<HelpItem> Items
    {
      get { return m_items; }
    }
    
    [DataMember]
    [DefaultValue(42)]
    public int Number { get; set; }

    [DefaultValue("Text unserializable!")]
    public string Text { get; set; }

    [DataMember]
    public OuterEnum Nom { get; set; }

    public override bool Validate(InfoBuffer buffer)
    {
      System.Threading.Thread.Sleep(700);
      
      if (this.Number < 1)
      {
        buffer.Add("Number must be more than 1", InfoLevel.Warning);
        return false;
      }

      return true;
    }
  }

  [DataContract(Name = "Outer_CONTRACT")]
  public class OuterSectionDataContractName : ConfigurationSection
  {
    [DataMember(Name="Items")]
    private readonly List<HelpItem> m_items = new List<HelpItem>();
    [DataMember(Name = "Map")]
    private readonly Dictionary<string, int> m_dic = new Dictionary<string, int>();

    public List<HelpItem> Items
    {
      get { return m_items; }
    }

    public Dictionary<string, int> Map
    {
      get { return m_dic; }
    }

    [DataMember]
    public int Number { get; set; }

    [DataMember(Name = "TEXT")]
    public string Text { get; set; }

    [DataMember]
    public OuterEnum Nom { get; set; }
  }

  [XmlRoot]
  public class OuterSectionXml : ConfigurationSection
  {
    private readonly List<HelpItem> m_items = new List<HelpItem>();

    public List<HelpItem> Items
    {
      get { return m_items; }
    }
    
    public int Number { get; set; }

    [DefaultValue("Valhalla")]
    public string Text { get; set; }

    public OuterEnum Nom { get; set; }
  }

  [XmlRoot(ElementName = "XML_NAME_OUT")]
  public class OuterSectionXmlName : ConfigurationSection
  {
    private readonly List<HelpItem> m_items = new List<HelpItem>();

    public List<HelpItem> Items
    {
      get { return m_items; }
    }
    
    public int Number { get; set; }

    [DefaultValue("Valhalla")]
    public string Text { get; set; }

    public OuterEnum Nom { get; set; }
  }    
}
