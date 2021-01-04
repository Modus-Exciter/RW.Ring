using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Notung.Properties;

namespace Notung.ComponentModel
{
  public static class ComponentExtensions
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

          var name = fi.GetCustomAttribute<DisplayNameAttribute>(true);

          if (name != null && !string.IsNullOrEmpty(name.DisplayName))
            item.Label = name.DisplayName;
          else
            item.Label = fi.Name;

          item.Value = fi.GetValue(null);
          list.Add(item);
        }

        _enum_labels.Add(enumType, list);
      }
      return list;
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

    /// <summary>
    /// Проверяет, изменилось ли указанное свойство в событии PropertyChanged
    /// </summary>
    /// <param name="property">Свойство, которое проверяется</param>
    /// <returns>True, если свойство изменилось. Иначе, false</returns>
    public static bool IsChanged(this PropertyChangedEventArgs e, string property)
    {
      return string.IsNullOrEmpty(e.PropertyName) || e.PropertyName.Equals(property);
    }

    /// <summary>
    /// Получает значение указанного типа от IServiceProvider
    /// </summary>
    /// <typeparam name="TService">Тип, который требуется получить</typeparam>
    /// <returns>Значение указанного типа, если IServiceProvider поддерживает этот тип</returns>
    public static TService GetService<TService>(this IServiceProvider provider) where TService: class
    {
      return provider.GetService(typeof(TService)) as TService;
    }

    /// <summary>
    /// Выполнение обработчика события в синхронизированном контексте
    /// </summary>
    /// <typeparam name="TArgs">Тип события</typeparam>
    /// <param name="eventHandler">Обработчик события</param>
    /// <param name="sender">Источник события</param>
    /// <param name="args">Событие</param>
    public static void InvokeSynchronized<TArgs>(this Delegate eventHandler, object sender, TArgs args)
      where TArgs : EventArgs
    {
      if (eventHandler == null)
        return;

      foreach (var dlgt in eventHandler.GetInvocationList())
      {
        var invoker = (dlgt.Target as ISynchronizeInvoke) ?? ConditionalServices.CurrentProcess.SynchronizingObject;

        if (invoker != null && invoker.InvokeRequired)
        {
          invoker.BeginInvoke(dlgt, new object[] { sender, args });
        }
        else
        {
          var handler = dlgt as EventHandler<TArgs>;

          if (handler != null)
            handler(sender, args);
          else
          {
            try
            {
              dlgt.DynamicInvoke(sender, args);
            }
            catch (Exception ex)
            {
              throw new ArgumentException(string.Format(Resources.INVALID_EVENT_HANDLER,
                eventHandler.GetType(), typeof(TArgs)), ex);
            }
          }
        }
      }
    }
  }
}
