using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Notung.Logging;
using Notung.Properties;

namespace Notung.Configuration
{
  [Serializable]
  [DataContract]
  public abstract class ConfigurationSection : IValidator
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(ConfigurationSection));

    protected ConfigurationSection()
    {
      this.LoadDefaults(this);
    }

    [OnDeserializing]
    private void OnDeserializing(StreamingContext context)
    {
      try
      {
        this.LoadDefaults(this);
      }
      catch (Exception ex)
      {
        _log.Error("OnDeserializing: exception", ex);
      }
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      var buffer = new InfoBuffer();

      try
      {
        this.ValidateAndRepair(buffer);
      }
      catch (Exception ex)
      {
        buffer.Add(ex);
      }

      for (int i = 0; i < buffer.Count; i++)
        _log.Alert(buffer[i]);
    }

    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      this.BeforeSave();
    }

    private void LoadDefaults(object component)
    {
      foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(component))
      {
        if (pd.IsReadOnly)
          continue;

        var def = pd.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;

        if (def != null)
        {
          pd.SetValue(component, def.Value);
        }
        else if (pd.PropertyType.IsClass && pd.PropertyType != typeof(string))
        {
          var nestedComponent = pd.GetValue(component);

          if (nestedComponent != null)
            this.LoadDefaults(nestedComponent);
        }
      }
    }

    private bool ValidateAndRepair(InfoBuffer buffer)
    {
      bool ret;

      try
      {
        ret = this.QuickValidate(buffer);
      }
      catch (Exception ex)
      {
        ret = false;
        buffer.Add(ex);
        _log.Error("ValidateAndRepair: exception", ex);
      }

      if (!ret)
      {
        buffer.Add(Resources.SETTINGS_RESTORE, InfoLevel.Debug);
        ret = this.Repair(buffer);
      }

      return ret;
    }

    public void RestoreDefaults()
    {
      _log.Debug(Resources.SETTINGS_RESTORE);
      this.LoadDefaults(this);
    }

    public virtual void LoadDefault(string propertyName)
    {
      PropertyDescriptor pd = TypeDescriptor.GetProperties(this)[propertyName];

      if (pd != null)
      {
        var def = pd.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;

        if (def != null)
          pd.SetValue(this, def.Value);
      }
    }

    protected virtual bool Repair(InfoBuffer buffer)
    {
      return false;
    }

    protected virtual void BeforeSave() { }

    protected virtual bool QuickValidate(InfoBuffer buffer)
    {
      return true;
    }

    public virtual bool Validate(InfoBuffer buffer)
    {
      return this.QuickValidate(buffer);
    }

    public virtual void ApplySettings() { }

    public virtual void OnError(Exception error) { }
  }
}