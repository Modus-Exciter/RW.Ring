using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notung.Loader;
using Notung.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NotungTest
{
  [TestClass]
  public class RpcTest
  {
    [TestMethod]
    public void RegexForIdentifier()
    {
      Regex check = new Regex("^[a-zA-Z_]+[a-zA-Z_1-9]*$", RegexOptions.Compiled);

      Assert.IsTrue(check.IsMatch("a"));
      Assert.IsTrue(check.IsMatch("ab"));
      Assert.IsTrue(check.IsMatch("AB"));
      Assert.IsFalse(check.IsMatch("aб"));
      Assert.IsFalse(check.IsMatch(""));
      Assert.IsTrue(check.IsMatch("_"));
      Assert.IsTrue(check.IsMatch("_a"));
      Assert.IsTrue(check.IsMatch("_Z"));
      Assert.IsTrue(check.IsMatch("_1"));
      Assert.IsFalse(check.IsMatch("2_"));
      Assert.IsFalse(check.IsMatch("AGG_<>"));
      Assert.IsTrue(check.IsMatch("a_2_b"));
      Assert.IsTrue(check.IsMatch("A2B_"));
      Assert.IsTrue(check.IsMatch("_DB23_34"));
    }

    [TestMethod]
    public void RemotableCommandCast()
    {
      using (var ms = new MemoryStream())
      {
        var bf = new BinaryFormatter();

        bf.Serialize(ms, new Test { Text = "Roma" });

        ms.Position = 0;

        ITest<object> value = (ITest<object>)bf.Deserialize(ms);

        Assert.AreEqual("Roma", value.Get());
      }
    }

    [TestMethod]
    public void MethodsInfo()
    {
      RpcServiceInfo.Register<ITestContract1>();

      var contract = RpcServiceInfo.GetByName("TEST_C");

      Assert.IsNotNull(contract);
      Assert.IsTrue(contract.HasMethod("DO_ONE"));
      Assert.IsFalse(contract.HasMethod("DoOne"));
    }

    [TestMethod]
    public void MethodsInfoContract2()
    {
      RpcServiceInfo.Register<ITestContract1>();

      var contract = RpcServiceInfo.Register(typeof(ITestContract2));

      Assert.IsNotNull(contract);
      Assert.IsTrue(contract.HasMethod("DO_ONE"));
      Assert.IsFalse(contract.HasMethod("DoOne"));
      Assert.AreEqual("TEST_C2", contract.ServiceName);
      Assert.AreEqual(contract, RpcServiceInfo.GetByName("TEST_C2"));

      Assert.AreEqual("DO_ONE", contract.GetMethodName(typeof(ITestContract2).GetMethod("DoOne")));
      Assert.AreEqual(typeof(int), contract.GetOperationInfo("DO_ONE").ResultType);
      Assert.IsTrue(contract.HasMethod("Swap"));
      Assert.AreEqual(2, contract.GetOperationInfo("Swap").ResultType.GetGenericArguments().Length);
      Assert.AreEqual(typeof(int), contract.GetOperationInfo("Swap").ResultType.GetGenericArguments()[0]);
      Assert.AreEqual(typeof(uint), contract.GetOperationInfo("Swap").ResultType.GetGenericArguments()[1]);
      Assert.AreEqual("ReturnWithOut`2", contract.GetOperationInfo("CALCULATE").ResultType.GetGenericTypeDefinition().Name);
      Assert.AreEqual(typeof(double), contract.GetOperationInfo("CALCULATE").ResultType.GetGenericArguments()[0]);
      Assert.AreEqual(3, contract.GetOperationInfo("CALCULATE").ResultType.GetGenericArguments()[1].GetGenericArguments().Length);
      Assert.AreEqual(typeof(string), contract.GetOperationInfo("CALCULATE").ResultType.GetGenericArguments()[1].GetGenericArguments()[0]);
      Assert.AreEqual(typeof(int), contract.GetOperationInfo("CALCULATE").ResultType.GetGenericArguments()[1].GetGenericArguments()[1]);
      Assert.AreEqual(typeof(object), contract.GetOperationInfo("CALCULATE").ResultType.GetGenericArguments()[1].GetGenericArguments()[2]);
    }

    [TestMethod]
    [ExpectedException(typeof(TypeInitializationException))]
    public void WrongType()
    {
      RpcServiceInfo.Register<DBNull>();
    }

    [TestMethod]
    [ExpectedException(typeof(TypeInitializationException))]
    public void WrongSymbol()
    {
      RpcServiceInfo.Register<IWrongSymbol>();
    }

    [TestMethod]
    [ExpectedException(typeof(TypeInitializationException))]
    public void ConflictMethods()
    {
      RpcServiceInfo.Register<ITestContractConflict>();
    }

    [TestMethod]
    public void CallFullComplect()
    {
      var caller = new ServerCaller(Factory.Wrapper<object>(new Contract2()));
      RpcServiceInfo.Register<ITestContract2>();

      var res = (IParametersList)caller.Call("TEST_C2/Swap".Split('/'), ParametersList.Create(
        typeof(ITestContract2).GetMethod("Swap"), 13, (uint)21)).Value;

      Assert.AreEqual(2, res.GetTypes().Length);
      Assert.AreEqual(21, res.GetValues()[0]);
      Assert.AreEqual((uint)13, res.GetValues()[1]);

      var res2 = (IRefReturnResult)caller.Call("TEST_C2/CALCULATE".Split('/'), ParametersList.Create(
        typeof(ITestContract2).GetMethod("Calc"), "24.1", 0, null)).Value;

      Assert.AreEqual(24.1, res2.Return);
      Assert.IsNull(res2.References.GetValues()[0]);
      Assert.AreEqual(24, res2.References.GetValues()[1]);
      Assert.AreEqual("A", res2.References.GetValues()[2]);

      var res3 = caller.Call("TEST_C2/DO_ONE".Split('/'), ParametersList.Create(
        typeof(ITestContract2).GetMethod("DoOne"), "Ric", TypeCode.Byte)).Value;

      Assert.AreEqual("Ric, Byte".Length, res3);

      Assert.AreEqual(RpcServiceInfo.GetByName("TEST_C2").GetOperationInfo("Swap").ResultType, res.GetType());
      Assert.AreEqual(RpcServiceInfo.GetByName("TEST_C2").GetOperationInfo("CALCULATE").ResultType, res2.GetType());
      Assert.AreEqual(RpcServiceInfo.GetByName("TEST_C2").GetOperationInfo("DO_ONE").ResultType, res3.GetType());
    }

    private class ClientCaller : IClientCaller
    {
      private readonly ServerCaller m_caller;

      public ClientCaller(ServerCaller caller)
      {
        m_caller = caller;
      }

      public ICallResult Call(string serverOperation, IParametersList request, RpcOperationInfo operation)
      {
        return m_caller.Call(serverOperation.Split('/'), request);
      }

      public void StreamExchange(string serverOperation, Action<System.IO.Stream> processRequest, Action<System.IO.Stream> processResponse)
      {
        throw new NotImplementedException();
      }

      public byte[] BinaryExchange(string serverOperation, byte[] data)
      {
        throw new NotImplementedException();
      }
    }

    [TestMethod]
    public void CallFullComplectWithProxy()
    {
      var contract = new Contract2();
      var caller = new ServerCaller(Factory.Wrapper<object>(contract));
      var proxy = new NetworkProxy<ITestContract2>(new ClientCaller(caller)).GetTransparentProxy();

      int a = 13;
      uint b = 24;

      proxy.Swap(ref a, ref b);

      Assert.AreEqual(24, a);
      Assert.AreEqual((uint)13, b);

      Assert.AreEqual("Ric, Byte".Length, proxy.DoOne("Ric", TypeCode.Byte));
      object value2;

      double ret = proxy.Calc("24.1", ref a, out value2);

      Assert.AreEqual(24.1, ret);
      Assert.AreEqual(24, a);
      Assert.AreEqual("A", value2);

      ret = proxy.Calc("asa", ref a, out value2);

      Assert.AreEqual(0.0, ret);
      Assert.AreEqual(0, a);
      Assert.AreEqual(Convert.DBNull, value2);

      proxy.JustSend("Fortune");

      Assert.AreEqual("Fortune", contract.Value);
    }
  }

  static class RpcExtensions
  {
    public static ICallResult Call(this ServerCaller caller, string[] bits, IParametersList request)
    {
      if (bits.Length != 2)
        throw new ArgumentException();

      var operation = RpcServiceInfo.GetByName(bits[0]).GetOperationInfo(bits[1]);

      return caller.Call(operation, request);
    }
  }

  [RpcService("TEST_C")]
  public interface ITestContract1
  {
    [RpcOperation("DO_ONE")]
    int DoOne(string name, object value);
  }

  [RpcService("TEST_C2")]
  public interface ITestContract2
  {
    [RpcOperation("DO_ONE")]
    int DoOne(string name, object value);

    [RpcOperation]
    void Swap(ref int a, ref uint b);

    [RpcOperation("CALCULATE")]
    double Calc(string text, ref int value, out object value2);

    [RpcOperation("JUST")]
    void JustSend(string text);
  }

  class Contract2 : ITestContract2
  {
    public int DoOne(string name, object value)
    {
      return string.Format("{0}, {1}", name, value).Length;
    }

    public void Swap(ref int a, ref uint b)
    {
      int tmp = a;
      a = (int)b;
      b = (uint)tmp;
    }

    public double Calc(string text, ref int value, out object value2)
    {
      double result;

      if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
        value2 = "A";
      else
        value2 = Convert.DBNull;

      value = (int)result;

      return result;
    }

    public string Value;

    public void JustSend(string text)
    {
      this.Value = text;
    }
  }


  [RpcService("TEST_C3")]
  public interface ITestContractConflict
  {
    [RpcOperation("DO_ONE")]
    int DoOne(string name, object value);
    [RpcOperation]
    void DO_ONE(out int a, out uint b);
  }

  [RpcService("Test/Symbol")]
  public interface IWrongSymbol { }

  public interface ITest<out T> where T : class
  {
    T Get();
  }

  [Serializable]
  public class Test : ITest<string>
  {
    public string Text;
    
    public string Get()
    {
      return Text;
    }
  }
}
