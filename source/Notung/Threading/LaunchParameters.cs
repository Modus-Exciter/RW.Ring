using System;
using System.ComponentModel;
using System.Drawing;
using Notung.ComponentModel;

namespace Notung.Threading
{
  /// <summary>
  /// Настройки отображения задачи с индикатором прогресса
  /// </summary>
  [Serializable]
  public sealed class LaunchParameters 
  {
    /// <summary>
    /// Инициализация нового набора настроек отображения задачи с индикатором прогресса
    /// </summary>
    public LaunchParameters()
    {
      this.CloseOnFinish = true;
    }

    /// <summary>
    /// Заголовок задачи
    /// </summary>
    public string Caption { get; set; }

    /// <summary>
    /// Картинка, отображающаяся при выполнении задачи
    /// </summary>
    public Bitmap Bitmap { get; set; }

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

    /// <summary>
    /// Строковое представление настроек задачи
    /// </summary>
    /// <returns>Заголовок задачи, если задан</returns>
    public override string ToString()
    {
      return !string.IsNullOrWhiteSpace(this.Caption) ? this.Caption : base.ToString();
    }

    #region Implementation

    internal void Setup(IRunBase work)
    {
#if DOMAIN_TASK
      if (work is RunBaseProxyWrapper)
      {
        var proxy = (RunBaseProxyWrapper)work;

        if (string.IsNullOrWhiteSpace(this.Caption))
          this.Caption = proxy.Caption;

        this.SupportsPercentNotification = proxy.SupportsPercentNotification;
      }
      else
#endif
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
