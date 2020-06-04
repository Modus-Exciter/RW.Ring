using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Notung.ComponentModel;
using Notung.Loader;
using Notung.Threading;

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

    #region Create reference ----------------------------------------------------------------------

    [Serializable]
    internal class CreateReferenceResult : RemotableResult
    {
      public Guid ObjectGuid { get; set; }
    }

    [Serializable]
    private class CreateReferenceCommand : RemotableCommand<CreateReferenceResult>
    {
      protected override void Fill(CreateReferenceResult result, IServiceProvider service)
      {
        result.ObjectGuid = InstanceDictionary<T>.AddInstance(service.GetService<T>());
      }
    }

    #endregion

    #region Clear session -------------------------------------------------------------------------

    [Serializable]
    internal class ClearItemCommand : RemotableCommand<RemotableResult>
    {
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

    [Serializable]
    internal class MethodCallResult : RemotableResult
    {
      public object ReturnValue { get; internal set; }

      public object[] RefParameters { get; internal set; }
    }

    [Serializable]
    private class MethodCallCommand : RemotableCommand<MethodCallResult>
    {
      private readonly string m_method;
      private readonly object[] m_parameters;
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

  /// <summary>
  /// Хранилище экземпляров объектов разных сервисов на стороне сервера
  /// </summary>
  /// <typeparam name="T">Тип контракта сервиса</typeparam>
  public static class InstanceDictionary<T> where T : class
  {
    private static readonly Dictionary<Guid, T> _instances = new Dictionary<Guid, T>();
    private static readonly SharedLock _lock = new SharedLock(false);

    public static Guid AddInstance(T instance)
    {
      if (instance == null)
        throw new ArgumentNullException("instance");

      using (_lock.WriteLock())
      {
        var guid = Guid.NewGuid();
        _instances[guid] = instance;
        return guid;
      }
    }

    public static T GetInstance(Guid guid)
    {
      using (_lock.ReadLock())
      {
        T ret;
        _instances.TryGetValue(guid, out ret);
        return ret;
      }
    }

    public static void Clear(Guid guid)
    {
      using (_lock.WriteLock())
      {
        _instances.Remove(guid);
      }
    }
  }
}