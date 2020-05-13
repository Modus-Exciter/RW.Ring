using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Notung.Log
{
  public class LogStringBuilder
  {
    private readonly IBuildBlock[] m_blocks;

    public LogStringBuilder(string template)
    {
      if (template == null)
        throw new ArgumentNullException("template");

      m_blocks = StateMachine(template).ToArray();
    }

    private static List<IBuildBlock> StateMachine(string template)
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
            blocks.Add(new FormatFieldBlock(template.Substring(string_start, i - string_start)));
            string_start = i + 1;
            state = State.Format;
          }
          else if (template[i] == '}')
          {
            blocks.Add(new FieldBlock(template.Substring(string_start, i - string_start)));
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

      public FieldBlock(string field)
      {
        m_field = field;

        if (Enum.IsDefined(typeof(FieldType), field))
          m_type = (FieldType)Enum.Parse(typeof(FieldType), field);
        else
          m_type = FieldType.Context;
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
            writer.Write(data.LoggingDate.ToString());
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

      public FormatFieldBlock(string field) : base(field) { }

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
              writer.Write(data.LoggingDate.ToString());
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
