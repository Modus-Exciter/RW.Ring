using System;
using System.Runtime.Serialization;

namespace Notung.Net
{
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
    
    public RemotableResult1 Call(IRemotableCommand command)
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