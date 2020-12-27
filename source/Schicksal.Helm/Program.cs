using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Notung.Configuration;
using Notung.Helm;
using Notung.Loader;

namespace Schicksal.Helm
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

      return RunApplication();
    }

    static int RunApplication()
    {
      return new ApplicationStarter(Factory.Default<Form, MainForm>(), 
        Factory.Default<ILoadingQueue, SchicksalLoadingQueue>())
        {
          AllowOnlyOneInstance = true
        }.RunApplication();
    }

    [DataContract]
    public class Preferences : ConfigurationSection
    {
      [DataMember(Name = "LastFiles")]
      private readonly Dictionary<string, DateTime> m_last_files = new Dictionary<string, DateTime>();
      [DataMember(Name = "AnovaSettings")]
      private Dictionary<string, string[]> m_anova_settings = new Dictionary<string, string[]>();

      public Dictionary<string, DateTime> LastFiles
      {
        get { return m_last_files; }
      }

      public Dictionary<string, string[]> AnovaSettings
      {
        get
        {
          if (m_anova_settings == null)
            m_anova_settings = new Dictionary<string, string[]>();

          return m_anova_settings;
        }
      }
    }
  }
}
