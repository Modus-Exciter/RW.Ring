using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Notung.Network
{
  public interface ICommandSerializer
  {
    void Serialize<T>(Stream stream, T data);

    T Deserialize<T>(Stream stream);
  }

  public class BinaryCommandSerializer : ICommandSerializer
  {
    private readonly BinaryFormatter m_formatter = new BinaryFormatter();

    public void Serialize<T>(Stream stream, T data)
    {
      m_formatter.Serialize(stream, data);
    }

    public T Deserialize<T>(Stream stream)
    {
      return (T)m_formatter.Deserialize(stream);
    }
  }
}
