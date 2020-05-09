using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml.Serialization;
using Notung.Configuration;
using System.Windows;

namespace ConfiguratorGraphicalTest
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      SplashScreen scr = new SplashScreen("splashscreen.jpg");
      scr.Show(false);
      RunApp(scr);
    }

    private static void RunApp(SplashScreen scr)
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      var frm = new Form1();
      frm.Shown += (e, args) => scr.Close(new TimeSpan(0,0, 1));
      Application.Run(frm);
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
