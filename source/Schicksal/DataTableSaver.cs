using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Schicksal.Properties;

namespace Schicksal
{
  /// <summary>
  /// Класс для компактного сохранения таблиц данных
  /// </summary>
  public static class DataTableSaver
  {
    /// <summary>
    /// Запись таблицы данных в поток
    /// </summary>
    /// <param name="table">Таблица, которую требуется сохранить</param>
    /// <param name="stream">Поток, куда требуется сохранить таблицу</param>
    public static void WriteDataTable(DataTable table, Stream stream)
    {
      if (table == null)
        throw new ArgumentNullException("table");

      var writer = new CompactBinaryWriter(stream, Encoding.UTF8);

      writer.Write(table.Columns.Count);
      writer.Write(table.Rows.Count);

      var runners = new IDataRunner[table.Columns.Count];
      var key_columns = table.PrimaryKey;

      foreach (DataColumn col in table.Columns)
      {
        var type_code = GetTypeCode(col.DataType);
        var code_with_flags = type_code;

        if (!col.AllowDBNull)
          code_with_flags |= _mandatory_flag;

        if (Array.Find(key_columns, (c) => c.Ordinal == col.Ordinal) != null)
          code_with_flags |= _primary_key_flag;

        if (col.AutoIncrement)
          code_with_flags |= _identity_flag;

        if (!(col.DefaultValue is DBNull))
          code_with_flags |= _default_flag;

        if (col.ReadOnly)
          code_with_flags |= _readonly_flag;

        if (!Equals(col.ColumnName, col.Caption) && !string.IsNullOrEmpty(col.Caption))
          code_with_flags |= _caption_flag;

        if (col.MaxLength > 0)
          code_with_flags |= _max_length_flag;

        runners[col.Ordinal] = Construct(type_code);

        writer.Write(col.ColumnName);
        writer.Write(code_with_flags);

        if (col.AutoIncrement)
        {
          writer.Write(col.AutoIncrementSeed);
          writer.Write(col.AutoIncrementStep);
        }

        if (col.MaxLength > 0)
          writer.Write(col.MaxLength);

        if (!(col.DefaultValue is DBNull))
          runners[col.Ordinal].Write(writer, col.DefaultValue);

        if (!Equals(col.ColumnName, col.Caption) && !string.IsNullOrEmpty(col.Caption))
          writer.Write(col.Caption);
      }

      WriteConstraints(table, writer, key_columns);

      foreach (DataRow row in table.Rows)
      {
        for (int i = 0; i < table.Columns.Count; i++)
        {
          if (table.Columns[i].AllowDBNull)
            writer.Write(!row.IsNull(i));
          if (!row.IsNull(i))
            runners[i].Write(writer, row[i]);
        }
      }

      writer.Flush();
    }

    /// <summary>
    /// Чтение таблицы данных из потока
    /// </summary>
    /// <param name="stream">Поток байт, из которого требуется прочитать таблицу</param>
    /// <returns>Таблица, прочитанная из потока байт</returns>
    public static DataTable ReadDataTable(Stream stream)
    {
      DataTable ret = new DataTable();
      ret.BeginLoadData();

      var reader = new CompactBinaryReader(stream, Encoding.UTF8);

      int columns_count = reader.ReadInt32();
      int rows_count = reader.ReadInt32();

      var runners = new IDataRunner[columns_count];
      var mandatories = new BitArray(columns_count, false);
      var primary_key = new List<int>();

      for (int i = 0; i < columns_count; i++)
      {
        var col_name = reader.ReadString();
        var col_type = reader.ReadInt32();

        bool identity = false;
        bool has_default = false;
        bool read_only = false;
        bool has_caption = false;
        bool max_length = false;

        if ((col_type & _mandatory_flag) != 0)
        {
          col_type &= ~_mandatory_flag;
          mandatories[i] = true;
        }

        if ((col_type & _primary_key_flag) != 0)
        {
          col_type &= ~_primary_key_flag;
          primary_key.Add(i);
        }

        if ((col_type & _identity_flag) != 0)
        {
          col_type &= ~_identity_flag;
          identity = true;
        }

        if ((col_type & _default_flag) != 0)
        {
          col_type &= ~_default_flag;
          has_default = true;
        }

        if ((col_type & _readonly_flag) != 0)
        {
          col_type &= ~_readonly_flag;
          read_only = true;
        }

        if ((col_type & _caption_flag) != 0)
        {
          col_type &= ~_caption_flag;
          has_caption = true;
        }

        if ((col_type & _max_length_flag) != 0)
        {
          col_type &= ~_max_length_flag;
          max_length = true;
        }

        runners[i] = Construct(col_type);

        var column = ret.Columns.Add(col_name, ToType(col_type));
        column.AllowDBNull = !mandatories[i];
        column.ReadOnly = read_only;

        if (identity)
        {
          column.AutoIncrement = true;
          column.AutoIncrementSeed = reader.ReadInt64();
          column.AutoIncrementStep = reader.ReadInt64();
        }

        if (max_length)
          column.MaxLength = reader.ReadInt32();

        if (has_default)
          column.DefaultValue = runners[i].Read(reader);

        if (has_caption)
          column.Caption = reader.ReadString();
      }

      ReadConstraints(ret, reader, primary_key);

      for (int i = 0; i < rows_count; i++)
      {
        var row = ret.NewRow();

        for (int j = 0; j < columns_count; j++)
        {
          if (mandatories[j] || reader.ReadBoolean())
            row[j] = runners[j].Read(reader);
        }

        ret.Rows.Add(row);
      }

      ret.EndLoadData();
      ret.AcceptChanges();

      return ret;
    }

