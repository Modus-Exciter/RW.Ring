using System;
using System.Runtime.InteropServices;
using Notung.Data;
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
    public CopyData(byte[] data, DataTypeCode typeCode)
    {
      if (data == null)
        throw new ArgumentNullException("data");
      
      m_data = data;
      m_type_code = typeCode.Code;
      m_can_send = true;
    }

    /// <summary>
    /// Создание объекта передачи данных на принимающей стороне
    /// </summary>
    /// <param name="lParam">Идентификатор объекта, полученный из Message.LParam</param>
    /// <param name="expectedTypeCode">Ожидаемый идентификатор типа или 0.
    /// Если получен идентификатор типа, отличный от ожидаемого, данные не загружаются</param>
    public unsafe CopyData(IntPtr lParam, DataTypeCode expectedTypeCode = default(DataTypeCode))
    {
      COPYDATASTRUCT* data_pointer = (COPYDATASTRUCT*)lParam.ToPointer();

      m_type_code = (uint)data_pointer->dwData.ToInt32();

      if (expectedTypeCode == DataTypeCode.Empty || m_type_code == expectedTypeCode.Code)
      {
        m_data = new byte[data_pointer->cbData];
        Marshal.Copy(data_pointer->lpData, m_data, 0, m_data.Length);
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
    public DataTypeCode TypeCode
    {
      get { return m_type_code; }
    }

    /// <summary>
    /// Отправка данных другому процессу
    /// </summary>
    /// <param name="destination">Дескриптор главного окна процесса, которому надо передать данные</param>
    /// <param name="timeout">Время ожидания ответа от принимающей стороны. Значение по умолчанию - не ограничено</param>
    /// <returns>Результат обработки сообщения принимающей стороной</returns>
    public unsafe IntPtr Send(IntPtr destination, TimeSpan timeout = default(TimeSpan))
    {
      if (!m_can_send)
        throw new InvalidOperationException(Resources.COPY_DATA_SEND_RECIEVE);

      IntPtr source = ApplicationInfo.Instance.CurrentProcess.MainWindowHandle;

      fixed (byte* array = m_data)
      {
        var copyData = default(COPYDATASTRUCT);
        copyData.lpData = new IntPtr(array);
        copyData.dwData = new IntPtr(m_type_code);
        copyData.cbData = (uint)m_data.Length;

        if (timeout == default(TimeSpan))
          return WinAPIHelper.SendMessage(destination, WinAPIHelper.WM_COPYDATA, source, new IntPtr(&copyData));
        else
        {
          IntPtr result;

          if (WinAPIHelper.SendMessageTimeoutA(destination, WinAPIHelper.WM_COPYDATA, source, new IntPtr(&copyData),
            WinAPIHelper.SMTO_BLOCK, (uint)timeout.TotalMilliseconds, new IntPtr(&result)) != IntPtr.Zero)
            return result;

          else return IntPtr.Zero;
        }
      }
    }

    public override string ToString()
    {
      return string.Format("{0} bytes {1}; type code {2} - {3}", (m_data ?? ArrayExtensions.Empty<byte>()).Length,
        m_can_send ? "to send" : "recieved", m_type_code, this.TypeCode);
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