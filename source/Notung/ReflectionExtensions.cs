using System;
using System.Collections.Generic;
using System.Reflection;

namespace Notung
{
  public static class ReflectionExtensions
  {
    /// <summary>
    /// Проверяет тип на допустимось присвоения null
    /// </summary>
    /// <param name="type">Проверяемый тип</param>
    /// <returns>True, если тип допускает присвоение null</returns>
    public static bool IsNullable(this Type type)
    {
      return !type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
    }

    /// <summary>
    /// Возвращает экземпляр атрибута у элемента
    /// </summary>
    /// <typeparam name="A">Тип атрибута</typeparam>
    /// <param name="item">Элемент</param>
    /// <param name="inherit">Указывает, нужно ли искать атрибут в предках</param>
    /// <returns>Если имеется единственный экземпляр, то его. Иначе, null</returns>
    public static A GetCustomAttribute<A>(this ICustomAttributeProvider item, bool inherit = false)
      where A : Attribute
    {
      if (item.IsDefined(typeof(A), inherit))
        return item.GetCustomAttributes(typeof(A), inherit)[0] as A;

      return null;
    }

    /// <summary>
    /// Настройка нового домена для работы с сервисами приложения. 
    /// Этот метод необходимо вызывать сразу после создания нового домена
    /// </summary>
    /// <param name="newDomain">Новый домен</param>
    public static void ShareServices(this AppDomain newDomain)
    {
      if (newDomain == null)
        throw new ArgumentNullException("newDomain");

      if (newDomain == AppDomain.CurrentDomain)
        return;

      foreach (var action in ShareableTypes.List)
        action(newDomain);
    }

    /// <summary>
    /// Получение всех типов, доступных в сборке,
    /// даже если при загрузке сборки возникла ошибка
    /// </summary>
    /// <param name="assembly">Сборка</param>
    /// <returns>Массив доступных типов</returns>
    public static Type[] GetAvailableTypes(this Assembly assembly, Action<Exception> handler = null)
    {
      try
      {
        return assembly.GetTypes();
      }
      catch (ReflectionTypeLoadException ex)
      {
        if (handler != null)
          handler(ex);
        
        return Array.FindAll(ex.Types, t => t != null);
      }
    }

    /// <summary>
    /// Создание делегата для вызова метода указанного объекта
    /// </summary>
    /// <typeparam name="T">Тип делегата</typeparam>
    /// <param name="item">Объект, метод которого требуется вызвать</param>
    /// <param name="methodName">Имя метода, который требуется вызвать</param>
    /// <returns>Делегат, позволяющий вызвать нужный метод</returns>
    public static T CreateDelegate<T>(this object item, string methodName) where T : class
    {
      var method = GetSuitableMethod<T>(item.GetType(), methodName, true);

      return (T)(object)Delegate.CreateDelegate(typeof(T), item, method, true);
    }

    /// <summary>
    /// Создание делегата для вызова статического метода указанного типа
    /// </summary>
    /// <typeparam name="T">Тип делегата</typeparam>
    /// <param name="objectType">Тип данных, метод которого нужно вызвать</param>
    /// <param name="methodName">Имя метода, который требуется вызвать</param>
    /// <returns>Делегат, позволяющий вызвать нужный метод</returns>
    public static T CreateDelegate<T>(this Type objectType, string methodName) where T : class
    {
      var method = GetSuitableMethod<T>(objectType, methodName, false);

      return (T)(object)Delegate.CreateDelegate(typeof(T), method, true);
    }

    private static MethodInfo GetSuitableMethod<T>(Type objectType, string methodName, bool instance) where T : class
    {
      var parametes = typeof(T).GetMethod("Invoke").GetParameters();
      var types = new Type[parametes.Length];

      for (int i = 0; i < parametes.Length; i++)
        types[i] = parametes[i].ParameterType;

      var method = objectType.GetMethod(methodName, 
        BindingFlags.Public | (instance ? BindingFlags.Instance : BindingFlags.Static), 
        Type.DefaultBinder, types, null);

      return method;
    }

    private static class ShareableTypes
    {
      public static readonly HashSet<Action<AppDomain>> List = new HashSet<Action<AppDomain>>();

      static ShareableTypes()
      {
        var parameters = new Type[] { typeof(AppDomain) };
        
        foreach (var type in typeof(ShareableTypes).Assembly.GetTypes())
        {
          if (type.IsDefined(typeof(AppDomainShareAttribute), false))
          {
            var method = type.GetMethod("Share",
              BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
              Type.DefaultBinder, parameters, null);

            if (method != null)
              List.Add((Action<AppDomain>)Delegate.CreateDelegate(typeof(Action<AppDomain>), method, true));
          }
        }
      }
    }
  }

  /// <summary>
  /// Помечает класс для расшаривания находящихся в нём сервисов между доменами приложений,
  /// за это отвечает метод static void Share(AppDomain newDomain)
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
  public sealed class AppDomainShareAttribute : Attribute { }
}
