using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Notung.Threading;

namespace Notung.Data
{
  /// <summary>
  /// Код типа данных для передачи данных между приложениями
  /// </summary>
  public struct DataTypeCode
  {
    private readonly uint m_code;
    private static readonly List<string> _names = new List<string>();
    private static readonly SharedLock _lock = new SharedLock(false);

    public static readonly DataTypeCode Empty = default(DataTypeCode);

    public DataTypeCode(uint code) : this()
    {
      m_code = code;
    }

    /// <summary>
    /// Числовое значение кода типа данных
    /// </summary>
    public uint Code
    {
      get { return m_code; }
    }

    /// <summary>
    /// Название кода типа данных
    /// </summary>
    public string Name
    {
      get
      {
        if (m_code == 0)
          return CoreResources.NULL;

        using (_lock.ReadLock())
        {
          if (m_code <= _names.Count)
            return _names[(int)m_code - 1] ?? CoreResources.UNKNOWN;
          else
            return CoreResources.UNKNOWN;
        }
      }
    }

    /// <summary>
    /// Поиск кода типа данных по названию. Если код с таким названием не найден, он добавляется
    /// </summary>
    /// <param name="typeCodeName">Название типа данных</param>
    /// <returns>Код типа данных</returns>
    public static DataTypeCode FindOrCreate(string typeCodeName)
    {
      if (string.IsNullOrEmpty(typeCodeName))
        return default(DataTypeCode);

      using (_lock.WriteLock())
      {
        var idx = _names.IndexOf(typeCodeName);

        if (idx >= 0)
          return new DataTypeCode((uint)(idx + 1));

        _names.Add(typeCodeName);
        return new DataTypeCode((uint)_names.Count);
      }
    }

    /// <summary>
    /// Задание названия для кода типа данных. Создаётся новое либо заменяется
    /// </summary>
    /// <param name="code">Код типа данных (числовое значение больше 0)</param>
    /// <param name="typeCodeName">Новое название кода типа данных</param>
    public static void Set(DataTypeCode code, string typeCodeName)
    {
      if (string.IsNullOrEmpty(typeCodeName) || code.m_code == 0)
        return;

      using (_lock.WriteLock())
      {
        if (code.m_code > _names.Count)
          _names.AddRange(new string[code.m_code - _names.Count]);

        _names[(int)code.m_code - 1] = typeCodeName;
      }
    }

    /// <summary>
    /// Получение всех кодов типов данных с названиями
    /// </summary>
    public static Dictionary<DataTypeCode, string> GetAllCodes()
    {
      using (_lock.ReadLock())
      {
        Dictionary<DataTypeCode, string> ret = new Dictionary<DataTypeCode, string>();

        for (int i = 0; i < _names.Count; i++)
        {
          if (_names[i] != null)
            ret.Add(new DataTypeCode((uint)i), _names[i]);
        }

        return ret;
      }
    }

    public override string ToString()
    {
      return this.Name;
    }

    public override bool Equals(object obj)
    {
      return obj is DataTypeCode && ((DataTypeCode)obj).m_code == m_code;
    }

    public override int GetHashCode()
    {
      return m_code.GetHashCode();
    }

    public static implicit operator DataTypeCode(uint code)
    {
      return new DataTypeCode(code);
    }

    public static bool operator ==(DataTypeCode first, DataTypeCode second)
    {
      return first.m_code == second.m_code;
    }

    public static bool operator !=(DataTypeCode first, DataTypeCode second)
    {
      return first.m_code != second.m_code;
    }
  }
}
