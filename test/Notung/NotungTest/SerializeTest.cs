using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notung;
using Notung.Data;
using Schicksal;
using System.Data;

namespace NotungTest
{
  [TestClass]
  public class SerializeTest
  {
    [TestMethod]
    public void SerializeSerializable()
    {
      byte[] serialized;
      using (var ms = new MemoryStream())
      {
        SerializeCondition<string> sc = new SerializeCondition<string>("Hello, World!");
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(ms, sc);

        serialized = ms.ToArray();
      }

      using (var ms = new MemoryStream(serialized))
      {
        ms.Position = 0;
        BinaryFormatter bf = new BinaryFormatter();
        SerializeCondition<string> sc = (SerializeCondition<string>)bf.Deserialize(ms);
        Assert.AreEqual("Hello, World!", sc.Value);
      }
    }

    private class Unserializable
    {
      public string Text { get; set; }

      public override string ToString()
      {
        return Text ?? base.ToString();
      }
    }

    [TestMethod]
    public void RepeatTextTest()
    {
      var table = new DataTable();

      table.Columns.Add("Name", typeof(string)).AllowDBNull = false;
      table.Columns.Add("Description", typeof(string)).AllowDBNull = true;

      table.Rows.Add(new[] { "First", "First row" });
      table.Rows.Add(new[] { "Second", "Second row" });
      table.Rows.Add(new[] { "First", "Second row" });
      table.Rows.Add(new[] { "Second", Convert.DBNull });

      table = ReplaceTable(table);

      Assert.AreEqual("Name", table.Columns[0].ColumnName);
      Assert.AreEqual("Description", table.Columns[1].ColumnName);
      Assert.AreEqual("First", table.Rows[0][0]);
      Assert.AreEqual("First row", table.Rows[0][1]);
      Assert.AreEqual("Second", table.Rows[1][0]);
      Assert.AreEqual("Second row", table.Rows[1][1]);
      Assert.AreEqual("First", table.Rows[2][0]);
      Assert.AreEqual("Second row", table.Rows[2][1]);
      Assert.AreEqual("Second", table.Rows[3][0]);
      Assert.AreEqual(Convert.DBNull, table.Rows[3][1]);
    }

    [TestMethod]
    public void SaveDateFromTable()
    {
      var table = new DataTable();
      var date = DateTime.Now;
      var today = DateTime.Today;

      table.Columns.Add("Date", typeof(DateTime));
      table.Rows.Add(new object[] { date });
      table.Rows.Add(new object[] { DateTime.MinValue });
      table.Rows.Add(new object[] { DateTime.MaxValue });
      table.Rows.Add(new object[] { today });

      table = ReplaceTable(table);

      Assert.AreEqual(DateTime.MinValue, table.Rows[1][0]);
      Assert.AreEqual(today, table.Rows[3][0]);
      Assert.AreEqual(date, table.Rows[0][0]);
      Assert.AreEqual(DateTime.MaxValue, table.Rows[2][0]);
    }

    [TestMethod]
    public void IntOrUintInTable()
    {
      var table = new DataTable();

      table.Columns.Add("Integer", typeof(int));
      table.Columns.Add("Uint", typeof(uint));

      table.Rows.Add(new object[] { int.MaxValue, uint.MaxValue });
      table.Rows.Add(new object[] { int.MinValue, (uint)(int.MaxValue) + 1 });
      table.Rows.Add(new object[] { -50376, 17000u });

      checked
      {
        table = ReplaceTable(table);
      }

      Assert.AreEqual(int.MaxValue, table.Rows[0][0]);
      Assert.AreEqual(uint.MaxValue, table.Rows[0][1]);
      Assert.AreEqual(int.MinValue, table.Rows[1][0]);
      Assert.AreEqual((uint)(int.MaxValue) + 1, table.Rows[1][1]);
      Assert.AreEqual(-50376, table.Rows[2][0]);
      Assert.AreEqual(17000u, table.Rows[2][1]);
    }

