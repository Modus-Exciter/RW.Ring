using System;
using System.Reflection;

namespace Notung.Loader
{
  /// <summary>
  /// Фабрика для отложенной загрузки
  /// </summary>
  /// <typeparam name="T">Тип порождаемого объекта</typeparam>
  public interface IFactory<T>
  {
    T Create();
  }

  public sealed class DefaultFactory<TContract, TService> : IFactory<TContract>
    where TContract : class
    where TService : TContract, new()
  {
    #region IFactory<TContract> Members

    public TContract Create()
    {
      return new TService();
    }

    #endregion
  }

  public sealed class DeferredFactory<T> : IFactory<T>
  {
    private readonly string m_assembly;
    private readonly string m_type;

    public DeferredFactory(string assembly, string type)
    {
      if (string.IsNullOrEmpty(assembly))
        throw new ArgumentNullException("assembly");

      if (string.IsNullOrEmpty(type))
        throw new ArgumentNullException("type");

      m_assembly = assembly;
      m_type = type;
    }
    
    public T Create()
    {
      Assembly asm = Assembly.Load(m_assembly);

      Type type = asm.GetType(m_type, true);

      if (typeof(T).IsAssignableFrom(type))
        return (T)Activator.CreateInstance(type);
      else
        throw new TypeLoadException();
    }
  }
}
