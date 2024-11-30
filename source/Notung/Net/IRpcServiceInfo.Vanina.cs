using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// Получение сведений об операции сервиса
    /// </summary>
    /// <param name="methodName">Логическое имя метода сервиса</param>
    /// <returns>Сведения об операции сервиса</returns>
    RpcOperationInfo GetOperationInfo(string methodName);

    /// <summary>
    /// Проверка наличия метода у сервиса с указанным логическим именем
    /// </summary>
    /// <param name="methodName">Логическое имя метода сервиса</param>
    /// <returns>True, если такой метод есть. Иначе, false</returns>
    bool HasMethod(string methodName);

    IEnumerable<KeyValuePair<string, RpcOperationInfo>> GetMethods();
  }

  /// <summary>
  /// Сведения об операции сервиса
  /// </summary>
  public sealed class RpcOperationInfo
  {
    private readonly MethodInfo m_method;
    private readonly Type m_request_type;
    private readonly Type m_result_type;
    private readonly Type m_response_type;
    private readonly bool m_has_refs;

    internal RpcOperationInfo(MethodInfo method)
    {
      m_method = method;
      m_request_type = ParametersList.GetRequiredType(method);
      m_has_refs = false;
      m_result_type = method.ReturnType;

      foreach (var pi in method.GetParameters())
      {
        if (pi.ParameterType.IsByRef)
        {
          if (m_result_type == typeof(void))
            m_result_type = m_request_type;
          else
            m_result_type = typeof(ReturnWithOut<,>).MakeGenericType(m_result_type, m_request_type);

          m_has_refs = true;
          break;
        }
      }

      if (m_result_type == typeof(void))
        m_result_type = typeof(string);

      m_response_type = typeof(CallResult<>).MakeGenericType(m_result_type);
    }

    /// <summary>
    /// Метод интерфейса сервиса
    /// </summary>
    public MethodInfo Method
    {
      get { return m_method; }
    }

    /// <summary>
    /// Тип, используемый для сериализации параметров операции
    /// </summary>
    public Type RequestType
    {
      get { return m_request_type; }
    }

    /// <summary>
    /// Тип, используемый для сериализации ответа операции
    /// </summary>
    public Type ResponseType
    {
      get { return m_response_type; }
    }

    /// <summary>
    /// Тип, используемый для передачи результатов операции
    /// </summary>
    public Type ResultType
    {
      get { return m_result_type; }
    }

    /// <summary>
    /// Наличие параметров, передаваемых по ссылке
    /// </summary>
    public bool HasReferenceParameters
    {
      get { return m_has_refs; }
    }
  }

  /// <summary>
  /// Результат удалённой операции, содержащий
  /// и возвращаемое значение, и параметры, передаваемые по ссылке
  /// </summary>
  public interface IRefReturnResult
  {
    /// <summary>
    /// Возвращаемое значение
    /// </summary>
    object Return { get; set; }

    /// <summary>
    /// Параметры, передаваемые по ссылке
    /// </summary>
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

  /// <summary>
  /// Сведения о методах конкретного сервиса
  /// </summary>
  /// <typeparam name="T">Тип сервиса для поиска сведений</typeparam>
  public class RpcServiceInfo<T> : IRpcServiceInfo where T : class
  {
    private static readonly string _service_name = GetServiceName();
    private static readonly Dictionary<string, RpcOperationInfo> _methods = BuildMethods();
    private static readonly RpcServiceInfo<T> _instance = new RpcServiceInfo<T>();

    private RpcServiceInfo() { }

    public static string ServiceName
    {
      get { return _service_name; }
    }

    public static IRpcServiceInfo Instance
    {
      get { return _instance; }
    }

    string IRpcServiceInfo.ServiceName
    {
      get { return _service_name; }
    }

    public string GetMethodName(MethodBase method)
    {
      var name = method.GetCustomAttribute<RpcOperationAttribute>().Name;

      if (string.IsNullOrWhiteSpace(name))
        return method.Name;
      else
        return name;
    }

    public bool HasMethod(string methodName)
    {
      return _methods.ContainsKey(methodName);
    }

    public RpcOperationInfo GetOperationInfo(string methodName)
    {
      return _methods[methodName];
    }

    public IEnumerable<KeyValuePair<string, RpcOperationInfo>> GetMethods()
    {
      return _methods.AsEnumerable();
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

    private static Dictionary<string, RpcOperationInfo> BuildMethods()
    {
      if (!typeof(T).IsInterface)
        throw new ApplicationException();

      var result = new Dictionary<string, RpcOperationInfo>();

      foreach (MethodInfo method in typeof(T).GetMethods())
      {
        if (method.DeclaringType == typeof(object))
          continue;

        var operation = method.GetCustomAttribute<RpcOperationAttribute>();

        if (operation == null)
          continue;

        string name = operation.Name;

        result.Add(string.IsNullOrWhiteSpace(name) ?
          method.Name : name, new RpcOperationInfo(method));
      }

      return result;
    }

    #endregion
  }
}