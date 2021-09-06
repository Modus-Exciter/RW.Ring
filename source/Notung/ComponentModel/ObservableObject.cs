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
  }
}
