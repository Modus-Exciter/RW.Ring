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
    RemotableResult1 Execute(IServiceProvider service);

    /// <summary>
    /// Заголовки команды
    /// </summary>
    ClientInfo Headers { get; set; }
  }

  /// <summary>
  /// Результат выполнения команды
  /// </summary>
  [Serializable, DataContract]
  public class RemotableResult1
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
  /// Базовая реализация интерфейса удалённых команд
  /// </summary>
  /// <typeparam name="TResult">Тип результата</typeparam>
  [Serializable, DataContract]
  public abstract class RemotableCommand<TResult> : IRemotableCommand where TResult : RemotableResult1
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

    RemotableResult1 IRemotableCommand.Execute(IServiceProvider service)
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
    ClientInfo IRemotableCommand.Headers { get; set; }
  }
}