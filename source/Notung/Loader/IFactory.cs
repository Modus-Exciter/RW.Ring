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

  /// <summary>
  /// Набор типовых фабрик для отложенной загрузки
  /// </summary>
  public static class Factory
  {
    /// <summary>
    /// Фабрика, создающая объект указанного типа вызовом конструктора без параметров
    /// </summary>
    /// <typeparam name="TContract">Требуемый тип данных</typeparam>
    /// <typeparam name="TService">Тип объекта, реально порождаемый фабрикой</typeparam>
    public static IFactory<TContract> Default<TContract, TService>()
      where TService : TContract, new()
    {
      return DefaultFactory<TContract, TService>.Instance;
    }

    /// <summary>
    /// Возвращает фабрику, которая всегда возвращает null в качестве созданного объекта
    /// </summary>
    /// <typeparam name="T">Тип объекта, порождаемого фабрикой</typeparam>
    /// <returns>Фабрика, которая всегда возвращает null</returns>
    public static IFactory<T> Empty<T>() where T : class
    {
      return EmptyFactory<T>.Instance;
    }

    /// <summary>
    /// Возвращает фабрику, которая возвращает объект, который уже был создан
    /// </summary>
    /// <typeparam name="T">Тип объекта, порождаемого фабрикой</typeparam>
    /// <param name="item">Реальный, который нужно получать от фабрики</param>
    /// <returns></returns>
    public static IFactory<T> Wrapper<T>(T item)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      return new WrapperFactory<T>(item);
    }

    #region Implementation ------------------------------------------------------------------------

    private sealed class DefaultFactory<TContract, TService> : IFactory<TContract>
      where TService : TContract, new()
    {
      public static readonly IFactory<TContract> Instance = new DefaultFactory<TContract, TService>();

      public TContract Create()
      {
        return new TService();
      }
    }

    private sealed class EmptyFactory<T> : IFactory<T> where T : class
    {
      public static readonly IFactory<T> Instance = new EmptyFactory<T>();

      public T Create() 
      { 
        return null;
      }
    }

    private sealed class WrapperFactory<T> : IFactory<T>
    {
      private readonly T m_item;

      public WrapperFactory(T item)
      {
        m_item = item;
      }

      public T Create()
      {
        return m_item;
      }
    }

    #endregion
  }

  /// <summary>
  /// Фабрика, которую можно создать, не загружая сборку, 
  /// в которой описан тип реально порождаемого объекта
  /// </summary>
  /// <typeparam name="T">Требуемый тип данных</typeparam>
  public sealed class DeferredFactory<T> : IFactory<T>
  {
    private readonly string m_assembly;
    private readonly string m_type;

    /// <summary>
    /// Инициализация фабрики
    /// </summary>
    /// <param name="assembly">Имя сборки, в которой объявлен искомый тип</param>
    /// <param name="type">Полное имя типа данный, который будет порождать фабрика</param>
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