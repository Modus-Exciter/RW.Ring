using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Notung.Net
{
  public interface ISerializer
  {
    object Deserialize(Stream serializationStream);

    void Serialize(Stream serializationStream, object graph);
  }

  public interface ISerializationFactory
  {
    ISerializer GetSerializer(Type typeToSerialize);
  }

  public class DataContractSerializationFactory : ISerializationFactory
  {
    public ISerializer GetSerializer(Type typeToSerialize)
    {
      if (typeToSerialize == null)
        throw new ArgumentNullException("typeToSerialize");

      return new DataContractSerializerImpl(typeToSerialize);
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
