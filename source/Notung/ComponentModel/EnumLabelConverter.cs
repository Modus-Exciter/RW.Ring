using System;
using System.ComponentModel;

namespace Notung.ComponentModel
{
  /// <summary>
  /// Конвертер для локализации перечислений
  /// </summary>
  public class EnumLabelConverter : EnumConverter
  {
    private readonly Type m_type;

    /// <summary>
    /// Инициализирует новый экземпляр конвертера
    /// </summary>
    /// <param name="type">Тип перечисления</param>
    public EnumLabelConverter(Type type)
      : base(type)
    {
      if (type == null)
        throw new ArgumentNullException("type");

      if (!type.IsEnum)
        throw new ArgumentException("type");

      m_type = type;
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
      System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
      if (value is Enum)
      {
        return ((Enum)value).GetLabel();
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }

    /// <summary>
    /// Конвертирует метку в значение перечисления
    /// </summary>
    /// <param name="context">Контекст конвертации</param>
    /// <param name="culture">Региональные настройки</param>
    /// <param name="value">Значение метки</param>
    /// <returns>Значение перечисения</returns>
    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
      if (value is string)
      {
        object ret = SystemExtensions.ParseEnumLabel(m_type, value as string);

        if (ret != null && ret.GetType() == m_type)
          return ret;
      }

      return base.ConvertFrom(context, culture, value);
    }
  }
}
