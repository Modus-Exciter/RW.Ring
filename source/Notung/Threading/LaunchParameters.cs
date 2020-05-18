using System;
using System.ComponentModel;
using System.Drawing;
using Notung.ComponentModel;

namespace Notung.Threading
{
  public sealed class LaunchParameters : MarshalByRefObject, INotifyPropertyChanged
  {
    private readonly IRunBase m_work;

    private string m_caption;
    private Image m_bitmap;
    private bool m_close_on_finish;
    private bool m_supports_cancellation;
    private bool m_can_cancel;
    private bool m_percent_notification;

    public LaunchParameters(IRunBase work = null)
    {
      m_work = work;
      m_close_on_finish = true;

      if (m_work == null)
      {
        m_caption = "";
        return;
      }

      m_caption = GetDefaultCaption();
      m_percent_notification = m_work.GetType().IsDefined(typeof(PercentNotificationAttribute), false);

      var cancelable = m_work as ICancelableRunBase;

      if (cancelable != null)
      {
        m_supports_cancellation = true;
        m_can_cancel = cancelable.CanCancel;
        cancelable.CanCancelChanged += this.HandleCanCancelChanged;
      }
    }
    
    public string Caption
    {
      get { return m_caption; }
      set
      {
        m_caption = value;
        this.OnPropertyChanged("Caption");
      }
    }

    public Image Bitmap
    {
      get { return m_bitmap; }
      set
      {
        m_bitmap = value;
        this.OnPropertyChanged("Bitmap");
      }
    }

    public bool CloseOnFinish
    {
      get { return m_close_on_finish; }
      set
      {
        m_close_on_finish = value;
        this.OnPropertyChanged("CloseOnFinish");
      }
    }

    public bool SupportsCancellation
    {
      get { return m_supports_cancellation; }
      set
      {
        m_supports_cancellation = value;
        this.OnPropertyChanged("SupportsCancellation");
      }
    }

    public bool CanCancel
    {
      get { return m_can_cancel; }
      set
      {
        m_can_cancel = value;
        this.OnPropertyChanged("CanCancel");
      }
    }

    public bool SupportsPercentNotification
    {
      get { return m_percent_notification; }
      set
      {
        m_percent_notification = value;
        this.OnPropertyChanged("SupportsPercentNotification");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public override string ToString()
    {
      return !string.IsNullOrWhiteSpace(m_caption) ? m_caption : base.ToString();
    }

    #region Implementation

    internal void Finish()
    {
      var cancelable = m_work as ICancelableRunBase;

      if (cancelable != null) 
        cancelable.CanCancelChanged -= this.HandleCanCancelChanged;
    }

    private string GetDefaultCaption()
    {
      var ret = m_work.ToString();

      if (!object.Equals(this.Caption, m_work.GetType().ToString()))
      {
        var dn = m_work.GetType().GetCustomAttribute<DisplayNameAttribute>(true);

        if (dn != null && !string.IsNullOrWhiteSpace(dn.DisplayName))
          ret = dn.DisplayName;
      }

      return ret;
    }

    private void HandleCanCancelChanged(object sender, EventArgs e)
    {
      this.CanCancel = ((ICancelableRunBase)m_work).CanCancel;
    }

    private void OnPropertyChanged(string propertyName)
    {
      this.PropertyChanged.InvokeSynchronized(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
  }
}