    #region Implementation methods ----------------------------------------------------------------

    private static readonly int _mandatory_flag;
    private static readonly int _primary_key_flag;
    private static readonly int _identity_flag;
    private static readonly int _default_flag;
    private static readonly int _readonly_flag;
    private static readonly int _caption_flag;
    private static readonly int _max_length_flag;

    private static readonly int _time_span_code;
    private static readonly int _guid_code;
    private static readonly int _binary_code;

    static DataTableSaver()
    {
      TypeCode max = TypeCode.String;

      foreach (TypeCode code in Enum.GetValues(typeof(TypeCode)))
      {
        if (max < code)
          max = code;
      }

      _time_span_code = (int)max + 1;
      _guid_code = _time_span_code + 1;
      _binary_code = _guid_code + 1;

      _mandatory_flag = 1;

      while (_mandatory_flag < _binary_code)
        _mandatory_flag <<= 1;

      _primary_key_flag = _mandatory_flag << 1;
      _identity_flag = _primary_key_flag << 1;
      _default_flag = _identity_flag << 1;
      _readonly_flag = _default_flag << 1;
      _caption_flag = _readonly_flag << 1;
      _max_length_flag = _caption_flag << 1;
    }

    private static void WriteConstraints(DataTable table, BinaryWriter writer, DataColumn[] keyColumns)
    {
      var pk_name = string.Empty;
      var uc_list = new List<UniqueConstraint>(table.Constraints.Count);

      foreach (Constraint c in table.Constraints)
      {
        var uc = c as UniqueConstraint;

        if (uc == null)
          continue;

        if (uc.IsPrimaryKey)
          pk_name = uc.ConstraintName;
        else
          uc_list.Add(uc);
      }

      if (keyColumns.Length > 0)
        writer.Write(pk_name);

      writer.Write(uc_list.Count);

      foreach (var uc in uc_list)
      {
        writer.Write(uc.Columns.Length);

        foreach (DataColumn col in uc.Columns)
          writer.Write(col.Ordinal);

        writer.Write(uc.ConstraintName);
      }
    }

    private static void ReadConstraints(DataTable table, BinaryReader reader, List<int> keyColumns)
    {
      if (keyColumns.Count != 0)
      {
        var key_columns = new DataColumn[keyColumns.Count];

        for (int i = 0; i < keyColumns.Count; i++)
          key_columns[i] = table.Columns[keyColumns[i]];

        table.Constraints.Add(new UniqueConstraint(reader.ReadString(), key_columns, true));
      }

      var other_unique_count = reader.ReadInt32();

      for (int i = 0; i < other_unique_count; i++)
      {
        var constraint_columns = new DataColumn[reader.ReadInt32()];

        for (int k = 0; k < constraint_columns.Length; k++)
          constraint_columns[k] = table.Columns[reader.ReadInt32()];

        table.Constraints.Add(new UniqueConstraint(reader.ReadString(), constraint_columns, false));
      }
    }

