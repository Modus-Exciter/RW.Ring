using System;
using System.Runtime.InteropServices;
using Notung.Helm.Properties;

namespace Notung.Helm
{
  /// <summary>
  /// Класс для передачи данных от одного процесса Windows к другому через WM_COPYDATA
  /// </summary>
  public class CopyData : IDisposable
  {
    private COPYDATASTRUCT m_struct;
    private readonly byte[] m_data;
    private readonly bool m_is_new;
    
    /// <summary>
    /// Создание объекта передачи данных на передающей стороне
    /// </summary>
    /// <param name="data">Данные, которые требуется передать другому процессу</param>
    /// <param name="typecode">Идентификатор типа данных для другого процесса</param>
    public CopyData(byte[] data, uint typecode)
    {
      if (data == null)
        throw new ArgumentNullException("data");
      
      m_data = data;
 
      m_struct.dwData = new IntPtr(typecode);
      m_struct.cbData = (uint)data.Length;
      m_struct.lpData = Marshal.AllocHGlobal(data.Length);

      m_is_new = true;
    }

    /// <summary>
    /// Создание объекта передачи данных на принимающей стороне
    /// </summary>
    /// <param name="lParam">Идентификатор объекта, полученный из Message.LParam</param>
    public CopyData(IntPtr lParam)
    {
      m_struct = (COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT));
      m_data = new byte[m_struct.cbData];
      Marshal.Copy(m_struct.lpData, m_data, 0, m_data.Length);
    }

    /// <summary>
    /// Данные, передаваемые от одного процесса к другому
    /// </summary>
    public byte[] Data
    {
      get { return m_data; }
    }

    /// <summary>
    /// Идентификатор типа данных
    /// </summary>
    public uint TypeCode
    {
      get { return (uint)m_struct.dwData.ToInt32(); }
    }

    /// <summary>
    /// Удалось ли создать объект для передачи данных
    /// </summary>
    public bool IsValid
    {
      get { return m_struct.lpData != IntPtr.Zero; }
    }

    /// <summary>
    /// Отправка данных другому процессу
    /// </summary>
    /// <param name="source">Дескриптор главного окна текущего процесса</param>
    /// <param name="destination">Дескриптор главного окна процесса, которому надо передать данные</param>
    /// <returns></returns>
    public IntPtr Send(IntPtr source, IntPtr destination)
    {
      if (!m_is_new)
        throw new InvalidOperationException(Resources.COPY_DATA_SEND_RECIEVE);
      
      if (m_struct.lpData == IntPtr.Zero)
        return IntPtr.Zero;

      IntPtr struct_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(COPYDATASTRUCT)));

      if (struct_ptr == IntPtr.Zero)
        return IntPtr.Zero;

      try
      {
        Marshal.Copy(m_data, 0, m_struct.lpData, m_data.Length);
        Marshal.StructureToPtr(m_struct, struct_ptr, false);
        return WinAPIHelper.SendMessage(destination, WinAPIHelper.WM_COPYDATA, source, struct_ptr);
      }
      finally
      {
        Marshal.FreeHGlobal(struct_ptr);
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    ~CopyData()
    {
      this.Dispose(false);
    }

    private void Dispose(bool disposing)
    {
      if (m_is_new && m_struct.lpData != IntPtr.Zero)
      {
        Marshal.FreeHGlobal(m_struct.lpData);
        m_struct.lpData = IntPtr.Zero;
      }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct COPYDATASTRUCT
    {
      public IntPtr dwData;
      public uint cbData;
      public IntPtr lpData;
    }
  }
}
