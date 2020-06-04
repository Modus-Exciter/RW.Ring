using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Notung.Loader;

namespace Notung.Net
{
  /// <summary>
  /// Транспорт запроса между двумя конечными точками
  /// </summary>
  public interface ITransport : IDisposable
  {
    /// <summary>
    /// Поток для чтения или записи данных запроса к серверу
    /// </summary>
    Stream RequestStream { get; }

    /// <summary>
    /// Поток для чтения или записи ответа от сервера
    /// </summary>
    Stream ResponseStream { get; }

    /// <summary>
    /// Подготовка запроса к серверу
    /// </summary>
    /// <param name="format">Формат данных, в которых будет сохранён запрос</param>
    void PrepareRequest(CommandSerializationFormat format);

    /// <summary>
    /// Завершение запроса к серверу
    /// </summary>
    void EndRequest();

    /// <summary>
    /// Подготовка ответа от сервера
    /// </summary>
    /// <param name="format">Формат данных, в которых будет сохранён ответ</param>
    void PrepareResponse(CommandSerializationFormat format);

    /// <summary>
    /// Завершение ответа от сервера
    /// </summary>
    void EndResponse();
  }

  /// <summary>
  /// Фабрика для создания объектов транспорта в ходе работы прокси на клиенте
  /// </summary>
  public interface IClientTransportFactory : IFactory<ITransport> { }

  /// <summary>
  /// Фабрика для создания объектов транспорта в ходе работы хоста на сервере
  /// </summary>
  public interface IServerTransportFactory : IFactory<ITransport>, IDisposable { }

  /// <summary>
  /// Транспорт на основе сокетов с передачей данных через двусторонний поток ввода-вывода
  /// </summary>
  public sealed class StreamSocketTransport : ITransport
  {
    private readonly Socket m_socket;
    private readonly NetworkStream m_stream;

    public StreamSocketTransport(Socket socket)
    {
      if (socket == null)
        throw new ArgumentNullException("socket");

      m_socket = socket;
      m_stream = new NetworkStream(socket);
    }
    
    public Stream RequestStream
    {
      get { return m_stream; }
    }

    public Stream ResponseStream
    {
      get { return m_stream; }
    }

    public void PrepareRequest(CommandSerializationFormat format) { }

    public void EndRequest()
    {
      m_socket.Shutdown(SocketShutdown.Send);
    }

    public void PrepareResponse(CommandSerializationFormat format) { }

    public void EndResponse()
    {
      m_socket.Shutdown(SocketShutdown.Both);
    }

    public void Dispose()
    {
      m_stream.Dispose();
      m_socket.Dispose();
    }

    public static AddressFamily AddressFamily = AddressFamily.InterNetwork;
  }

  /// <summary>
  /// Фабрика для транспорта на основе сокетов с потоковой передачей для клиентской стороны
  /// </summary>
  public class ClientStreamSocketTransportFactory : IClientTransportFactory
  {
    private readonly EndPoint m_endpoint;

    public ClientStreamSocketTransportFactory(EndPoint endPoint)
    {
      if (endPoint == null)
        throw new ArgumentNullException("endPoint");

      m_endpoint = endPoint;
    }

    public ITransport Create()
    {
      var socket = new Socket(StreamSocketTransport.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
      socket.Connect(m_endpoint);

      return new StreamSocketTransport(socket);
    }
  }

  /// <summary>
  /// Фабрика для транспорта на основе сокетов с потоковой передачей для серверной стороны
  /// </summary>
  public class ServerStreamSocketTransportFactory : IServerTransportFactory
  {
    private readonly Socket m_socket;

    public ServerStreamSocketTransportFactory(EndPoint endPoint, int listeners)
    {
      if (endPoint == null)
        throw new ArgumentNullException("endPoint");

      m_socket = new Socket(StreamSocketTransport.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
      m_socket.Bind(endPoint);
      m_socket.Listen(listeners);
    }

    public ITransport Create()
    {
      return new StreamSocketTransport(m_socket.Accept());
    }

    public void Dispose()
    {
      m_socket.Dispose();
    }
  }
}