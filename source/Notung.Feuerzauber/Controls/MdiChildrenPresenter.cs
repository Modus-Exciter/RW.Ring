using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
namespace Notung.Feuerzauber.Controls
{
  public class MdiChildrenPresenter : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    private readonly ObservableCollection<MdiChild> m_children = new ObservableCollection<MdiChild>();
    private readonly MdiChildCloseComand m_close_command;
    private int m_active_child = -1;

    public MdiChildrenPresenter()
    {
      m_children.CollectionChanged += this.HandleCollectionChanged;
      m_close_command = new MdiChildCloseComand(m_children);
    }

    public ObservableCollection<MdiChild> MdiChildren
    {
      get { return m_children; }
    }

    public MdiChild ActiveMdiChild
    {
      get
      {
        if (m_active_child >= 0 && m_active_child < m_children.Count)
          return m_children[m_active_child];
        else
          return null;
      }
      set
      {
        if (value != null)
          m_active_child = m_children.IndexOf(value);
        else
          m_active_child = -1;

        OnPropertyChanged("ActiveMdiChild");
      }
    }

    public ICommand CloseMdiChild
    {
      get { return m_close_command; }
    }

    public void ActivateWindow(Func<MdiChild> factory, Func<MdiChild, bool> predicate = null)
    {
      Debug.Assert(factory != null, "Factory to create MDI child cannot be null");

      if (predicate != null)
      {
        foreach (var item in this.MdiChildren)
        {
          if (predicate(item))
          {
            this.ActiveMdiChild = item;
            return;
          }
        }
      }

      var child = factory();

      if (child != null)
        this.MdiChildren.Add(child);
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetActiveChild()
    {
      OnPropertyChanged("ActiveMdiChild");
    }

    private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Add)
      {
        m_active_child = e.NewStartingIndex;

        if (m_children.Count == 1)
        {
          ThreadPool.QueueUserWorkItem(state =>
          {
            ((Dispatcher)state).BeginInvoke(new Action(this.SetActiveChild));
          }, Dispatcher.CurrentDispatcher);
        }
        else
          SetActiveChild();
      }
      else if (e.Action == NotifyCollectionChangedAction.Remove)
      {
        if (e.OldStartingIndex <= m_active_child)
        {
          if (e.OldStartingIndex < m_active_child || e.OldStartingIndex == m_children.Count)
            m_active_child--;

          if (m_active_child < 0 && m_children.Count > 0)
            m_active_child = 0;

          OnPropertyChanged("ActiveMdiChild");
        }
      }
    }

    private class MdiChildCloseComand : CollectionRemoveCommand<MdiChild>
    {
      public MdiChildCloseComand(ICollection<MdiChild> collection) : base(collection) { }

      public override bool CanExecute(object parameter)
      {
        return base.CanExecute(parameter) && ((MdiChild)parameter).CanClose;
      }

      public override void Execute(object parameter)
      {
        if (parameter is ICloseCheck && !((ICloseCheck)parameter).CheckBeforeClose())
          return;

        base.Execute(parameter);
      }
    }
  }

  public class MdiChild : INotifyPropertyChanged
  {
    private Control m_control;
    private string m_caption;
    private Bitmap m_icon;
    private object m_tag;
    private bool m_can_close = true;

    public MdiChild(Control control, string caption = null, Bitmap icon = null)
    {
      Debug.Assert(control != null, "Value cannot be null. Parameter name: control");

      m_control = control;
      m_caption = caption ?? control.ToString();
      m_icon = icon;
    }

    public Control Control
    {
      get { return m_control; }
    }

    public string Caption
    {
      get { return m_caption; }
      set
      {
        if (object.Equals(m_caption, value))
          return;

        m_caption = value;
        OnPropertyChanged("Caption");
      }
    }

    public Bitmap Icon
    {
      get { return m_icon; }
      set
      {
        if (object.Equals(m_icon, value))
          return;

        m_icon = value;
        OnPropertyChanged("Icon");
      }
    }

    public object Tag
    {
      get { return m_tag; }
      set
      {
        if (object.Equals(m_icon, value))
          return;

        m_tag = value;
        OnPropertyChanged("Tag");
      }
    }

    public bool CanClose
    {
      get { return m_can_close; }
      set
      {
        if (m_can_close == value)
          return;

        m_can_close = value;
        OnPropertyChanged("CanClose");
      }
    }

    public override string ToString()
    {
      return this.Caption ?? base.ToString();
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
  }
}
