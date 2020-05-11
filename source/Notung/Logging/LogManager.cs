﻿using System;

namespace Notung.Log
{
  /// <summary>
  /// Пока не решил, использовать log4net или свои асинхронные логи, частично дублируем его API
  /// </summary> 
  public static class LogManager
  {
    private static LogProcess _process;
    private static readonly object _lock = new object();
    
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

    public static void AddAcceptor(ILogAcceptor acceptor)
    {
      lock (_lock)
      {
        if (_process != null)
          _process.AddAcceptor(acceptor);
      }
    }

    public static void Start(ILogBaseSettings settings)
    {
      lock (_lock)
      {
        if (_process != null)
          _process.Dispose();

        _process = new LogProcess(settings);
      }
    }

    internal static void StartIfNotRunning(ILogBaseSettings settings)
    {
      if (IsRunning)
        return;
      
      lock (_lock)
      {
        if (_process != null && !_process.Stopped)
          return;

        _process = new LogProcess(settings);
      }
    }

    public static void WaitUntilStop()
    {
      lock (_lock)
      {
        if (_process == null)
          return;

        _process.WaitUntilStop();
        _process = null;
      }
    }

    public static bool IsRunning
    {
      get
      {
        var process = _process;
        return process != null && !process.Stopped;
      }
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
        var process = _process;

        if (process != null)
          process.WriteMessage(new LoggingData(m_source, message, level, data));
      }
    }
  }
}
