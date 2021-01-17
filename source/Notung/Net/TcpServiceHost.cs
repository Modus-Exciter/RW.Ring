using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Notung.Threading;
using Notung.Loader;
using System.Threading;
using System.IO;

namespace Notung.Net
{
  public class TcpServiceHost : IDisposable
  {
    private Thread m_working_thread;
    private readonly object m_lock = new object();
    private readonly Socket m_socket;
    private readonly int m_listeners;
    private readonly Dictionary<string, ServerCaller> m_callers = new Dictionary<string, ServerCaller>();
    private readonly SharedLock m_callers_lock = new SharedLock(false);
    private readonly ISerializationFactory m_serialization;
    private IBinaryService m_binary_service;

    public TcpServiceHost(ISerializationFactory serializationFactory, EndPoint endPoint, int listeners)
    {
      if (serializationFactory == null)
        throw new ArgumentNullException("serializationFactory");

      if (endPoint == null)
        throw new ArgumentNullException("endPoint");

      m_serialization = serializationFactory;
      m_listeners = listeners;
      m_socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
      m_socket.Bind(endPoint);
    }

    public IBinaryService BinaryService
    {
      get { return m_binary_service; }
      set
      {
        using (m_callers_lock.WriteLock())
          m_binary_service = value;
      }
    }

    public void AddService<T>(IFactory<T> creator) where T : class
    {
      if (creator == null)
        throw new ArgumentNullException("creator");

      foreach (var contract in typeof(T).GetInterfaces())
      {
        if (!contract.IsDefined(typeof(RpcServiceAttribute), false))
          continue;

        using (m_callers_lock.WriteLock())
          m_callers.Add(RpcServiceInfo.Register(contract).ServiceName, new ServerCaller(creator));
      }

      using (m_callers_lock.WriteLock())
      {
        if (typeof(IBinaryService).IsAssignableFrom(typeof(T)) && m_binary_service == null)
          m_binary_service = new FactoryBinaryService(creator);
      }
    }

    public void Start()
    {
      lock (m_lock)
      {
        if (m_working_thread != null)
          throw new InvalidOperationException();
      }

      m_socket.Listen(m_listeners);

      lock (m_lock)
      {
        m_working_thread = new Thread(ListeningThread);
        m_working_thread.Start();
      }
    }
    
    public void Dispose()
    {
      lock (m_lock)
      {
        if (m_working_thread != null && m_working_thread.IsAlive)
        {
          m_working_thread.Abort();
          m_working_thread = null;
        }

        m_socket.Shutdown(SocketShutdown.Both);
        m_socket.Dispose();
      }
    }

    private void ListeningThread()
    {
      while (m_working_thread != null)
      {
        ThreadPool.QueueUserWorkItem(ProcessRequest, m_socket.Accept());
      }
    }

    private void ProcessRequest(object state)
    {
      using (var socket = (Socket)state)
      {
        using (var stream = new NetworkStream(socket))
        {
          var reader = new BinaryReader(stream);

          var command = reader.ReadString();

          switch (command.Substring(0, 2))
          {
            case "c:":
              ProcessCall(command.Substring(2), stream, socket);
              break;

            case "s:":
              break;

            case "b:":
              break;
          }

          socket.Shutdown(SocketShutdown.Both);
        }
      }
    }

    private ServerCaller GetCaller(string serviceName)
    {
      ServerCaller caller;

      using (m_callers_lock.ReadLock())
        caller = m_callers[serviceName];

      return caller;
    }

    private void ProcessCall(string command, NetworkStream stream, Socket socket)
    {
      var bits = command.Split('/');

      var info = RpcServiceInfo.GetByName(bits[0]).GetOperationInfo(bits[1]);

      var req_serializer = m_serialization.GetSerializer(info.RequestType);

      var result = GetCaller(bits[0]).Call(info, (IParametersList)req_serializer.Deserialize(stream));

      if (result != null)
      {
        var return_type = info.ResponseType;
        var serializer = m_serialization.GetSerializer(return_type);

        serializer.Serialize(stream, result);
      }
    }
  }
}
