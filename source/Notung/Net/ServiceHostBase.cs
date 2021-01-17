using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Notung.Loader;
using Notung.Threading;

namespace Notung.Net
{
  public abstract class ServiceHostBase : IDisposable
  {
    private Thread m_working_thread;
    private IBinaryService m_binary_service;
    protected readonly object m_lock = new object();
    private readonly Dictionary<string, ServerCaller> m_callers = new Dictionary<string, ServerCaller>();
    private readonly SharedLock m_callers_lock = new SharedLock(false);
    private readonly ISerializationFactory m_serialization;

    protected ServiceHostBase(ISerializationFactory serializationFactory)
    {
      if (serializationFactory == null)
        throw new ArgumentNullException("serializationFactory");

      m_serialization = serializationFactory;
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

      this.StartListener();

      lock (m_lock)
      {
        m_working_thread = new Thread(ListeningThread);
        m_working_thread.Start();
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;

      lock (m_lock)
      {
        if (m_working_thread != null && m_working_thread.IsAlive)
        {
          m_working_thread.Abort();
          m_working_thread = null;
        }
      }
    }

    private void ListeningThread()
    {
      while (m_working_thread != null)
      {
        ThreadPool.QueueUserWorkItem(ProcessRequest, GetState());
      }
    }

    protected SerializationFormat SerializationFormat
    {
      get { return m_serialization.Format; }
    }

    protected ISerializer GetSerializer(Type serializationType)
    {
      return m_serialization.GetSerializer(serializationType);
    }

    protected ServerCaller GetCaller(string serviceName)
    {
      ServerCaller caller;

      using (m_callers_lock.ReadLock())
        caller = m_callers[serviceName];

      return caller;
    }

    protected bool RunBinaryService(string command, Action<string, IBinaryService, object> action, object context)
    {
      using (m_callers_lock.ReadLock())
      {
        if (m_binary_service != null)
        {
          action(command, m_binary_service, context);
          return true;
        }
        else
          return false;
      }
    }

    protected bool StreamExchange(string command, Stream input, Stream output)
    {
      using (m_callers_lock.ReadLock())
      {
        if (m_binary_service != null)
        {
          m_binary_service.StreamExchange(PrepareCommand(command), input, output);

          return true;
        }
        else
          return false;
      }
    }

    protected bool BinaryExchange(string command, Stream input, Stream output)
    {
      using (m_callers_lock.ReadLock())
      {
        if (m_binary_service != null)
        {
          var ret = m_binary_service.BinaryExchange(PrepareCommand(command), ReadFromStream(input));
          output.Write(ret, 0, ret.Length);

          return true;
        }
        else
          return false;
      }
    }

    protected byte[] ReadFromStream(Stream stream)
    {
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

    protected virtual string PrepareCommand(string command)
    {
      return command;
    }

    protected abstract object GetState();

    protected abstract void ProcessRequest(object state);

    protected abstract void StartListener();
  }
}