using System;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Notung.ComponentModel
{
  public abstract class ObservableObject : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
      var handler = this.PropertyChanged;

      if (handler != null)
        handler(this, new PropertyChangedEventArgs(propertyName));
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
