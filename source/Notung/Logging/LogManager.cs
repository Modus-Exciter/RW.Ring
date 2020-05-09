using System;

namespace Notung.Log
{
  /// <summary>
  /// Пока не решил, использовать log4net или свои асинхронные логи, частично дублируем его API
  /// </summary> 
  public static class LogManager
  {
    public static ILog GetLogger(string source)
    {
      if (string.IsNullOrEmpty(source))
        throw new ArgumentNullException("name");

      return new Logger(source);
    }

    public static ILog GetLogger(Type type)
    {
      return new Logger(type.FullName);
    }

    #region Extensions

    public static void Alert(this ILog logger, Info info)
    {
      logger.WriteLog(info.Message, info.Level, info.Details);

      foreach (var inner in info.InnerMessages)
        logger.Alert(inner);
    }

    public static void Debug(this ILog logger, string message)
    {
      logger.WriteLog(message, InfoLevel.Debug);
    }

    public static void Info(this ILog logger, string message)
    {
      logger.WriteLog(message, InfoLevel.Info);
    }

    public static void Warn(this ILog logger, string message)
    {
      logger.WriteLog(message, InfoLevel.Warning);
    }

    public static void Warn(this ILog logger, string message, Exception ex)
    {
      logger.WriteLog(message, InfoLevel.Warning, ex);
    }

    public static void Error(this ILog logger, string message)
    {
      logger.WriteLog(message, InfoLevel.Error);
    }

    public static void Error(this ILog logger, string message, Exception ex)
    {
      logger.WriteLog(message, InfoLevel.Error, ex);
    }

    public static void Fatal(this ILog logger, string message)
    {
      logger.WriteLog(message, InfoLevel.Fatal);
    }

    public static void Fatal(this ILog logger, string message, Exception ex)
    {
      logger.WriteLog(message, InfoLevel.Fatal, ex);
    }

    public static void DebugFormat(this ILog logger, string format, params object[] args)
    {
      logger.WriteLog(string.Format(format, args), InfoLevel.Debug);
    }

    public static void InfoFormat(this ILog logger, string format, params object[] args)
    {
      logger.WriteLog(string.Format(format, args), InfoLevel.Info);
    }

    public static void WarnFormat(this ILog logger, string format, params object[] args)
    {
      logger.WriteLog(string.Format(format, args), InfoLevel.Warning);
    }

    public static void ErrorFormat(this ILog logger, string format, params object[] args)
    {
      logger.WriteLog(string.Format(format, args), InfoLevel.Error);
    }

    public static void FatalFormat(this ILog logger, string format, params object[] args)
    {
      logger.WriteLog(string.Format(format, args), InfoLevel.Fatal);
    }

    #endregion

    private class Logger : ILog
    {
      private readonly string m_source;

      public Logger(string source)
      {
        m_source = source;
      }

      public override string ToString()
      {
        return string.Format("ILog({0})", m_source);
      }

      public void WriteLog(string message, InfoLevel level, object data)
      {
        // TODO: for async logs knowing of main thread is required
      }
    }
  }
}
