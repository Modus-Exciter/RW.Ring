using System;
using System.Runtime.Serialization;

namespace Notung.Net
{
  /// <summary>
  /// Заглушка, выполняющая команду локально
  /// </summary>
  public sealed class RemotableCallerStub<TService> : IRemotableCaller, IServiceProvider where TService : new()
  {
    public RemotableResult Call(IRemotableCommand command)
    {
      return command.Execute(this);
    }

    public object GetService(Type serviceType)
    {
      if (serviceType.IsAssignableFrom(typeof(TService)))
        return new TService();
      else
        return null;
    }
  }

  /// <summary>
  /// Объект для запуска команды удалённо
  /// </summary>
  public class RemoteCaller : IRemotableCaller
  {
    private readonly IClientTransportFactory m_transport_factory;
    private readonly ICommandSerializer m_serializer;

    public RemoteCaller(IClientTransportFactory transportFactory, ICommandSerializer serializer)
    {
      if (transportFactory == null)
        throw new ArgumentNullException("factory");

      if (serializer == null)
        throw new ArgumentNullException("serizlizer");

      m_transport_factory = transportFactory;
      m_serializer = serializer;
    }
    
    public RemotableResult Call(IRemotableCommand command)
    {
      command.Headers = HostedService.GlobalHeaders;
      
      using (var transport = m_transport_factory.Create())
      {
        transport.PrepareRequest(m_serializer.Format, command);
        m_serializer.WriteCommand(transport.RequestStream, command);
        transport.EndRequest();
        return m_serializer.ReadResult(transport.ResponseStream);
      }
    }
  }
}