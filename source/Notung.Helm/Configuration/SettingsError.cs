using System;
using System.ComponentModel;

namespace Notung.Helm.Configuration
{
  /// <summary>
  /// Описание ошибки конфигурации, по которой можно перейти к нужной странице настроек
  /// </summary>
  public sealed class SettingsError
  {
    /// <summary>
    /// Соообщение об ошибке
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Уровень сообщения
    /// </summary>
    public InfoLevel Level { get; set; }

    /// <summary>
    /// Тип конфигурационной секции, к которой привязано сообщение
    /// </summary>
    [Browsable(false)]
    public Type SectionType { get; set; }
  }

  /// <summary>
  /// Аргументы события, возникающего передс созданием страницы настроек
  /// </summary>
  public class SkipPageEventArgs : CancelEventArgs
  {
    private readonly IConfigurationPage m_page;

    public SkipPageEventArgs(IConfigurationPage page)
      : base(false)
    {
      if (page == null)
        throw new ArgumentNullException("page");

      m_page = page;
    }

    /// <summary>
    /// Создаваемая страница настроек
    /// </summary>
    public IConfigurationPage Page
    {
      get { return m_page; }
    }
  }

  /// <summary>
  /// 
  /// </summary>
  public class PageEventArgs : EventArgs
  {
    private readonly IConfigurationPage m_page;

    public PageEventArgs(IConfigurationPage page)
    {
      if (page == null)
        throw new ArgumentNullException("page");

      m_page = page;
    }

    /// <summary>
    /// Создаваемая страница настроек
    /// </summary>
    public IConfigurationPage Page
    {
      get { return m_page; }
    }
  }
}
