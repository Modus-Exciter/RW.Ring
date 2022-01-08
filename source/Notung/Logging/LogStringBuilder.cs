using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Notung.Properties;
using Notung.Threading;

namespace Notung.Logging
{
  /// <summary>
  /// Преобразование сообщений лога в текст и обратно
  /// </summary>
  public sealed class LogStringBuilder
  {
    private readonly IBuildBlock[] m_blocks;
    private readonly ThreadField<char[]> m_date_converter = new ThreadField<char[]>();
    private volatile bool m_data_included;

    private static readonly int _pid = Global.CurrentProcess.Id;

    private const int DATE_CONVERTER_SIZE = 23;

    /// <summary>
    /// Инициализация преобразования на основе шаблона
    /// </summary>
    /// <param name="template">Шаблон сообщения лога</param>
    public LogStringBuilder(string template)
    {
      if (template == null)
        throw new ArgumentNullException("template");

      m_blocks = this.StateMachine(template).ToArray();
    }

    /// <summary>
    /// Преобразование сообщения логгирования в текст
    /// </summary>
    /// <param name="writer">Поток, в который нужно записать текст</param>
    /// <param name="data">Сообщение логгирования</param>
    public void BuildString(TextWriter writer, LoggingEvent data)
    {
#if DEBUG
      if (writer == null)
        throw new ArgumentNullException("writer");
#endif
      this.InitDateConverter();

      for (int i = 0; i < m_blocks.Length; i++)
        m_blocks[i].Build(writer, data);

      if (!m_data_included && data.Data != null)
      {
        writer.WriteLine();
        writer.Write(data.Data);
      }
    }

