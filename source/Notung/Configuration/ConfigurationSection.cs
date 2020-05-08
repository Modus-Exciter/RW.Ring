using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Notung.Configuration
{
  [Serializable]
  [DataContract]
  public abstract class ConfigurationSection : IValidator
  {
    protected ConfigurationSection()
    {
      LoadDefaults();
    }

    [OnDeserializing]
    private void OnDeserializing(StreamingContext context)
    {
      try
      {
        LoadDefaults();
      }
      catch
      {
        // TODO: log error
      }
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      try
      {
        // TODO: do something with InfoBuffer
        ValidateAndRepair(new InfoBuffer());
      }
      catch
      {
        // TODO: log error
      }    
    }

    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      BeforeSave();
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
            LoadDefaults(nestedComponent);
        }
      }
    }

    private bool ValidateAndRepair(InfoBuffer buffer)
    {
      bool ret = true;

      try
      {
        ret = Validate(buffer);
      }
      catch
      {
        ret = false;
        // TODO: log error
      }

      if (!ret)
      {
        // TODO: log repairing defaults
        ret = Repair(buffer);
      }

      return true;
    }

    public void RestoreDefaults()
    {
      // TODO: log restoring defaults
      LoadDefaults(this);
    }

    public virtual void LoadDefault(string propertyName)
    {
      PropertyDescriptor pd = TypeDescriptor.GetProperties(this)[propertyName];

      if (pd != null)
      {
        var def = pd.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;

        if (def != null)
        {
          pd.SetValue(this, def.Value);
        }
      }
    }

    protected virtual void LoadDefaults()
    {
      LoadDefaults(this);
    }

    protected virtual bool Repair(InfoBuffer buffer)
    {
      return false;
    }

    protected virtual void BeforeSave() { }

    public virtual bool Validate(InfoBuffer buffer)
    {
      return true;
    }

    public virtual void ApplySettings() { }
  }
}
