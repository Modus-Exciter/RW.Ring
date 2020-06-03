using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting;
using System.Reflection;
using Notung.Threading;

namespace Notung.Network
{
  public abstract class GenericProxy<T> : RealProxy, IRemotingTypeInfo where T : class
  {
    private readonly Dictionary<Type, object> m_local_services = new Dictionary<Type, object>();
    private readonly SharedLock m_lock = new SharedLock(false);

    protected GenericProxy() : base(typeof(T)) { }

    protected GenericProxy(IntPtr stub, object stubData) : base(typeof(T), stub, stubData) { }
    
    public override IMessage Invoke(IMessage msg)
    {
      IMethodCallMessage message = (IMethodCallMessage)msg;

      if (message.MethodBase.DeclaringType == typeof(object))
      {
        switch (message.MethodName)
        {
          case "GetType": 
            return CreateReturnMesage(typeof(T), message);

          case "Equals":
            return CreateReturnMesage(ReferenceEquals(base.GetTransparentProxy(), message.Args[0]), message);

          case "GetHashCode":
            return CreateReturnMesage(this.GetHashCode(), message);

          case "ToString":
            return CreateReturnMesage(this.GetProxyName(), message);
        }
      }

      using (m_lock.ReadLock())
      {
        object item;

        if (m_local_services.TryGetValue(message.MethodBase.DeclaringType, out item))
          return InvokeByReflection(message, item);
      }

      return this.Invoke(message);
    }

    public new T GetTransparentProxy()
    {
      return (T)base.GetTransparentProxy();
    }

    protected void AddLocalService<TContract>(TContract localService) where TContract : class 
    {
      if (localService == null)
        throw new ArgumentNullException("localService");

      using (m_lock.WriteLock())
      {
        m_local_services.Add(typeof(TContract), localService);

        foreach (var itf in typeof(TContract).GetInterfaces())
          m_local_services[itf] = localService;
      }
    }

    protected virtual string GetProxyName()
    {
      return typeof(T).FullName;
    }

    protected abstract ReturnMessage Invoke(IMethodCallMessage message);

    protected IMethodReturnMessage CreateReturnMesage(object value, IMethodCallMessage message)
    {
      return new ReturnMessage(value, null, 0, message.LogicalCallContext, message);
    }

    private static IMethodReturnMessage InvokeByReflection(IMethodCallMessage message, object item)
    {
      try
      {
        var ret = message.MethodBase.Invoke(item, message.Args);

        return new ReturnMessage(ret, null, 0, message.LogicalCallContext, message);
      }
      catch (TargetInvocationException ex)
      {
        return new ReturnMessage(ex.InnerException, message);
      }
    }

    bool IRemotingTypeInfo.CanCastTo(Type fromType, object o)
    {
      if (fromType.IsAssignableFrom(typeof(T)))
        return true;

      using (m_lock.ReadLock())
      {
        if (m_local_services.ContainsKey(fromType))
          return true;
      }

      return false;
    }

    string IRemotingTypeInfo.TypeName
    {
      get { return typeof(T).FullName; }
      set { }
    }
  }
}