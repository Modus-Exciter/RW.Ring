using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;

namespace Notung.Threading
{
  /// <summary>
  /// Настройки отображения задачи с индикатором прогресса
  /// </summary>
  [Serializable]
  public sealed class LaunchParameters 
  {
    [NonSerialized]
    private Bitmap m_bitmap;
    private byte[] m_image_data;
    
    /// <summary>
    /// Инициализация нового набора настроек отображения задачи с индикатором прогресса
    /// </summary>
    public LaunchParameters()
    {
      this.CloseOnFinish = true;
    }

    [OnSerializing]
    void OnSerializing(StreamingContext context)
    {
      if (m_bitmap != null)
      {
        using (var ms = new MemoryStream())
        {
          m_bitmap.Save(ms, ImageFormat.Png);
          m_image_data = ms.ToArray();
        }
      }
    }

    [OnDeserialized]
    void OnDeserialized(StreamingContext context)
    {
      if (m_image_data != null)
      {
        using (var ms = new MemoryStream(m_image_data))
        {
          m_bitmap = (Bitmap)Image.FromStream(ms);
        }
      }
    }

    /// <summary>
    /// Заголовок задачи
    /// </summary>
    public string Caption { get; set; }

    /// <summary>
    /// Картинка, отображающаяся при выполнении задачи
    /// </summary>
    public Bitmap Bitmap
    {
      get { return m_bitmap; }
      set
      {
        m_bitmap = value;
        m_image_data = null;
      }
    }

    /// <summary>
    /// Закрывать ли диалог с прогрессом выполнения после завершения задачи
    /// </summary>
    public bool CloseOnFinish { get; set; }

    /// <summary>
    /// Поддерживается ли оповещение о прогрессе операции в процентах
    /// </summary>
    public bool SupportsPercentNotification { get; internal set; }

    /// <summary>
    /// Поддерживается ли отмена задачи
    /// </summary>
    public bool SupportsCancellation { get; internal set; }

    public override string ToString()
    {
      return !string.IsNullOrWhiteSpace(this.Caption) ? this.Caption : base.ToString();
    }

    #region Implementation

    internal void Setup(IRunBase work)
    {
      if (work is RunBaseProxyWrapper)
      {
        var proxy = (RunBaseProxyWrapper)work;

        if (string.IsNullOrWhiteSpace(this.Caption))
          this.Caption = proxy.Caption;

        this.SupportsPercentNotification = proxy.SupportsPercentNotification;
      }
      else
      {
        if (string.IsNullOrWhiteSpace(this.Caption))
          this.Caption = GetDefaultCaption(work);

        this.SupportsPercentNotification = work.GetType().IsDefined(typeof(PercentNotificationAttribute), false);
      }

      this.SupportsCancellation = work is ICancelableRunBase;
    }

    internal static string GetDefaultCaption(IRunBase work)
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

    #endregion
  }
}
