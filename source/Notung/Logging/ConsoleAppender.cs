using System;

namespace Notung.Logging
{
  public sealed class ConsoleAppender : ILogAppender
  {
    public static ConsoleColor DebugColor = ConsoleColor.DarkCyan;
    public static ConsoleColor InfoColor = ConsoleColor.Gray;
    public static ConsoleColor WarningColor = ConsoleColor.Yellow;
    public static ConsoleColor ErrorColor = ConsoleColor.Red;
    public static ConsoleColor FatalColor = ConsoleColor.Magenta;
    
    public void WriteLog(LoggingData data)
    {
      for (int i = 0; i < data.Length; i++)
      {
        using (new ConsoleColorSetter(data[i].Level))
          data[i].Write(Console.Out);

        Console.WriteLine("\n");
      }
    }

    private class ConsoleColorSetter : IDisposable
    {
      private readonly ConsoleColor m_old_color;
      private static readonly object _lock = new object();

      public ConsoleColorSetter(InfoLevel level)
      {
        m_old_color = Console.ForegroundColor;
        try
        {
          switch (level)
          {
            case InfoLevel.Debug:
              Console.ForegroundColor = DebugColor;
              break;

            case InfoLevel.Info:
              Console.ForegroundColor = InfoColor;
              break;

            case InfoLevel.Warning:
              Console.ForegroundColor = WarningColor;
              break;

            case InfoLevel.Error:
              Console.ForegroundColor = ErrorColor;
              break;

            case InfoLevel.Fatal:
              Console.ForegroundColor = FatalColor;
              break;
          }
        }
        catch
        {
          throw;
        }
      }

      public void Dispose()
      {
        Console.ForegroundColor = m_old_color;
      }
    }
  }
}