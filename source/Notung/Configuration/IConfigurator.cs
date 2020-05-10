using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Notung.ComponentModel;
using Notung.Log;
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

  public sealed class DataContractConfigurator : IConfigurator
  {
    private readonly Dictionary<Type, ConfigurationSection> m_sections = new Dictionary<Type, ConfigurationSection>();
    private readonly Dictionary<string, Type> m_section_names = new Dictionary<string, Type>();
    private readonly LockSource m_lock = new LockSource();
    private readonly ConfigurationFile m_file;

    private static readonly ILog _log = LogManager.GetLogger(typeof(DataContractConfigurator));

    public DataContractConfigurator(ConfigurationFile file)
    {
      if (file == null)
        throw new ArgumentNullException("file");

      m_file = file;
    }

    public DataContractConfigurator() : this(new ConfigurationFile()) { }

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
          section = this.CreateSection(sectionType);
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
          ret = CreateSection(typeof(TSection));

          InfoBuffer buffer = new InfoBuffer();

          m_sections.Add(typeof(TSection), ret);
          PushSection(ret);
        }
      }

      return (TSection)ret;
    }

    public void SaveSection<TSection>(TSection section)
      where TSection : ConfigurationSection, new()
    {
      if (section == null)
        throw new ArgumentNullException("section");

      using (m_lock.WriteLock())
      {
        m_sections[typeof(TSection)] = section;
        PushSection(section);
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
        {
          PushSection(kv.Value);
        }
        m_file.Save();
      }
    }

    private void PushSection(ConfigurationSection section)
    {
      string section_name;
      bool data_contract;

      _log.DebugFormat("PushSection(): {0}", section.GetType().FullName);

      section_name = GetSectionName(section.GetType(), out data_contract);

      var sb = new StringBuilder();

      using (var sw = new StringWriter(sb))
      {
        using (var writer = new XmlTextWriter(sw) { Formatting = Formatting.Indented, IndentChar = '\t', Indentation = 1, })
        {
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
        }
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

    private ConfigurationSection CreateSection(Type sectionType)
    {
      if (sectionType == null) throw new ArgumentNullException("sectionType");

      ConfigurationSection ret = null;

      string section_name;
      bool data_contract;

      _log.DebugFormat("CreateSection(): {0}", sectionType.FullName);

      section_name = GetSectionName(sectionType, out data_contract);

      if (m_section_names.ContainsKey(section_name))
      {
        if (m_section_names[section_name] != sectionType)
          throw new SerializationException(string.Format(Resources.DUPLICATE_SECTION_NAME, 
            m_section_names[section_name], sectionType));
      }
      else
        m_section_names[section_name] = sectionType;

      string section_xml = null;

      if (m_file.TryGetSection(section_name, out section_xml))
      {
        using (var reader = new StringReader(section_xml))
        {
          using (var xml_reader = new XmlTextReader(reader))
          {
            if (data_contract)
            {
              var sr = new DataContractSerializer(sectionType);
              try
              {
                ret = (ConfigurationSection)sr.ReadObject(xml_reader);
              }
              catch (Exception ex)
              {
                _log.Error("CreateSection(): exception", ex);
                ret = (ConfigurationSection)Activator.CreateInstance(sectionType);
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
                _log.Error("CreateSection(): exception", ex);
                ret = (ConfigurationSection)Activator.CreateInstance(sectionType);
              }
            }
          }
        }
      }
      else
        ret = (ConfigurationSection)Activator.CreateInstance(sectionType);

      return ret;
    }

    private static string GetSectionName(Type sectionType, out bool data_contract)
    {
      if (sectionType == null) throw new ArgumentNullException("sectionType");

      string section_name;
      data_contract = sectionType.IsDefined(typeof(DataContractAttribute), false);
      if (data_contract)
      {
        section_name = new XsdDataContractExporter().GetRootElementName(sectionType).Name;
      }
      else
      {
        var root = sectionType.GetCustomAttribute<XmlRootAttribute>(false);

        if (root == null || string.IsNullOrEmpty(root.ElementName))
          section_name = sectionType.Name;
        else
          section_name = root.ElementName;
      }
      return section_name;
    }
  }
}