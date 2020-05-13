using System;
using System.Configuration;
using System.IO;
using Notung.Log;
using Notung.Properties;

namespace Notung
{
  // This class allows you to handle specific events on the settings class:
  //  The SettingChanging event is raised before a setting's value is changed.
  //  The PropertyChanged event is raised after a setting's value is changed.
  //  The SettingsLoaded event is raised after the setting values are loaded.
  //  The SettingsSaving event is raised before the setting values are saved.
  internal sealed partial class LogSettings
  {
    private LogStringBuilder m_builder;

    public void BuildString(TextWriter writer, LoggingEvent data)
    {
      if (m_builder == null)
        m_builder = new LogStringBuilder(MessageTemplate);

      m_builder.BuildString(writer, data);
    }

    protected override void OnSettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
    {
      base.OnSettingsLoaded(sender, e);

      if (string.IsNullOrWhiteSpace(this.DefaultLogger))
        throw new ConfigurationErrorsException(Resources.EMPTY_DEFAULT_LOGGER);
    }
  }
}
