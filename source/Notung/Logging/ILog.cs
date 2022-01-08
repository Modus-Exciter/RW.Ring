using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Notung.Logging
{
  public interface ILog
  {
    void WriteLog(string message, InfoLevel level, object data = null);
  }

  public struct LoggingData : IEnumerable<LoggingEvent>
  {
    private readonly LoggingEvent[] m_data;
    private readonly int m_length;

    public LoggingData(LoggingEvent[] data, int length) : this()
    {
      Debug.Assert(data == null || (length > 0 && length <= data.Length), "Length is out of range");

      m_data = data;

      if (m_data != null)
        m_length = length;
    }

    public int Length
    {
      get { return m_length; }
    }

    public LoggingEvent this[int index]
    {
      get
      {
        Debug.Assert(m_data != null);
        return m_data[index];
      }
    }

    public IEnumerator<LoggingEvent> GetEnumerator()
    {
      for (int i = 0; i < m_length; i++)
        yield return m_data[i];
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }

  /// <summary>
  /// Смысл этой структуры - запомнить всё, что требуется писать в лог 
  /// (к стандартному комплекту Info добавляется источник и дата события)
  /// </summary>
  [Serializable]
  public struct LoggingEvent
  {
    private static readonly LogStringBuilder _builder = new LogStringBuilder("[{Date}][{Level}][{Source}]\n{Message}");

    public LoggingEvent(string source, string message, InfoLevel level, object data) : this()
    {
      if (string.IsNullOrEmpty(source))
        throw new ArgumentNullException("source");

      if (string.IsNullOrEmpty(message))
        throw new ArgumentNullException("message");

      Source = source;
      Message = message;
      Level = level;
      this.Data = data;
      LoggingDate = DateTime.Now;

      m_thread_context = LoggingContext.Thread;
    }

    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      if (this.Data != null && !this.Data.GetType().IsDefined(typeof(SerializableAttribute), false))
        this.Data = this.Data.ToString();
    }

    public readonly string Message;

    public readonly InfoLevel Level;

    public readonly DateTime LoggingDate;

    public readonly string Source;

    public object Data { get; private set; }

    private readonly IThreadLoggingContext m_thread_context;

    public System.Threading.Thread Thread
    {
      get
      {
        if (m_thread_context == null)
          return null;
        else
          return m_thread_context.CurrentThread;
      }
    }

    public object this[string key]
    {
      get
      {
        if (m_thread_context != null)
          return m_thread_context[key] ?? LoggingContext.Global[key];
        else
          return LoggingContext.Global[key];
      }
    }

    public bool Contains(string key)
    {
      if (string.IsNullOrWhiteSpace(key))
        return false;

      if (m_thread_context != null)
        return m_thread_context.Contains(key) || LoggingContext.Global.Contains(key);
      else
        return LoggingContext.Global.Contains(key);
    }

    public void Write(TextWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");

      _builder.BuildString(writer, this);
    }

    public override string ToString()
    {
      var builder = new StringBuilder(256);

      using (var writer = new StringWriter(builder))
      {
        _builder.BuildString(writer, this);
      }

      return builder.ToString();
    }
  }
}