using System.IO;

namespace Notung.Logging
{
  /// <summary>
  /// Осуществляет запись данных в приёмник лога
  /// </summary>
  public interface ILogAppender
  {
    /// <summary>
    /// Запись события логгирования в приёмник
    /// </summary>
    /// <param name="data">Событие, которое требуется запротоколировать</param>
    void WriteLog(LoggingData data);
  }

  internal sealed class FileLogAppender : ILogAppender
  {
    private readonly string m_working_path;
    private uint m_file_count;
    private uint m_file_number;
    private FileInfo m_file_info;
    private readonly LogStringBuilder m_builder;
    private const string COUNTER = ".counter";

    public FileLogAppender(string template)
    {
      m_builder = new LogStringBuilder(template);
      m_working_path = Path.Combine(ApplicationInfo.Instance.GetWorkingPath(), "Logs");

      if (!Directory.Exists(m_working_path))
        Directory.CreateDirectory(m_working_path);

      this.InitializeCounter();
    }

    public FileLogAppender() : this(LogSettings.Default.MessageTemplate) { }

    private void InitializeCounter()
    {
      if (!File.Exists(Path.Combine(m_working_path, COUNTER)))
      {
        using (var sw = new StreamWriter(Path.Combine(m_working_path, COUNTER)))
        {
          sw.WriteLine(0);
        }
      }
      else
      {
        using (var sr = new StreamReader(Path.Combine(m_working_path, COUNTER)))
        {
          m_file_count = uint.Parse(sr.ReadLine());
        }
      }
    }

    private FileInfo GetFileInfo()
    {
      if (m_file_info == null || m_file_number != m_file_count)
      {
        m_file_info = new FileInfo(Path.Combine(m_working_path, string.Format("{0}.log", m_file_count + 1)));
        m_file_number = m_file_count;
      }

      return m_file_info;
    }

    public void WriteLog(LoggingData data)
    {
      FileInfo fi = this.GetFileInfo();

      if (fi.Exists && fi.Length > LogSettings.Default.LogFileSize)
      {
        m_file_count++;

        using (var sw = new StreamWriter(Path.Combine(m_working_path, COUNTER)))
        {
          sw.WriteLine(m_file_count);
        }

        fi = this.GetFileInfo();
      }

      using (var fs = fi.Open(FileMode.Append, FileAccess.Write, FileShare.Write))
      {
        var writer = new StreamWriter(fs, System.Text.Encoding.UTF8);

        for (int i = 0; i < data.Length; i++)
        {
          m_builder.BuildString(writer, data[i]);
          writer.WriteLine();
          writer.WriteLine(LogSettings.Default.Separator);
          writer.WriteLine();
        }

        writer.Flush();
      }
    }
  }
}