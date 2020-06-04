using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using Notung.ComponentModel;

namespace Notung.Net
{
  /// <summary>
  /// Прокси для сервисов, использующих механизм удалённых команд для обмена данными
  /// </summary>
  /// <typeparam name="T">Тип контракта сервиса</typeparam>
  public class NetworkProxy<T> : GenericProxy<T> where T : class
  {
    private readonly Guid m_object_id;
    private readonly IRemotableCaller m_caller;

    /// <summary>
    /// Создание прокси с возможностью абстрагироваться от сети
    /// </summary>
    /// <param name="caller">Абстрактный исполнитель команд</param>
    public NetworkProxy(IRemotableCaller caller)
    {
      if (caller == null)
        throw new ArgumentNullException("caller");

      m_caller = caller;

      var res = new CreateReferenceCommand().Execute(caller);

      if (res.State == RemotableResultState.Success)
        m_object_id = res.ObjectGuid;
      else
        throw res.Exception;

      this.AddLocalService<IDisposable>(this);
    }

    /// <summary>
    /// Создание прокси, подключенного к сети
    /// </summary>
    /// <param name="transportFactory">Транспорт команд</param>
    /// <param name="serializer">Сериализация команд</param>
    public NetworkProxy(IClientTransportFactory transportFactory, ICommandSerializer serializer)
      : this(new RemoteCaller(transportFactory, serializer)) { }

    #region Overridables --------------------------------------------------------------------------

    protected override ReturnMessage Invoke(IMethodCallMessage message)
    {
      var result = new MethodCallCommand(message.MethodName, message.Args, m_object_id).Execute(m_caller);

      if (result.State == RemotableResultState.Error)
        return new ReturnMessage(result.Exception, message);
      else if (result.RefParameters != null && result.RefParameters.Length != 0)
        return new ReturnMessage(result.ReturnValue, result.RefParameters, result.RefParameters.Length, message.LogicalCallContext, message);
      else
        return CreateReturnMesage(result.ReturnValue, message);
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);

      if (disposing)
        m_caller.Call(new ClearItemCommand(m_object_id));
    }

    #endregion

    #region Create reference ----------------------------------------------------------------------

    [Serializable, DataContract(Name = "Ref", Namespace = "http://ari.ru/notung")]
    internal class CreateReferenceResult : RemotableResult
    {
      [DataMember(Name = "Guid")]
      public Guid ObjectGuid;
    }

    [Serializable, DataContract(Name = "CreateRef", Namespace = "http://ari.ru/notung")]
    private class CreateReferenceCommand : RemotableCommand<CreateReferenceResult>
    {
      protected override void Fill(CreateReferenceResult result, IServiceProvider service)
      {
        result.ObjectGuid = InstanceDictionary<T>.AddInstance(service.GetService<T>());

        var session = service.GetService<ISessionManager>();

        if (session != null && session.SupportsSession)
          session.Init(result.ObjectGuid, typeof(T));
      }
    }

    #endregion

    #region Clear cache ---------------------------------------------------------------------------

    [Serializable, DataContract(Name = "Clear", Namespace = "http://ari.ru/notung")]
    internal class ClearItemCommand : RemotableCommand<RemotableResult>
    {
      [DataMember(Name = "Guid")]
      private readonly Guid m_guid;

      public ClearItemCommand(Guid guid)
      {
        m_guid = guid;
      }

      protected override void Fill(RemotableResult result, IServiceProvider service)
      {
        InstanceDictionary<T>.Clear(m_guid);
      }
    }

    #endregion

    #region Method call ---------------------------------------------------------------------------

    [Serializable, DataContract(Name = "CallResult", Namespace = "http://ari.ru/notung")]
    internal class MethodCallResult : RemotableResult
    {
      [DataMember]
      public object ReturnValue;

      [DataMember]
      public object[] RefParameters;
    }

    [Serializable, DataContract(Name = "Call", Namespace = "http://ari.ru/notung")]
    private class MethodCallCommand : RemotableCommand<MethodCallResult>
    {
      [DataMember(Name = "Method")]
      private readonly string m_method;

      [DataMember(Name = "Params")]
      private readonly object[] m_parameters;

      [DataMember(Name = "Guid")]
      private readonly Guid m_object_id;

      public MethodCallCommand(string method, object[] parameters, Guid objectId)
      {
        m_method = method;
        m_parameters = parameters;
        m_object_id = objectId;
      }
      
      protected override void Fill(MethodCallResult result, IServiceProvider service)
      {
        T instance = InstanceDictionary<T>.GetInstance(m_object_id);

        if (instance == null)
          throw new NullReferenceException();

        var session = service.GetService<ISessionManager>();

        if (session != null && session.SupportsSession)
          session.Renew(m_object_id, typeof(T));

        try
        {
          var method = typeof(T).GetMethod(m_method);
          var ret = method.Invoke(instance, m_parameters);

          result.ReturnValue = ret;

          if (method.GetParameters().Any(pi => pi.ParameterType.IsByRef))
            result.RefParameters = m_parameters;
        }
        catch (TargetInvocationException ex)
        {
          throw ex.InnerException;
        }
      }
    }

    #endregion
  }
}