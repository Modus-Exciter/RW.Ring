using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Notung.Data;
using Notung.Properties;

namespace Notung.Loader
{
  /// <summary>
  /// Загрузчик компонента приложения с зависимостями
  /// </summary>
  public interface IApplicationLoader : IDependencyItem<Type>
  {    
    /// <summary>
    /// Загрузка компонента приложения
    /// </summary>
    /// <param name="context">Контекст загрузки</param>
    /// <returns><code>true</code>, если компонент загружен. <code>false</code>, если не загружен</returns>
    bool Load(LoadingContext context);

    /// <summary>
    /// Дополнительные действия после того, как всё будет загружено
    /// </summary>
    /// <param name="context">Контекст загрузки</param>
    void Setup(LoadingContext context);
  }

  /// <summary>
  /// Загрузчик компонента по типу
  /// </summary>
  /// <typeparam name="TContract">Контракт, реализуемый типом компонента</typeparam>
  /// <typeparam name="TService">Реальный тип компонента</typeparam>
  public class ApplicationLoader<TContract, TService> : IApplicationLoader
    where TService : TContract
    where TContract : class
  {
    public static readonly bool _synchronization_required
      = typeof(ISynchronizeInvoke).IsAssignableFrom(typeof(TService));

    public bool Load(LoadingContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");

      var ctor_params = new object[Ctor.Types.Length];
      var lookup = new Dictionary<Type, object>();

      for (int i = 0; i < ctor_params.Length; i++)
      {
        object value;

        if (!lookup.TryGetValue(Ctor.Types[i], out value))
        {
          value = context.Container.GetService(Ctor.Types[i]);
          lookup[Ctor.Types[i]] = value;
        }

        ctor_params[i] = value;

        if (ctor_params[i] == null)
          return false;
      }

      object item = null;

      if (_synchronization_required && context.Invoker.InvokeRequired)
        item = context.Invoker.Invoke(Ctor.Method, new object[] { ctor_params });
      else
        item = Ctor.Method(ctor_params);

      if (item == null)
        return false;

      context.Container.SetService(typeof(TContract), item);

      return true;
    }

    public void Setup(LoadingContext context)
    {
      var lookup = new Dictionary<Type, object>();
      var item = context.Container.GetService(typeof(TContract));

      foreach (var pi in Props.List)
      {
        if (!this.FilterProperty(pi.Key))
          continue;

        object value;

        if (!lookup.TryGetValue(pi.Key.PropertyType, out value))
        {
          value = context.Container.GetService(pi.Key.PropertyType);
          lookup[pi.Key.PropertyType] = value;
        }

        if (value != null)
        {
          if (_synchronization_required && context.Invoker.InvokeRequired)
            context.Invoker.Invoke(pi.Value, new object[] { item, value });
          else
            pi.Value(item, value);
        }
      }
    }

    /// <summary>
    /// Фильтрация свойств, участвующих в обмене данными при вызове Setup
    /// </summary>
    protected virtual bool FilterProperty(PropertyInfo property)
    {
      return true;
    }

    #region IDependencyItem<Type> Members

    Type IDependencyItem<Type>.Key
    {
      get { return typeof(TContract); }
    }

    ICollection<Type> IDependencyItem<Type>.Dependencies
    {
      get { return Ctor.Dependencies; }
    }

    #endregion

    private static Action<object, object> CreateSetter(PropertyInfo pi)
    {
      return (obj, val) => pi.SetValue(obj, val, null);
    }

    private static bool IsScalar(Type type)
    {
      return type.IsValueType || type == typeof(string);
    }

    private static class Ctor
    {
      public static readonly Func<object[], object> Method;
      public static readonly Type[] Types;
      public static readonly ReadOnlySet<Type> Dependencies;

      static Ctor()
      {
        if (typeof(TService).IsAbstract)
          throw new InvalidProgramException(string.Format(Resources.ABSTRACT_COMPONENT_TYPE, typeof(TService)));

        var constructor = (from ci in typeof(TService).GetConstructors()
                           let item = new
                           {
                             Method = ci,
                             Params = ci.GetParameters()
                           }
                           where item.Params.Length == 0 ||
                           item.Params.All(p => !IsScalar(p.ParameterType))
                           orderby item.Params.Length descending
                           select item).First();

        Method = constructor.Method.Invoke;
        Types = new Type[constructor.Params.Length];

        for (int i = 0; i < Types.Length; i++)
          Types[i] = constructor.Params[i].ParameterType;

        Dependencies = new ReadOnlySet<Type>(new HashSet<Type>(Types));
      }
    }

    private static class Props
    {
      public static readonly ReadOnlyCollection<KeyValuePair<PropertyInfo, Action<object, object>>> List;
        
      static Props()
      {
        var properties = new List<KeyValuePair<PropertyInfo, Action<object, object>>>();
        
        foreach (var pi in typeof(TService).GetProperties())
        {
          if (IsScalar(pi.PropertyType))
            continue;

          if (pi.GetIndexParameters().Length > 0)
            continue;

          if (pi.GetSetMethod(false) == null)
            continue;

          properties.Add(new KeyValuePair<PropertyInfo, Action<object, object>>(pi, CreateSetter(pi)));
        }

        List = new ReadOnlyCollection<KeyValuePair<PropertyInfo, Action<object, object>>>(properties);
      }
    }
  }
}