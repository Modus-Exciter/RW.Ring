using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Notung.Threading;
using Notung.Loader;

namespace Notung.Network
{
  public class NetworkProxy<T> : GenericProxy<T> where T : class
  {
    private readonly Guid m_object_id;
    private readonly IRemotableCaller m_caller;

    public NetworkProxy(IRemotableCaller caller)
    {
      if (caller == null)
        throw new ArgumentNullException("caller");

      m_caller = caller;
      var res = caller.Call(new CreateReferenceCommand<T>());

      if (res.Type == MethodCallResultType.Success)
        m_object_id = (Guid)res.ReturnValue;
      else
        throw res.Exception;

      this.AddLocalService<IDisposable>(this);
    }

    public NetworkProxy(IFactory<ITransport> transportFactory, ICommandSerializer serializer)
      : this(new RemoteCaller(transportFactory, serializer)) { }

    protected override ReturnMessage Invoke(IMethodCallMessage message)
    {
      var result = m_caller.Call(new MethodCallCommand<T>(message.MethodName, message.Args, m_object_id));

      if (result.Type == MethodCallResultType.Error)
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
        m_caller.Call(new ClearItemCommand<T>(m_object_id));
    }
  }

  [Serializable]
  public class CreateReferenceCommand<T> : IRemotableCommand where T : class
  {
    public RemotableResult Execute(CommandContext context)
    {
      RemotableResult ret = new RemotableResult();
      try
      {
        ret.ReturnValue = InstanceDictionary<T>.AddInstance((T)Activator.CreateInstance(context.HostType));
      }
      catch (Exception ex)
      {
        ret.Exception = ex;
      }

      return ret;
    }
  }

  [Serializable]
  public class ClearItemCommand<T> : IRemotableCommand where T : class
  {
    private readonly Guid m_guid;

    public ClearItemCommand(Guid guid)
    {
      m_guid = guid;
    }
    
    public RemotableResult Execute(CommandContext context)
    {
      InstanceDictionary<T>.Clear(m_guid);

      return new RemotableResult { Type = MethodCallResultType.Success };
    }
  }


  /// <summary>
  /// Команда для вызова метода через рефлексию
  /// </summary>
  [Serializable]
  public class MethodCallCommand<T> : IRemotableCommand where T : class
  {
    private readonly string m_method;
    private readonly object[] m_parameters;
    private readonly Guid m_object_id;

    public MethodCallCommand(string method, object[] parameters, Guid objectId)
    {
      if (method == null)
        throw new ArgumentNullException("method");

      if (typeof(T).GetMethod(method) == null)
        throw new MissingMethodException(typeof(T).FullName, method);

      m_method = method;
      m_parameters = parameters;
      m_object_id = objectId;
    }

    public RemotableResult Execute(CommandContext context)
    {
      T instance = InstanceDictionary<T>.GetInstance(m_object_id);

      if (instance == null)
      {
        return new RemotableResult
        {
          Type = MethodCallResultType.Error,
          Exception = new NullReferenceException()
        };
      }

      try
      {
        var method = typeof(T).GetMethod(m_method);
        var ret = method.Invoke(instance, m_parameters);

        var res = new RemotableResult
        {
          ReturnValue = ret,
          Type = MethodCallResultType.Success
        };

        if (method.GetParameters().Any(pi => pi.ParameterType.IsByRef))
          res.RefParameters = m_parameters;

        return res;
      }
      catch (TargetInvocationException ex)
      {
        return new RemotableResult
        {
          Type = MethodCallResultType.Error,
          Exception = ex.InnerException
        };
      }
    }
  }

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