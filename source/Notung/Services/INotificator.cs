using System;
using System.ComponentModel;
using System.Threading;
using Notung.Logging;
using Notung.Properties;
using Notung.Threading;

namespace Notung.Services
{
  /// <summary>
  /// Сервис для отображения пользователю информационных сообщений
  /// </summary>
  public interface INotificator 
  {
    /// <summary>
    /// Отображение сообщения, не требующего от пользователя принятия решения
    /// </summary>
    /// <param name="info">Сообщение</param>
    void Show(Info info);

    /// <summary>
    /// Отображение сообщения, не требующего от пользователя принятия решения
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    /// <param name="level">Уровень сообщения</param>
    void Show(string message, InfoLevel level);

    /// <summary>
    /// Отображение сообщения, не требующего от пользователя принятия решения
    /// </summary>
    /// <param name="buffer">Буфер сообщений</param>
    /// <param name="summary">Обобщение сообщений</param>
    void Show(InfoBuffer buffer, string summary = null);

    /// <summary>
    /// Отображение сообщения, требующее от пользователя подтверждения или отмены
    /// </summary>
    /// <param name="info">Сообщение</param>
    /// <returns>True, если пользователь подтвердил. Иначе, false</returns>
    bool Confirm(Info info);

    /// <summary>
    /// Отображение сообщения, требующее от пользователя подтверждения или отмены
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    /// <param name="level">Уровень сообщения</param>
    /// <returns>True, если пользователь подтвердил. Иначе, false</returns>
    bool Confirm(string message, InfoLevel level);

    /// <summary>
    /// Отображение сообщения, требующее от пользователя подтверждения или отмены
    /// </summary>
    /// <param name="buffer">Буфер сообщений</param>
    /// <param name="summary">Обобщение сообщений</param>
    /// <returns>True, если пользователь подтвердил. Иначе, false</returns>
    bool Confirm(InfoBuffer buffer, string summary = null);

    /// <summary>
    /// Отображение сообщения, требующее от пользователя согласия, отказа или отмены
    /// </summary>
    /// <param name="info">Сообщение</param>
    /// <returns>True, если пользователь согласился, false, если пользователь отказался,
    /// null, если пользователь отменил принятие решения</returns>
    bool? ConfirmOrCancel(Info info);

    /// <summary>
    /// Отображение сообщения, требующее от пользователя согласия, отказа или отмены
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    /// <param name="level">Уровень сообщения</param>
    /// <returns>True, если пользователь согласился, false, если пользователь отказался,
    /// null, если пользователь отменил принятие решения</returns>
    bool? ConfirmOrCancel(string message, InfoLevel level);

    /// <summary>
    /// Отображение сообщения, требующее от пользователя согласия, отказа или отмены
    /// </summary>
    /// <param name="buffer">Буфер сообщений</param>
    /// <param name="summary">Обобщение сообщений</param>
    /// <returns>True, если пользователь согласился, false, если пользователь отказался,
    /// null, если пользователь отменил принятие решения</returns>
    bool? ConfirmOrCancel(InfoBuffer buffer, string summary = null);
  }

  public sealed class Notificator : MarshalByRefObject, INotificator
  {
    private readonly INotificatorView m_view;
    private ISynchronizeInvoke m_last_ivoker;
    private IOperationWrapper m_last_wrapper;

    private static readonly ILog _log = LogManager.GetLogger(typeof(Notificator));

    public Notificator(INotificatorView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      m_view = view;
    }

    internal Notificator() : this(new ConsoleNotificatorView()) { }

    public void Show(Info info)
    {
      if (info == null)
        throw new ArgumentNullException("info");

      if (info.InnerMessages.Count == 0)
        this.AlertSync(info, ConfirmationRegime.None);
      else
        this.AlertSync(info.Message, info.Level, info.InnerMessages, ConfirmationRegime.None);
    }

    public void Show(InfoBuffer buffer, string summary)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");

      if (buffer.Count == 1 && buffer[0].InnerMessages.Count == 0)
        this.AlertSync(buffer[0], ConfirmationRegime.None);
      else
      {
        var level = GetMaxLevel(buffer);

        if (string.IsNullOrEmpty(summary))
          summary = GetDefaultSummary(level);

        this.AlertSync(summary, level, buffer, ConfirmationRegime.None);
      }
    }

    public void Show(string message, InfoLevel level)
    {
      this.AlertSync(new Info(message, level), ConfirmationRegime.None);
    }

    public bool Confirm(Info info)
    {
      if (info == null)
        throw new ArgumentNullException("info");

      if (info.InnerMessages.Count == 0)
        return this.AlertSync(info, ConfirmationRegime.Confirm).GetValueOrDefault();
      else
        return this.AlertSync(info.Message, info.Level, 
          info.InnerMessages, ConfirmationRegime.Confirm).GetValueOrDefault();
    }

    public bool Confirm(InfoBuffer buffer, string summary)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");

