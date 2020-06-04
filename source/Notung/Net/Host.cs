using System;
using System.Linq;
using System.Threading;
using Notung.Logging;
using System.Reflection;
using System.Text;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;

namespace Notung.Net
{
  /// <summary>
  /// Базовый хост, позволяющий выполнять удалённые команды
  /// </summary>
  public class Host : IDisposable
  {
    private readonly HostedService[] m_services;
    private readonly Thread[] m_threads;

    public Host(params HostedService[] services)
    {
      if (services == null)
        throw new ArgumentNullException("services");

      m_services = services.Where(s => s != null).ToArray();
      m_threads = new Thread[m_services.Length];

      for (int i = 0; i < m_services.Length; i++)
        m_threads[i] = new Thread(m_services[i].Listen);
    }

    public Host(IServerTransportFactory transportFactory, ICommandSerializer serializer)
      : this(new HostedService(transportFactory, serializer)) { }

    public void Start()
    {
      for (int i = 0; i < m_services.Length; i++)
        m_threads[i].Start();
    }

    public void Dispose()
    {
      for (int i = 0; i < m_services.Length; i++)
      {
        m_threads[i].Abort();
        m_services[i].Dispose();
      }
    }
  }

  /// <summary>
  /// Сервис, выполняющий удалённые команды
  /// </summary>
  public class HostedService : IDisposable, IServiceProvider
  {
    private readonly IServerTransportFactory m_transport_factory;
    private readonly ICommandSerializer m_serializer;
    private Type m_service_type;
    private SessionManager m_session_manager;

    [ThreadStatic]
    private static ProcessHeaders _command_headers;
    private static readonly ProcessHeaders _global_headers = new ProcessHeaders
    {
      Application = (Assembly.GetEntryAssembly() ?? typeof(HostedService).Assembly).GetName().Name,
      MachineName = Environment.MachineName,
      UserName = Environment.UserName
    };

    public HostedService(IServerTransportFactory transportFactory, ICommandSerializer serializer)
    {
      if (transportFactory == null)
        throw new ArgumentNullException("factory");

      if (serializer == null)
        throw new ArgumentNullException("serizlizer");

      m_transport_factory = transportFactory;
      m_serializer = serializer;
    }

    public Type ServiceType
    {
      get { return m_service_type; }
      set
      {
        if (m_service_type == value)
          return;

        m_service_type = value;

        if (m_service_type != null)
          m_session_manager = new SessionManager(m_service_type);
      }
    }

    public static ProcessHeaders GlobalHeaders
    {
      get { return _global_headers; }
    }

    public static ProcessHeaders CommandHeaders
    {
      get { return _command_headers; }
    }

    public static ProcessHeaders CurrentHeaders
    {
      get { return _command_headers ?? _global_headers; }
    }

    public void Listen()
    {
      while (true)
        ThreadPool.QueueUserWorkItem(Accept, m_transport_factory.Create());
    }

    private void Accept(object state)
    {
      using (var transport = (ITransport)state)
      {
        var command = m_serializer.ReadCommand(transport.RequestStream);

        _command_headers = command.Headers;

        if (_command_headers != null)
        {
          LoggingContext.Thread["Application"] = _command_headers.Application;
          LoggingContext.Thread["UserName"] = _command_headers.UserName;
          LoggingContext.Thread["MachineName"] = _command_headers.MachineName;
        }

        try
        {
          var result = command.Execute(this);

          transport.PrepareResponse(m_serializer.Format, result);

          m_serializer.WriteResult(transport.ResponseStream, result);
          transport.EndResponse();
        }
        finally
        {
          if (_command_headers != null)
          {
            _command_headers = null;
            LoggingContext.Thread.Clear();
          }
        }
      }
    }

#if DEBUG

    private static void WriteToConsole(IRemotableCommand command, RemotableResult result)
    {
      var xml_writer = new XmlTextWriter(Console.Out);
      {
        var ser = new DataContractSerializer(command.GetType());
        xml_writer.Formatting = Formatting.Indented;
        ser.WriteObject(xml_writer, command);

        Console.WriteLine();

        ser = new DataContractSerializer(result.GetType());
        ser.WriteObject(xml_writer, result);

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
      }
    }

#endif

    public void Dispose()
    {
      m_transport_factory.Dispose();

      if (m_session_manager != null)
        m_session_manager.Dispose();
    }

    public object GetService(Type serviceType)
    {
      if (serviceType == typeof(ISessionManager))
        return m_session_manager;      
      if (serviceType.IsAssignableFrom(this.GetType()))
        return this;
      else if (this.ServiceType != null && serviceType.IsAssignableFrom(this.ServiceType))
        return Activator.CreateInstance(this.ServiceType);
      else
        return null;
    }
  }

  /// <summary>
  /// Хост, позволяющий выполнять удалённый вызов методов определённого сервиса
  /// </summary>
  /// <typeparam name="TService">Тип сервиса, реализующего определённый контракт</typeparam>
  public class Host<TService> : Host
  {
    public Host(params HostedService[] services) : base(services)
    {
      foreach (var service in services)
      {
        if (service != null)
          service.ServiceType = typeof(TService);
      }
    }

    public Host(IServerTransportFactory transportFactory, ICommandSerializer serializer)
      : this(new HostedService(transportFactory, serializer)) { }
  }
}