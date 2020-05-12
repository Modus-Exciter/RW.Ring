using System;
using System.IO;
using System.Text;

namespace Notung.Log
{
  public interface ILog
  {
    void WriteLog(string message, InfoLevel level, object data = null);
  }

  /// <summary>
  /// Смысл этой структуры - запомнить всё, что требуется писать в лог 
  /// (к стандартному комплекту Info добавляется источник и дата события)
  /// </summary>
  public struct LoggingData
  {
    public LoggingData(string source, string message, InfoLevel level, object data) : this()
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
        LogSettings.Default.BuildString(writer, this);
      }

      return builder.ToString();
    }
  }
}