    [TestMethod]
    public void TimeSpanSerialize()
    {
      var table = new DataTable();
      var date = DateTime.Now - DateTime.Today;

      table.Columns.Add("Time", typeof(TimeSpan));
      table.Rows.Add(new object[] { date });
      table.Rows.Add(new object[] { TimeSpan.MinValue });
      table.Rows.Add(new object[] { TimeSpan.MaxValue });
      table.Rows.Add(new object[] { TimeSpan.Zero });

      table = ReplaceTable(table);
      Assert.AreEqual(date, table.Rows[0][0]);
      Assert.AreEqual(TimeSpan.MinValue, table.Rows[1][0]);
      Assert.AreEqual(TimeSpan.MaxValue, table.Rows[2][0]);
      Assert.AreEqual(TimeSpan.Zero, table.Rows[3][0]);
    }

    [TestMethod]
    public void GuidColumn()
    {
      var table = new DataTable();
      table.Columns.Add("ObjectGuid", typeof(Guid));
      table.Columns.Add("Name", typeof(string));
      var guid = Guid.NewGuid();

      table.Rows.Add(new object[] { guid, guid.ToString() });

      table = ReplaceTable(table);

      Assert.AreEqual(guid, table.Rows[0][0]);
      Assert.AreEqual(guid.ToString(), table.Rows[0][1]);
    }

    [TestMethod]
    public void BinaryColumn()
    {
      var table = new DataTable();
      table.Columns.Add("Arr", typeof(byte[]));
      table.Rows.Add(new object[] { new byte[] { 3, 2, 1, 7 } });
      table.Rows.Add(new object[] { new byte[] { 2, 3, 5, 5, 4 } });

      table = ReplaceTable(table);

      var res1 = (byte[])table.Rows[0][0];
      var res2 = (byte[])table.Rows[1][0];

      Assert.AreEqual(2, table.Rows.Count);
      Assert.AreEqual(1, table.Columns.Count);
      Assert.AreEqual(4, res1.Length);
      Assert.AreEqual(3, res1[0]);
      Assert.AreEqual(2, res1[1]);
      Assert.AreEqual(1, res1[2]);
      Assert.AreEqual(7, res1[3]);

      Assert.AreEqual(5, res2.Length);
      Assert.AreEqual(2, res2[0]);
      Assert.AreEqual(3, res2[1]);
      Assert.AreEqual(5, res2[2]);
      Assert.AreEqual(5, res2[3]);
      Assert.AreEqual(4, res2[4]);
    }

    [TestMethod]
    public void EmptyTable()
    {
      var table = new DataTable();

      byte[] bytes;
      using (var ms = new MemoryStream())
      {
        DataTableSaver.WriteDataTable(table, ms);
        bytes = ms.ToArray();
      }

      using (var ms = new MemoryStream(bytes))
      {
        table = DataTableSaver.ReadDataTable(ms);
      }

      Assert.AreEqual(0, table.Rows.Count);
      Assert.AreEqual(0, table.Columns.Count);
      Assert.AreEqual(0, table.Constraints.Count);
    }

    [TestMethod]
    public void IdentityTable()
    {
      var table = new DataTable();
      table.Columns.Add("ID", typeof(int));

      table.Columns[0].AllowDBNull = false;
      table.Columns[0].AutoIncrement = true;
      table.Columns[0].AutoIncrementSeed = 3;

      table.Columns.Add("Name", typeof(string));

      table.PrimaryKey = new [] { table.Columns[0]};

      table.Rows.Add(new object[] { null, "Nastya" });
      table.Rows.Add(new object[] { null, "Masha" });
      table.Rows.Add(new object[] { null, "Pasha" });

      table = ReplaceTable(table);

      table.Rows.Add(new object[] { null, "Loysha" });

      Assert.IsTrue(table.Columns[0].AutoIncrement);
      Assert.AreEqual(3, table.Rows[0][0]);
      Assert.AreEqual(4, table.Rows[1][0]);
      Assert.AreEqual(5, table.Rows[2][0]);
      Assert.AreEqual(6, table.Rows[3][0]);

      Assert.AreEqual("Nastya", table.Rows[0][1]);
      Assert.AreEqual("Masha", table.Rows[1][1]);
      Assert.AreEqual("Pasha", table.Rows[2][1]);
    }

