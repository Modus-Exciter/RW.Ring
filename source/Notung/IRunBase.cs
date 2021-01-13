using System;
using System.ComponentModel;
using System.Threading;

namespace Notung
{
  /// <summary>
  /// Выполняемая задача с индикатором прогресса
  /// </summary>
  public interface IRunBase
  {
    /// <summary>
    /// Выполнить задачу
    /// </summary>
    void Run();

    /// <summary>
    /// Происходит при изменении прогресса выполнения задачи
    /// </summary>
    event ProgressChangedEventHandler ProgressChanged;
  }

  /// <summary>
  /// Выполняемая задача с индикатором проргресса и возможностью отмены
  /// </summary>
  public interface ICancelableRunBase : IRunBase
  {
    /// <summary>
    /// Токен отмены задачи
    /// </summary>
    CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// Возможно ли отменить задачу в текущий момент
    /// </summary>
    bool CanCancel { get; }

    /// <summary>
    /// Происходит, когда возможность отмены задачи меняется
    /// </summary>
    event EventHandler CanCancelChanged;
  }

  /// <summary>
  /// Управление отображением процента выполнения операции
  /// </summary>
  public static class ProgressPercentage
  {
    /// <summary>
    /// Минимальное значение процента выполнения (0 %)
    /// </summary>
    public const int Started = 0;

    /// <summary>
    /// Максимальное значение процента выполнения (100 %)
    /// </summary>
    public const int Completed = 100;

    /// <summary>
    /// Процент выполнения неизвестен
    /// </summary>
    public const int Unknown = -1;
  }

  /// <summary>
  /// Изменение параметров запуска задачи в ходе выполнения
  /// </summary>
  [Flags]
  public enum LaunchParametersChange : byte
  {
    /// <summary>
    /// Заголовок задачи
    /// </summary>
    Caption = 1,

    /// <summary>
    /// Изображение
    /// </summary>
    Image = 2
  }

  /// <summary>
  /// Базовая реализация интерфейса IRunBase
  /// </summary>
  public abstract class RunBase : IRunBase
  {
    private volatile int m_percent = ProgressPercentage.Unknown;
    private volatile object m_state;
    private readonly InfoBuffer m_infolog = new InfoBuffer();

    protected RunBase() { }

    /// <summary>
    /// Буфер, в который можно писать сообщения, накапливающиеся во время выполнения задачи
    /// </summary>
    protected InfoBuffer Infolog
    {
      get { return m_infolog; }
    }

    public abstract void Run();

    public event ProgressChangedEventHandler ProgressChanged;

    protected void ReportProgress(int percent, object state)
    {
      if (m_percent == percent && object.Equals(m_state, state))
        return;

      m_percent = percent;
      m_state = state;

      this.OnProgressChanged();
    }

    protected void ReportProgress(int percent)
    {
      if (m_percent == percent)
        return;

      m_percent = percent;

      this.OnProgressChanged();
    }

    protected void ReportProgress(object state)
    {
      if (object.Equals(m_state, state))
        return;

      m_state = state;

      this.OnProgressChanged();
    }

    private void OnProgressChanged()
    {
      var handler = this.ProgressChanged;

      if (handler != null)
        handler(this, new ProgressChangedEventArgs(m_percent, m_state));
    }

    public virtual object GetService(Type serviceType)
    {
      return serviceType == typeof(InfoBuffer) ? m_infolog : null;
    }

    /// <summary>
    /// Получение названия задачи по умолчанию
    /// </summary>
    /// <returns>Если у задачи переопределён метод ToString(), то его. Иначе, значение атрибута DisplayName</returns>
    public static string GetDefaultCaption(IRunBase work)
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
  }

  /// <summary>
  /// Базовая реализация интерфейса ICancelableRunBase
  /// </summary>
  public abstract class CancelableRunBase : RunBase, ICancelableRunBase
  {
    private volatile bool m_can_cancel = true;
    
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// Возможность отмены задачи. Текущая задача может это свойство менять, остальные только получать
    /// </summary>
    public bool CanCancel
    {
      get { return m_can_cancel; }
      protected set
      {
        if (m_can_cancel == value)
          return;

        m_can_cancel = value;

        var handler = this.CanCancelChanged;

       if (handler != null)
         handler(this, EventArgs.Empty);
      }
    }

    public event EventHandler CanCancelChanged;
  }
}