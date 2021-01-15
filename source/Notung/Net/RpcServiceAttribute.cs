using System;
using System.Text.RegularExpressions;

namespace Notung.Net
{
  /// <summary>
  /// Помечает интерфейс как предназначенный для удалённого вызова
  /// </summary>
  [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
  public sealed class RpcServiceAttribute : Attribute, INamedAttribute
  {
    /// <summary>
    /// Помечает интерфейс как предназначенный для удалённого вызова
    /// </summary>
    public RpcServiceAttribute() { }

    /// <summary>
    /// Помечает интерфейс как предназначенный для удалённого вызова
    /// </summary>
    /// <param name="name">Имя сервиса</param>
    public RpcServiceAttribute(string name)
    {
      this.Name = RpcAttributeHelper.Check(name);
    }
    
    /// <summary>
    /// Имя сервиса, если задано
    /// </summary>
    public string Name { get; private set; }

    public override bool Equals(object obj)
    {
      return RpcAttributeHelper.AreEqual(this, obj);
    }

    public override int GetHashCode()
    {
      return RpcAttributeHelper.GetHashCode(this);
    }

    public override bool IsDefaultAttribute()
    {
      return RpcAttributeHelper.IsDefault(this);
    }
  }

  /// <summary>
  /// Помечает метод интерфейса как предназначенный для удалённого вызова
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
  public sealed class RpcOperationAttribute : Attribute, INamedAttribute
  {
    /// <summary>
    /// Помечает метод интерфейса как предназначенный для удалённого вызова
    /// </summary>
    public RpcOperationAttribute() { }

    /// <summary>
    /// Помечает метод интерфейса как предназначенный для удалённого вызова
    /// </summary>
    /// <param name="name">Имя операции, если оно отличается от имени метода в коде</param>
    public RpcOperationAttribute(string name)
    {
      this.Name = RpcAttributeHelper.Check(name);
    }

    /// <summary>
    /// Имя операции, если оно отличается от имени метода в коде
    /// </summary>
    public string Name { get; private set; }

    public override bool Equals(object obj)
    {
      return RpcAttributeHelper.AreEqual(this, obj);
    }

    public override int GetHashCode()
    {
      return RpcAttributeHelper.GetHashCode(this);
    }

    public override bool IsDefaultAttribute()
    {
      return RpcAttributeHelper.IsDefault(this);
    }
  }

  internal interface INamedAttribute
  {
    string Name { get; }
  }

  internal static class RpcAttributeHelper
  {
    private static readonly Regex _check = new Regex("^[a-zA-Z_]+[a-zA-Z_1-9]*$", RegexOptions.Compiled);

    public static bool IsDefault<T>(T instance) where T : Attribute, INamedAttribute, new()
    {
      return Instances<T>.Instance.Equals(instance);
    }

    public static bool AreEqual<T>(T instance, object obj) where T : Attribute, INamedAttribute
    {
      var other = obj as RpcServiceAttribute;

      if (other == null)
        return false;

      return object.Equals(instance.Name, other.Name);
    }

    public static int GetHashCode<T>(T instance) where T : Attribute, INamedAttribute
    {
      var name = instance.Name;

      if (name == null)
        return 0;

      return name.GetHashCode();
    }

    public static string Check(string name)
    {
      if (string.IsNullOrEmpty(name))
        return name;

      if (!_check.IsMatch(name))
        throw new FormatException();

      return name;
    }

    private static class Instances<T> where T : Attribute, INamedAttribute, new()
    {
      public static T Instance = new T();
    }
  }
}
