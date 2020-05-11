using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Notung.Helm
{
  public sealed class Atom : IDisposable
  {
    private volatile ushort m_handle;
    private readonly int m_buffer_size;
    private readonly string m_text;
    private readonly bool m_created_new;

    public Atom(string text)
    {
      if (string.IsNullOrEmpty(text))
        throw new ArgumentNullException("text");

      m_handle = GlobalAddAtom(text);
      m_text = text;

      if (m_handle != 0)
      {
        m_buffer_size = text.Length + 1;
        m_created_new = true;
      }
      else
        GC.SuppressFinalize(this);
    }

    public Atom(ushort handle, int bufferSize)
    {
      m_handle = handle;
      m_buffer_size = (int)bufferSize;
      var m_buffer = new StringBuilder(m_buffer_size);

      if (GlobalGetAtomName(m_handle, m_buffer, m_buffer_size) != 0)
        m_text = m_buffer.ToString();
    }

    public bool IsValid
    {
      get { return m_handle != 0; }
    }

    public IntPtr Handle
    {
      get { return new IntPtr(m_handle); }
    }

    public uint BufferSize
    {
      get { return (uint)m_buffer_size; }
    }

    public string Text
    {
      get { return m_text; }
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

      if (disposing)
      {
        GlobalDeleteAtom(m_handle);
        m_handle = 0;
      }
      else
        AppManager.Instance.ApartmentWrapper.Invoke(delegate()
        { 
          GlobalDeleteAtom(m_handle);
          m_handle = 0;
        });
    }

    #region Import

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern ushort GlobalAddAtom(string lpString);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern uint GlobalGetAtomName(ushort nAtom, StringBuilder lpBuffer, int nSize);

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    private static extern ushort GlobalDeleteAtom(ushort nAtom);

    #endregion
  }
}
