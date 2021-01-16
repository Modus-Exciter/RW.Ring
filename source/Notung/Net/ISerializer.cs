using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Notung.Net
{
  /// <summary>
  /// Сериализатор данных
  /// </summary>
  public interface ISerializer
  {
    object Deserialize(Stream serializationStream);

    void Serialize(Stream serializationStream, object graph);
  }

  /// <summary>
  /// Фабрика, от которой сервис получает сериализаторы
  /// </summary>
  public interface ISerializationFactory
  {
    /// <summary>
    /// Получение сериализатора для указанного типа данных
    /// </summary>
    /// <param name="typeToSerialize">Тип, который требуется сериализовывать</param>
    /// <returns>Сериализатор для этого типа</returns>
    ISerializer GetSerializer(Type typeToSerialize);

    /// <summary>
    /// Формат, в котором сериализуются команды
    /// </summary>
    SerializationFormat Format { get; }
  }

  /// <summary>
  /// Формат, в котором сериализуются команды
  /// </summary>
  public enum SerializationFormat : byte
  {
    Binary,
    Xml,
    JSON
  }

  public class DataContractSerializationFactory : ISerializationFactory
  {
    public ISerializer GetSerializer(Type typeToSerialize)
    {
      if (typeToSerialize == null)
        throw new ArgumentNullException("typeToSerialize");

      return new DataContractSerializerImpl(typeToSerialize);
    }

    public SerializationFormat Format
    {
      get { return SerializationFormat.JSON; }
    }

    private class DataContractSerializerImpl : ISerializer
    {
      private readonly DataContractJsonSerializer m_serializer;

      public DataContractSerializerImpl(Type type)
      {
        m_serializer = new DataContractJsonSerializer(type);
      }

      public object Deserialize(Stream serializationStream)
      {
        return m_serializer.ReadObject(serializationStream);
      }

      public void Serialize(Stream serializationStream, object graph)
      {
        m_serializer.WriteObject(serializationStream, graph);
      }
    }
  }
}
