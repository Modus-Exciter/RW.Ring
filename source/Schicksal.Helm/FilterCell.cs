using Notung;
using Notung.Logging;
using Schicksal.Helm.Dialogs;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Schicksal.Helm
{
  public partial class FilterCell : UserControl
  {
    private readonly string m_property;
    private readonly ITypeParser m_parser;

    public FilterCell(DataGridViewColumn column)
    {
      m_property = column.DataPropertyName;
      m_parser = TypeParser.GetParser(column.ValueType);

      this.InitializeComponent();
    }

    public override string Text
    {
      get { return m_text_box.Text; }
      set { m_text_box.Text = value; }
    }

    public string Property
    {
      get { return m_property; }
    }

    public ITypeParser Parser
    {
      get { return m_parser; }
    }

    private void HandleTextChanged(object sender, EventArgs e)
    {
      this.OnTextChanged(e);
    }

    private void m_text_box_SizeChanged(object sender, EventArgs e)
    {
      this.Height = m_text_box.Height + this.Padding.Top + this.Padding.Bottom
        + m_text_border.Padding.Top + m_text_border.Padding.Bottom;
    }

    private void m_text_box_MouseEnter(object sender, EventArgs e)
    {
      this.BackColor = SystemColors.GrayText;
    }

    private void m_text_box_MouseLeave(object sender, EventArgs e)
    {
      this.BackColor = SystemColors.ControlDark;
    }
  }


  /// <summary>
  /// Преобразование строки в значение колонки таблицы с избеганием исключений
  /// </summary>
  public interface ITypeParser
  {
    /// <summary>
    /// Выполнение преобразование строки в значение типа колонки
    /// </summary>
    /// <param name="text">Строка, которую нужно преобразовать</param>
    /// <param name="success">Удалось ли выполнить преобразование</param>
    /// <returns>Значение, которое получилось при преобразовании, если преобразование успешно. Иначе, исходная строка</returns>
    object ParseIfPossible(string text, out bool success);
  }

  /// <summary>
  /// Фабрика парсеров для разных типов данных
  /// </summary>
  public static class TypeParser
  {
    private static readonly ConcurrentDictionary<Type, ITypeParser> _cache = new ConcurrentDictionary<Type, ITypeParser>();

    /// <summary>
    /// Получение парсера для запрошенного типа данных
    /// </summary>
    /// <param name="type">Тип данных, для которого требуется парсер</param>
    /// <returns>Парсер для запрошенного типа данных</returns>
    public static ITypeParser GetParser(Type type)
    {
      if (type is null)
        throw new ArgumentNullException(nameof(type));

      return _cache.GetOrAdd(type, CreateParser);
    }

    private static ITypeParser CreateParser(Type type)
    {
      bool nullable = false;

      if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
      {
        type = type.GetGenericArguments()[0];
        nullable = true;
      }

      if (type.IsPrimitive || type == typeof(decimal) || type == typeof(DateTime) || type == typeof(TimeSpan))
        return Activator.CreateInstance(typeof(SimpleTypeParser<>).MakeGenericType(type), new object[] { nullable }) as ITypeParser;

      if (type == typeof(string))
        return new StringTypeParser();

      return new ConverterTypeParser(type, nullable);
    }

    private class StringTypeParser : ITypeParser
    {
      public object ParseIfPossible(string text, out bool success)
      {
        success = true;
        return text;
      }
    }

    private class ConverterTypeParser : ITypeParser
    {
      private readonly TypeConverter m_type;
      private readonly bool m_nullable;

      private static readonly ILog _log = LogManager.GetLogger(typeof(ConverterTypeParser));

      public ConverterTypeParser(Type type, bool nullable)
      {
        m_type = TypeDescriptor.GetConverter(type);
        m_nullable = nullable;
      }

      public object ParseIfPossible(string text, out bool success)
      {
        if (m_nullable && string.IsNullOrEmpty(text))
        {
          success = true;
          return null;
        }

        try
        {
          var ret = m_type.ConvertFromString(text);
          success = true;
          return ret;
        }
        catch(Exception ex)
        {
          _log.Error("ParseIfPossible() exception", ex);
          success = false;
          return text;
        }
      }
    }

    private class SimpleTypeParser<T> : ITypeParser
    {
      private delegate bool TryParseDelegate(string text, out T success);
      private readonly bool m_nullable;
      private static readonly TryParseDelegate _method
        = typeof(T).CreateDelegate<TryParseDelegate>("TryParse");

      public SimpleTypeParser(bool nullable)
      {
        m_nullable = nullable;
      }

      public object ParseIfPossible(string text, out bool success)
      {
        if (m_nullable && string.IsNullOrEmpty(text))
        {
          success = true;
          return null;
        }

        T value;

        success = _method(text, out value);

        return success ? (object)value : text;
      }
    }
  }
}