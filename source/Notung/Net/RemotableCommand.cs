using System;
using System.Runtime.Serialization;

namespace Notung.Net
{
  /// <summary>
  /// Команда, которая может быть исполнена удалённо
  /// </summary>
  /// <typeparam name="TResult">Тип результата</typeparam>
  [Serializable, DataContract]
  public abstract class RemotableCommand<TResult> : IRemotableCommand
    where TResult : RemotableResult
  {
    RemotableResult IRemotableCommand.Execute(IServiceProvider service)
    {
      var res = this.CreateEmptyResult(service);
      try
      {
        this.Fill(res, service);
        res.Success = true;
      }
      catch (Exception ex)
      {
        res.Exception = new ClientServerException(ex.Message, ex.StackTrace);
        res.Success = false;
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

  interface IRemotableCommand
  {
    RemotableResult Execute(IServiceProvider service);

    ClientInfo Headers { get; set; }
  }

  /// <summary>
  /// Результат выполнения команды
  /// </summary>
  [Serializable, DataContract]
  public class RemotableResult
  {
    private bool m_ok;
    private ClientServerException m_err;
    
    /// <summary>
    /// Исключение, возникшее при выполнении команды
    /// </summary>
    [DataMember]
    public ClientServerException Exception
    {
      get { return m_err; }
      internal set { m_err = value; }
    }

    /// <summary>
    /// Успешность выполнения команды
    /// </summary>
    [DataMember]
    public bool Success
    {
      get { return m_ok; }
      internal set { m_ok = value; }
    }
  }

  public static class RemotableCommandExtensions
  {
    /// <summary>
    /// Исполнитель команд на клиенте, который будет исполнять по умолчанию
    /// </summary>
    public static IRemotableCaller DefaultCaller { get; set; }

    public static TResult Execute<TResult>(this RemotableCommand<TResult> command)
      where TResult : RemotableResult
    {
      if (command == null)
        throw new ArgumentNullException("command");

      var caller = DefaultCaller;

      if (caller == null)
        throw new InvalidOperationException("Default caller is not set");

      return caller.Call(command);
    }
  }
}