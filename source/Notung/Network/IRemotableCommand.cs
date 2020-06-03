using System;
using System.Runtime.Remoting.Messaging;
using Notung.Loader;

namespace Notung.Network
{
  /// <summary>
  /// Команда, которую можно выполнить удалённо
  /// </summary>
  public interface IRemotableCommand
  {
    /// <summary>
    /// Выполнение команды
    /// </summary>
    /// <param name="context">Контекст выполнения команды</param>
    /// <returns>Результат выполнения команды</returns>
    RemotableResult Execute(CommandContext context);
  }

  /// <summary>
  /// Контекст выполнения команды
  /// </summary>
  public class CommandContext
  {
    /// <summary>
    /// Тип хоста, выполняющего команду
    /// </summary>
    public Type HostType { get; set; }
  }

  [Serializable]
  public class RemotableResult
  {
    public object ReturnValue { get; internal set; }

    public object[] RefParameters { get; internal set; }

    public Exception Exception { get; internal set; }

    public MethodCallResultType Type { get; internal set; }
  }

  [Serializable]
  public enum MethodCallResultType
  {
    Success,
    Error,
    Callback
  }

  /// <summary>
  /// Обмен сообщениями с сервером
  /// </summary>
  /// <typeparam name="T">Тип контракта сервиса, с которому относятся сообщения</typeparam>
  public interface IRemotableCaller
  {
    /// <summary>
    /// Выполнение команды, потенциально удалённой
    /// </summary>
    /// <param name="command">Выполняемая команда</param>
    /// <returns>Результат выполнения командыы</returns>
    RemotableResult Call(IRemotableCommand command);
  }

  public sealed class RemotableCallerStub<TService> : IRemotableCaller
  {
    public RemotableResult Call(IRemotableCommand command)
    {
      return command.Execute(new CommandContext { HostType = typeof(TService) });
    }
  }

  public class RemoteCaller : IRemotableCaller
  {
    private readonly IFactory<ITransport> m_transport_factory;
    private readonly ICommandSerializer m_serializer;

    public RemoteCaller(IFactory<ITransport> transportFactory, ICommandSerializer serializer)
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
      using (var transport = m_transport_factory.Create())
      {
        m_serializer.Serialize<IRemotableCommand>(transport.RequestStream, command);
        transport.EndRequest();
        return m_serializer.Deserialize<RemotableResult>(transport.ResponseStream);
      }
    }
  }
}