using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Notung.Helm.Windows
{
  /// <summary>
  /// Обёртка над объектом Windows для передачи текстовых
  /// сообщений размером до 255 байт между приложениями
  /// </summary>
  public sealed class Atom : IDisposable
  {
    private volatile ushort m_handle;
    private readonly int m_buffer_size;
    private readonly string m_text;
    private readonly bool m_created_new;

    /// <summary>
    /// Создание объекта операционной системы на передающей стороне
    /// </summary>
    /// <param name="text">Текст, который требуется передать</param>
    public Atom(string text)
    {
      if (string.IsNullOrEmpty(text))
        throw new ArgumentNullException("text");

      bool new_required = true;

      m_handle = GlobalFindAtom(text);

      if (m_handle == 0)
        m_handle = GlobalAddAtom(text);
      else
        new_required = false;

      if (m_handle != 0)
      {
        m_buffer_size = text.Length + 1;
        m_created_new = new_required;
        m_text = text;
      }
      else
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Создание объекта операционной системы на принимающей стороне
    /// </summary>
    /// <param name="handle">Дескриптор объекта в операционной системе</param>
    /// <param name="bufferSize">Длина передаваемой строки</param>
    public Atom(IntPtr handle, int bufferSize)
    {
      if (bufferSize < 1)
        throw new ArgumentOutOfRangeException("bufferSize");

      m_handle = (ushort)handle;
      m_buffer_size = bufferSize;
      var buffer = new StringBuilder(m_buffer_size);

      if (GlobalGetAtomName(m_handle, buffer, m_buffer_size) != 0)
        m_text = buffer.ToString();
      else
        m_handle = 0;
    }

    /// <summary>
    /// Удалось ли создать объект в системе
    /// </summary>
    public bool IsValid
    {
      get { return m_handle != 0; }
    }

    /// <summary>
    /// Дескриптор объекта в операционной системе
    /// </summary>
    public IntPtr Handle
    {
      get { return new IntPtr(m_handle); }
    }

    /// <summary>
    /// Длина передаваемой строки
    /// </summary>
    public int BufferSize
    {
      get { return m_buffer_size; }
    }

    /// <summary>
    /// Текст, передаваемый от одного приложения к другому
    /// </summary>
    public string Text
    {
      get { return m_text; }
    }

    /// <summary>
    /// Отправка атома другому приложению
    /// </summary>
    /// <param name="destination">Дескриптор главного окна другого приложения</param>
    /// <param name="messageCode">Тип сообщения, который должно принять другое приложения</param>
    /// <returns>Отклик от другого приложения</returns>
    public IntPtr Send(IntPtr destination, uint messageCode)
    {
      return WinAPIHelper.SendMessage(destination, messageCode, new IntPtr(m_buffer_size), this.Handle);
    }

    public override bool Equals(object obj)
    {
      var other = obj as Atom;

      if (other == null)
        return false;

      return m_handle == other.m_handle;
    }

    public override int GetHashCode()
    {
      return m_handle;
    }

    public override string ToString()
    {
      return m_text ?? CoreResources.NULL;
    }

    ~Atom()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
      if (m_handle == 0 || !m_created_new) // Taras Bulba
        return;

      GlobalDeleteAtom(m_handle);

      if (disposing)
        m_handle = 0;
    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern ushort GlobalFindAtom(string lpString);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern ushort GlobalAddAtom(string lpString);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern uint GlobalGetAtomName(ushort nAtom, StringBuilder lpBuffer, int nSize);

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    private static extern ushort GlobalDeleteAtom(ushort nAtom);
  }
}
