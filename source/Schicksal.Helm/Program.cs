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
      return new ApplicationStarter(
        Factory.Default<Form, MainForm>())
        {
          AllowOnlyOneInstance = true
        }.RunApplication();
    }

    [DataContract]
    public class Preferences : ConfigurationSection
    {
      [DataMember(Name = "LastFiles")]
      private readonly HashSet<string> m_last_files = new HashSet<string>();

      public HashSet<string> LastFiles
      {
        get { return m_last_files; }
      }
    }
  }
}
