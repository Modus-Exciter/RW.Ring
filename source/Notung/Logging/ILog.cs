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

    public override string ToString()
    {
      if (!string.IsNullOrWhiteSpace(this.Source))
        return string.Format("{0}: {1}: {2}", this.Source, this.Level, this.Message);
      else
        return string.Format("{0}: {1}", this.Level, this.Message);
    }
  }
}
