using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using Schicksal.Properties;
using System.Collections;
using System.Collections.Generic;

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

      if (stream == null)
        throw new ArgumentNullException("stream");

      using (var writer = new BinaryWriter(stream, Encoding.UTF8))
      {
        writer.Write(table.Columns.Count);
        writer.Write(table.Rows.Count);

        var runners = new IDataRunner[table.Columns.Count];
        var mandatories = new BitArray(table.Columns.Count, false);
        var key_columns = table.PrimaryKey;

        foreach (DataColumn col in table.Columns)
        {
          var type_code = Type.GetTypeCode(col.DataType);
          var code_with_flags = (int)type_code;

          if (!col.AllowDBNull)
          {
            mandatories[col.Ordinal] = true;
            code_with_flags |= _mandatory_flag;
          }

          if (Array.Find(key_columns, (c) => c.Ordinal == col.Ordinal) != null)
            code_with_flags |= _primary_key_flag;
          
          runners[col.Ordinal] = Construct(type_code);

          writer.Write(col.ColumnName);
          writer.Write(code_with_flags);
        }

        if (key_columns.Length > 0)
        {
          string pk_name = string.Empty;
          var ucs = new List<UniqueConstraint>(table.Constraints.Count);

          foreach (Constraint c in table.Constraints)
          {
            var uc = c as UniqueConstraint;

            if (uc == null)
              continue;

            if (uc.IsPrimaryKey)
              pk_name = uc.ConstraintName;
            else
              ucs.Add(uc);
          }

          writer.Write(pk_name);
          writer.Write(ucs.Count);

          foreach (var c in ucs)
          {
            writer.Write(c.Columns.Length);

            foreach (DataColumn col in c.Columns)
              writer.Write(col.Ordinal);

            writer.Write(c.ConstraintName);
          }
        }

        foreach (DataRow row in table.Rows)
        {
          for (int i = 0; i < table.Columns.Count; i++)
          {
            if (mandatories[i])
              runners[i].Write(writer, row[i]);
            else if (row.IsNull(i))
              writer.Write(false);
            else
            {
              writer.Write(true);
              runners[i].Write(writer, row[i]);
            }
          }
        }

        writer.Flush();
      }
    }

    /// <summary>
    /// Чтение таблицы данных из потока
    /// </summary>
    /// <param name="stream">Поток байт, из которого требуется прочитать таблицу</param>
    /// <returns>Таблица, прочитанная из потока байт</returns>
    public static DataTable ReadDataTable(Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      DataTable ret = new DataTable();

      ret.BeginLoadData();

      using (var reader = new BinaryReader(stream, Encoding.UTF8))
      {
        int columns_count = reader.ReadInt32();
        int rows_count = reader.ReadInt32();

        var runners = new IDataRunner[columns_count];
        var mandatories = new BitArray(columns_count, false);
        var primary_key = new List<int>();
        
        for (int i = 0; i < columns_count; i++)
        {
          var col_name = reader.ReadString();
          var col_type = reader.ReadInt32();

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

          ret.Columns.Add(col_name, 
            Type.GetType("System." + (TypeCode)col_type)).AllowDBNull = !mandatories[i];

          runners[i] = Construct((TypeCode)col_type);
        }

        if (primary_key.Count != 0)
        {
          var key_columns = new DataColumn[primary_key.Count];

          for (int i = 0; i < primary_key.Count; i++)
            key_columns[i] = ret.Columns[primary_key[i]];

          ret.Constraints.Add(new UniqueConstraint(reader.ReadString(), key_columns, true));

          var other_unique_count = reader.ReadInt32();

          for (int i = 0; i < other_unique_count; i++)
          {
            var constraint_columns = new DataColumn[reader.ReadInt32()];

            for (int k = 0; k < constraint_columns.Length; k++)
              constraint_columns[k] = ret.Columns[reader.ReadInt32()];

            ret.Constraints.Add(new UniqueConstraint(reader.ReadString(), constraint_columns, false));
          }
        }

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
      }

      ret.EndLoadData();
      ret.AcceptChanges();

      return ret;
    }

    #region Implementation

    private static readonly int _mandatory_flag;
    private static readonly int _primary_key_flag;

    static DataTableSaver()
    {
      TypeCode max = TypeCode.String;

      foreach (TypeCode code in Enum.GetValues(typeof(TypeCode)))
      {
        if (max < code)
          max = code;
      }

      _mandatory_flag = 1;

      while (_mandatory_flag < (int)max)
        _mandatory_flag <<= 1;

      _primary_key_flag = _mandatory_flag << 1;
    }

    private interface IDataRunner
    {
      void Write(BinaryWriter writer, object value);

      object Read(BinaryReader reader);
    }

    private class BooleanDataRunner : IDataRunner
    {
      public void Write(BinaryWriter writer, object value)
      {
        writer.Write((Boolean)value);
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
        writer.Write((Char)value);
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
        writer.Write((SByte)value);
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
        writer.Write((Byte)value);
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
        writer.Write((Int16)value);
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
        writer.Write((UInt16)value);
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
        writer.Write((Int32)value);
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
        writer.Write((UInt32)value);
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
        writer.Write((Int64)value);
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
        writer.Write((UInt64)value);
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
        writer.Write((Single)value);
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
        writer.Write((Double)value);
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
        writer.Write((Decimal)value);
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
        writer.Write(((DateTime)value).ToString(CultureInfo.InvariantCulture));
      }

      public object Read(BinaryReader reader)
      {
        return DateTime.Parse(reader.ReadString(), CultureInfo.InvariantCulture);
      }
    }

    private class StringDataRunner : IDataRunner
    {
      private Dictionary<object, int> m_values;
      private List<string> m_strings;
      private object last;
      
      public void Write(BinaryWriter writer, object value)
      {
        if (!Equals(value, last))
        {
          if (m_values == null)
            m_values = new Dictionary<object, int>();

          int key;

          if (m_values.TryGetValue(value, out key))
          {
            writer.Write((byte)1);
            writer.Write(key);
          }
          else
          {
            writer.Write((byte)0);
            writer.Write((string)value);
            m_values[value] = m_values.Count;
          }
        }
        else
          writer.Write((byte)2);

        last = value;
      }

      public object Read(BinaryReader reader)
      {
        if (m_strings == null)
          m_strings = new List<string>();

        var status = reader.ReadByte();

        if (status == 0)
        {
          var ret = reader.ReadString();
          m_strings.Add(ret);
          last = ret;
          return ret;
        }
        else if (status == 1)
          return m_strings[reader.ReadInt32()];
        else
          return last;
      }
    }

    private static IDataRunner Construct(TypeCode columnType)
    {
      switch (columnType)
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
        default: throw new ArgumentException(Resources.INVALID_COLUMN_TYPE);
      }
    }

    #endregion
  }
}