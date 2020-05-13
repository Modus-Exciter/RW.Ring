using System;
using System.Runtime.InteropServices;
using Notung.Helm.Properties;

namespace Notung.Helm
{
  /// <summary>
  /// Класс для передачи данных от одного процесса Windows к другому через WM_COPYDATA
  /// </summary>
  public class CopyData
  {
    private readonly byte[] m_data;
    private readonly uint m_type_code;
    private readonly bool m_can_send;
    
    /// <summary>
    /// Создание объекта передачи данных на передающей стороне
    /// </summary>
    /// <param name="data">Данные, которые требуется передать другому процессу</param>
    /// <param name="typeCode">Идентификатор типа данных для другого процесса</param>
    public CopyData(byte[] data, uint typeCode)
    {
      if (data == null)
        throw new ArgumentNullException("data");
      
      m_data = data;
      m_type_code = typeCode;
      m_can_send = true;
    }

    /// <summary>
    /// Создание объекта передачи данных на принимающей стороне
    /// </summary>
    /// <param name="lParam">Идентификатор объекта, полученный из Message.LParam</param>
    public CopyData(IntPtr lParam, uint expectedTypeCode)
    {
      var copyData = (COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT));

      m_type_code = (uint)copyData.dwData.ToInt32();

      if (m_type_code == expectedTypeCode || expectedTypeCode == 0)
      {
        m_data = new byte[copyData.cbData];
        Marshal.Copy(copyData.lpData, m_data, 0, m_data.Length);
      }
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
      get { return m_type_code; }
    }

    /// <summary>
    /// Отправка данных другому процессу
    /// </summary>
    /// <param name="source">Дескриптор главного окна текущего процесса</param>
    /// <param name="destination">Дескриптор главного окна процесса, которому надо передать данные</param>
    /// <returns>Результат обработки сообщения принимающей стороной</returns>
    public unsafe IntPtr Send(IntPtr source, IntPtr destination)
    {
      if (!m_can_send)
        throw new InvalidOperationException(Resources.COPY_DATA_SEND_RECIEVE);

      fixed (byte* array = m_data)
      {
        var copyData = default(COPYDATASTRUCT);
        copyData.lpData = new IntPtr(array);
        copyData.dwData = new IntPtr(m_type_code);
        copyData.cbData = (uint)m_data.Length;

        return WinAPIHelper.SendMessage(destination, WinAPIHelper.WM_COPYDATA, source, new IntPtr(&copyData));
      }
    }

    public override string ToString()
    {
      return string.Format(m_can_send ? "{0} bytes to send, type code {1}" : "{0} bytes recieved, type code {1}",
        (m_data ?? ArrayExtensions.Empty<byte>()).Length, m_type_code);
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