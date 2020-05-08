using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Notung.ComponentModel;
using Notung.Properties;

namespace Notung
{
  [Serializable]
  public sealed class Info
  {
    private readonly InfoBuffer m_inner_messages = new InfoBuffer();
    private SerializeCondition<object> m_details;
    private string m_details_string;

    /// <summary>
    /// Инициализирует новый экземпляр сообщения
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    /// <param name="state">Уровень сообщения</param>
    public Info(string message, InfoLevel state)
    {
      this.Message = message;
      this.Level = state;
    }

    /// <summary>
    /// Инициализирует новый экземпляр сообщения
    /// </summary>
    /// <param name="ex">Исключение, на основе которого формируется сообщение</param>
    public Info(Exception ex)
    {
      this.Message = ex.Message;
      this.Level = InfoLevel.Error;
      this.Details = ex;

      Exception inner = ex.InnerException;

      while (inner != null)
      {
        var inf = new Info(inner.Message, InfoLevel.Error);
        inf.Details = inner;
        m_inner_messages.Add(inf);
        inner = inner.InnerException;
      }
      // TODO: AggregateException, ValidationFailException
    }

    /// <summary>
    /// Текст сообщения
    /// </summary>
    public string Message { get; private set; }

    /// <summary>
    /// Уровень сообщения
    /// </summary>
    public InfoLevel Level { get; private set; }

    /// <summary>
    /// Вложенные сообщения
    /// </summary>
    public InfoBuffer InnerMessages
    {
      get { return m_inner_messages; }
    }

    /// <summary>
    /// Дополнительные данные, связанные с сообщением
    /// </summary>
    public object Details
    {
      get { return m_details.Value ?? m_details_string; }
      set
      {
        m_details.Value = value;

        if (value != null && !m_details.CanSerialize)
          m_details_string = value.ToString();
        else
          m_details_string = null;
      }
    }

    public override string ToString()
    {
      return string.Format("{0}: {1}", this.Level, this.Message);
    }
  }

  [DisplayNameRes("INFO_LEVEL", typeof(Resources))]
  [TypeConverter(typeof(EnumLabelConverter))]
  public enum InfoLevel
  {
    [DisplayNameRes("DEBUG", typeof(Resources))]
    Debug = 0,
    [DisplayNameRes("INFO", typeof(Resources))]
    Info = 1,
    [DisplayNameRes("WARNING", typeof(Resources))]
    Warning = 2,
    [DisplayNameRes("ERROR", typeof(Resources))]
    Error = 3,
    [DisplayNameRes("FATAL", typeof(Resources))]
    Fatal = 4
  }
  
  /// <summary>
  /// Буфер сообщений
  /// </summary>
  [Serializable]
  public sealed class InfoBuffer : IEnumerable<Info>
  {
    private readonly List<Info> m_items = new List<Info>();

    /// <summary>
    /// Добавляет сообщение в буфер
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    /// <param name="state">Уровень сообщения</param>
    public Info Add(string message, InfoLevel state)
    {
      Info ret = new Info(message, state);
      m_items.Add(ret);
      return ret;
    }

    /// <summary>
    /// Добавляет сообщение в буфер
    /// </summary>
    /// <param name="info">Сообщение</param>
    public void Add(Info info)
    {
      m_items.Add(info);
    }

    /// <summary>
    /// Добавляет сообщение об ошибке
    /// </summary>
    /// <param name="ex">Исключение, на основе которого формируется сообщение</param>
    public Info Add(Exception ex)
    {
      Info ret = new Info(ex);
      m_items.Add(ret);
      return ret;
    }

    /// <summary>
    /// Конвертирует все сообщения в список указанного типа
    /// </summary>
    /// <typeparam name="TOutput">Тип, к которому необходимо преобразовать сообщения</typeparam>
    /// <param name="converter">Функция, осуществляющая преобразования</param>
    /// <returns>Список сконвертированных в указанный тп сообщений</returns>
    public List<TOutput> ConvertAll<TOutput>(Converter<Info, TOutput> converter)
    {
      return m_items.ConvertAll<TOutput>(converter);
    }

    /// <summary>
    /// Возвращает сообщение по индексу
    /// </summary>
    /// <param name="index">Индекс</param>
    /// <returns>Сообщение, если индекс не выходит за границы; иначе, бросает исключение</returns>
    public Info this[int index]
    {
      get { return m_items[index]; }
    }

    /// <summary>
    /// Количество сообщений в буфере
    /// </summary>
    public int Count
    {
      get { return m_items.Count; }
    }

    #region IEnumerable<Info> Members

    /// <summary>
    /// Возвращает перечислитель для сообщений
    /// </summary>
    /// <returns>IEnumerator</returns>
    public IEnumerator<Info> GetEnumerator()
    {
      return m_items.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return m_items.GetEnumerator();
    }

    #endregion
  }

  /// <summary>
  /// Интерфейс объекта, поддерживающего проверку
  /// </summary>
  public interface IValidator
  {
    bool Validate(InfoBuffer buffer);
  }
}
