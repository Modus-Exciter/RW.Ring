using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using Notung.Threading;

namespace Notung.Net
{
  /// <summary>
  /// Сериализатор удалённых команд и результатов их выполнения
  /// </summary>
  public interface ICommandSerializer
  {
    /// <summary>
    /// Запись команды в поток ввода-вывода
    /// </summary>
    /// <param name="stream">Поток ввода-вывода</param>
    /// <param name="command">Команда</param>
    void WriteCommand(Stream stream, IRemotableCommand command);

    /// <summary>
    /// Запись результата выполнения команды в поток ввода-вывода
    /// </summary>
    /// <param name="stream">Поток ввода-вывода</param>
    /// <param name="result">Результат выполнения команды</param>
    void WriteResult(Stream stream, RemotableResult result);

    /// <summary>
    /// Чтение команды из потока
    /// </summary>
    /// <param name="stream">Поток ввода-вывода</param>
    /// <returns>Прочитанная команда</returns>
    IRemotableCommand ReadCommand(Stream stream);

    /// <summary>
    /// Чтение результата выполнения команды из потока ввода-вывода
    /// </summary>
    /// <param name="stream">Поток ввода-вывода</param>
    /// <returns>Результат выполнения команды</returns>
    RemotableResult ReadResult(Stream stream);

    /// <summary>
    /// Формат, в котором сериализуются команды
    /// </summary>
    CommandSerializationFormat Format { get; }
  }

  /// <summary>
  /// Формат, в котором сериализуются команды
  /// </summary>
  public enum CommandSerializationFormat : byte
  {
    Binary,
    Xml,
    JSON
  }

  /// <summary>
  /// Бинарный сериализатор удалённых команд
  /// </summary>
  public class BinaryCommandSerializer : ICommandSerializer
  {
    private readonly BinaryFormatter m_formatter = new BinaryFormatter();

    public void WriteCommand(Stream stream, IRemotableCommand command)
    {
      m_formatter.Serialize(stream, command);
    }

    public void WriteResult(Stream stream, RemotableResult result)
    {
      m_formatter.Serialize(stream, result);
    }

    public IRemotableCommand ReadCommand(Stream stream)
    {
      return (IRemotableCommand)m_formatter.Deserialize(stream);
    }

    public RemotableResult ReadResult(Stream stream)
    {
      return (RemotableResult)m_formatter.Deserialize(stream);
    }

    public CommandSerializationFormat Format
    {
      get { return CommandSerializationFormat.Binary; }
    }
  }

  /// <summary>
  /// Сериализатор удалённых команд на основе контрактов данных
  /// </summary>
  public class DataContractCommandSerializer : ICommandSerializer
  {
    private readonly Dictionary<Type, DataContractSerializer> m_seriazlizers = new Dictionary<Type, DataContractSerializer>();
    private readonly SharedLock m_lock = new SharedLock(false);

    private DataContractSerializer GetSerializer(Type type)
    {
      DataContractSerializer ret;

      using (m_lock.ReadLock())
      {
        if (m_seriazlizers.TryGetValue(type, out ret))
          return ret;
      }

      using (m_lock.WriteLock())
      {
        if (!m_seriazlizers.TryGetValue(type, out ret))
        {
          ret = new DataContractSerializer(type);
          m_seriazlizers.Add(type, ret);
        }
      }

      return ret;
    }

    private void Write(Stream stream, object item)
    {
      StreamWriter sw = new StreamWriter(stream);
      sw.WriteLine("<!--{0}-->", item.GetType().AssemblyQualifiedName);
      sw.Flush();
      GetSerializer(item.GetType()).WriteObject(stream, item);
    }

    private object Read(Stream stream)
    {
      StreamReader sr = new StreamReader(stream);
      string type_name = sr.ReadLine();

      if (type_name.StartsWith("<!--"))
        type_name = type_name.Substring("<!--".Length, type_name.Length - "<!--".Length - "-->".Length);

      return GetSerializer(Type.GetType(type_name)).ReadObject(new XmlTextReader(sr));
    }

    public void WriteCommand(Stream stream, IRemotableCommand command)
    {
      Write(stream, command);
    }

    public void WriteResult(Stream stream, RemotableResult result)
    {
      Write(stream, result);
    }

    public IRemotableCommand ReadCommand(Stream stream)
    {
      return (IRemotableCommand)Read(stream);
    }

    public RemotableResult ReadResult(Stream stream)
    {
      return (RemotableResult)Read(stream);
    }

    public CommandSerializationFormat Format
    {
      get { return CommandSerializationFormat.Xml; }
    }
  }
}