    private static int GetTypeCode(Type type)
    {
      if (type == typeof(TimeSpan))
        return _time_span_code;
      if (type == typeof(Guid))
        return _guid_code;
      if (type == typeof(byte[]))
        return _binary_code;

      return (int)Type.GetTypeCode(type);
    }

    private static Type ToType(int typeNumber)
    {
      if (typeNumber == _time_span_code)
        return typeof(TimeSpan);
      if (typeNumber == _guid_code)
        return typeof(Guid);
      if (typeNumber == _binary_code)
        return typeof(byte[]);

      return Type.GetType("System." + (TypeCode)typeNumber);
    }

    private static IDataRunner Construct(int typeNumber)
    {
      if (typeNumber == _time_span_code)
        return new TimeSpanDataRunner();
      else if (typeNumber == _guid_code)
        return new GuidDataRunner();
      else if (typeNumber == _binary_code)
        return new BinaryDataRunner();

      switch ((TypeCode)typeNumber)
      {
        case TypeCode.Boolean: return new BooleanDataRunner();
        case TypeCode.Char: return new CharDataRunner();
        case TypeCode.SByte: return new SByteDataRunner();
        case TypeCode.Byte: return new ByteDataRunner();
        case TypeCode.Int16: return new Int16DataRunner();
        case TypeCode.UInt16: return new UInt16DataRunner();
        case TypeCode.Int32: return new Int32DataRunner();
        case TypeCode.UInt32: return new UInt32DataRunner();
        case TypeCode.Int64: return new Int64DataRunner();
        case TypeCode.UInt64: return new UInt64DataRunner();
        case TypeCode.Single: return new SingleDataRunner();
        case TypeCode.Double: return new DoubleDataRunner();
        case TypeCode.Decimal: return new DecimalDataRunner();
        case TypeCode.DateTime: return new DateTimeDataRunner();
        case TypeCode.String: return new StringDataRunner();

        default:
          throw new ArgumentException(Resources.INVALID_COLUMN_TYPE);
      }
    }

    #endregion

    #region Implementation types ------------------------------------------------------------------

    private interface IDataRunner
    {
      void Write(BinaryWriter writer, object value);

      object Read(BinaryReader reader);
    }

    private class BooleanDataRunner : IDataRunner
    {
      public void Write(BinaryWriter writer, object value)
      {
        writer.Write((bool)value);
      }

      public object Read(BinaryReader reader)
      {
        return reader.ReadBoolean();
      }
    }

    private class CharDataRunner : IDataRunner
    {
      public void Write(BinaryWriter writer, object value)
      {
        writer.Write((char)value);
      }

      public object Read(BinaryReader reader)
      {
        return reader.ReadChar();
      }
    }

    private class SByteDataRunner : IDataRunner
    {
      public void Write(BinaryWriter writer, object value)
      {
        writer.Write((sbyte)value);
      }

      public object Read(BinaryReader reader)
      {
        return reader.ReadSByte();
      }
    }

    private class ByteDataRunner : IDataRunner
    {
      public void Write(BinaryWriter writer, object value)
      {
        writer.Write((byte)value);
      }

      public object Read(BinaryReader reader)
      {
        return reader.ReadByte();
      }
    }

    private class Int16DataRunner : IDataRunner
    {
      public void Write(BinaryWriter writer, object value)
      {
        writer.Write((short)value);
      }

      public object Read(BinaryReader reader)
      {
        return reader.ReadInt16();
      }
    }

    private class UInt16DataRunner : IDataRunner
    {
      public void Write(BinaryWriter writer, object value)
      {
        writer.Write((ushort)value);
      }

      public object Read(BinaryReader reader)
      {
        return reader.ReadUInt16();
      }
    }

    private class Int32DataRunner : IDataRunner
    {
      public void Write(BinaryWriter writer, object value)
      {
        writer.Write((int)value);
      }

      public object Read(BinaryReader reader)
      {
        return reader.ReadInt32();
      }
    }

    private class UInt32DataRunner : IDataRunner
    {
      public void Write(BinaryWriter writer, object value)
      {
        writer.Write((uint)value);
      }

      public object Read(BinaryReader reader)
      {
        return reader.ReadUInt32();
      }
    }

    private class Int64DataRunner : IDataRunner
    {
      public void Write(BinaryWriter writer, object value)
      {
        writer.Write((long)value);
      }

      public object Read(BinaryReader reader)
      {
        return reader.ReadInt64();
      }
    }

