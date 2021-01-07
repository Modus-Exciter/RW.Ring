using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using Notung.Data;
using Notung.Logging;
using Notung.Threading;

namespace Notung.ComponentModel
{
  /// <summary>
  /// Компонент для переключения между языками
  /// </summary>
  [DefaultEvent("LanguageChanged")]
  public sealed class LanguageSwitcher : Component, ISupportInitialize
  {
    private static readonly WeakSet<LanguageSwitcher> _instances = new WeakSet<LanguageSwitcher>();
    private static CultureInfo _current_culture = Thread.CurrentThread.CurrentUICulture;
    private static object _changed_event_key = new object();

    private static ILog _log = LogManager.GetLogger(typeof(LanguageSwitcher));

    static LanguageSwitcher() 
    {
      ThreadTracker.AddThreadHandlers(OnThreadRegistered, null, OnThreadError);
    }

    /// <summary>
    /// Инициализирует новый объект-перключатель
    /// </summary>
    public LanguageSwitcher()
    {
      _instances.Add(this);

      ThreadTracker.RegisterThread(Thread.CurrentThread);
    }

    /// <summary>
    /// Инициализирует новый объект-перключатель
    /// </summary>
    /// <param name="container">Контейнер, содержащий компонент</param>
    public LanguageSwitcher(IContainer container) : this()
    {
      container.Add(this);
    }

    /// <summary>
    /// Происходит, когда произошло переключение
    /// </summary>
    public event EventHandler<LanguageEventArgs> LanguageChanged
    {
      add
      {
          this.Events.AddHandler(_changed_event_key, value);
      }
      remove
      {
          this.Events.RemoveHandler(_changed_event_key, value);
      }
    }

    private static void OnThreadRegistered(Thread thread)
    {
      thread.CurrentCulture = _current_culture;
      thread.CurrentUICulture = _current_culture;
    }

    private static void OnThreadError(Thread thread, Exception error)
    {
      _log.Error(string.Format("Error in thread {0}",
        string.IsNullOrEmpty(thread.Name) ? (object)thread.ManagedThreadId : thread.Name), error);
    }

    /// <summary>
    /// Генерирует событие переключения языка
    /// </summary>
    /// <param name="args">Аргумент события</param>
    private void OnLanguageChanged(LanguageEventArgs args)
    {
      var handler = this.Events[_changed_event_key] as EventHandler<LanguageEventArgs>;

      if (handler != null)
        handler(this, args);
    }

    /// <summary>
    /// Отключает переключатель
    /// </summary>
    /// <param name="disposing">Явное или неявное уничтожение объекта</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
        _instances.Remove(this);

      base.Dispose(disposing);
    }

    /// <summary>
    /// Текущий язык
    /// </summary>
    public static CultureInfo CurrentCulture
    {
      get { return _current_culture; }
    }

    /// <summary>
    /// Переключает на новый язык
    /// </summary>
    /// <param name="culture">Культурные настройки</param>
    public static void Switch(CultureInfo culture)
    {
      if (culture == null)
        return;

      var expired_threads = new List<Thread>();

      foreach (var thread in ThreadTracker.Threads)
      {
        try
        {
          if (thread.IsAlive)
            thread.CurrentUICulture = culture;
          else
            expired_threads.Add(thread);
        }
        catch(Exception error)
        {
          OnThreadError(thread, error);
        }
      }

      foreach (var corr in expired_threads)
        ThreadTracker.RemoveThread(corr);

      try
      {
        EnumLabelConverter.ClearEnumLabels();

        foreach (var instance in _instances)
          instance.OnLanguageChanged(new LanguageEventArgs(culture));
      }
      catch (Exception ex)
      {
        _log.Error(string.Format("Switch(\"{0}\"): exception", culture), ex);
      }

      _current_culture = culture;
    }

    /// <summary>
    /// Переключает на новый язык
    /// </summary>
    /// <param name="cultureCode">Строковый код культуры</param>
    public static void Switch(string cultureCode)
    {
      Switch(new CultureInfo(cultureCode));
    }

    #region ISupportInitialize Members ------------------------------------------------------------

    void ISupportInitialize.BeginInit() { }

    void ISupportInitialize.EndInit()
    {
      OnLanguageChanged(new LanguageEventArgs(_current_culture));
    }

    #endregion
  }

  /// <summary>
  /// Событие переключения языка
  /// </summary>
  [Serializable]
  public class LanguageEventArgs : EventArgs
  {
    private readonly CultureInfo m_culture_info;
    
    /// <summary>
    /// Инициализирует новый экземпляр события
    /// </summary>
    /// <param name="language"></param>
    public LanguageEventArgs(CultureInfo language)
    {
      if (language == null)
        throw new ArgumentNullException("language");

      m_culture_info = language;
    }

    /// <summary>
    /// Язык, на который требуется переключиться
    /// </summary>
    public CultureInfo Language
    {
      get { return m_culture_info; }
    }
  }
}