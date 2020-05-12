using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;

namespace Notung.Log
{
  public class LogStringBuilder
  {
    private readonly ReadOnlyCollection<IBuildBlock> m_blocks;

    private enum State
    {
      Start,
      Escape,
      Bracket,
      Parameter,
      Format,
      Token
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
        return m_token;
      }
    }

    private class FieldBlock : IBuildBlock
    {
      private readonly string m_field;
      private readonly FieldType m_type;
      public string Format;

      public FieldBlock(string field)
      {
        m_field = field;

        if (Enum.IsDefined(typeof(FieldType), field))
          m_type = (FieldType)Enum.Parse(typeof(FieldType), field);
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
      
      public void Build(TextWriter writer, LoggingData data)
      {
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
        if (!string.IsNullOrEmpty(Format))
          return m_field + ':' + Format;
        else
          return m_field;
      }
    }
    
    public LogStringBuilder(string template)
    {
      if (template == null)
        throw new ArgumentNullException("template");

      m_blocks = new ReadOnlyCollection<IBuildBlock>(StateMachine(template));
    }

    private IList<IBuildBlock> StateMachine(string template)
    {
      List<IBuildBlock> blocks = new List<IBuildBlock>();
      
      State state = State.Start;
      int string_start = 0;

      for (int i = 0; i < template.Length; i++)
      {
        if (state == State.Start)
        {
          if (template[i] == '\\')
          {
            blocks.Add(new TokenBlock(template.Substring(string_start, i - string_start)));
            string_start = i + 1;
          }
          else if (template[i] == '{')
          {
            blocks.Add(new TokenBlock(template.Substring(string_start, i - string_start)));
            string_start = i + 1;
            state = State.Bracket;
          }
        }
        else if (state == State.Bracket)
        {
          if (template[i] == ':')
          {
            blocks.Add(new FieldBlock(template.Substring(string_start, i - string_start)));
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
            ((FieldBlock)blocks[blocks.Count - 1]).Format = template.Substring(string_start, i - string_start);
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
      foreach (var block in m_blocks)
        block.Build(writer, data);

      if (data.Data != null)
      {
        writer.WriteLine();
        writer.Write(data.Data);
      }
    }
  }
}
