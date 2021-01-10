using System.Configuration;
using Notung.Properties;

namespace Notung.Logging
{
  // This class allows you to handle specific events on the settings class:
  //  The SettingChanging event is raised before a setting's value is changed.
  //  The PropertyChanged event is raised after a setting's value is changed.
  //  The SettingsLoaded event is raised after the setting values are loaded.
  //  The SettingsSaving event is raised before the setting values are saved.
  internal sealed partial class LogSettings
  {
    protected override void OnSettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
    {
      base.OnSettingsLoaded(sender, e);

      if (string.IsNullOrWhiteSpace(this.DefaultLogger))
        throw new ConfigurationErrorsException(Resources.EMPTY_DEFAULT_LOGGER);
    }
  }
}
