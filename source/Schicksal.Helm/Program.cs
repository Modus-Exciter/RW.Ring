using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Notung.ComponentModel;
using Notung.Configuration;
using Notung.Helm;
using Notung.Loader;
using Notung.Services;
using Schicksal.Exchange;

namespace Schicksal.Helm
{
  static class Program
  {
    [STAThread]
    static int Main()
    {
      ApplicationStarter.ShowSplashScreen("splashscreen.png");

      return RunApplication();
    }

    private static int RunApplication()
    {
      return new ApplicationStarter(Factory.Default<Form, MainForm>(),
        Factory.Default<ILoadingQueue, SchicksalLoadingQueue>())
      {
        AllowOnlyOneInstance = true
      }.RunApplication();
    }

    private class SchicksalLoadingQueue : LoadingQueue
    {
      protected override void FillLoaders(Action<IApplicationLoader> add, Func<Type, bool> contains)
      {
        add(new PluginsApplicationLoader("Plugins"));
        add(new PluginsApplicationLoader<ITableImport>("*.import", LoadPluginsMode.CurrentDomain));
      }
    }

    [DataContract]
    public class Preferences : ConfigurationSection
    {
      [DataMember(Name = "LastFiles")]
      private readonly Dictionary<string, DateTime> m_last_files = new Dictionary<string, DateTime>();
      [DataMember(Name = "AnovaSettings")]
      private Dictionary<string, string[]> m_anova_settings = new Dictionary<string, string[]>();
      [DataMember(Name = "BaseStatSettings")]
      private Dictionary<string, string[]> m_bs_settings = new Dictionary<string, string[]>();

      public Dictionary<string, DateTime> LastFiles
      {
        get { return m_last_files; }
      }

      [DataMember]
      [DefaultValue("RU")]
      public string Language { get; set; }

      [DataMember]
      [DefaultValue(typeof(Color), "Red")]
      public Color SignificatColor { get; set; }

      [DataMember]
      [DefaultValue(typeof(Color), "Blue")]
      public Color ExclusiveColor { get; set; }

      public Dictionary<string, string[]> AnovaSettings
      {
        get
        {
          if (m_anova_settings == null)
            m_anova_settings = new Dictionary<string, string[]>();

          return m_anova_settings;
        }
      }

      public Dictionary<string, string[]> BaseStatSettings
      {
        get
        {
          if (m_bs_settings == null)
            m_bs_settings = new Dictionary<string, string[]>();

          return m_bs_settings;
        }
      }

      public override void ApplySettings()
      {
        if (!string.IsNullOrEmpty(this.Language))
          LanguageSwitcher.Switch(this.Language);
      }
    }
  }
}