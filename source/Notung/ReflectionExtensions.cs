using System;
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
      {
        return item.GetCustomAttributes(typeof(A), inherit)[0] as A;
      }

      return null;
    }
  }
}
