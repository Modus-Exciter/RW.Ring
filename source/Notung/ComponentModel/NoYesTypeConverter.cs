using System;
using System.ComponentModel;
using System.Threading;

namespace Notung.ComponentModel
{
  /// <summary>
  /// Конвертер для локализации логических свойств
  /// </summary>
  public class NoYesTypeConverter : BooleanConverter
  {
    /// <summary>
    /// Конвертирует локализованную строку в логическое значение
    /// </summary>
    /// <param name="context">Контекст</param>
    /// <param name="culture">Настройки локализации</param>
    /// <param name="value">Конвертируемое значение</param>
    /// <returns>Логическое значение, соответствущее строке</returns>
    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
      if (value is string)
      {
        string str = value as string;

        if (culture != null && !Equals(culture, Thread.CurrentThread.CurrentUICulture))
        {
          if (str == CoreResources.ResourceManager.GetString("NO", culture))
            return false;
          if (str == CoreResources.ResourceManager.GetString("YES", culture))
            return true;
        }
        else
        {
          if (str == CoreResources.NO)
            return false;
          if (str == CoreResources.YES)
            return true;
        }
      }
      return base.ConvertFrom(context, culture, value);
    }

    /// <summary>
    /// Конвертирует логическое значение в локализованную строку
    /// </summary>
    /// <param name="context">Контекст</param>
    /// <param name="culture">Настройки локализации</param>
    /// <param name="value">Конвертируемое значение</param>
    /// <param name="destinationType">Тип, в который требуется сконвертировать</param>
    /// <returns>Значение, сконвертированное в нужный тип</returns>
    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
      var ret = (bool)value;

      if (destinationType == typeof(string))
        return ret ? CoreResources.YES : CoreResources.NO;

      return base.ConvertTo(context, culture, value, destinationType);
    }
  }
}