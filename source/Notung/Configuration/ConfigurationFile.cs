using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Notung.Logging;
using Notung.Threading;

namespace Notung.Configuration
{
  public sealed class ConfigurationFile
  {
    private readonly IConfigFileFinder m_file_finder;
    private readonly SharedLock m_lock = new SharedLock();
    private Dictionary<string, string> m_file_cache;

    private readonly ILog _log = LogManager.GetLogger(typeof(ConfigurationFile));

    public ConfigurationFile(IConfigFileFinder fileFinder)
    {
      if (fileFinder == null)
        throw new ArgumentNullException("fileFinder");

      m_file_finder = fileFinder;
    }

    public ConfigurationFile() : this(new ProductVersionConfigFileFinder()) { }

    public IConfigFileFinder Finder
    {
      get { return m_file_finder; }
    }

    public string this[string section]
    {
      get
      {
        if (section == null) 
          throw new ArgumentNullException("section"); 

        return this.Sections[section];
      }
      set
      {
        if (section == null) 
          throw new ArgumentNullException("section"); 

        this.Sections[section] = value;
      }
    }

    public bool TryGetSection(string sectionName, out string sectionXml)
    {
      if (sectionName == null) 
        throw new ArgumentNullException("sectionName");

      return this.Sections.TryGetValue(sectionName, out sectionXml);
    }

    public void Save()
    {
      using (m_lock.WriteLock())
      {
        this.LoadFile();

        var file_name = m_file_finder.OutputFilePath;

        _log.DebugFormat("Save(): {0}", file_name);

        if (File.Exists(file_name))
          File.Delete(file_name);

        var dir = Path.GetDirectoryName(file_name);
        if (!Directory.Exists(dir))
          Directory.CreateDirectory(dir);

        using (var fs = new FileStream(file_name, FileMode.Create, FileAccess.Write))
        {
          var writer = new XmlTextWriter(fs, Encoding.UTF8)
          {
            Formatting = Formatting.Indented,
            IndentChar = '\t',
            Indentation = 1,
          };

          writer.WriteStartDocument();
          writer.WriteStartElement("configuration");
          writer.WriteWhitespace(Environment.NewLine);

          foreach (var section in this.Sections.Values)
            writer.WriteRaw(section);

          writer.WriteEndElement();
          writer.Flush();
        }
      }
    }

    private Dictionary<string, string> Sections
    {
      get
      {
        using (m_lock.ReadLock())
        {
          if (m_file_cache != null)
            return m_file_cache;
        }

        using (m_lock.WriteLock())
        {
          this.LoadFile();
          return m_file_cache;
        }
      }
    }

    private void LoadFile()
    {
      if (m_file_cache != null)
        return;

      m_file_cache = new Dictionary<string, string>();

      var file_name = m_file_finder.InputFilePath;

      if (string.IsNullOrEmpty(file_name) || !File.Exists(file_name))
        return;

      try
      {
        using (FileStream fs = new FileStream(file_name, FileMode.Open, FileAccess.Read))
        {
          XmlTextReader reader = new XmlTextReader(fs);

          while (reader.Read())
          {
            if (reader.Depth == 1 && reader.NodeType == XmlNodeType.Element)
              break;
          }

          while (!reader.EOF)
          {
            if (reader.Depth == 1 && reader.NodeType == XmlNodeType.Element)
              m_file_cache.Add(reader.Name, reader.ReadOuterXml());
            else
              reader.Read();
          }

          _log.DebugFormat("LoadFile(): {0}", file_name);
        }
      }
      catch (Exception ex)
      {
        _log.Error("LoadFile(): exception", ex);
      }
    }
  }
}