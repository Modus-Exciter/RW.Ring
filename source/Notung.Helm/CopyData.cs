using System;
using System.Diagnostics;
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
    /// <param name="expectedTypeCode">Ожидаемый идентификатор типа или 0.
    /// Если получен идентификатор типа, отличный от ожидаемого, данные не загружаются</param>
    public unsafe CopyData(IntPtr lParam, uint expectedTypeCode = 0)
    {
      var copyData = *((COPYDATASTRUCT*)lParam.ToPointer());
      m_type_code = (uint)copyData.dwData.ToInt32();

      if (expectedTypeCode == 0 || m_type_code == expectedTypeCode)
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
    /// <param name="destination">Дескриптор главного окна процесса, которому надо передать данные</param>
    /// <returns>Результат обработки сообщения принимающей стороной</returns>
    public unsafe IntPtr Send(IntPtr destination)
    {
      if (!m_can_send)
        throw new InvalidOperationException(Resources.COPY_DATA_SEND_RECIEVE);

      IntPtr source;

      using (var process = Process.GetCurrentProcess())
        source = process.MainWindowHandle;

      fixed (byte* array = m_data)
      {
        var copyData = default(COPYDATASTRUCT);
        copyData.lpData = new IntPtr(array);
        copyData.dwData = new IntPtr(m_type_code);
        copyData.cbData = (uint)m_data.Length;

        return WinAPIHelper.SendMessage(destination, WinAPIHelper.WM_COPYDATA, source, new IntPtr(&copyData));
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