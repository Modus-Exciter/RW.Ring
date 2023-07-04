using System;
using System.ComponentModel;

namespace Notung.Feuerzauber.Configuration
{

    /*Класс взят как есть из Notung.Helm*/
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
}