    [TestMethod]
    public void SetUnique()
    {
      var table = new DataTable();
      table.Columns.Add("ID", typeof(int)).Unique = true;

      table = ReplaceTable(table);

      Assert.IsTrue(table.Columns[0].Unique);
    }

    [TestMethod]
    public void ColumnWithMaxLength()
    {
      var table = new DataTable();
      table.Columns.Add("ID", typeof(int));

      table.Columns[0].AllowDBNull = false;
      table.Columns[0].AutoIncrement = true;
      table.Columns[0].AutoIncrementSeed = 3;

      table.Columns.Add("Name", typeof(string));
      table.Columns[1].MaxLength = 64;
      table.Columns[1].DefaultValue = "KU";
      table.Columns[1].Caption = "Имя";

      table.PrimaryKey = new[] { table.Columns[0] };

      table.Rows.Add(new object[] { null, "Nastya" });
      table.Rows.Add(new object[] { null, "Masha" });
      table.Rows.Add(new object[] { null, "Pasha" });

      table = ReplaceTable(table);

      table.Rows.Add(new object[] { null, "Loysha" });

      Assert.IsTrue(table.Columns[0].AutoIncrement);
      Assert.AreEqual(3, table.Rows[0][0]);
      Assert.AreEqual(4, table.Rows[1][0]);
      Assert.AreEqual(5, table.Rows[2][0]);
      Assert.AreEqual(6, table.Rows[3][0]);

      Assert.AreEqual("Nastya", table.Rows[0][1]);
      Assert.AreEqual("Masha", table.Rows[1][1]);
      Assert.AreEqual("Pasha", table.Rows[2][1]);
      Assert.AreEqual(64, table.Columns[1].MaxLength);
      Assert.AreEqual("KU", table.Columns[1].DefaultValue);
      Assert.AreEqual("Имя", table.Columns[1].Caption);
    }

    [TestMethod]
    public void DefaultValueColumn()
    {
      var table = new DataTable();

      table.Columns.Add("Name", typeof(string)).AllowDBNull = false;
      table.Columns.Add("Description", typeof(string)).AllowDBNull = true;

      table.Columns[1].DefaultValue = "Laska";

      table.Rows.Add(new[] { "First", "First row" });
      table.Rows.Add(new[] { "Second", null });      
      
      table = ReplaceTable(table);

      Assert.AreEqual("First", table.Rows[0][0]);
      Assert.AreEqual("First row", table.Rows[0][1]);
      Assert.AreEqual("Second", table.Rows[1][0]);
      Assert.AreEqual("Laska", table.Rows[1][1]);

      Assert.AreEqual("Laska", table.Columns[1].DefaultValue);
    }

    [TestMethod]
    public void ReadOnlyColumn()
    {
      var table = new DataTable();

      table.Columns.Add("Name", typeof(string)).AllowDBNull = false;
      table.Columns.Add("Description", typeof(string)).AllowDBNull = true;

      table.Columns[0].ReadOnly = true;

      table.Rows.Add(new[] { "First", "First row" });
      table.Rows.Add(new[] { "Second", "Laska" });

      table = ReplaceTable(table);

      Assert.AreEqual("First", table.Rows[0][0]);
      Assert.AreEqual("First row", table.Rows[0][1]);
      Assert.AreEqual("Second", table.Rows[1][0]);
      Assert.AreEqual("Laska", table.Rows[1][1]);

      Assert.IsTrue(table.Columns[0].ReadOnly);
      Assert.IsFalse(table.Columns[1].ReadOnly);
    }

