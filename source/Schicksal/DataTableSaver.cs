using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Globalization;

namespace Schicksal
{
  public class DataTableSaver
  {
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
      public void Write(BinaryWriter writer, object value)
      {
        writer.Write((String)value);
      }

      public object Read(BinaryReader reader)
      {
        return reader.ReadString();
      }
    }

    private static IDataRunner Construct(Type columnType)
    {
      switch (Type.GetTypeCode(columnType))
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
        default: throw new ArgumentException();
      }
    }

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

        foreach (DataColumn col in table.Columns)
        {
          writer.Write(col.ColumnName);
          writer.Write((int)Type.GetTypeCode(col.DataType));
          runners[col.Ordinal] = Construct(col.DataType);
        }

        foreach (DataRow row in table.Rows)
        {
          for (int i = 0; i < table.Columns.Count; i++)
          {
            if (row.IsNull(i))
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

    public static DataTable ReadDataTable(Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      DataTable ret = new DataTable();

      using (var reader = new BinaryReader(stream, Encoding.UTF8))
      {
        int columns_count = reader.ReadInt32();
        int rows_count = reader.ReadInt32();

        var runners = new IDataRunner[columns_count];
        
        for (int i = 0; i < columns_count; i++)
        {
          var col_name = reader.ReadString();
          var col_type = Type.GetType("System." + (TypeCode)reader.ReadInt32());
          ret.Columns.Add(col_name, col_type);
          runners[i] = Construct(col_type);
        }

        for (int i = 0; i < rows_count; i++)
        {
          var row = ret.NewRow();

          for (int j = 0; j < columns_count; j++)
          {
            if (reader.ReadBoolean())
              row[j] = runners[j].Read(reader);
          }

          ret.Rows.Add(row);
        }
      }

      return ret;
    }
  }
}
