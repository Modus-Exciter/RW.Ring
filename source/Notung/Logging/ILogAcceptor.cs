using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using Notung.Configuration;

namespace Notung.Log
{
  public interface ILogAcceptor
  {
    void WriteLog(LoggingData[] data);
  }

  internal sealed class FileLogAcceptor : ILogAcceptor
  {
    private readonly string m_working_path;
    private uint m_file_number;
    private FileInfo m_file_info;

    public FileLogAcceptor(IConfigFileFinder finder)
    {
      if (finder == null)
        throw new ArgumentNullException("finder");

      m_working_path = Path.Combine(Path.GetDirectoryName(finder.WorkingPath), "Logs");
      InitializeCounter();
    }

    public FileLogAcceptor() : this(new ProductVersionConfigFileFinder("log.ini")) { }

    public void WriteLog(LoggingData[] data)
    {
      FileInfo fi = GetFileInfo();

      if (fi.Exists && fi.Length > LogSettings.Default.LogFileSize)
      {
        LogSettings.Default.FileCount++;
        LogSettings.Default.Save();
        
        fi = GetFileInfo();
      }

      using (var fs = fi.AppendText())
      {
        for (int i = 0; i < data.Length; i++)
        {
          fs.WriteLine(data[i]);
          fs.WriteLine();
          fs.WriteLine(LogSettings.Default.Separator);
          fs.WriteLine();
        }
      }
    }

    private FileInfo GetFileInfo()
    {
      if (m_file_info == null || m_file_number != LogSettings.Default.FileCount)
      {
        m_file_info = new FileInfo(Path.Combine(m_working_path, string.Format("{0}.log", LogSettings.Default.FileCount + 1)));
        m_file_number = LogSettings.Default.FileCount;
      }

      return m_file_info;
    }

    private void InitializeCounter()
    {
      if (!Directory.Exists(m_working_path))
        Directory.CreateDirectory(m_working_path);

      if (Directory.GetFiles(m_working_path, "*.log").Length == 0)
      {
        LogSettings.Default.FileCount = 0;
        LogSettings.Default.Save();
      }
    }
  }
}