    private class UInt64DataRunner : IDataRunner
    {
      public void Write(BinaryWriter writer, object value)
      {
        writer.Write((ulong)value);
      }

      public object Read(BinaryReader reader)
      {
        return reader.ReadUInt64();
      }
    }

    private class SingleDataRunner : IDataRunner
    {
      public void Write(BinaryWriter writer, object value)
      {
        writer.Write((float)value);
      }

      public object Read(BinaryReader reader)
      {
        return reader.ReadSingle();
      }
    }

    private class DoubleDataRunner : IDataRunner
    {
      public void Write(BinaryWriter writer, object value)
      {
        writer.Write((double)value);
      }

      public object Read(BinaryReader reader)
      {
        return reader.ReadDouble();
      }
    }

    private class DecimalDataRunner : IDataRunner
    {
      public void Write(BinaryWriter writer, object value)
      {
        writer.Write((decimal)value);
      }

      public object Read(BinaryReader reader)
      {
        return reader.ReadDecimal();
      }
    }

    private class DateTimeDataRunner : IDataRunner
    {
      public void Write(BinaryWriter writer, object value)
      {
        writer.Write(((DateTime)value).ToBinary());
      }

      public object Read(BinaryReader reader)
      {
        return DateTime.FromBinary(reader.ReadInt64());
      }
    }

    private class TimeSpanDataRunner : IDataRunner
    {
      public void Write(BinaryWriter writer, object value)
      {
        writer.Write(((TimeSpan)value).Ticks);
      }

      public object Read(BinaryReader reader)
      {
        return TimeSpan.FromTicks(reader.ReadInt64());
      }
    }

    private class GuidDataRunner : IDataRunner
    {
      public void Write(BinaryWriter writer, object value)
      {
        writer.Write(((Guid)value).ToByteArray());
      }

      public object Read(BinaryReader reader)
      {
        return new Guid(reader.ReadBytes(16));
      }
    }

    private class BinaryDataRunner : IDataRunner
    {
      public void Write(BinaryWriter writer, object value)
      {
        writer.Write(((byte[])value).Length);
        writer.Write((byte[])value);
      }

      public object Read(BinaryReader reader)
      {
        return reader.ReadBytes(reader.ReadInt32());
      }
    }

    private class StringDataRunner : IDataRunner
    {
      private Dictionary<object, int> m_values;
      private List<string> m_strings;
      private object last;

      private const byte FULL_TEXT = 0;
      private const byte STRING_NUMBER = 1;
      private const byte REPEAT = 2;

      public void Write(BinaryWriter writer, object value)
      {
        if (!Equals(value, last))
        {
          if (m_values == null)
            m_values = new Dictionary<object, int>();

          int key;

          if (m_values.TryGetValue(value, out key))
          {
            writer.Write(STRING_NUMBER);
            writer.Write(key);
          }
          else
          {
            writer.Write(FULL_TEXT);
            writer.Write((string)value);
            m_values[value] = m_values.Count;
          }
        }
        else
          writer.Write(REPEAT);

        last = value;
      }

      public object Read(BinaryReader reader)
      {
        if (m_strings == null)
          m_strings = new List<string>();

        var status = reader.ReadByte();

        if (status == FULL_TEXT)
        {
          var ret = reader.ReadString();
          m_strings.Add(ret);
          last = ret;
          return ret;
        }
        else if (status == STRING_NUMBER)
        {
          var ret = m_strings[reader.ReadInt32()];
          last = ret;
          return ret;
        }
        else
          return last;
      }
    }

    sealed class CompactBinaryWriter : BinaryWriter
    {
      public CompactBinaryWriter(Stream stream, Encoding encoding) : base(stream, encoding) { }

      public override void Write(int value)
      {
        base.Write7BitEncodedInt(value);
      }

      public override void Write(uint value)
      {
        base.Write7BitEncodedInt((int)value);
      }

      protected override void Dispose(bool disposing) { }
    }

    sealed class CompactBinaryReader : BinaryReader
    {
      public CompactBinaryReader(Stream stream, Encoding encoding) : base(stream, encoding) { }

      public override int ReadInt32()
      {
        return base.Read7BitEncodedInt();
      }

      public override uint ReadUInt32()
      {
        return (uint)base.Read7BitEncodedInt();
      }

      protected override void Dispose(bool disposing) { }
    }

    #endregion
  }
}