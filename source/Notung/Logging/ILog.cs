using System;
using System.Collections.Generic;
using System.IO;
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
#if DEBUG
      if (data != null && length < 0 || length > data.Length)
        throw new ArgumentOutOfRangeException("length");
#endif
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
#if DEBUG
        if (m_data == null)
          throw new IndexOutOfRangeException();
#endif
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
      return GetEnumerator();
    }
  }

  /// <summary>
  /// Смысл этой структуры - запомнить всё, что требуется писать в лог 
  /// (к стандартному комплекту Info добавляется источник и дата события)
  /// </summary>
  public struct LoggingEvent
  {
    private static readonly LogStringBuilder _builder = new LogStringBuilder("[{Date}][{Level}][{Source}]\n{Message}");

    public LoggingEvent(string source, string message, InfoLevel level, object data) : this()
    {
      if (string.IsNullOrEmpty(source))
        throw new ArgumentNullException("source"); 

      if (string.IsNullOrEmpty(message))
        throw new ArgumentNullException("message");

      this.Source = source;
      this.Message = message;
      this.Level = level;
      this.Data = data;
      this.LoggingDate = DateTime.Now;

      m_thread_context = LoggingContext.Thread;
    }
    
    public readonly string Message;

    public readonly InfoLevel Level;

    public readonly object Data;

    public readonly DateTime LoggingDate;

    public readonly string Source;

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
