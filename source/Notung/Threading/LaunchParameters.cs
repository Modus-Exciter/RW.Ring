using System;
using System.ComponentModel;
using System.Drawing;
using Notung.ComponentModel;
using System.IO;
using System.Drawing.Imaging;

namespace Notung.Threading
{
  public sealed class LaunchParameters : MarshalByRefObject, INotifyPropertyChanged
  {
    private string m_caption;
    private Bitmap m_bitmap;
    private bool m_close_on_finish = true;
    private bool m_cancelable;
    private bool m_can_cancel;
    private bool m_percent_notification;

    internal void Setup(IRunBase work)
    {
      if (work is RunBaseProxyWrapper)
      {
        var proxy = (RunBaseProxyWrapper)work;
        m_percent_notification = proxy.SupportPercentNotification;
        m_caption = proxy.ToString();
      }
      else
      {
        if (string.IsNullOrWhiteSpace(m_caption))
          m_caption = GetDefaultCaption(work);
        
        m_percent_notification = work.GetType().IsDefined(typeof(PercentNotificationAttribute), false);
      }

      m_cancelable = m_can_cancel = work is ICancelableRunBase;
    }
    
    public string Caption
    {
      get { return m_caption; }
      set
      {
        if (string.IsNullOrEmpty(value))
          return;
        
        if (m_caption == value)
          return;

        m_caption = value;
        this.OnPropertyChanged("Caption");
      }
    }

    public Bitmap Bitmap
    {
      get { return m_bitmap; }
      set
      {
        if (m_bitmap == value)
          return;

        m_bitmap = value;
        this.OnPropertyChanged("Bitmap");
      }
    }

    public bool CloseOnFinish
    {
      get { return m_close_on_finish; }
      set
      {
        if (m_close_on_finish == value)
          return;

        m_close_on_finish = value;
        this.OnPropertyChanged("CloseOnFinish");
      }
    }

    public bool CanCancel
    {
      get { return m_can_cancel; }
      set
      {
        if (m_can_cancel == value)
          return;

        m_can_cancel = value;
        this.OnPropertyChanged("CanCancel");
      }
    }

    public bool SupportsPercentNotification
    {
      get { return m_percent_notification; }
      internal set { m_percent_notification = value; }
    }

    public bool SupportsCancellation
    {
      get { return m_cancelable; }
      internal set { m_cancelable = value; }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void ShowCurrentSettings()
    {
      this.OnPropertyChanged(null); 
    }

    public override string ToString()
    {
      return !string.IsNullOrWhiteSpace(m_caption) ? m_caption : base.ToString();
    }

    #region Implementation

    private string GetDefaultCaption(IRunBase work)
    {
      var ret = work.ToString();

      if (object.Equals(ret, work.GetType().ToString()))
      {
        var dn = work.GetType().GetCustomAttribute<DisplayNameAttribute>(true);

        if (dn != null && !string.IsNullOrWhiteSpace(dn.DisplayName))
          ret = dn.DisplayName;
      }

      return ret;
    }

    private void OnPropertyChanged(string propertyName)
    {
      this.PropertyChanged.InvokeSynchronized(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
  }
}