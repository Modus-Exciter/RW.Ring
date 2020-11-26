using System;
using System.Runtime.Serialization;

namespace Notung.Net
{
  /// <summary>
  /// Команда, которую можно выполнить удалённо
  /// </summary>
  public interface IRemotableCommand
  {
    /// <summary>
    /// Выполнение команды
    /// </summary>
    /// <param name="service">Сервис, выполняющий команду</param>
    /// <returns>Результат выполнения команды</returns>
    RemotableResult Execute(IServiceProvider service);

    /// <summary>
    /// Заголовки команды
    /// </summary>
    ProcessHeaders Headers { get; set; }
  }

  /// <summary>
  /// Результат выполнения команды
  /// </summary>
  [Serializable, DataContract]
  public class RemotableResult
  {
    /// <summary>
    /// Исключение, возникшее при выполнении команды
    /// </summary>
    [DataMember]
    public Exception Exception { get; internal set; }

    /// <summary>
    /// Успешность выполнения команды
    /// </summary>
    [DataMember]
    public RemotableResultState State { get; internal set; }
  }

  /// <summary>
  /// Успех выполнения команды
  /// </summary>
  [Serializable, DataContract]
  public enum RemotableResultState
  {
    /// <summary>
    /// Команда выполнена успешно
    /// </summary>
    [EnumMember] Success,

    /// <summary>
    /// Ошибка при выполнении команды
    /// </summary>
    [EnumMember] Error,

    /// <summary>
    /// Команда запросила обратную связь
    /// </summary>
    [EnumMember] Callback
  }

  /// <summary>
  /// Заголовки команды
  /// </summary>
  [Serializable, DataContract(Name = "Headers")]
  public sealed class ProcessHeaders
  {
    /// <summary>
    /// Имя пользователя, запустившего команду
    /// </summary>
    [DataMember(Name="User")]
    public string UserName { get; internal set; }

    /// <summary>
    /// Приложение, через которое запущена команда
    /// </summary>
    [DataMember(Name = "App")]
    public string Application { get; internal set; }

    /// <summary>
    /// Имя компьютера, с которого запущена команда
    /// </summary>
    [DataMember(Name = "Machine")]
    public string MachineName { get; internal set; }
  }

  /// <summary>
  /// Базовая реализация интерфейса удалённых команд
  /// </summary>
  /// <typeparam name="TResult">Тип результата</typeparam>
  [Serializable, DataContract]
  public abstract class RemotableCommand<TResult> : IRemotableCommand where TResult : RemotableResult
  {
    /// <summary>
    /// Запуск команды на выполнение
    /// </summary>
    /// <param name="caller">Объект запуска команды</param>
    /// <returns>Результат выполнения команды</returns>
    public TResult Execute(IRemotableCaller caller)
    {
      if (caller == null)
        throw new ArgumentNullException("caller");

      return (TResult)caller.Call(this);
    }
    
    RemotableResult IRemotableCommand.Execute(IServiceProvider service)
    {
      var res = this.CreateEmptyResult(service);
      try
      {
        this.Fill(res, service);

        if (res.State != RemotableResultState.Callback)
          res.State = RemotableResultState.Success;
      }
      catch (Exception ex)
      {
        res.Exception = ex;
        res.State = RemotableResultState.Error;
      }

      return res;
    }

    protected virtual TResult CreateEmptyResult(IServiceProvider service)
    {
      return Activator.CreateInstance<TResult>();
    }

    protected abstract void Fill(TResult result, IServiceProvider service);

    [DataMember(Name = "Headers")]
    ProcessHeaders IRemotableCommand.Headers { get; set; }
  }

  /// <summary>
  /// Абстракция для выполнеия команды удалённо
  /// </summary>
  public interface IRemotableCaller
  {
    /// <summary>
    /// Выполнение команды, потенциально удалённой
    /// </summary>
    /// <param name="command">Выполняемая команда</param>
    /// <returns>Результат выполнения командыы</returns>
    RemotableResult Call(IRemotableCommand command);
  }

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