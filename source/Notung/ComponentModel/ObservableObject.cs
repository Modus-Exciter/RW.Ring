using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;

namespace Notung.ComponentModel
{
  public abstract class ObservableObject : INotifyPropertyChanged
  {
    private static readonly ConcurrentDictionary<string, PropertyChangedEventArgs> _property_cache =
      new ConcurrentDictionary<string, PropertyChangedEventArgs>();

    private static readonly ConcurrentDictionary<Type, EventArgsCreator> _type_cache =
      new ConcurrentDictionary<Type, EventArgsCreator>();

    private static readonly Func<Type, EventArgsCreator> _creator =
      type => new EventArgsCreator(type);

    private static readonly PropertyChangedEventArgs _all_properties =
      new PropertyChangedEventArgs(null);

    private readonly EventArgsCreator m_creator;

    protected ObservableObject()
    {
      m_creator = _type_cache.GetOrAdd(this.GetType(), _creator);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
      if (!string.IsNullOrEmpty(propertyName))
      {
        this.PropertyChanged.InvokeIfSubscribed(this,
          _property_cache.GetOrAdd(propertyName, m_creator.Create));
      }
      else
        this.PropertyChanged.InvokeIfSubscribed(this, _all_properties);
    }

    protected void ChangeValue<T>(ref T field, T value, string propertyName)
    {
      if (typeof(T).IsValueType)
      {
        if (field.Equals(value))
          return;
      }
      else if (object.Equals(field, value))
        return;

      field = value;

      this.OnPropertyChanged(propertyName);
    }

    private class EventArgsCreator
    {
      private readonly PropertyDescriptorCollection m_properties;
      public readonly Func<string, PropertyChangedEventArgs> Create;

      public EventArgsCreator(Type type)
      {
        m_properties = TypeDescriptor.GetProperties(type);
        Create = propertyName =>
        {
          Debug.Assert(m_properties[propertyName] != null,
            string.Format("Property {0} not found", propertyName));

          return new PropertyChangedEventArgs(propertyName);
        };
      }
    }
  }
}