using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notung.Net;

namespace NotungTest
{
  [TestClass]
  public class ParametersListTest
  {
    [TestMethod]
    public void ThreeParameters()
    {
      Func<string, float, DateTime, int> func = this.DoSomething;

      var parList = ParametersList.Create(func.Method, "ABBA", 325, DateTime.Now);

      Assert.AreEqual(3, parList.GetTypes().Length);
      Assert.AreEqual(typeof(string), parList.GetTypes()[0]);
      Assert.AreEqual(325f, parList.GetValues()[1]);
    }

    [TestMethod]
    public void FourParameters()
    {
      Func<string, float, DateTime, AddClass, int> func = this.DoSomething;

      var parList = ParametersList.Create(func.Method, "ABBA", 325, DateTime.Now, new AddClass());

      Assert.AreEqual(4, parList.GetTypes().Length);
      Assert.AreEqual(typeof(string), parList.GetTypes()[0]);
      Assert.AreEqual(typeof(AddClass), parList.GetTypes()[3]);
      Assert.AreEqual(325f, parList.GetValues()[1]);
    }

    [TestMethod]
    public void BinarySerialization()
    {
      Func<string, float, DateTime, AddClass, int> func = this.DoSomething;

      var parList = ParametersList.Create(func.Method, "ABBA", 325, DateTime.Now, new AddClass());

      using (var ms = new MemoryStream())
      {
        var bf = new BinaryFormatter();
        bf.Serialize(ms, parList);

        ms.Position = 0;

        parList = (IParametersList)bf.Deserialize(ms);
      }

      Assert.AreEqual(4, parList.GetTypes().Length);
      Assert.AreEqual(typeof(string), parList.GetTypes()[0]);
      Assert.AreEqual(typeof(AddClass), parList.GetTypes()[3]);
      Assert.AreEqual(325f, parList.GetValues()[1]);
    }

    [TestMethod]
    public void ContractSerialization()
    {
      Func<string, float, DateTime, AddClass, int> func = this.DoSomething;

      var parList = ParametersList.Create(func.Method, "ABBA", 325, DateTime.Now, new AddClass());
      IParametersList parList2 = null;

      using (var ms = new MemoryStream())
      {
        var serializer = new DataContractSerializer(ParametersList.GetRequiredType(func.Method));

        serializer.WriteObject(ms, parList);
        ms.Position = 0;

        var reader = new StreamReader(ms);
        {
          var line = reader.ReadToEnd();
          line.ToLower();
        }

        ms.Position = 0;
        serializer = new DataContractSerializer(ParametersList.GetRequiredType(func.Method));

        parList2 = (IParametersList)serializer.ReadObject(ms);
      }

      Assert.AreEqual(4, parList2.GetTypes().Length);
      Assert.AreEqual(typeof(string), parList2.GetTypes()[0]);
      Assert.AreEqual(typeof(AddClass), parList2.GetTypes()[3]);
      Assert.AreEqual(325f, parList2.GetValues()[1]);
    }

    [Serializable, DataContract(Namespace = "")]
    private class AddClass { }

    private int DoSomething(string name, float price, DateTime date)
    {
      return 0;
    }

    private int DoSomething(string name, float price, DateTime date, AddClass add)
    {
      return 0;
    }
  }

  public interface IAryx
  {
    int Sum(int a, int b);

    int DivMod(int a, int b, out int mod);
  }

  public class Aryx : IAryx
  {
    public int Sum(int a, int b)
    {
      return a + b;
    }

    public int DivMod(int a, int b, out int mod)
    {
      var res = a / b;

      mod = a - res * b;

      return res;
    }
  }
}