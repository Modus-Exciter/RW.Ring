#if MULTI_LANG

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using Notung.Data;

namespace Notung.ComponentModel
{
  /// <summary>
  /// Компонент для переключения между языками
  /// </summary>
  [DefaultEvent("LanguageChanged")]
  public class LanguageSwitcher : Component, ISupportInitialize
  {
    private static readonly WeakSet<LanguageSwitcher> _instances = new WeakSet<LanguageSwitcher>();
    private static readonly HashSet<Thread> _threads = new HashSet<Thread>();

    private static CultureInfo _current_culture = Thread.CurrentThread.CurrentUICulture;

    static LanguageSwitcher() 
    {
      // Скорее всего, первый объект будет создан в потоке пользовательского интерфейса
      _threads.Add(Thread.CurrentThread); 
    }

    /// <summary>
    /// Инициализирует новый объект-перключатель
    /// </summary>
    public LanguageSwitcher()
    {
      _instances.Add(this);
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
        lock (this.Events)
          this.Events.AddHandler("LanguageChanged", value);
      }
      remove
      {
        lock (this.Events)
          this.Events.RemoveHandler("LanguageChanged", value);
      }
    }

    /// <summary>
    /// Список зарегистрированных потоков (испольуется для задач, выполняемых в фоновом режиме)
    /// </summary>
    public static Thread[] Threads
    {
      get { return _threads.ToArray(); }
    }

    /// <summary>
    /// Добавляет поток в список зарегистрированных потоков
    /// </summary>
    /// <param name="thread">Добавляемый поток</param>
    public static void RegisterThread(Thread thread)
    {
      if (thread == null)
        return;

      lock (_threads)
      {
        if (_threads.Add(thread))
        {
          thread.CurrentCulture = _current_culture;
          thread.CurrentUICulture = _current_culture;
        }
      }
    }

    private static void RemoveThread(Thread thread)
    {
      if (thread == null)
        return;

      _threads.Remove(thread);
    }

    /// <summary>
    /// Генерирует событие переключения языка
    /// </summary>
    /// <param name="args">Аргумент события</param>
    protected virtual void OnLanguageChanged(LanguageEventArgs args)
    {
      lock (this.Events)
      {
        this.Events["LanguageChanged"].InvokeSynchronized(this, args);
      }
    }

    /// <summary>
    /// Отключает переключатель
    /// </summary>
    /// <param name="disposing">Явное или неявное уничтожение объекта</param>
    protected override void Dispose(bool disposing)
    {
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

      var corrupted_threads = new List<Thread>();

      foreach (var thread in _threads)
      {
        try
        {
          if (thread.IsAlive)
            thread.CurrentUICulture = culture;
          else
            corrupted_threads.Add(thread);
        }
        catch
        {
          corrupted_threads.Add(thread);
        }
      }

      foreach (var corr in corrupted_threads)
        RemoveThread(corr);

      try
      {
        ComponentExtensions.ClearEnumLabels();

        foreach (var instance in _instances)
          instance.OnLanguageChanged(new LanguageEventArgs(culture));
      }
      catch { }

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

    #region ISupportInitialize Members

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

#endif