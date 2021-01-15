using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace Notung.Net
{
  /// <summary>
  /// Сведения о типах методов сервиса
  /// </summary>
  public interface IRpcServiceInfo
  {
    /// <summary>
    /// Имя сервиса
    /// </summary>
    string ServiceName { get; }

    /// <summary>
    /// Получение имени метода сервиса
    /// </summary>
    /// <param name="method">Метод интерфейса сервиса</param>
    /// <returns>Логическое имя метода сервиса</returns>
    string GetMethodName(MethodBase method);

    /// <summary>
    /// Получение метода сервиса по логическому имени
    /// </summary>
    /// <param name="methodName">Логическое имя метода сервиса</param>
    /// <returns>Метод интерфейса сервиса</returns>
    MethodInfo GetMethod(string methodName);

    /// <summary>
    /// Получение типа для сериализации параметров сервиса
    /// </summary>
    /// <param name="methodName">Логическое имя метода сервиса</param>
    /// <returns>Тип, используемый для сериализации параметров сервиса</returns>
    Type GetParametersType(string methodName);

    /// <summary>
    /// Получение типа для сериализации ответа сервиса
    /// </summary>
    /// <param name="methodName">Логическое имя метода сервиса</param>
    /// <returns>Тип, используемый для сериализации параметров сервиса</returns>
    Type GetReturnType(string methodName);

    /// <summary>
    /// Получение информации о наличии параметров, передаваемых по ссылке
    /// </summary>
    /// <param name="methodName">Логическое имя метода сервиса</param>
    /// <returns>True, если есть параметры, передаваемые по ссылке. Иначе, false</returns>
    bool HasReferenceParameters(string methodName);

    /// <summary>
    /// Проверка наличия метода у сервиса с указанным логическим именем
    /// </summary>
    /// <param name="methodName">Логическое имя метода сервиса</param>
    /// <returns>True, если такой метод есть. Иначе, false</returns>
    bool HasMethod(string methodName);
  }

  public interface IRefReturnResult
  {
    object Return { get; set; }

    IParametersList References { get; set; }
  }

  /// <summary>
  /// Тип, используемый для сериализации ответа метода сервиса, имеющего
  /// и возвращаемое значение, и параметры, передаваемые по ссылке
  /// </summary>
  /// <typeparam name="TReturn">Тип возвращаемого значения метода</typeparam>
  /// <typeparam name="TRef">Тип для сериализации копии параметров</typeparam>
  [Serializable, DataContract(Name = "RET", Namespace = "")]
  internal class ReturnWithOut<TReturn, TRef> : IRefReturnResult where TRef : class, IParametersList
  {
    [DataMember(Name = "Ret")]
    private TReturn m_ret;
    [DataMember(Name = "Ref")]
    private TRef m_ref;

    public object Return
    {
      get { return m_ret; }
      set { m_ret = (TReturn)value; }
    }

    public IParametersList References
    {
      get { return m_ref; }
      set { m_ref = (TRef)value; }
    }
  }
  
  /// <summary>
  /// Хранилище информации о методах сервиса, которую можно получить по логическому имени сервиса
  /// </summary>
  public static class RpcServiceInfo
  {
    private static readonly RpcServiceCollection _services = new RpcServiceCollection();

    public static IRpcServiceInfo Register<T>() where T : class
    {
      var name = RpcServiceInfo<T>.ServiceName;

      lock (_services)
      {
        if (!_services.Contains(name))
          _services.Add(RpcServiceInfo<T>.Instance);
      }

      return RpcServiceInfo<T>.Instance;
    }
    
    public static IRpcServiceInfo Register(Type serviceType)
    {
      var info_type = typeof(RpcServiceInfo<>).MakeGenericType(serviceType);
      var name = info_type.CreateDelegate<Func<string>>("get_ServiceName")();
      var inst = info_type.CreateDelegate<Func<IRpcServiceInfo>>("get_Instance");

      lock (_services)
      {
        if (!_services.Contains(name))
          _services.Add(inst()); 
      }

      return inst();
    }

    public static IRpcServiceInfo GetByName(string serviceName)
    {
      return _services[serviceName];
    }

    private class RpcServiceCollection : KeyedCollection<string, IRpcServiceInfo>
    {
      protected override string GetKeyForItem(IRpcServiceInfo item)
      {
        return item.ServiceName;
      }
    }
  }

  public class RpcServiceInfo<T> : IRpcServiceInfo where T : class
  {
    private static string _serviceName = GetServiceName();
    private static Dictionary<string, MethodDescription> _methods = BuildMethods();
    private static RpcServiceInfo<T> _instance = new RpcServiceInfo<T>();

    private RpcServiceInfo() { }

    public static string ServiceName
    {
      get { return _serviceName; }
    }

    public static IRpcServiceInfo Instance
    {
      get { return _instance; }
    }

    string IRpcServiceInfo.ServiceName
    {
      get { return _serviceName; }
    }

    public string GetMethodName(MethodBase method)
    {
      var name = method.GetCustomAttribute<RpcOperationAttribute>().Name;

      if (string.IsNullOrWhiteSpace(name))
        return method.Name;
      else
        return name;
    }

    public MethodInfo GetMethod(string methodName)
    {
      return _methods[methodName].Method;
    }

    public Type GetParametersType(string methodName)
    {
      return _methods[methodName].ParametersType;
    }

    public Type GetReturnType(string methodName)
    {
      return _methods[methodName].ReturnType;
    }

    public bool HasReferenceParameters(string methodName)
    {
      return _methods[methodName].HasReferenceParameters;
    }

    public bool HasMethod(string methodName)
    {
      return _methods.ContainsKey(methodName);
    }

    #region Implementation ------------------------------------------------------------------------

    private static string GetServiceName()
    {
      var service = typeof(T).GetCustomAttribute<RpcServiceAttribute>();

      if (string.IsNullOrWhiteSpace(service.Name))
        return typeof(T).Name;
      else
        return service.Name;
    }

    private static Dictionary<string, MethodDescription> BuildMethods()
    {
      if (!typeof(T).IsInterface)
        throw new ApplicationException();

      var result = new Dictionary<string, MethodDescription>();

      foreach (MethodInfo method in typeof(T).GetMethods())
      {
        if (method.DeclaringType == typeof(object))
          continue;

        var operation = method.GetCustomAttribute<RpcOperationAttribute>();

        if (operation == null)
          continue;

        string name = operation.Name;

        if (string.IsNullOrWhiteSpace(name))
          name = method.Name;

        var params_type = ParametersList.GetRequiredType(method);
        var return_type = method.ReturnType;
        var has_refs = false;

        foreach (var pi in method.GetParameters())
        {
          if (pi.ParameterType.IsByRef)
          {
            if (return_type == typeof(void))
              return_type = params_type;
            else
              return_type = typeof(ReturnWithOut<,>).MakeGenericType(return_type, params_type);

            has_refs = true;
            break;
          }
        }

        result.Add(name, new MethodDescription
        {
          Method = method,
          ParametersType = params_type,
          ReturnType = return_type,
          HasReferenceParameters = has_refs
        });
      }

      return result;
    }

    private struct MethodDescription
    {
      public MethodInfo Method;
      public Type ParametersType;
      public Type ReturnType;
      public bool HasReferenceParameters;
    }

    #endregion
  }
}