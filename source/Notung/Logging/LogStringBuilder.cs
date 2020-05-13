using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Notung.Threading;

namespace Notung.Log
{
  public class LogStringBuilder
  {
    private readonly IBuildBlock[] m_blocks;
    private readonly ThreadField<char[]> m_date_converter = new ThreadField<char[]>();

    public LogStringBuilder(string template)
    {
      if (template == null)
        throw new ArgumentNullException("template");

      m_blocks = StateMachine(template).ToArray();
    }

    private List<IBuildBlock> StateMachine(string template)
    {
      List<IBuildBlock> blocks = new List<IBuildBlock>(0x10);
      
      State state = State.Start;
      int string_start = 0;

      for (int i = 0; i < template.Length; i++)
      {
        if (state == State.Start)
        {
          if (template[i] == '\\')
          {
            blocks.Add(new TokenBlock(template.Substring(string_start, i - string_start)));
            state = State.Escape;
            string_start = i + 1;
          }
          else if (template[i] == '{')
          {
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
            blocks.Add(new FormatFieldBlock(template.Substring(string_start, i - string_start), m_date_converter));
            string_start = i + 1;
            state = State.Format;
          }
          else if (template[i] == '}')
          {
            blocks.Add(new FieldBlock(template.Substring(string_start, i - string_start), m_date_converter));
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

    public void BuildString(TextWriter writer, LoggingData data)
    {
#if DEBUG
      if (writer == null)
        throw new ArgumentNullException("writer");
#endif
      InitDateConverter();
      
      for (int i = 0; i < m_blocks.Length; i++)
        m_blocks[i].Build(writer, data);

      if (data.Data != null)
      {
        writer.WriteLine();
        writer.Write(data.Data);
      }
    }

    public override string ToString()
    {
      return string.Join("", m_blocks.Select(b => b.ToString()));
    }

    private void InitDateConverter()
    {
      if (m_date_converter.Instance == null)
      {
        var array = new char[23];

        array[2] = '.';
        array[5] = '.';
        array[10] = ' ';
        array[13] = ':';
        array[16] = ':';
        array[19] = '.';

        m_date_converter.Instance = array;
      }
    }

    #region Implementation

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
      void Build(TextWriter writer, LoggingData data);
    }

    private class TokenBlock : IBuildBlock
    {
      private readonly string m_token;

      public TokenBlock(string token)
      {
        m_token = token;
      }

      public void Build(TextWriter writer, LoggingData data)
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
      protected readonly string m_field;
      protected readonly FieldType m_type;
      private readonly ThreadField<char[]> m_date_converter;

      public FieldBlock(string field, ThreadField<char[]> dateConverter)
      {
        m_field = field;
        m_date_converter = dateConverter;

        if (Enum.IsDefined(typeof(FieldType), field))
          m_type = (FieldType)Enum.Parse(typeof(FieldType), field);
        else
          m_type = FieldType.Context;
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

      public virtual void Build(TextWriter writer, LoggingData data)
      {
        switch (m_type)
        {
          case FieldType.Source:
            if (!string.IsNullOrEmpty(data.Source))
              writer.Write(data.Source);
            break;
          case FieldType.Date:
            WriteDate(writer, data.LoggingDate);
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

      public override void Build(TextWriter writer, LoggingData data)
      {
        if (m_format == null)
          m_format = "{0" + Format + "}";

        switch (m_type)
        {
          case FieldType.Source:
            if (!string.IsNullOrEmpty(data.Source))
              writer.Write(data.Source);
            break;
          case FieldType.Date:
            if (!string.IsNullOrEmpty(Format))
              writer.Write(data.LoggingDate.ToString(Format));
            else
              WriteDate(writer, data.LoggingDate);
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
        if (!string.IsNullOrEmpty(Format))
          return "{" + m_field + ':' + Format + "}";
        else
          return "{" + m_field + "}";
      }
    }

    #endregion
  }
}
