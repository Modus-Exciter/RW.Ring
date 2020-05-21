using System;
using System.Collections.Generic;
using System.Reflection;

namespace Notung.Logging
{
  /// <summary>
  /// Пока не решил, использовать log4net или свои асинхронные логи, частично дублируем его API
  /// </summary> 
  public static partial class LogManager
  {
    private static readonly HashSet<ILogAcceptor> _acceptors = new HashSet<ILogAcceptor>
    {
      new FileLogAcceptor()
    };
    private static LogProcess _process = new SyncLogProcess(_acceptors);
    private static readonly object _lock = new object();
    private static readonly Dictionary<string, ILog> _source_loggers = new Dictionary<string, ILog>();
    private static readonly Dictionary<Type, ILog> _type_loggers = new Dictionary<Type, ILog>();
  
    public static ILog GetLogger(string source)
    {
      if (string.IsNullOrEmpty(source))
        source = LogSettings.Default.DefaultLogger;

      lock (_source_loggers)
      {
        ILog ret;

        if (!_source_loggers.TryGetValue(source, out ret))
        {
          ret = new Logger(source);
          _source_loggers.Add(source, ret);
        }

        return ret;
      }
    }

    public static ILog GetLogger(Type type)
    {
      if (type == null)
        throw new ArgumentNullException("type");
      
      lock (_type_loggers)
      {
        ILog ret;

        if (!_type_loggers.TryGetValue(type, out ret))
        {
          ret = new Logger(type.FullName);
          _type_loggers.Add(type, ret);
        }

        return ret;
      }
    }

    public static void AddAcceptor(ILogAcceptor acceptor)
    {
      if (acceptor == null)
        throw new ArgumentNullException("acceptor");

      lock (_acceptors)
        _acceptors.Add(acceptor);
    }

    public static void SetMainThreadInfo(IMainThreadInfo info)
    {
      if (info == null || !info.ReliableThreading)
        return;

      lock (_lock)
      {
        if (_process != null && !_process.Stopped)
          _process.Stop();

        _process = new AsyncLogProcess(info, _acceptors);
      }
    }

    public static void Stop()
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

    #region Sharing log between domains

    internal static void Share(AppDomain newDomain)
    {
      var acceptor = (DomainAcceptor)newDomain.CreateInstanceAndUnwrap(
        Assembly.GetExecutingAssembly().FullName, typeof(DomainAcceptor).FullName);

      acceptor.Accept(_proxy);
    }

    private class ProcessProxy : MarshalByRefObject
    {
      public void WriteMessage(LoggingEvent data)
      {
        var process = _process;

        if (process != null)
          process.WriteMessage(ref data);
      }
    }

    private class DomainAcceptor : MarshalByRefObject
    {
      public void Accept(ProcessProxy proxy)
      {
        _proxy = proxy;
      }
    }

    private static ProcessProxy _proxy = new ProcessProxy();

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
        _proxy.WriteMessage(new LoggingEvent(m_source, message, level, data));
      }
    }
  }
}
