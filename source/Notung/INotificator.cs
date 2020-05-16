using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Notung.Threading;
using Notung.Properties;

namespace Notung
{
  public interface INotificator : IDisposable
  {
    void Show(Info info);

    void Show(string message, InfoLevel level);

    void Show(InfoBuffer buffer, string summary = null);

    bool Confirm(Info info);

    bool Confirm(string message, InfoLevel level);

    bool Confirm(InfoBuffer buffer, string summary = null);

    bool? ConfirmOrCancel(Info info);

    bool? ConfirmOrCancel(string message, InfoLevel level);

    bool? ConfirmOrCancel(InfoBuffer buffer, string summary = null);
  }

  public sealed class Notificator : INotificator
  {
    private readonly INotificatorView m_view;

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

      m_view.Alert(info, ConfirmationRegime.None);
    }

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

    private string GetDefaultSummary(InfoBuffer buffer)
    {
      switch (GetMaxLevel(buffer))
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

    public void Show(InfoBuffer buffer, string summary)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");

      if (string.IsNullOrEmpty(summary))
        summary = GetDefaultSummary(buffer);

      m_view.Alert(summary, buffer, ConfirmationRegime.None);
    }

    public void Show(string message, InfoLevel level)
    {
      this.Show(new Info(message, level));
    }

    public bool Confirm(InfoBuffer buffer, string summary)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");

      if (string.IsNullOrEmpty(summary))
        summary = GetDefaultSummary(buffer);

      return m_view.Alert(summary, buffer, ConfirmationRegime.Confirm).GetValueOrDefault();
    }
    
    public bool Confirm(Info info)
    {
      if (info == null)
        throw new ArgumentNullException("info");

      return m_view.Alert(info, ConfirmationRegime.Confirm).GetValueOrDefault();
    }
    
    public bool Confirm(string message, InfoLevel level)
    {
      return this.Confirm(new Info(message, level));
    }

    public bool? ConfirmOrCancel(InfoBuffer buffer, string summary)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");

      if (string.IsNullOrEmpty(summary))
        summary = GetDefaultSummary(buffer);

      return m_view.Alert(summary, buffer, ConfirmationRegime.CancelableConfirm);
    }

    public bool? ConfirmOrCancel(Info info)
    {
      if (info == null)
        throw new ArgumentNullException("info");

      return m_view.Alert(info, ConfirmationRegime.CancelableConfirm).GetValueOrDefault();
    }

    public bool? ConfirmOrCancel(string message, InfoLevel level)
    {
      return this.ConfirmOrCancel(new Info(message, level));
    }

    public void Dispose() { }
  }

  public interface INotificatorView : ISynchronizeProvider
  {
    bool? Alert(Info info, ConfirmationRegime confirm);

    bool? Alert(string summary, InfoBuffer buffer, ConfirmationRegime confirm);
  }

  public enum ConfirmationRegime
  {
    None,
    Confirm,
    CancelableConfirm
  }

  public class ConsoleNotificatorView : SynchronizeProviderStub, INotificatorView
  {
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

    public bool? Alert(string summary, InfoBuffer buffer, ConfirmationRegime confirm)
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

      public ConsoleColorSetter(InfoLevel level)
      {
        m_old_color = Console.ForegroundColor;
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

      public void Dispose()
      {
        Console.ForegroundColor = m_old_color;
      }
    }
  }
}
