using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Notung.Logging;
using Notung.Properties;
using Notung.Threading;

namespace Notung.Configuration
{
  public interface IConfigurator
  {
    bool LoadSection(Type sectionType, InfoBuffer buffer);

    TSection GetSection<TSection>() where TSection : ConfigurationSection, new();

    void SaveSection<TSection>(TSection section) where TSection : ConfigurationSection, new();

    void ApplySettings();

    void SaveSettings();

    void HandleError(Exception error);

    string WorkingPath { get; }
  }

  public sealed class DataContractConfigurator : MarshalByRefObject, IConfigurator
  {
    private readonly Dictionary<Type, ConfigurationSection> m_sections = new Dictionary<Type, ConfigurationSection>();
    private readonly Dictionary<string, Type> m_section_names = new Dictionary<string, Type>();
    private readonly SharedLock m_lock = new SharedLock(false);
    private readonly ConfigurationFile m_file;

    private static readonly ILog _log = LogManager.GetLogger(typeof(DataContractConfigurator));

    public DataContractConfigurator(ConfigurationFile file)
    {
      if (file == null)
        throw new ArgumentNullException("file");

      m_file = file;
    }

    internal DataContractConfigurator() : this(new ConfigurationFile()) { }

    public DataContractConfigurator(IConfigFileFinder finder) : this(new ConfigurationFile(finder)) { }

    public bool LoadSection(Type sectionType, InfoBuffer buffer)
    {
      if (sectionType == null)
        throw new ArgumentNullException("sectionType");

      if (buffer == null)
        throw new ArgumentNullException("buffer");

      bool ret = true;

      using (m_lock.WriteLock())
      {
        if (!typeof(ConfigurationSection).IsAssignableFrom(sectionType)
          || sectionType.IsAbstract || sectionType.GetConstructor(Type.EmptyTypes) == null)
          throw new ArgumentException(Resources.INVALID_CONFIG_SECTION_TYPE, "sectionType");

        ConfigurationSection section;

        if (!m_sections.TryGetValue(sectionType, out section))
        {
          section = this.ReadSection(sectionType);
          m_sections.Add(sectionType, section);
        }

        ret = section.Validate(buffer);
      }

      return ret;
    }

    public string WorkingPath
    {
      get { return m_file.Finder.WorkingPath; }
    }

    public TSection GetSection<TSection>() where TSection : ConfigurationSection, new()
    {
      ConfigurationSection ret;

      using (m_lock.ReadLock())
      {
        if (m_sections.TryGetValue(typeof(TSection), out ret))
          return (TSection)ret;
      }

      using (m_lock.WriteLock())
      {
        if (!m_sections.TryGetValue(typeof(TSection), out ret))
        {
          ret = this.ReadSection(typeof(TSection));
          m_sections.Add(typeof(TSection), ret);
        }
      }

      return (TSection)ret;
    }

    public void SaveSection<TSection>(TSection section)
      where TSection : ConfigurationSection, new()
    {
      if (section == null)
        throw new ArgumentNullException("section");

      if (section.GetType() != typeof(TSection))
        throw new ArgumentException(string.Format(Resources.SECTION_TYPE_UNINHERITABLE, 
          typeof(TSection), section.GetType()));

      using (m_lock.WriteLock())
      {
        m_sections[typeof(TSection)] = section;
        this.WriteSection(section);
      }
    }

    public void HandleError(Exception error)
    {
      using (m_lock.ReadLock())
      {
        foreach (var section in m_sections)
          section.Value.OnError(error);
      }
    }

    public void ApplySettings()
    {
      using (m_lock.WriteLock())
      {
        foreach (var kv in m_sections)
          kv.Value.ApplySettings();
      }
    }

    public void SaveSettings()
    {
      using (m_lock.WriteLock())
      {
        foreach (var kv in m_sections)
          this.WriteSection(kv.Value);

        m_file.Save();
      }
    }

    private void WriteSection(ConfigurationSection section)
    {
      string section_name;
      bool data_contract;

      _log.DebugFormat("WriteSection(): {0}", section.GetType().FullName);

      section_name = this.GetSectionName(section.GetType(), out data_contract);
      var sb = new StringBuilder();

      using (var sw = new StringWriter(sb))
      {
        var writer = new XmlTextWriter(sw) 
        { 
          Formatting = Formatting.Indented, 
          IndentChar = '\t', 
          Indentation = 1, 
        };

        if (data_contract)
        {
          var sr = new DataContractSerializer(section.GetType());
          sr.WriteObject(writer, section);
        }
        else
        {
          var sr = new XmlSerializer(section.GetType());
          sr.Serialize(writer, section);
        }

        writer.Flush();
      }

      sb.AppendLine();

      if (data_contract)
      {
        m_file[section_name] = sb.ToString();
      }
      else
      {
        var tmp = sb.ToString();

        if (tmp.StartsWith("<?"))
        {
          int idx = tmp.IndexOf("?>");

          if (idx >= 0)
          {
            idx += 2;

            while (char.IsControl(tmp[idx]))
              idx++;

            tmp = tmp.Substring(idx);
          }
        }
        m_file[section_name] = tmp;
      }
    }

    private ConfigurationSection ReadSection(Type sectionType)
    {
      ConfigurationSection ret = null;
      string section_name;
      bool data_contract;

      _log.DebugFormat("ReadSection(): {0}", sectionType.FullName);

      section_name = this.GetSectionName(sectionType, out data_contract);
      string section_xml = null;

      if (m_file.TryGetSection(section_name, out section_xml))
      {
        using (var reader = new StringReader(section_xml))
        {
          var xml_reader = new XmlTextReader(reader);
          if (data_contract)
          {
            var sr = new DataContractSerializer(sectionType);
            try
            {
              ret = (ConfigurationSection)sr.ReadObject(xml_reader);
            }
            catch (Exception ex)
            {
              _log.Error("ReadSection(): exception", ex);
              ret = (ConfigurationSection)Activator.CreateInstance(sectionType);
              this.WriteSection(ret);
            }
          }
          else
          {
            var sr = new XmlSerializer(sectionType);
            try
            {
              ret = (ConfigurationSection)sr.Deserialize(xml_reader);
            }
            catch (Exception ex)
            {
              _log.Error("ReadSection(): exception", ex);
              ret = (ConfigurationSection)Activator.CreateInstance(sectionType);
              this.WriteSection(ret);
            }
          }
        }
      }
      else
      {
        ret = (ConfigurationSection)Activator.CreateInstance(sectionType);
        this.WriteSection(ret);
      }

      return ret;
    }

    private string GetSectionName(Type sectionType, out bool data_contract)
    {
      string section_name;
      data_contract = sectionType.IsDefined(typeof(DataContractAttribute), false);

      if (!data_contract)
      {
        var root = sectionType.GetCustomAttribute<XmlRootAttribute>();

        if (root == null || string.IsNullOrEmpty(root.ElementName))
          section_name = sectionType.Name;
        else
          section_name = root.ElementName;
      }
      else
        section_name = new XsdDataContractExporter().GetRootElementName(sectionType).Name;

      if (m_section_names.ContainsKey(section_name))
      {
        if (m_section_names[section_name] != sectionType)
          throw new SerializationException(string.Format(Resources.DUPLICATE_SECTION_NAME,
            m_section_names[section_name], sectionType));
      }
      else
        m_section_names[section_name] = sectionType;

      return section_name;
    }
  }
}