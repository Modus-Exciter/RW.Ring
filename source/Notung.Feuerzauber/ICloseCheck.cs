using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

namespace Notung.Feuerzauber
{
  public interface ICloseCheck
  {
    bool CheckBeforeClose();
  }

  public class CollectionRemoveCommand<T> : ICommand
  {
    private readonly ICollection<T> m_collection;

    public CollectionRemoveCommand(ICollection<T> collection)
    {
      Debug.Assert(collection != null, "Value cannot be null. Parameter name: collection");

      m_collection = collection;
    }

    public virtual bool CanExecute(object parameter)
    {
      return m_collection.Count > 0 && parameter is T;
    }

    protected void OnCanExecuteChanged(EventArgs e)
    {
      if (this.CanExecuteChanged != null)
        this.CanExecuteChanged(this, e ?? EventArgs.Empty);
    }

    public event EventHandler CanExecuteChanged;

    public virtual void Execute(object parameter)
    {
      m_collection.Remove((T)parameter);
    }
  }
}
