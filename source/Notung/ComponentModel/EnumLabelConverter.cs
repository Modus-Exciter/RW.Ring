using System;
using System.ComponentModel;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Notung.ComponentModel
{
  /// <summary>
  /// Конвертер для локализации перечислений. Локализованная
  /// метка задаётся атрибутом DisplayName или DisplayNameRes.
  /// </summary>
  public class EnumLabelConverter : EnumConverter
  {
    private static readonly Dictionary<Type, EnumLabelList> _enum_labels = new Dictionary<Type, EnumLabelList>();

    /// <summary>
    /// Инициализирует новый экземпляр конвертера
    /// </summary>
    /// <param name="type">Тип перечисления</param>
    public EnumLabelConverter(Type type) : base(type)
    {
      if (type == null)
        throw new ArgumentNullException("type");

      if (!type.IsEnum)
        throw new ArgumentException("type");
    }

    /// <summary>
    /// Конвертирует перечисление в строку, доставая его метку
    /// </summary>
    /// <param name="context">Контекст конвертации</param>
    /// <param name="culture">Региональные настройки</param>
    /// <param name="value">Значение</param>
    /// <param name="destinationType"></param>
    /// <returns>Текст метки</returns>
    public override object ConvertTo(ITypeDescriptorContext context, 
      CultureInfo culture, object value, Type destinationType)
    {
      if (value is Enum)
        return GetLabel(value);

      return base.ConvertTo(context, culture, value, destinationType);
    }

    /// <summary>
    /// Конвертирует метку в значение перечисления
    /// </summary>
    /// <param name="context">Контекст конвертации</param>
    /// <param name="culture">Региональные настройки</param>
    /// <param name="value">Значение метки</param>
    /// <returns>Значение перечисения</returns>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (value is string)
      {
        object ret = ParseEnumLabel(base.EnumType, value as string);

        if (ret != null && ret.GetType() == base.EnumType)
          return ret;
      }

      return base.ConvertFrom(context, culture, value);
    }

    #region Internal ------------------------------------------------------------------------------

    internal static void ClearEnumLabels()
    {
      lock (_enum_labels)
        _enum_labels.Clear();
    }

    internal static string GetLabel(object value)
    {
      if (value == null)
        return null;

      var list = GetLabels(value.GetType());

      if (list.Contains(value))
        return list[value].Label;
      else
        return null;
    }

    #endregion

    #region Implementation ------------------------------------------------------------------------

    private struct EnumLabel
    {
      public object Value { get; set; }

      public string Label { get; set; }
    }

    private class EnumLabelList : KeyedCollection<object, EnumLabel>
    {
      protected override object GetKeyForItem(EnumLabel item)
      {
        return item.Value;
      }
    }

    private static object ParseEnumLabel(Type enumType, string label)
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

    private static EnumLabelList GetLabels(Type enumType)
    {
      lock (_enum_labels)
      {
        EnumLabelList list;

        if (!_enum_labels.TryGetValue(enumType, out list))
        {
          list = new EnumLabelList();

          foreach (var fi in enumType.GetFields(BindingFlags.Public
            | BindingFlags.NonPublic | BindingFlags.Static))
          {
            EnumLabel item = new EnumLabel();

            var name = fi.GetCustomAttribute<DisplayNameAttribute>(true);

            if (name != null && !string.IsNullOrEmpty(name.DisplayName))
              item.Label = name.DisplayName;
            else
              item.Label = fi.Name;

            item.Value = (Enum)fi.GetValue(null);
            list.Add(item);
          }

          _enum_labels.Add(enumType, list);
        }

        return list;
      }
    }

    #endregion
  }

  public static class EnumLabelExtensions
  {
    /// <summary>
    /// Получает метку у перечисления
    /// </summary>
    /// <param name="value">Объект перечислимого типа</param>
    /// <returns>Метка</returns>
    public static string GetLabel(this Enum value)
    {
      return EnumLabelConverter.GetLabel(value);
    }
  }
}