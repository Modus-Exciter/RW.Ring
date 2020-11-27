using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Notung.Data;

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
    void Prepare(LoadingContext context);
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
    private static readonly Func<object[], object> _factory_method;
    private static readonly Type[] _constructor_types;

    private static readonly List<KeyValuePair<PropertyInfo, Action<object, object>>> _properties
      = new List<KeyValuePair<PropertyInfo, Action<object, object>>>();
    private static readonly bool _synchronization_required;

    private readonly ReadOnlySet<Type> m_mandatory_dependencies;

    static ApplicationLoader()
    {
      if (typeof(TService).IsAbstract)
        throw new InvalidProgramException(string.Format("Тип компонента \"{0}\" абстрактный", typeof(TService)));

      _synchronization_required = typeof(ISynchronizeInvoke).IsAssignableFrom(typeof(TService));

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

      _factory_method = (pr) => constructor.Method.Invoke(pr);
      _constructor_types = new Type[constructor.Params.Length];

      for (int i = 0; i < _constructor_types.Length; i++)
        _constructor_types[i] = constructor.Params[i].ParameterType;

      foreach (var pi in typeof(TService).GetProperties())
      {
        if (IsScalar(pi.PropertyType))
          continue;
 
        if (pi.GetIndexParameters().Length > 0)
          continue;

        if (pi.GetSetMethod(false) == null)
          continue;

        _properties.Add(new KeyValuePair<PropertyInfo, Action<object, object>>(pi, CreateSetter(pi)));
      }
    }

    private static Action<object, object> CreateSetter(PropertyInfo pi)
    {
      return (obj, val) => pi.SetValue(obj, val, null);
    }

    private static bool IsScalar(Type type)
    {
      return type.IsValueType || type == typeof(string);
    }

    public ApplicationLoader()
    {
      m_mandatory_dependencies = new ReadOnlySet<Type>(new HashSet<Type>(_constructor_types));
    }

    protected virtual bool FilterProperty(PropertyInfo property)
    {
      return true;
    }

    #region IApplicationLoader Members

    public bool Load(LoadingContext context)
    {
      if (context == null) 
        throw new ArgumentNullException("context");

      var ctor_params = new object[_constructor_types.Length];
      var lookup = new Dictionary<Type, object>();

      for (int i = 0; i < ctor_params.Length; i++)
      {
        object value;

        if (!lookup.TryGetValue(_constructor_types[i], out value))
        {
          value = context.Container.GetService(_constructor_types[i]);
          lookup[_constructor_types[i]] = value;
        }

        ctor_params[i] = value;

        if (ctor_params[i] == null)
          return false;
      }

      object item = null;

      if (_synchronization_required && context.Invoker.InvokeRequired)
        item = context.Invoker.Invoke(_factory_method, new object[] { ctor_params });
      else
        item = _factory_method(ctor_params);

      if (item == null)
        return false;

      context.Container.SetService(typeof(TContract), item);

      return true;
    }

    public void Prepare(LoadingContext context)
    {
      var lookup = new Dictionary<Type, object>();
      var item = context.Container.GetService(typeof(TContract));

      foreach (var pi in _properties)
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

    #endregion

    #region IDependencyItem<Type> Members

    Type IDependencyItem<Type>.Key
    {
      get { return typeof(TContract); }
    }

    ICollection<Type> IDependencyItem<Type>.Dependencies
    {
      get { return m_mandatory_dependencies; }
    }

    #endregion
  }
}
