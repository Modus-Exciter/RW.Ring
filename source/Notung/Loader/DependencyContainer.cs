using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Notung.Threading;

namespace Notung.Loader
{
  /// <summary>
  /// Строго типизированный потокобезопасный контейнер компонентов
  /// </summary>
  public sealed class DependencyContainer : IServiceContainer, IDisposable
  {
    private static readonly TypeComparer _comparer = new TypeComparer();
    private static readonly Func<object> _empty_creator = () => null;

    private readonly Dictionary<Type, Func<object>> m_creators = new Dictionary<Type, Func<object>>(_comparer);
    private readonly SharedLock m_lock = new SharedLock(false);
    private volatile bool m_search_descendants = true;

    /// <summary>
    /// Следует ли при поиске компонента проверять его подтипы
    /// </summary>
    public bool SearchDescendants
    {
      get { return m_search_descendants; }
      set
      {
        using (m_lock.WriteLock())
          m_search_descendants = value;
      }
    }

    /// <summary>
    /// Возвращает экземпляр компонента по типу
    /// </summary>
    /// <param name="serviceType">Тип компонента</param>
    /// <returns>Экземпляр компонента запрошенного типа, если компонент зарегистрирован. 
    /// Иначе, возвращает <code>null</code></returns>
    public object GetService(Type serviceType)
    {
      if (serviceType == null)
        throw new ArgumentNullException("serviceType");

      return (this.GetCreator(serviceType) ?? _empty_creator).Invoke();
    }

    /// <summary>
    /// Регистрация компонента
    /// </summary>
    /// <typeparam name="TService">Тип компонента</typeparam>
    /// <param name="instance">Экземпляр компонента</param>
    public void AddService<TService>(TService instance)
    {
      this.AddService(typeof(TService), instance);
    }

    /// <summary>
    /// Регистрация компонента
    /// </summary>
    /// <typeparam name="TService">Тип компонента</typeparam>
    /// <param name="creator">Метод, порождающий компонент</param>
    public void AddService<TService>(Func<TService> creator) where TService : class
    {
      this.AddService(typeof(TService), creator);
    }

    /// <summary>
    /// Регистрация компонента
    /// </summary>
    /// <param name="serviceType">Тип компонента</param>
    /// <param name="instance">Экземпляр компонента</param>
    public void AddService(Type serviceType, object instance)
    {
      if (serviceType == null)
        throw new ArgumentNullException("serviceType");

      if (instance == null)
        throw new ArgumentNullException("instance");

      using (m_lock.WriteLock())
      {
        if (!serviceType.IsInstanceOfType(instance))
          throw new ArgumentException("\"instance\" is not instance of type \"type\"", "instance");

        m_creators[serviceType] = () => instance;
      }
    }

    /// <summary>
    /// Регистрация компонента
    /// </summary>
    /// <param name="serviceType">Тип компонента</param>
    /// <param name="creator">Метод, порождающий компонент</param>
    public void AddService(Type serviceType, Func<object> creator)
    {
      if (serviceType == null)
        throw new ArgumentNullException("serviceType");

      if (creator == null)
        throw new ArgumentNullException("creator");

      using (m_lock.WriteLock())
        m_creators[serviceType] = creator;
    }

    /// <summary>
    /// Удаление сервиса из списка сервисов
    /// </summary>
    /// <param name="serviceType"></param>
    public void RemoveService(Type serviceType)
    {
      using (m_lock.WriteLock())
        m_creators.Remove(serviceType);
    }

    /// <summary>
    /// Очистка ресурсов, используемых текущим объектом DependencyContainer
    /// </summary>
    public void Dispose()
    {
      m_creators.Clear();
      m_lock.Close();
    }

    #region IServiceContainer members -------------------------------------------------------------

    void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
    {
      this.AddService(serviceType, () => callback(this, serviceType));
    }

    void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback)
    {
      this.AddService(serviceType, () => callback(this, serviceType));
    }

    void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote)
    {
      this.AddService(serviceType, serviceInstance);
    }

    void IServiceContainer.RemoveService(Type serviceType, bool promote)
    {
      this.RemoveService(serviceType);
    }

    #endregion

    #region Private methods -----------------------------------------------------------------------

    private Func<object> GetCreator(Type serviceType)
    {
      if (serviceType == null)
        throw new ArgumentNullException("serviceType");

      using (m_lock.ReadLock())
      {
        Func<object> creator;

        if (!m_creators.TryGetValue(serviceType, out creator) && m_search_descendants)
        {
          foreach (var kv in m_creators)
          {
            if (serviceType.IsAssignableFrom(kv.Key))
            {
              if (creator != null) // Проверка на однозначность результата
              {
                creator = null;
                break;
              }
              else
                creator = kv.Value;
            }
          }
        }

        return creator;
      }
    }

    #endregion

    #region Implementation ------------------------------------------------------------------------

    private sealed class TypeComparer : IEqualityComparer<Type>
    {
      public bool Equals(Type x, Type y)
      {
        return x.IsEquivalentTo(y);
      }

      public int GetHashCode(Type obj)
      {
        return obj.FullName.GetHashCode();
      }
    }

    #endregion
  }
}