using Notung.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Schicksal.Optimization
{
  /// <summary>
  /// Класс для вывода массивов информации в текст
  /// </summary>
  class FileExportMath
  {
    const string FILE_TYPE = ".txt";
    static readonly string _doc_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    private static FileInfo InilializeFile(string fileName) 
    {
      FileInfo file;

      string baseName = Path.Combine(_doc_path, fileName);
      string probName = baseName;
      int i = 1;
      while (File.Exists(probName + FILE_TYPE))
      {
        probName = baseName + i.ToString();
        i++;
      }

      file = new FileInfo(probName + FILE_TYPE);
      return file;
    }

    public static void Write(object data, string fileName)
    {
      if(data.GetType().GetInterface(nameof(IEnumerable)) != null)
        WriteCollection((IEnumerable)data, fileName);
      else
        WriteSingleObject(data, fileName);
    }

    private static void WriteCollection(IEnumerable data, string fileName)
    {
      FileInfo file = InilializeFile(fileName);
      using (var fileStream = file.OpenWrite())
      {
        var writer = new StreamWriter(fileStream);
        foreach (var value in data)
          writer.WriteLine(value.ToString().Replace(',', '.'));
        writer.Flush();
      }
    }

    private static void WriteSingleObject(object data, string fileName)
    {
      FileInfo file = InilializeFile(fileName);
      using (var fileStream = file.OpenWrite())
      {
        var writer = new StreamWriter(fileStream);
        writer.WriteLine(data.ToString());
        writer.Flush();
      }
    }
  }
}