      if (buffer.Count == 1 && buffer[0].InnerMessages.Count == 0 
        && (string.IsNullOrEmpty(summary) || buffer[0].Message.Equals(summary)))
      {
        return this.AlertSync(buffer[0], ConfirmationRegime.Confirm).GetValueOrDefault();
      }
      else
      {
        var level = GetMaxLevel(buffer);

        if (string.IsNullOrEmpty(summary))
          summary = GetDefaultSummary(level);

        return this.AlertSync(summary, level, buffer, ConfirmationRegime.Confirm).GetValueOrDefault();
      }
    }
    
    public bool Confirm(string message, InfoLevel level)
    {
      return this.AlertSync(new Info(message, level), ConfirmationRegime.Confirm).GetValueOrDefault();
    }

    public bool? ConfirmOrCancel(Info info)
    {
      if (info == null)
        throw new ArgumentNullException("info");

      if (info.InnerMessages.Count == 0)
        return this.AlertSync(info, ConfirmationRegime.CancelableConfirm);
      else
        return this.AlertSync(info.Message, info.Level, 
          info.InnerMessages, ConfirmationRegime.CancelableConfirm);
    }

    public bool? ConfirmOrCancel(InfoBuffer buffer, string summary)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");

      if (buffer.Count == 1 && buffer[0].InnerMessages.Count == 0)
        return this.AlertSync(buffer[0], ConfirmationRegime.CancelableConfirm);
      else
      {
        var level = GetMaxLevel(buffer);

        if (string.IsNullOrEmpty(summary))
          summary = GetDefaultSummary(level);

        return this.AlertSync(summary, level, buffer, ConfirmationRegime.CancelableConfirm);
      }
    }

    public bool? ConfirmOrCancel(string message, InfoLevel level)
    {
      return this.AlertSync(new Info(message, level), ConfirmationRegime.CancelableConfirm);
    }

    #region Implementation ------------------------------------------------------------------------

    private InfoLevel GetMaxLevel(InfoBuffer buffer)
    {
      InfoLevel ret = InfoLevel.Debug;

      foreach (var info in buffer)
      {
        if (info.Level > ret)
          ret = info.Level;

        var inner = GetMaxLevel(info.InnerMessages);

        if (inner > ret)
          ret = inner;
      }

      return ret;
    }

    private string GetDefaultSummary(InfoLevel level)
    {
      switch(level)
      {
        case InfoLevel.Debug:
        case InfoLevel.Info:
          return Resources.SUMMARY_INFO;

        case InfoLevel.Warning:
          return Resources.SUMMARY_WARNING;

        case InfoLevel.Error:
        case InfoLevel.Fatal:
          return Resources.SUMMARY_ERROR;
      }

      return CoreResources.UNKNOWN;
    }

    private IOperationWrapper GetOperationWrapper()
    {
      if (!ReferenceEquals(m_last_ivoker, m_view.Invoker))
        m_last_wrapper = null;

      return m_last_wrapper ?? (m_last_wrapper = new SynchronizeOperationWrapper(m_last_ivoker = m_view.Invoker, true));
    }

    private bool? AlertSync(Info info, ConfirmationRegime confirm)
    {
      _log.Alert(info);
      return this.GetOperationWrapper().Invoke(() => m_view.Alert(info, confirm));
    }

    private bool? AlertSync(string summary, InfoLevel summaryLevel, InfoBuffer buffer, ConfirmationRegime confirm)
    {
      foreach (var info in buffer)
        _log.Alert(info);

      return this.GetOperationWrapper().Invoke(() => m_view.Alert(summary, summaryLevel, buffer, confirm));
    }

    #endregion
  }

  public interface INotificatorView : ISynchronizeProvider
  {
    bool? Alert(Info info, ConfirmationRegime confirm);

    bool? Alert(string summary, InfoLevel summaryLevel, InfoBuffer buffer, ConfirmationRegime confirm);
  }

  public enum ConfirmationRegime
  {
    None,
    Confirm,
    CancelableConfirm
  }

  public class ConsoleNotificatorView : EmptySynchronizeProvider, INotificatorView
  {
    public INotificator Source { get; set; }

    public bool? Alert(Info info, ConfirmationRegime confirm)
    {
      using (new ConsoleColorSetter(info.Level))
      {
        Console.WriteLine(info.Message);

        if (info.Details != null)
          Console.WriteLine("Details: {0}", info.Details);

        foreach (var inner in info.InnerMessages)
          Alert(inner, ConfirmationRegime.None);

        return ConfirmIfNeeded(confirm);
      }
    }

    private static bool? ConfirmIfNeeded(ConfirmationRegime confirm)
    {
      if (confirm != ConfirmationRegime.None)
      {
        Console.Write("Y / N: ");

        if (Console.ReadLine().ToUpper().Trim() == "Y")
          return true;
        else
          return false;
      }
      else
        return null;
    }

    public bool? Alert(string summary, InfoLevel summaryLevel, InfoBuffer buffer, ConfirmationRegime confirm)
    {
      if (!string.IsNullOrWhiteSpace(summary))
      {
        ConsoleColor old_color = Console.ForegroundColor;

        try
        {
          Console.ForegroundColor = ConsoleColor.White;
          Console.WriteLine(summary);
        }
        finally
        {
          Console.ForegroundColor = old_color;
        }
      }

      foreach (var inner in buffer)
        Alert(inner, ConfirmationRegime.None);

      return ConfirmIfNeeded(confirm);
    }

    private class ConsoleColorSetter : IDisposable
    {
      private readonly ConsoleColor m_old_color;
      private static readonly object _lock = new object();

      public ConsoleColorSetter(InfoLevel level)
      {
        m_old_color = Console.ForegroundColor;
        Monitor.Enter(_lock);
        try
        {
          switch (level)
          {
            case InfoLevel.Debug:
              Console.ForegroundColor = ConsoleColor.DarkCyan;
              break;

            case InfoLevel.Info:
              Console.ForegroundColor = ConsoleColor.Gray;
              break;

            case InfoLevel.Warning:
              Console.ForegroundColor = ConsoleColor.Yellow;
              break;

            case InfoLevel.Error:
              Console.ForegroundColor = ConsoleColor.Red;
              break;

            case InfoLevel.Fatal:
              Console.ForegroundColor = ConsoleColor.Magenta;
              break;
          }
        }
        catch
        {
          Monitor.Exit(_lock);
          throw;
        }
      }

      public void Dispose()
      {
        try
        {
          Console.ForegroundColor = m_old_color;
        }
        finally
        {
          Monitor.Exit(_lock);
        }
      }
    }
  }
}