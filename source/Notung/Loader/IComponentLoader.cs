using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Notung.Loader
{
  /// <summary>
  /// Загрузчик компонента приложения
  /// </summary>
  public interface IComponentLoader
  {
    /// <summary>
    /// Загрузка компонента приложения
    /// </summary>
    /// <param name="context">Контекст загрузки</param>
    /// <returns><code>true</code>, если компонент загружен. <code>false</code>, если не загружен</returns>
    bool Load(LoadingContext context);
  }

  /// <summary>
  /// Загрузчик компонента приложения с зависимостями
  /// </summary>
  public interface IApplicationLoader : IDependencyItem<Type>, IComponentLoader { }

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

    private readonly ReadOnlyCollection<Type> m_mandatory_dependencies;
    private readonly ReadOnlyCollection<Type> m_optional_dependencies;

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
        if (IsScalar(pi.PropertyType) || pi.GetIndexParameters().Length > 0)
          continue;

        var set = pi.GetSetMethod(false);

        if (set == null)
          continue;

        _properties.Add(new KeyValuePair<PropertyInfo, Action<object, object>>(pi, 
          (obj, val) => pi.SetValue(obj, val, null)));
      }
    }

    private static bool IsScalar(Type type)
    {
      if (type == null) throw new ArgumentNullException("type");

      return type.IsValueType || type == typeof(string);
    }

    public ApplicationLoader()
    {
      m_mandatory_dependencies = new ReadOnlyCollection<Type>(
        new HashSet<Type>(_constructor_types).ToList());

      m_optional_dependencies = new ReadOnlyCollection<Type>(
        new HashSet<Type>(_properties
          .Where(kv => this.FilterProperty(kv.Key))
          .Select(kv => kv.Key.PropertyType)).ToList());
    }

    protected virtual bool FilterProperty(PropertyInfo property)
    {
      if (property == null) throw new ArgumentNullException("property");

      return true;
    }

    #region IApplicationLoader Members

    public bool Load(LoadingContext context)
    {
      if (context == null) throw new ArgumentNullException("context");

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

      context.Container.SetService(typeof(TContract), item);
      return true;
    }

    #endregion

    #region IDependencyItem<Type> Members

    Type IDependencyItem<Type>.Key
    {
      get { return typeof(TContract); }
    }

    ICollection<Type> IDependencyItem<Type>.MandatoryDependencies
    {
      get { return m_mandatory_dependencies; }
    }

    ICollection<Type> IDependencyItem<Type>.OptionalDependencies
    {
      get { return m_optional_dependencies; }
    }

    #endregion
  }
}
