using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml.Serialization;
using ConfiguratorGraphicalTest.Properties;
using Notung;
using Notung.ComponentModel;
using Notung.Configuration;
using Notung.Helm;
using Notung.Helm.Windows;
using Notung.Logging;
using Notung.Services;
using Notung.Helm.Dialogs;

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
          var export_list = NativeDll.GetExportList(dlg.FileName);

          if (export_list != null && export_list.Length > 0)
          {
            using (var selector = new SelectFunctionDialog(export_list))
            {
              if (selector.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
              {
                using (var dll = new NativeDll(dlg.FileName))
                {
                  var work = new ExternalCallWork(dll, selector.SelectedItem);
                  AppManager.OperationLauncher.Run(work);
                }
              }
            }
          }
        }
      }
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

    [DisplayName("Tesovo worka")]
    private class TestWork : CancelableRunBase, IServiceProvider
    {
       private bool m_ok = false;

      private static readonly ILog _log = LogManager.GetLogger(typeof(TestWork));
      
      public override void Run()
      {
        this.CanCancel = false;
        this.ReportProgress(string.Format(Resources.SOME_STATE, ""));
        for (int i = 1; i <= 100; i++)
        {
          this.ReportProgress(i, string.Format(Resources.SOME_STATE, i / 11));
          System.Threading.Thread.Sleep(50);

          if (i == 50)
            this.CanCancel = true;
          else if (i == 80)
          {
            this.CanCancel = false;
          }
          if (i % 10 == 0)
            _log.DebugFormat(Resources.MESSAGE, i / 10);

          this.CancellationToken.ThrowIfCancellationRequested();
        }
        m_ok = true;
        _log.Debug(Resources.ALL_RIGHT);
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

    private void languageSwitcher_LanguageChanged(object sender, Notung.ComponentModel.LanguageEventArgs e)
    {
      buttonDLL.Text = Resources.DLL;
      buttonOpenFolder.Text = Resources.OPEN_FOLDER;
      buttonRestart.Text = Resources.RESTART;
      buttonWork.Text = Resources.BACKGROUND;
      m_settings_button.Text = Resources.SETTINGS;
      buttonInfoBufferView.Text = Resources.INFO_VIEW;
    }

    private void comboBoxLang_SelectedIndexChanged(object sender, EventArgs e)
    {
      LanguageSwitcher.Switch(comboBoxLang.Text);
    }

    private void m_settings_button_Click(object sender, EventArgs e)
    {
      using (var dlg = new SettingsDialog())
      {
        dlg.ShowDialog(this);
      }
    }

    private void buttonInfoBufferView_Click(object sender, EventArgs e)
    {
      AppManager.Notificator.Show(new InfoBuffer
          {
            new Info("Good message", InfoLevel.Info),
            new Info("Bad message" ,InfoLevel.Warning)
          }, "aaaaaaaaaaaaa aaaaaaaaaaaaaa aaaaaa aaaaaaaaaaaaaaa aaaaaaaaaaaaaa aaaaaaaaaaaaaaaaaaaaa bbbbbbbbbbbbbbb   bbbbbbbbbbbbbbbbbbbbbb bbbbbbbbbbbbbbbbb        bbbbbbbbbbb   aaaa aaaaaaaaaaaa aaaaaaaa aaaaaaaaaaaaaa aaaaaaaaaaaa aaaaaaaa a aaaaaaaaaa aaaaaaaaa aaaaaa aaaaaa aaa nnnnnnnnnn");

    }
  }
}