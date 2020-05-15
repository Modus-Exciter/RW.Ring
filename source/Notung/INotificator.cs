using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Notung.Threading;

namespace Notung
{
  public interface INotificator : IDisposable
  {
    void Show(string summary, InfoBuffer buffer);

    bool Confirm(string summary, InfoBuffer buffer);

    void Show(Info info);

    bool Confirm(Info info);
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

    public void Show(string summary, InfoBuffer buffer)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      
      m_view.Alert(summary, buffer, false);
    }

    public bool Confirm(string summary, InfoBuffer buffer)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");

      return m_view.Alert(summary, buffer, true);
    }

    public void Show(Info info)
    {
      if (info == null)
        throw new ArgumentNullException("info");

      m_view.Alert(info, false);
    }

    public bool Confirm(Info info)
    {
      if (info == null)
        throw new ArgumentNullException("info");

      return m_view.Alert(info, true);
    }

    public void Dispose() { }
  }

  public interface INotificatorView : ISynchronizeProvider
  {
    bool Alert(Info info, bool confirm);

    bool Alert(string summary, InfoBuffer buffer, bool confirm);
  }

  public class ConsoleNotificatorView : SynchronizeProviderStub, INotificatorView
  {
    public bool Alert(Info info, bool confirm)
    {
      using (var setter = new ConsoleColorSetter(info.Level))
      {
        Console.WriteLine(info.Message);

        if (info.Details != null)
          Console.WriteLine("Details: {0}", info.Details);

        foreach (var inner in info.InnerMessages)
          Alert(inner, false);

        return ConfirmIfNeeded(confirm);
      }
    }

    private static bool ConfirmIfNeeded(bool confirm)
    {
      if (confirm)
      {
        Console.Write("Y / N: ");
        if (Console.ReadLine().ToUpper().Trim() == "Y")
          return true;
        else
          return false;
      }
      else
        return true;
    }

    public bool Alert(string summary, InfoBuffer buffer, bool confirm)
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
        Alert(inner, false);

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
