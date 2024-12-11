using Notung;
using Notung.Logging;
using Schicksal.Basic;
using Schicksal.Helm.Dialogs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Schicksal.Helm
{
  public partial class FilterCell : UserControl
  {
    private readonly string m_property;
    private readonly ITypeParser m_parser;
    private int m_autocomplete_width;

    public FilterCell(DataGridViewColumn column)
    {
      m_property = column.DataPropertyName;
      m_parser = TypeParser.GetParser(column.ValueType);
      var table = column.DataGridView.DataSource as DataTable;

      this.InitializeComponent();

      var height = m_text_box.Height + this.Padding.Top + this.Padding.Bottom
        + m_text_border.Padding.Top + m_text_border.Padding.Bottom;

      this.MaximumSize = new Size(Screen.PrimaryScreen.Bounds.Width, height);
      this.MinimumSize = new Size(0, height);

      if (table != null)
        m_worker.RunWorkerAsync(table);
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

    private void HandleMouseEnter(object sender, EventArgs e)
    {
      this.BackColor = SystemColors.GrayText;
    }

    private void HandleMouseLeave(object sender, EventArgs e)
    {
      this.BackColor = SystemColors.ControlDark;
    }

    private void HandleDoWork(object sender, DoWorkEventArgs e)
    {
      var table = e.Argument as DataTable;
      var column = table.Columns[m_property];
      var values = new HashSet<string>();

      foreach (DataRow row in table.Rows)
      {
        var value = row[column];
        values.Add(TypeParser.GetValueText(value));
      }

      string[] prefixes = new string[] { "=", "!=", "> ", "< ", "<>", ">=", "<=" };
      string[] suggestions = new string[values.Count * prefixes.Length + 2];

      int index = 0;

      suggestions[index++] = "=\xA0Ø";
      suggestions[index++] = "!=\xA0Ø";

      using (var g = Graphics.FromHwnd(IntPtr.Zero))
      {
        foreach (var value in values)
        {
          foreach (var prefix in prefixes)
          {
            suggestions[index++] = string.Format("{0}{1}", prefix, value);
            var size = (int)g.MeasureString(suggestions[index - 1], m_text_box.Font).Width;
              m_autocomplete_width = Math.Max(size, m_autocomplete_width);
          }
        }
      }

      e.Result = suggestions;
    }

    private void HandleRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      var suggestions = e.Result as string[];

      if (suggestions != null)
        m_text_box.AutoCompleteCustomSource.AddRange(suggestions);
    }

    private void HandleSizeChanged(object sender, EventArgs e)
    {
      m_text_box.Width = Math.Max(m_autocomplete_width + SystemInformation.VerticalScrollBarWidth, 
        this.Width - (this.Padding.Left + this.Padding.Right
        + m_text_border.Padding.Left + m_text_border.Padding.Right));
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

    /// <summary>
    /// Выполнение преобразование значения в строку
    /// </summary>
    /// <param name="value">Значение, которое нужно преобразовать</param>
    /// <returns>Строка, в которое преобразовано значение</returns>
    public static string GetValueText(object value)
    {
      if (value is string)
        return ((string)value).Replace("'", "''");

      if (value is DateTime)
      {
        var dt = (DateTime)value;

        if (dt.TimeOfDay == TimeSpan.Zero)
          return dt.ToShortDateString();
      }

      return value.ToString();
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

  public struct FilterCondition
  {
    public string Column;
    public object Value;
    public bool ConvertColumn;
    public char Operation;

    public FilterCondition(string key, string value, ITypeParser converter)
    {
      this.Column = key;

      if (value.StartsWith("!=") || value.StartsWith("<>"))
      {
        this.Operation = '≠';
        value = value.Substring(2).TrimStart();
      }
      else if (value.StartsWith(">=") || value.StartsWith("≥"))
      {
        this.Operation = '≥';
        value = value[0] == '≥' ? value.Substring(1).TrimStart() : value.Substring(2).TrimStart();
      }
      else if (value.StartsWith("<=") || value.StartsWith("≤"))
      {
        this.Operation = '≤';
        value = value[0] == '≤' ? value.Substring(1).TrimStart() : value.Substring(2).TrimStart();
      }
      else if (value[0] == '>' || value[0] == '<' || value[0] == '=' || value[0] == '≠')
      {
        this.Operation = value[0];
        value = value.Substring(1).TrimStart();
      }
      else if (value[0] == '\'')
      {
        this.Operation = '≈';
        value = value.Substring(1).TrimStart();
      }
      else if (value == "!")
      {
        this.Operation = '≈';
        value = string.Empty;
      }
      else
        this.Operation = '≈';

      if (this.Operation == '≈')
        value += "%";

      if (converter.GetType() != typeof(StringConverter))
      {
        this.Value = converter.ParseIfPossible(value, out bool success);
        this.ConvertColumn = !success;
      }
      else
      {
        this.Value = value;
        this.ConvertColumn = false;
      }
    }

    public override string ToString()
    {
      return string.Format("{0} {1} {2}", this.Column, this.Operation, GroupKey.GetInvariant(this.Value));
    }

    private string GetOperationName()
    {
      switch (this.Operation)
      {
        case '>':
        case '<':
        case '=':
          return this.Operation.ToString();

        case '≤':
          return "<=";
        case '≥':
          return ">=";
        case '≠':
          return "<>";

        default:
          return "LIKE";
      }
    }

    public string Query
    {
      get
      {
        if (string.Empty.Equals(this.Value) && this.Operation != '≠')
          return string.Format("Convert([{0}], 'System.String') LIKE '%'", this.Column);

        if ("Ø".Equals(this.Value))
        {
          if (this.Operation == '=')
            return string.Format("([{0}] IS NULL OR Convert([{0}], 'System.String') = '')", this.Column);

          if (this.Operation == '≠')
            return string.Format("([{0}] IS NOT NULL AND Convert([{0}], 'System.String') <> '')", this.Column);
        }

        return string.Format(this.ConvertColumn ?
          "Convert([{0}], 'System.String') {1} {2}" : "[{0}] {1} {2}",
          this.Column, this.GetOperationName(), GroupKey.GetInvariant(this.Value));
      }
    }
  }
}