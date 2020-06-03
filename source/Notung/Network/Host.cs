using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Notung.Loader;

namespace Notung.Network
{
  public class Host<TContract, TService> : IDisposable
    where TContract : class
    where TService : class, TContract, new()
  {
    private readonly HostedService<TContract, TService>[] m_services;
    private readonly Thread[] m_threads;

    public Host(params HostedService<TContract, TService>[] services)
    {
      if (services == null)
        throw new ArgumentNullException("services");

      m_services = services.Where(s => s != null).ToArray();
      m_threads = new Thread[m_services.Length];

      for (int i = 0; i < m_services.Length; i++)
        m_threads[i] = new Thread(m_services[i].Listen);
    }

    public Host(IServerTransportFactory transportFactory, ICommandSerializer serializer)
      : this(new HostedService<TContract, TService>(transportFactory, serializer)) { }

    public void Start()
    {
      for (int i = 0; i < m_services.Length; i++)
        m_threads[i].Start();
    }

    public void Dispose()
    {
      for (int i = 0; i < m_services.Length; i++)
      {
        m_services[i].Dispose();
        m_threads[i].Abort();
      }
    }
  }

  public class HostedService<TContract, TService> : IDisposable
    where TContract : class
    where TService : class, TContract, new()
  {
    private readonly IServerTransportFactory m_transport_factory;
    private readonly ICommandSerializer m_serializer;

    public HostedService(IServerTransportFactory transportFactory, ICommandSerializer serializer)
    {
      if (transportFactory == null)
        throw new ArgumentNullException("factory");

      if (serializer == null)
        throw new ArgumentNullException("serizlizer");

      m_transport_factory = transportFactory;
      m_serializer = serializer;
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
        var command = m_serializer.Deserialize<IRemotableCommand>(transport.RequestStream);

        m_serializer.Serialize<RemotableResult>(transport.ResponseStream, command.Execute(
          new CommandContext { HostType = typeof(TService) }));

        transport.EndResponse();
      }
    }

    public void Dispose()
    {
      m_transport_factory.Dispose();
    }
  }
}
