using System.Threading;

namespace Notung.Logging
{
  /// <summary>
  /// Информация о главном потоке, необходимая для работы логов
  /// </summary>
  public interface IMainThreadInfo
  {
    /// <summary>
    /// Ссылка на главный поток приложения, чтобы отслеживать его завершение
    /// </summary>
    Thread MainThread { get; }

    /// <summary>
    /// Надёжна ли информация о главном потоке
    /// </summary>
    bool ReliableThreading { get; }
  }

  public sealed class CurrentMainThreadInfo : IMainThreadInfo
  {
    private readonly Thread m_main_thred = Thread.CurrentThread;
   
    public Thread MainThread
    {
      get { return m_main_thred; }
    }

    public bool ReliableThreading
    {
      get { return true; }
    }
  }
}
