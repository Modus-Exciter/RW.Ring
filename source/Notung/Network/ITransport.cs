using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net.Sockets;
using Notung.Loader;
using System.Net;

namespace Notung.Network
{
  public interface ITransport : IDisposable
  {
    Stream RequestStream { get; }

    Stream ResponseStream { get; }

    void EndRequest();

    void EndResponse();
  }

  public interface IServerTransportFactory : IFactory<ITransport>, IDisposable { }

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

    public void EndRequest()
    {
      m_socket.Shutdown(SocketShutdown.Send);
    }

    public void EndResponse()
    {
      m_socket.Shutdown(SocketShutdown.Both);
    }

    public void Dispose()
    {
      m_stream.Dispose();
      m_socket.Dispose();
    }
  }

  public class ClientStreamSocketTransportFactory : IFactory<ITransport>
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
      var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      socket.Connect(m_endpoint);

      return new StreamSocketTransport(socket);
    }
  }

  public class ServerStreamSocketTransportFactory : IServerTransportFactory
  {
    private readonly Socket m_socket;

    public ServerStreamSocketTransportFactory(EndPoint endPoint, int listeners)
    {
      if (endPoint == null)
        throw new ArgumentNullException("endPoint");

      m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
