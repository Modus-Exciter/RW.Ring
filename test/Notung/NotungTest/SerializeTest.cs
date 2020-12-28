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
