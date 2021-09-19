using System;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace Notung.ComponentModel
{
  public abstract class ObservableObject : INotifyPropertyChanged
  {
    private static readonly ConcurrentDictionary<string, PropertyChangedEventArgs> _property_hash =
      new ConcurrentDictionary<string, PropertyChangedEventArgs>();

    private static readonly Func<string, PropertyChangedEventArgs> _creator =
      name => new PropertyChangedEventArgs(name);

    private static readonly PropertyChangedEventArgs _all_properties =
      new PropertyChangedEventArgs(null);
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
      if (string.IsNullOrEmpty(propertyName))
        this.PropertyChanged.InvokeIfSubscribed(this, _all_properties);
      else
        this.PropertyChanged.InvokeIfSubscribed(this,
          _property_hash.GetOrAdd(propertyName, _creator));
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
  }
}