    [TestMethod]
    public void ColumnWithCaption()
    {
      var table = new DataTable();

      table.Columns.Add("Name", typeof(string)).AllowDBNull = false;
      table.Columns.Add("Description", typeof(string)).AllowDBNull = true;

      table.Columns[1].Caption = "description of element";

      table.Rows.Add(new[] { "First", "First row" });
      table.Rows.Add(new[] { "Second", "Laska" });

      table = ReplaceTable(table);

      Assert.AreEqual("First", table.Rows[0][0]);
      Assert.AreEqual("First row", table.Rows[0][1]);
      Assert.AreEqual("Second", table.Rows[1][0]);
      Assert.AreEqual("Laska", table.Rows[1][1]);

      Assert.AreEqual("Name", table.Columns[0].Caption);
      Assert.AreEqual("description of element", table.Columns[1].Caption);
    }

    [TestMethod]
    public void IsBetterThanStandard()
    {
      var table = new DataTable();

      table.RemotingFormat = SerializationFormat.Binary;

      table.Columns.Add("Name", typeof(string)).AllowDBNull = false;
      table.Columns.Add("Description", typeof(string)).AllowDBNull = true;

      var rnd =new Random();

      for (int i = 0; i < 2000; i++)
      {
        table.Rows.Add(new[] { Guid.NewGuid().ToString(), "descrpiption no." + rnd.Next(10) });
      }

      table.AcceptChanges();

      int size;

      using (var ms = new MemoryStream())
      {
        DataTableSaver.WriteDataTable(table, ms);
        size = ms.ToArray().Length;
      }

      using (var ms = new MemoryStream())
      {
        var bf = new BinaryFormatter();
        bf.Serialize(ms, table);

        var size_std = ms.ToArray().Length;
        Assert.IsTrue(size < size_std);
      }
    }

