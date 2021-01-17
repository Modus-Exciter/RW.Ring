using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Notung.Net
{
  public class TcpClientCaller : IClientCaller
  {
    private readonly EndPoint m_endpoint;
    private readonly ISerializationFactory m_factory;

    public TcpClientCaller(EndPoint endPoint, ISerializationFactory serializationFactory)
    {
      if (endPoint == null)
        throw new ArgumentNullException("endPoint");

      if (serializationFactory == null)
        throw new ArgumentNullException("serializationFactory");

      m_endpoint = endPoint;
      m_factory = serializationFactory;
    }

    public ICallResult Call(string serverOperation, IParametersList request, RpcOperationInfo operation)
    {
      using (var socket = new Socket(m_endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
      {
        socket.Connect(m_endpoint);

        using (var stream = new NetworkStream(socket))
        {
          var writer = new BinaryWriter(stream);

          writer.Write(string.Format("A:{0},U:{1},M:{2}", 
            ClientInfo.ProcessInfo.Application,
            ClientInfo.ProcessInfo.UserName,
            ClientInfo.ProcessInfo.MachineName));

          writer.Write(string.Format("c:{0}", serverOperation));

          m_factory.GetSerializer(request.GetType()).Serialize(stream, request);
          socket.Shutdown(SocketShutdown.Send);

          return (ICallResult)m_factory.GetSerializer(operation.ResponseType).Deserialize(stream);
        }
      }
    }

    public void StreamExchange(string command, Action<Stream> processRequest, Action<Stream> processResponse)
    {
      using (var socket = new Socket(m_endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
      {
        socket.Connect(m_endpoint);

        using (var stream = new NetworkStream(socket))
        {
          var writer = new BinaryWriter(stream);

          writer.Write(string.Format("A:{0},U:{1},M:{2}",
            ClientInfo.ProcessInfo.Application,
            ClientInfo.ProcessInfo.UserName,
            ClientInfo.ProcessInfo.MachineName));

          writer.Write(string.Format("s:{0}", command));

          processRequest(stream);
          socket.Shutdown(SocketShutdown.Send);

          processResponse(stream);
        }
      }
    }

    public byte[] BinaryExchange(string command, byte[] data)
    {
      using (var socket = new Socket(m_endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
      {
        socket.Connect(m_endpoint);

        using (var stream = new NetworkStream(socket))
        {
          var writer = new BinaryWriter(stream);

          writer.Write(string.Format("A:{0},U:{1},M:{2}",
            ClientInfo.ProcessInfo.Application,
            ClientInfo.ProcessInfo.UserName,
            ClientInfo.ProcessInfo.MachineName));

          writer.Write(string.Format("b:{0}", command));

          stream.Write(data, 0, data.Length);
          socket.Shutdown(SocketShutdown.Send);

          List<byte> result = new List<byte>();
          byte[] buffer = new byte[512];
          int count;

          while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
          {
            for (int i = 0; i < count; i++)
              result.Add(buffer[i]);
          }

          return result.ToArray();
        }
      }
    }
  }
}