    /// <summary>
    /// Заполнение строки таблицы данными из текста сообщения логгиирования
    /// </summary>
    /// <param name="rawText">Сообщение логгирования, преобразованное в текст</param>
    /// <param name="table">Таблица, в которой требуется создать строку</param>
    /// <param name="createDetailsColumn">Создавать ли колонку с деталями в таблицу</param>
    public void FillRow(string rawText, DataTable table, bool createDetailsColumn = false)
    {
      int[] token_starts = this.GetTokenStarts(rawText);

      if (table.Rows.Count == 0)
        this.CreateTableColumns(table, createDetailsColumn);

      var row = table.NewRow();

      for (int i = 0; i < token_starts.Length; i++)
      {
        var field = m_blocks[i] as FieldBlock;
        if (field != null)
        {
          string token = i < token_starts.Length - 1 ?
            rawText.Substring(token_starts[i], token_starts[i + 1] - token_starts[i]) :
            rawText.Substring(token_starts[i]);

          switch (field.Type)
          {
            case FieldType.Date:
              var format = field as FormatFieldBlock;

              if (format != null && !string.IsNullOrEmpty(format.Format))
                row[field.m_field] = DateTime.ParseExact(token, format.Format, CultureInfo.InvariantCulture);
              else
                row[field.m_field] = DateTime.ParseExact(token, "dd.MM.yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);

              break;

            case FieldType.Process:
              row[field.m_field] = int.Parse(token);
              break;

            default:
              row[field.m_field] = token;
              break;
          }
        }
      }

      table.Rows.Add(row);
    }

    public override string ToString()
    {
      return string.Join("", m_blocks.Select(b => b.ToString()));
    }

    #region Private methods -----------------------------------------------------------------------

    private List<IBuildBlock> StateMachine(string template)
    {
      var blocks = new List<IBuildBlock>(0x10);

      State state = State.Start;
      int string_start = 0;

      for (int i = 0; i < template.Length; i++)
      {
        if (state == State.Start)
        {
          if (template[i] == '\\')
          {
            if (string_start < i)
              blocks.Add(new TokenBlock(template.Substring(string_start, i - string_start)));
            state = State.Escape;
            string_start = i + 1;
          }
          else if (template[i] == '{')
          {
            if (string_start < i)
              blocks.Add(new TokenBlock(template.Substring(string_start, i - string_start)));
            string_start = i + 1;
            state = State.Bracket;
          }
        }
        else if (state == State.Escape)
        {
          state = State.Start;
        }
        else if (state == State.Bracket)
        {
          if (template[i] == ':')
          {
            var block = new FormatFieldBlock(template.Substring(string_start, i - string_start), m_date_converter);

            if (block.Type == FieldType.Data)
              m_data_included = true;

            blocks.Add(block);
            string_start = i + 1;
            state = State.Format;
          }
          else if (template[i] == '}')
          {
            var block = new FieldBlock(template.Substring(string_start, i - string_start), m_date_converter);

            if (block.Type == FieldType.Data)
              m_data_included = true;

            blocks.Add(block);
            string_start = i + 1;
            state = State.Start;
          }
        }
        else if (state == State.Format)
        {
          if (template[i] == '}')
          {
            ((FormatFieldBlock)blocks[blocks.Count - 1]).Format = template.Substring(string_start, i - string_start);
            string_start = i + 1;
            state = State.Start;
          }
        }
      }

      if (string_start < template.Length)
        blocks.Add(new TokenBlock(template.Substring(string_start, template.Length - string_start)));

      return blocks;
    }

    private void InitDateConverter()
    {
      if (m_date_converter.Instance == null)
      {
        var array = new char[DATE_CONVERTER_SIZE];

        array[2] = '.';
        array[5] = '.';
        array[10] = ' ';
        array[13] = ':';
        array[16] = ':';
        array[19] = '.';

        m_date_converter.Instance = array;
      }
    }

    private int[] GetTokenStarts(string rawText)
    {
      int[] token_starts = new int[m_blocks.Length];
      int start = 0;

      for (int i = 0; i < m_blocks.Length; i++)
      {
        var token = m_blocks[i] as TokenBlock;

        if (token != null)
        {
          start = rawText.IndexOf(token.Token, start);

          if (start >= 0)
          {
            token_starts[i] = start;
            start += token.Token.Length;

            if (i < m_blocks.Length - 1)
              token_starts[i + 1] = start;
          }
          else
            throw new FormatException(Resources.LOG_PARSE_FAIL);
        }
      }

      for (int i = 1; i < token_starts.Length; i++)
      {
        if (token_starts[i] > 0)
          continue;

        var block = m_blocks[i - 1] as FieldBlock;

        if (block != null)
          token_starts[i] = token_starts[i - 1] + this.GetTokenLength(rawText, block, i - 1, token_starts);
      }

      return token_starts;
    }

    private int GetTokenLength(string rawText, FieldBlock block, int index, int[] token_starts)
    {
      switch (block.Type)
      {
        case FieldType.Level:
          var level = Enum.GetValues(typeof(InfoLevel)).Cast<InfoLevel>().Select(l =>
            l.ToString()).SingleOrDefault(l => rawText.IndexOf(l, token_starts[index]) > 0);

          if (level != null)
            return level.Length;
          else
            throw new FormatException(string.Format(Resources.ENUM_PARSE_FAIL, typeof(InfoLevel)));

        case FieldType.Date:
          var format = block as FormatFieldBlock;

          if (format != null && !string.IsNullOrEmpty(format.Format))
            return DateTime.Now.ToString(format.Format).Length;
          else
            return DATE_CONVERTER_SIZE;

        case FieldType.Process:
          var x = token_starts[index];

          while (char.IsDigit(rawText[x]))
            x++;

          return x - token_starts[index];

        default:
          throw new InvalidExpressionException(string.Format(Resources.NO_TOKEN_LENGTH, block.Type));
      }
    }

    private void CreateTableColumns(DataTable table, bool createDetailsColumn)
    {
      for (int i = 0; i < m_blocks.Length; i++)
      {
        var field = m_blocks[i] as FieldBlock;

        if (field != null)
        {
          if (table.Columns.Contains(field.m_field))
            continue;

          var column = table.Columns.Add(field.m_field);

          switch (field.Type)
          {
            case FieldType.Date:
              column.DataType = typeof(DateTime);
              break;

            case FieldType.Process:
              column.DataType = typeof(int);
              break;

            default:
              column.DataType = typeof(string);
              break;
          }
        }
      }

      if (!table.Columns.Contains("Details") && createDetailsColumn)
        table.Columns.Add("Details").DataType = typeof(string);
    }

    #endregion

    #region Implementation types ------------------------------------------------------------------

    private enum State
    {
      Start,
      Escape,
      Bracket,
      Parameter,
      Format,
      Token
    }

    private enum FieldType
    {
      Source,
      Date,
      Message,
      Level,
      Data,
      Process,
      Thread,
      Context
    }

    private interface IBuildBlock
    {
      void Build(TextWriter writer, LoggingEvent data);
    }

    private class TokenBlock : IBuildBlock
    {
      private readonly string m_token;

      public TokenBlock(string token)
      {
        m_token = token;
      }

      public string Token
      {
        get { return m_token; }
      }

      public void Build(TextWriter writer, LoggingEvent data)
      {
        writer.Write(m_token);
      }

      public override string ToString()
      {
        if (m_token.Length > 0 && m_token[0] == '{')
          return "\\" + m_token;

        return m_token;
      }
    }

    private class FieldBlock : IBuildBlock
    {
      public readonly string m_field;
      public readonly FieldType Type;
      private readonly ThreadField<char[]> m_date_converter;

      public FieldBlock(string field, ThreadField<char[]> dateConverter)
      {
        m_field = field;
        m_date_converter = dateConverter;

        if (Enum.IsDefined(typeof(FieldType), field))
          Type = (FieldType)Enum.Parse(typeof(FieldType), field);
        else
          Type = FieldType.Context;
      }

      protected void WriteDate(TextWriter writer, DateTime date)
      {
        var array = m_date_converter.Instance;

        if (array == null)
          throw new InvalidOperationException();

        array[0] = (char)('0' + date.Day / 10);
        array[1] = (char)('0' + date.Day % 10);
        array[3] = (char)('0' + date.Month / 10);
        array[4] = (char)('0' + date.Month % 10);
        array[6] = (char)('0' + date.Year / 1000);
        array[7] = (char)('0' + (date.Year / 100) % 10);
        array[8] = (char)('0' + (date.Year / 10) % 10);
        array[9] = (char)('0' + date.Year % 10);
        array[11] = (char)('0' + date.TimeOfDay.Hours / 10);
        array[12] = (char)('0' + date.TimeOfDay.Hours % 10);
        array[14] = (char)('0' + date.TimeOfDay.Minutes / 10);
        array[15] = (char)('0' + date.TimeOfDay.Minutes % 10);
        array[17] = (char)('0' + date.TimeOfDay.Seconds / 10);
        array[18] = (char)('0' + date.TimeOfDay.Seconds % 10);
        array[20] = (char)('0' + date.TimeOfDay.Milliseconds / 100);
        array[21] = (char)('0' + date.TimeOfDay.Milliseconds / 10 % 10);
        array[22] = (char)('0' + date.TimeOfDay.Milliseconds % 10);

        writer.Write(array);
      }

      protected static void WriteThread(TextWriter writer, LoggingEvent data)
      {
        if (data.Thread == null)
          return;

        writer.Write(data.Thread.ManagedThreadId);

        if (!string.IsNullOrEmpty(data.Thread.Name))
        {
          writer.Write(' ');
          writer.Write(data.Thread.Name);
        }
      }

      public virtual void Build(TextWriter writer, LoggingEvent data)
      {
        switch (Type)
        {
          case FieldType.Source:
            if (!string.IsNullOrEmpty(data.Source))
              writer.Write(data.Source);
            break;

          case FieldType.Date:
            this.WriteDate(writer, data.LoggingDate);
            break;

          case FieldType.Message:
            if (!string.IsNullOrEmpty(data.Message))
              writer.Write(data.Message);
            break;

          case FieldType.Level:
            writer.Write(data.Level);
            break;

          case FieldType.Data:
            if (data.Data != null)
              writer.Write(data.Data);
            break;

          case FieldType.Process:
            writer.Write(_pid);
            break;

          case FieldType.Thread:
            WriteThread(writer, data);
            break;

          default:
            if (data.Contains(m_field))
              writer.Write(data[m_field]);
            break;
        }
      }

      public override string ToString()
      {
        return "{" + m_field + "}";
      }
    }

    private class FormatFieldBlock : FieldBlock
    {
      public string Format;
      private string m_format;

      public FormatFieldBlock(string field, ThreadField<char[]> dateConverter) : base(field, dateConverter) { }

      public override void Build(TextWriter writer, LoggingEvent data)
      {
        if (m_format == null)
          m_format = "{0:" + Format + "}";

        switch (Type)
        {
          case FieldType.Source:
            if (!string.IsNullOrEmpty(data.Source))
              writer.Write(data.Source);
            break;

          case FieldType.Date:
            if (!string.IsNullOrEmpty(Format))
              writer.Write(m_format, data.LoggingDate);
            else
              this.WriteDate(writer, data.LoggingDate);
            break;

          case FieldType.Process:
            writer.Write(_pid);
            break;

          case FieldType.Thread:
            WriteThread(writer, data);
            break;

          case FieldType.Message:
            if (!string.IsNullOrEmpty(data.Message))
              writer.Write(data.Message);
            break;

          case FieldType.Level:
            writer.Write(data.Level);
            break;

          case FieldType.Data:
            if (data.Data == null)
              return;
            if (string.IsNullOrEmpty(Format))
              writer.Write(data.Data);
            else
              writer.Write(m_format, data.Data);
            break;

          default:
            if (!data.Contains(m_field))
              return;
            if (string.IsNullOrEmpty(Format))
              writer.Write(data[m_field]);
            else
              writer.Write(m_format, data[m_field]);
            break;
        }
      }

      public override string ToString()
      {
        if (string.IsNullOrEmpty(Format))
          return "{" + m_field + "}";
        else
          return "{" + m_field + ':' + Format + "}";
      }
    }

    #endregion
  }
}