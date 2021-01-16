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
    /// <param name="command">Команда, которую нужно выполнить над бинарными данными</param>
    /// <param name="request">Поток данных, полученный от клиента</param>
    /// <param name="response">Поток данных для отправки клиенту</param>
    void StreamExchange(string command, Stream request, Stream response);

    /// <summary>
    /// Обмен массивами байт
    /// </summary>
    /// <param name="command">Команда, которую нужно выполнить над бинарными данными</param>
    /// <param name="data">Массив байт, полученный от клиента</param>
    /// <returns>Массив байт для отправки клиенту</returns>
    byte[] BinaryExchange(string command, byte[] data);
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

    public void StreamExchange(string command, Stream request, Stream response)
    {
      ((IBinaryService)m_factory.Create()).StreamExchange(command, request, response);
    }

    public byte[] BinaryExchange(string command, byte[] data)
    {
      return ((IBinaryService)m_factory.Create()).BinaryExchange(command, data);
    }
  }

  /// <summary>
  /// Сервис для выполнения удалённых команд на сервере
  /// </summary>
  public class BinaryCommandService : IBinaryService
  {
    private readonly ISerializationFactory m_factory;
    private readonly IServiceProvider m_service_provider;

    public BinaryCommandService(ISerializationFactory factory, IServiceProvider provider = null)
    {
      if (factory == null)
        throw new ArgumentNullException("factory");

      m_factory = factory;
      m_service_provider = provider;
    }

    public void StreamExchange(string command, Stream request, Stream response)
    {
      var serializer = m_factory.GetSerializer(Type.GetType(command));
      var cmd = (IRemotableCommand)serializer.Deserialize(request);

      ClientInfo.ThreadInfo = cmd.Headers;
      try
      {
        var result = cmd.Execute(m_service_provider);

        serializer = m_factory.GetSerializer(result.GetType());
        serializer.Serialize(response, result);
      }
      finally
      {
        ClientInfo.ThreadInfo = null;
      }
    }

    public byte[] BinaryExchange(string command, byte[] data)
    {
      using (var ms = new MemoryStream(data))
      {
        using (var ms2 = new MemoryStream())
        {
          this.StreamExchange(command, ms, ms2);
          return ms2.ToArray();
        }
      }
    }
  }
}