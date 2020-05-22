using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml.Serialization;
using Notung;
using Notung.Configuration;
using Notung.Helm;
using Notung.Threading;
using System;
using ConfiguratorGraphicalTest.Properties;
using Notung.Logging;

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

      [XmlAttribute]
      [DefaultValue(KnownColor.Transparent)]
      public KnownColor Colorite { get; set; }
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

      [DataMember]
      [DefaultValue(typeof(Color), "Red")]
      public Color Colorite { get; set; }
    }

    [DataContract(Name = "INNER_CONTRACT")]
    public class InnerSectionDataContractName : ConfigurationSection
    {
      [DataMember]
      public int Number { get; set; }

      [DataMember(Name = "TEXT")]
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

    [XmlRoot(ElementName = "XML_NAME")]
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
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      if (AppManager.Instance.CommandLineArgs.Count != 0)
        this.Text = string.Join(" ", AppManager.Instance.CommandLineArgs);
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

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

    private void button1_Click(object sender, System.EventArgs e)
    {
      AppManager.Instance.Restart();
    }

    protected override void WndProc(ref Message msg)
    {
      base.WndProc(ref msg);

      if (msg.Msg == WinAPIHelper.WM_COPYDATA)
      {
        string[] fileNames = MainFormView.GetStringArgs(msg);
        this.Text = string.Join(" ", fileNames);
        msg.Result = new System.IntPtr(1);
      }
    }

    private void buttonDLL_Click(object sender, System.EventArgs e)
    {
      using (var dlg = new OpenFileDialog())
      {
        dlg.Filter = "Dll files|*.dll";

        if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
        {
          using (var dll = new NativeDll(dlg.FileName))
          {
            this.Text = string.Join(", ", dll.GetExportList());
          }
        }
      }

      throw new ArgumentNullException("LALA");
    }

    private void buttonOpenFolder_Click(object sender, System.EventArgs e)
    {
#if APPLICATION_INFO
      System.Diagnostics.Process.Start(ApplicationInfo.Instance.GetWorkingPath()); 
#endif
    }

    private void button2_Click(object sender, System.EventArgs e)
    {
      AppManager.OperationLauncher.SyncWaitingTime = TimeSpan.FromSeconds(0.2);
#if MULTI_DOMAIN
      var domain = AppDomain.CreateDomain("Parallel");
      domain.ShareServices();

      var wrk = (RunWorkMain)domain.CreateInstanceAndUnwrap(
        typeof(Program).Assembly.FullName, typeof(RunWorkMain).FullName);

      wrk.Run();

      AppDomain.Unload(domain);
#else
      var wrk = new TestWork();
      AppManager.OperationLauncher.Run(wrk, new LaunchParameters
      { 
        Bitmap = Resources.DOS_TRACK,
        CloseOnFinish = false
      });
#endif
    }

    private class RunWorkMain : MarshalByRefObject
    {
      public void Run()
      {
        AppManager.OperationLauncher.Run(new TestWork(), new LaunchParameters 
        { 
          Bitmap = Resources.DOS_TRACK,
          CloseOnFinish = false
        });
      }
    }

    [PercentNotification]
    [DisplayName("Tesovo worka")]
    private class TestWork : CancelableRunBase, IServiceProvider
    {
       private bool m_ok = false;

      private static readonly ILog _log = LogManager.GetLogger(typeof(TestWork));
      
      public override void Run()
      {
        this.CanCancel = false;
        this.ReportProgress("Some state");
        for (int i = 1; i <= 100; i++)
        {
          this.ReportProgress(i, string.Format("Some state {0}", i / 11));
          System.Threading.Thread.Sleep(50);

          if (i == 50)
            this.CanCancel = true;
          else if (i == 80)
          {
            this.CanCancel = false;
          }
          if (i % 10 == 0)
            _log.DebugFormat("Message from task {0}", i / 10);

          this.CancellationToken.ThrowIfCancellationRequested();
        }
        m_ok = true;
        _log.Debug("ALL RIGHT!");
      }

      //public override string ToString()
      //{
      //  return "Gotterdammerung";
      //}

      public override object GetService(Type serviceType)
      {
        if (serviceType == typeof(InfoBuffer) && !m_ok)
          return new InfoBuffer
          {
            new Info("Good message", InfoLevel.Info),
            new Info("Bad message" ,InfoLevel.Warning)
          };
        else
          return null;
      }
    }
  }
}