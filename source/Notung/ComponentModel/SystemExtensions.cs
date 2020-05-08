using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Notung.ComponentModel
{
  public static class SystemExtensions
  {
    private static readonly Dictionary<Type, List<EnumLabel>> _enum_labels =
      new Dictionary<Type, List<EnumLabel>>();

    private class EnumLabel
    {
      public object Value { get; set; }

      public string Label { get; set; }
    }

    internal static void ClearEnumLabels()
    {
      lock (_enum_labels)
        _enum_labels.Clear();
    }

    internal static object ParseEnumLabel(Type enumType, string label)
    {
      if (string.IsNullOrEmpty(label))
        return null;

      foreach (var enumLabel in GetLabels(enumType))
      {
        if (label.Equals(enumLabel.Label))
          return enumLabel.Value;
      }

      return null;
    }

    private static List<EnumLabel> GetLabels(Type enumType)
    {
      List<EnumLabel> list;
      if (!_enum_labels.TryGetValue(enumType, out list))
      {
        list = new List<EnumLabel>();
        foreach (var fi in enumType.GetFields(BindingFlags.Public
          | BindingFlags.NonPublic | BindingFlags.Static))
        {
          EnumLabel item = new EnumLabel();

          if (fi.IsDefined(typeof(DisplayNameAttribute), true))
          {
            item.Label = (fi.GetCustomAttributes(typeof(DisplayNameAttribute), true)[0]
              as DisplayNameAttribute).DisplayName;
          }

          if (string.IsNullOrEmpty(item.Label))
            item.Label = fi.Name;

          item.Value = fi.GetValue(null);
          list.Add(item);
        }

        _enum_labels.Add(enumType, list);
      }
      return list;
    }

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
    public static A GetCustomAttribute<A>(this MemberInfo item, bool inherit)
      where A : Attribute
    {
      if (item.IsDefined(typeof(A), inherit))
      {
        return item.GetCustomAttributes(typeof(A), inherit)[0] as A;
      }

      return null;
    }
    /// <summary>
    /// Возвращает экземпляр атрибута у элемента
    /// </summary>
    /// <typeparam name="A">Тип атрибута</typeparam>
    /// <param name="item">Элемент</param>
    /// <param name="inherit">Указывает, нужно ли искать атрибут в предках</param>
    /// <returns>Если имеется единственный экземпляр, то его. Иначе, null</returns>
    public static A GetCustomAttribute<A>(this Assembly item)
      where A : Attribute
    {
      if (item.IsDefined(typeof(A), false))
      {
        return item.GetCustomAttributes(typeof(A), false)[0] as A;
      }

      return null;
    }
    /// <summary>
    /// Получает метку у перечисления
    /// </summary>
    /// <param name="value">Объект перечислимого типа</param>
    /// <returns>Метка</returns>
    public static string GetLabel(this Enum value)
    {
      if (value == null)
        return null;

      lock (_enum_labels)
      {
        foreach (var item in GetLabels(value.GetType()))
        {
          if (item.Value.Equals(value))
            return item.Label;
        }

        return value.ToString();
      }
    }

    // TODO
    public static void InvokeSynchronized<TArgs>(this Delegate eventHandler, object sender, TArgs args)
      where TArgs : EventArgs
    {
    }
  }
}
