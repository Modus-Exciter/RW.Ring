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
    /// <summary>
    /// Запуск команды на выполнение
    /// </summary>
    /// <returns>Результат выполнения команды на другой стороне</returns>
    public TResult Execute()
    {
      var caller = RemoteCaller.DefaultCaller;

      if (caller == null)
        throw new InvalidOperationException("Default caller is not set");

      return caller.Call(this);
    }
    
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

    /// <summary>
    /// Выполнение команды на стороне сервера
    /// </summary>
    /// <param name="result">Результат выполнения команды, в который можно добавить данные</param>
    /// <param name="service">Сервис, выполняющий команду</param>
    protected abstract void Fill(TResult result, IServiceProvider service);

    Type IRemotableCommand.ResultType 
    { 
      get { return typeof(TResult); } 
    }
  }

  interface IRemotableCommand
  {
    RemotableResult Execute(IServiceProvider service);

    Type ResultType { get; }
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
}