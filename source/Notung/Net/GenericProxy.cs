using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Notung.Threading;

namespace Notung.Net
{
  /// <summary>
  /// Базовый класс для прокси к произвольным серверам
  /// </summary>
  /// <typeparam name="T">Тип контракта сервиса, для которого создаётся прокси</typeparam>
  public abstract class GenericProxy<T> : RealProxy, IRemotingTypeInfo, IDisposable where T : class
  {
    private readonly Dictionary<Type, object> m_local_services = new Dictionary<Type, object>();
    private readonly SharedLock m_lock = new SharedLock(false);

    /// <summary>
    /// Создание прокси с настройками по умолчанию
    /// </summary>
    protected GenericProxy() : base(typeof(T)) { }

    /// <summary>
    /// Создание прокси с указанием заглушки
    /// </summary>
    /// <param name="stub">Указатель на заглушку</param>
    /// <param name="stubData">Данные заглушки</param>
    protected GenericProxy(IntPtr stub, object stubData) : base(typeof(T), stub, stubData) { }

    /// <summary>
    /// Обеспечивает общую инфраструктуру локальных и удалённых вызовов
    /// </summary>
    public sealed override IMessage Invoke(IMessage msg)
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

    /// <summary>
    /// Получение экземпляра прозрачного прокси для работы с сервисом
    /// </summary>
    /// <returns>Экземпляр прозрачного прокси нужного типа</returns>
    public new T GetTransparentProxy()
    {
      return (T)base.GetTransparentProxy();
    }

    /// <summary>
    /// Добавление дополнительных интерфейсов, которые должен реализовывать прозрачный прокси
    /// </summary>
    /// <typeparam name="TContract">Тип интерфейса, реализуемый прозрачным прокси</typeparam>
    /// <param name="localService">Локальный объект, реализующий этот объект</param>
    /// <param name="overwrite">Затереть ли существующие сервисы того же типа или его предков</param>
    protected void AddLocalService<TContract>(TContract localService,
      LocalServiceOverride overwrite = LocalServiceOverride.No) where TContract : class
    {
      if (localService == null)
        throw new ArgumentNullException("localService");

      using (m_lock.WriteLock())
      {
        if (overwrite == LocalServiceOverride.All)
          m_local_services[typeof(TContract)] = localService;
        else
          m_local_services.Add(typeof(TContract), localService);

        foreach (var itf in typeof(TContract).GetInterfaces())
        {
          if (overwrite == LocalServiceOverride.No && m_local_services.ContainsKey(itf))
            continue;

          m_local_services[itf] = localService;
        }
      }
    }

    protected enum LocalServiceOverride : byte
    {
      No,
      OnlyParent,
      All
    }

    /// <summary>
    /// Создание результата, возвращаемого методом, для передачи прозрачному прокси
    /// для случая, когда у метода нет ни ref, ни out параметров
    /// </summary>
    /// <param name="value">Возвращаемое значение метода</param>
    /// <param name="message">Сообщение, описывающее вызов метода прозрачного прокси</param>
    /// <returns>Сообщение, в котором помещён результат вызова метода прозрачного прокси</returns>
    protected ReturnMessage CreateReturnMesage(object value, IMethodCallMessage message)
    {
      return new ReturnMessage(value, null, 0, message.LogicalCallContext, message);
    }

    /// <summary>
    /// Получение текстового представления прозрачного прокси
    /// </summary>
    /// <returns>То, что должен вернуть метод ToString() прозрачного прокси</returns>
    protected virtual string GetProxyName()
    {
      return typeof(T).FullName;
    }

    /// <summary>
    /// При перекрытии в произвольном классе, реализует специфическую логику вызова метода через прокси
    /// </summary>
    /// <param name="message">Сообщение, описывающее вызов метода</param>
    /// <returns>Сообщение, описывающее результат вызова метода</returns>
    protected abstract ReturnMessage Invoke(IMethodCallMessage message);

    #region Implementation ------------------------------------------------------------------------

    private static IMethodReturnMessage InvokeByReflection(IMethodCallMessage message, object item)
    {
      try
      {
        var args = message.Args;
        var ret = message.MethodBase.Invoke(item, args);

        if (message.MethodBase.GetParameters().Any(pi => pi.ParameterType.IsByRef))
          return new ReturnMessage(ret, args, args.Length, message.LogicalCallContext, message);
        else
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

    #endregion

    #region Destroy -------------------------------------------------------------------------------

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
        new Action(m_lock.Close).BeginInvoke(null, null);
    }

    #endregion
  }
}