    private static DataTable ReplaceTable(DataTable table)
    {
      byte[] bytes;
      using (var ms = new MemoryStream())
      {
        DataTableSaver.WriteDataTable(table, ms);
        bytes = ms.ToArray();
      }

      using (var ms = new MemoryStream(bytes))
      {
        table = DataTableSaver.ReadDataTable(ms);
      }

      return table;
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void EmptyStreamSerizlize()
    {
      DataTableSaver.WriteDataTable(new DataTable(), null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void EmptyTableSerizlize()
    {
      DataTableSaver.WriteDataTable(null, new MemoryStream());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void EmptyStreamDeserizlize()
    {
      DataTableSaver.ReadDataTable(null);
    }

    [TestMethod]
    public void SerializeUnserializable()
    {
      byte[] serialized;
      using (var ms = new MemoryStream())
      {
        SerializeCondition<object> sc = new SerializeCondition<object>(new Unserializable { Text = "Hello, World!" });
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(ms, sc);

        serialized = ms.ToArray();
      }

      using (var ms = new MemoryStream(serialized))
      {
        ms.Position = 0;
        BinaryFormatter bf = new BinaryFormatter();
        SerializeCondition<object> sc = (SerializeCondition<object>)bf.Deserialize(ms);
        Assert.IsNull(sc.Value);
      }
    }

    [TestMethod]
    [ExpectedException(typeof(TypeInitializationException))]
    public void SerializeUnserializable2()
    {
      using (var ms = new MemoryStream())
      {
        SerializeCondition<Unserializable> sc = new SerializeCondition<Unserializable>(new Unserializable { Text = "Hello, World!" });
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(ms, sc);
      }
    }

    [TestMethod]
    public void SerializeInterface()
    {
      byte[] serialized;
      using (var ms = new MemoryStream())
      {
        SerializeCondition<IComparable> sc = new SerializeCondition<IComparable>("Hello, World!");
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(ms, sc);

        serialized = ms.ToArray();
      }

      using (var ms = new MemoryStream(serialized))
      {
        ms.Position = 0;
        BinaryFormatter bf = new BinaryFormatter();
        SerializeCondition<IComparable> sc = (SerializeCondition<IComparable>)bf.Deserialize(ms);
        Assert.AreEqual("Hello, World!", sc.Value);
      }
    }

    [TestMethod]
    public void StringValue()
    {
      SerializeCondition<string> sc = new SerializeCondition<string>("Burn, Valhalla!");
      SerializeCondition<string> sc2 = new SerializeCondition<string>();
      Assert.AreEqual("Burn, Valhalla!", sc.ToString());
      Assert.AreEqual("null", sc2.ToString());
    }

    [TestMethod]
    public void HashCode()
    {
      SerializeCondition<string> sc = new SerializeCondition<string>();
      sc.GetHashCode();
      sc.Value = "Siegfried is the greatest hero!";
      Assert.AreEqual("Siegfried is the greatest hero!".GetHashCode(), sc.GetHashCode());
    }

    [TestMethod]
    public void Equality()
    {
      SerializeCondition<string> sc = new SerializeCondition<string>("Mime");
      Assert.IsFalse(sc.Equals(null));
      Assert.IsFalse(sc.Equals("Mime"));
      Assert.IsFalse(sc.Equals(new SerializeCondition<string>()));
      Assert.IsTrue(sc.Equals(new SerializeCondition<string>("Mime")));
    }

    [TestMethod]
    public void SerializeInfo()
    {
      Info info = new Info("Valhalla", InfoLevel.Info) { Details = new Unserializable { Text = "will burn" } };
      byte[] data;
      using (var ms = new MemoryStream())
      {
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(ms, info);
        data = ms.ToArray();
      }

      Assert.IsTrue(info.Details is Unserializable);
      Assert.AreEqual("will burn", info.Details.ToString());

      using (var ms = new MemoryStream(data))
      {
        BinaryFormatter bf = new BinaryFormatter();
        info = (Info)bf.Deserialize(ms);
      }
      Assert.IsFalse(info.Details is Unserializable);
      Assert.AreEqual("will burn", info.Details.ToString());
    }

    [TestMethod]
    public void SerializeInfoWithString()
    {
      Info info = new Info("Valhalla", InfoLevel.Info) { Details = "will burn"};
      byte[] data;
      using (var ms = new MemoryStream())
      {
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(ms, info);
        data = ms.ToArray();
      }

      Assert.IsTrue(info.Details is string);
      Assert.AreEqual("will burn", info.Details);

      using (var ms = new MemoryStream(data))
      {
        BinaryFormatter bf = new BinaryFormatter();
        info = (Info)bf.Deserialize(ms);
      }

      Assert.IsTrue(info.Details is string);
      Assert.AreEqual("will burn", info.Details);
    }

    [TestMethod]
    public void DomainsTest()
    {
      AppDomain newDomain = AppDomain.CreateDomain("SOME DOMAIN",
        AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation);

      try
      {
        IExchange exc = (IExchange)newDomain.CreateInstanceAndUnwrap(
          typeof(DomainExchange).Assembly.FullName, typeof(Exchangeer).FullName);

        Assert.AreEqual("SOME DOMAIN", exc.Get().Value);
        Assert.AreEqual("SOME DOMAIN".Length, exc.Get().Number);
      }
      finally
      {
        AppDomain.Unload(newDomain);
      }
    }
  }

  [Serializable]
  public struct DomainExchange
  {
    private readonly SerializeCondition<object> m_field;
    public readonly int Number;

    public DomainExchange(object value) : this()
    {
      m_field.Value = value;

      if (value != null)
        Number = value.ToString().Length;
    }

    public object Value
    {
      get { return m_field.Value; }
    }
  }

  public interface IExchange
  {
    DomainExchange Get();
  }

  public class Exchangeer : MarshalByRefObject, IExchange
  {
    public DomainExchange Get()
    {
      return new DomainExchange(AppDomain.CurrentDomain.FriendlyName);
    }
  }

}
