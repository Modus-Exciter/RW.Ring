using System;
using System.Collections.Generic;
using System.Threading;
using Notung.Threading;

namespace Notung.Net
{
  /// <summary>
  /// Управление временем жизни экземпляра сервиса
  /// </summary>
  public interface ISessionManager
  {
    /// <summary>
    /// Вызывается при создании новой сессии
    /// </summary>
    /// <param name="objectGuid">Глобальный идентификатор объекта, поддерживающего сессию</param>
    /// <param name="serviceType">Тип объекта, поддерживающего сессию</param>
    void Init(Guid objectGuid, Type serviceType);

    /// <summary>
    /// Продление жизни объекта, поддерживающего сессию
    /// </summary>
    /// <param name="objectGuid">Глобальный идентификатор объекта, поддерживающего сессию</param>
    /// <param name="serviceType">Тип объекта, поддерживающего сессию</param>
    /// <returns>True, если время жизни объекта успешно продлено. Иначе, false</returns>
    bool Renew(Guid objectGuid, Type serviceType);

    /// <summary>
    /// Поддержка сессионности
    /// </summary>
    bool SupportsSession { get; }
  }

  /// <summary>
  /// Поддержка сессионности сервисов
  /// </summary>
  public sealed class SupportsSessionAttribute : Attribute { }

  public sealed class SessionManager : ISessionManager, IDisposable
  {
    private readonly Type m_service_type;
    private readonly TimeSpan m_init_life_time;
    private readonly TimeSpan m_renew_time;
    private readonly Timer m_timer;
    private readonly Dictionary<Guid, ExpireInfo> m_expire = new Dictionary<Guid, ExpireInfo>();
    private readonly SharedLock m_lock = new SharedLock(false);

    public SessionManager(Type serviceType, TimeSpan initLifeTime, TimeSpan renewLifeTime)
    {
      if (serviceType == null)
        throw new ArgumentNullException("serviceType");

      m_service_type = serviceType;
      m_timer = new Timer(this.ClearExpired, null, 1, 5000);
      m_init_life_time = initLifeTime;
      m_renew_time = renewLifeTime;
    }

    public SessionManager(Type serviceType) 
      : this(serviceType, TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(5)) { }

    private void ClearExpired(object state)
    {
      var expired = new Dictionary<Guid, ExpireInfo>();

      using (m_lock.ReadLock())
      {
        foreach (var kv in m_expire)
        {
          if (kv.Value.Expire <= DateTime.Now)
            expired.Add(kv.Key, kv.Value);
        }
      }

      if (expired.Count == 0)
        return;

      using (m_lock.WriteLock())
      {
        foreach (var kv in expired)
        {
          m_expire.Remove(kv.Key);
          kv.Value.Clear(kv.Key);
        }
      }
    }
    
    public void Init(Guid objectGuid, Type serviceType)
    {
      using (m_lock.WriteLock())
      {
        m_expire[objectGuid] = new ExpireInfo(serviceType)
        {
          Expire = DateTime.Now.Add(m_init_life_time)
        };
      }
    }

    public bool Renew(Guid objectGuid, Type serviceType)
    {
      using (m_lock.ReadLock())
      {
        ExpireInfo expire;

        if(!m_expire.TryGetValue(objectGuid, out expire))
          return false;

        DateTime new_expire = DateTime.Now.Add(m_renew_time);

        if (new_expire > expire.Expire)
          expire.Expire = new_expire;

        return true;
      }
    }

    public bool SupportsSession
    {
      get { return m_service_type.IsDefined(typeof(SupportsSessionAttribute), false); }
    }

    public void Dispose()
    {
      m_timer.Dispose();
    }

    private class ExpireInfo
    {
      public DateTime Expire { get; set; }

      public Type ContractType { get; private set; }

      public Action<Guid> Clear { get; private set; }

      public ExpireInfo(Type contractType)
      {
        this.ContractType = contractType;
        this.Clear = typeof(InstanceDictionary<>).MakeGenericType(contractType)
          .CreateDelegate<Action<Guid>>("Clear");
      }
    }
  }

  /// <summary>
  /// Хранилище экземпляров объектов разных сервисов на стороне сервера
  /// </summary>
  /// <typeparam name="T">Тип контракта сервиса</typeparam>
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
