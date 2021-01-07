using System;
using System.ComponentModel;

namespace Notung.Threading
{
  /// <summary>
  /// Индикатор прогресса для выполнения задачи
  /// </summary>
  public interface IProgressIndicator
  {
    /// <summary>
    /// Отображает прогресс выполнения задачи
    /// </summary>
    /// <param name="percentage">Процент выполнения задачи</param>
    /// <param name="state">Текстовое описание состояния задачи. Object нельзя, потому что Remoting</param>
    void ReportProgress(int percentage, string state);
  }

  public sealed class BackgroundWorkProgressIndicator : IProgressIndicator
  {
    private readonly BackgroundWorker m_worker;

    public BackgroundWorkProgressIndicator(BackgroundWorker worker)
    {
      if (worker == null)
        throw new ArgumentNullException("worker");

      m_worker = worker;
    }
    
    public void ReportProgress(int percentage, string state)
    {
      if (m_worker.WorkerReportsProgress)
        m_worker.ReportProgress(percentage, state);
    }
  }
}
