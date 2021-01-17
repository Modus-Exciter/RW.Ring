using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Notung.Logging;

namespace Notung.Net
{
  public class TcpServiceHost : ServiceHostBase
  {
    private readonly Socket m_socket;
    private readonly int m_listeners;

    private static readonly ILog _log = LogManager.GetLogger(typeof(TcpServiceHost));

    public TcpServiceHost(ISerializationFactory serializationFactory, EndPoint endPoint, int listeners)
      : base(serializationFactory)
    {
      if (endPoint == null)
        throw new ArgumentNullException("endPoint");

      m_listeners = listeners;
      m_socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
      m_socket.Bind(endPoint);
    }

    #region Override ------------------------------------------------------------------------------

    protected override void StartListener()
    {
      m_socket.Listen(m_listeners);
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);

      lock (m_lock)
      {
        m_socket.Dispose();
      }
    }

    protected override object GetState()
    {
      return m_socket.Accept();
    }

    protected override void ProcessRequest(object state)
    {
      using (var socket = (Socket)state)
      {
        using (var stream = new NetworkStream(socket))
        {
          var reader = new BinaryReader(stream);

          ClientInfo.ThreadInfo = ParseClientInfo(reader.ReadString());

          var command = reader.ReadString();

          LogCommand(command);

          try
          {
            switch (command.Substring(0, 2))
            {
              case "c:":
                ProcessCall(command.Substring(2), stream, socket);
                break;

              case "s:":
                StreamExchange(command.Substring(2), stream, stream);
                break;

              case "b:":
                BinaryExchange(command.Substring(2), stream, stream);
                break;
            }
          }
          catch (Exception ex)
          {
            _log.Error("ProcessRequest(): exception", ex);

            if (!command.StartsWith("c:"))
              this.GetSerializer(typeof(ClientServerException)).Serialize(stream,
                new ClientServerException(ex.Message, ex.StackTrace));
          }
          finally
          {
            ClientInfo.ThreadInfo = null;
            socket.Shutdown(SocketShutdown.Both);
          }
        }
      }
    }

    #endregion

    private ClientInfo ParseClientInfo(string value)
    {
      var bits = value.Split(',');

      if (bits.Length != 3)
        return null;

      if (bits[0].StartsWith("A:") && bits[1].StartsWith("U:") && bits[2].StartsWith("M:"))
      {
        return new ClientInfo
        {
          Application = bits[0].Substring(2),
          UserName = bits[1].Substring(2),
          MachineName = bits[2].Substring(2)
        };
      }
      else
        return null;
    }

    private void LogCommand(string command)
    {
      if (ClientInfo.ThreadInfo != null)
      {
        command = string.Format("{0}{1}User: {2}, Application: {3}, Machine: {4}",
           command, Environment.NewLine,
           ClientInfo.ThreadInfo.UserName,
           ClientInfo.ThreadInfo.Application,
           ClientInfo.ThreadInfo.MachineName);
      }

      _log.Info(command);
    }

    private void ProcessCall(string command, NetworkStream stream, Socket socket)
    {
      var bits = command.Split('/');
      var info = RpcServiceInfo.GetByName(bits[0]).GetOperationInfo(bits[1]);

      var result = GetCaller(bits[0]).Call(info,
        (IParametersList)this.GetSerializer(info.RequestType).Deserialize(stream));

      if (result != null)
        this.GetSerializer(info.ResponseType).Serialize(stream, result);
    }
  }
}