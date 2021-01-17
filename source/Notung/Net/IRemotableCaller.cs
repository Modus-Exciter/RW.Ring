using System;
using System.IO;

namespace Notung.Net
{
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
    TResult Call<TResult>(RemotableCommand<TResult> command) where TResult : RemotableResult;
  }

  /// <summary>
  /// Заглушка, выполняющая команду локально
  /// </summary>
  public sealed class RemotableCallerStub<TService> : IRemotableCaller, IServiceProvider where TService : new()
  {
    public TResult Call<TResult>(RemotableCommand<TResult> command) where TResult : RemotableResult
    {
      return (TResult)((IRemotableCommand)command).Execute(this);
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
  /// Реальный исполнитель команды на стороне сервера
  /// </summary>
  public class RemoteCaller : IRemotableCaller
  {
    private readonly ISerializationFactory m_factory;
    private readonly IClientCaller m_caller;

    public RemoteCaller(ISerializationFactory factory, IClientCaller caller)
    {
      if (factory == null)
        throw new ArgumentNullException("factory");

      if (caller == null)
        throw new ArgumentNullException("call");

      m_factory = factory;
      m_caller = caller;
    }

    public TResult Call<TResult>(RemotableCommand<TResult> command) where TResult : RemotableResult
    {
      var query = new Query
      {
        Caller = this,
        Command = command,
        ResultType = typeof(TResult)
      };

      m_caller.StreamExchange(string.Format("{0}, {1}", command.GetType().FullName,
        command.GetType().Assembly.GetName().Name), query.ProcessRequest, query.ProcessResponse);

      if (((TResult)query.Result).Exception != null)
        throw ((TResult)query.Result).Exception;

      return (TResult)query.Result;
    }

    #region Implementation ------------------------------------------------------------------------

    private class Query
    {
      public RemoteCaller Caller;

      public object Result;

      public IRemotableCommand Command;

      public Type ResultType;

      public void ProcessRequest(Stream request)
      {
        var serializer = this.Caller.m_factory.GetSerializer(this.Command.GetType());
        serializer.Serialize(request, this.Command);
      }

      public void ProcessResponse(Stream response)
      {
        var serializer = this.Caller.m_factory.GetSerializer(ResultType);
        this.Result = serializer.Deserialize(response);
      }
    }

    #endregion
  }
}