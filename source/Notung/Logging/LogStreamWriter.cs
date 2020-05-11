using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Notung.Properties;

namespace Notung.Log
{
  public class LogStreamWriter : TextWriter
  {
    public override Encoding Encoding
    {
      get { return Encoding.UTF8; }
    }

    private readonly ILog m_logger;
    private readonly InfoLevel m_level;
    private readonly List<string> m_buffer = new List<string>();

    public LogStreamWriter(string source, InfoLevel level = InfoLevel.Debug)
    {
      m_logger = LogManager.GetLogger(source);
      m_level = level;
      this.DefaultMessage = "[" + Resources.STREAM_FLUSH + "]";
    }

    public InfoLevel Level
    {
      get { return m_level; }
    }

    public string DefaultMessage { get; set; }

    public override void Write(string value)
    {
      if (string.IsNullOrEmpty(value))
        return;

      lock (m_buffer)
        m_buffer.Add(value);

      if (value == this.NewLine)
      {
        this.Flush();
      }
    }

    protected override void Dispose(bool disposing)
    {
      this.Flush();
      base.Dispose(disposing);
    }

    public override void Close()
    {
      this.Flush();
    }

    public override void Flush()
    {
      lock (m_buffer)
      {
        if (m_buffer.Count > 0)
        {
          string message = m_buffer.Count > 1 ? this.DefaultMessage : m_buffer[0];
          string description = m_buffer.Count > 1 ? string.Join(this.NewLine, m_buffer.ToArray()).Trim() : null;
          m_logger.WriteLog(message, m_level, description);
          m_buffer.Clear();
        }
      }
    }

    public override void WriteLine(string format, params object[] arg)
    {
      this.WriteLine(string.Format(format, arg));
    }

    public override void Write(bool value)
    {
      this.Write(value.ToString());
    }

    public override void Write(char[] buffer)
    {
      this.Write(new string(buffer));
    }

    public override void Write(char value)
    {
      this.Write(value.ToString());
    }

    public override void Write(char[] buffer, int index, int count)
    {
      base.Write(new string(buffer.Skip(index).Take(count).ToArray()));
    }

    public override void Write(decimal value)
    {
      this.Write(value.ToString());
    }

    public override void Write(double value)
    {
      this.Write(value.ToString());
    }

    public override void Write(float value)
    {
      this.Write(value.ToString());
    }

    public override void Write(int value)
    {
      this.Write(value.ToString());
    }

    public override void Write(long value)
    {
      this.Write(value.ToString());
    }

    public override void Write(object value)
    {
      this.Write((value ?? "").ToString());
    }

    public override void Write(string format, object arg0)
    {
      this.Write(string.Format(format, arg0));
    }

    public override void Write(string format, object arg0, object arg1)
    {
      this.Write(string.Format(format, arg0, arg1));
    }

    public override void Write(string format, object arg0, object arg1, object arg2)
    {
      this.Write(string.Format(format, arg0, arg1, arg2));
    }

    public override void Write(string format, params object[] arg)
    {
      this.Write(string.Format(format, arg));
    }

    public override void Write(uint value)
    {
      this.Write(value.ToString());
    }

    public override void Write(ulong value)
    {
      this.Write(value.ToString());
    }

    public override void WriteLine()
    {
      this.Write(this.NewLine);
    }

    public override void WriteLine(string value)
    {
      this.Write(value + this.NewLine);
    }

    public override void WriteLine(bool value)
    {
      this.WriteLine(value.ToString());
    }

    public override void WriteLine(char value)
    {
      this.WriteLine(value.ToString());
    }

    public override void WriteLine(char[] buffer)
    {
      this.WriteLine(new string(buffer));
    }

    public override void WriteLine(char[] buffer, int index, int count)
    {
      this.WriteLine(new string(buffer.Skip(index).Take(count).ToArray()));
    }

    public override void WriteLine(decimal value)
    {
      this.WriteLine(value.ToString());
    }

    public override void WriteLine(double value)
    {
      this.WriteLine(value.ToString());
    }

    public override void WriteLine(float value)
    {
      this.WriteLine(value.ToString());
    }

    public override void WriteLine(int value)
    {
      this.WriteLine(value.ToString());
    }

    public override void WriteLine(long value)
    {
      this.WriteLine(value.ToString());
    }

    public override void WriteLine(object value)
    {
      this.WriteLine((value ?? "").ToString());
    }

    public override void WriteLine(string format, object arg0)
    {
      this.WriteLine(string.Format(format, arg0));
    }

    public override void WriteLine(string format, object arg0, object arg1)
    {
      this.WriteLine(string.Format(format, arg0, arg1));
    }

    public override void WriteLine(string format, object arg0, object arg1, object arg2)
    {
      this.WriteLine(string.Format(format, arg0, arg2));
    }

    public override void WriteLine(uint value)
    {
      this.WriteLine(value.ToString());
    }

    public override void WriteLine(ulong value)
    {
      this.WriteLine(value.ToString());
    }
  }
}
