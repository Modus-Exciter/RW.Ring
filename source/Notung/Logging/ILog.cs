using System;

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
    }
    
    public readonly string Message;

    public readonly InfoLevel Level;

    public readonly object Data;

    public readonly DateTime LoggingDate;

    public readonly string Source;

    private readonly static string _template1 = @"[{0:dd.MM.yyyy hh:mm:ss}] [{1}] {2}";
    private readonly static string _template2 = @"[{0:dd.MM.yyyy hh:mm:ss}] [{1}] {2}: {3}";
    private readonly static string _template3 = @"[{0:dd.MM.yyyy hh:mm:ss}] [{1}] {2}
{3}";
    private readonly static string _template4 = @"[{0:dd.MM.yyyy hh:mm:ss}] [{1}] {2}: {3}
{4}";

    public override string ToString()
    {
      if (this.Data == null)
      {
        if (string.IsNullOrWhiteSpace(this.Source))
          return string.Format(_template1, this.LoggingDate, this.Level, this.Message);
        else
          return string.Format(_template2, this.LoggingDate, this.Level, this.Source, this.Message);
      }
      else
      {
        if (string.IsNullOrWhiteSpace(this.Source))
          return string.Format(_template3, this.LoggingDate, this.Level, this.Message, this.Data);
        else
          return string.Format(_template4, this.LoggingDate, this.Level, this.Source, this.Message, this.Data);
      }
    }
  }
}
