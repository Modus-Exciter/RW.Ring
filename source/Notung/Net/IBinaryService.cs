using System;
using System.IO;
using Notung.Loader;

namespace Notung.Net
{
  /// <summary>
  /// Сервис для обмена потоками и бинарными массивами между клиентом и сервером
  /// </summary>
  public interface IBinaryService
  {
    /// <summary>
    /// Обмен потоками
    /// </summary>
    /// <param name="request">Поток данных, полученный от клиента</param>
    /// <param name="response">Поток данных для отправки клиенту</param>
    void StreamExchange(Stream request, Stream response);

    /// <summary>
    /// Обмен массивами байт
    /// </summary>
    /// <param name="data">Массив байт, полученный от клиента</param>
    /// <returns>Массив байт для отправки клиенту</returns>
    byte[] BinaryExchange(byte[] data);
  }

  internal class FactoryBinaryService : IBinaryService
  {
    private readonly IFactory<object> m_factory;

    public FactoryBinaryService(IFactory<object> factory)
    {
      if (factory == null)
        throw new ArgumentNullException("factory");

      m_factory = factory;
    }

    public void StreamExchange(Stream request, Stream response)
    {
      ((IBinaryService)m_factory.Create()).StreamExchange(request, response);
    }

    public byte[] BinaryExchange(byte[] data)
    {
      return ((IBinaryService)m_factory.Create()).BinaryExchange(data);
    }
  }
}
