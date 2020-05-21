using System;
using System.Threading;

namespace Notung.Logging
{
  /// <summary>
  /// Информация о главном потоке, необходимая для работы логов
  /// </summary>
  public interface IMainThreadInfo
  {
    /// <summary>
    /// Работает ли главный поток
    /// </summary>
    bool IsAlive { get; }

    /// <summary>
    /// Надёжна ли информация о главном потоке
    /// </summary>
    bool ReliableThreading { get; }
  }

  public sealed class CurrentMainThreadInfo : MarshalByRefObject, IMainThreadInfo
  {
    private readonly Thread m_main_thred = Thread.CurrentThread;

    public bool IsAlive
    {
      get { return m_main_thred.IsAlive; }
    }

    public bool ReliableThreading
    {
      get { return true; }
    }
  }
}
