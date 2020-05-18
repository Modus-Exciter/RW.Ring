using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notung;
using Notung.Data;
using Notung.Loader;
using System.Collections.Generic;
using System.Reflection;
using System;
using Notung.Threading;
using System.Threading;

namespace NotungTest
{
  [TestClass]
  public class CommonTest
  {
    [TestMethod]
    public void ApartmentState()
    {
      Assert.AreEqual("hello, world!", AppManager.Instance.ApartmentWrapper.Invoke(() => "Hello, World!".ToLower()));
    }

    [TestMethod]
    public void PrefixTreeTest()
    {
      PrefixTree tree = new PrefixTree();
      tree.AddPrefix("DevExpress.");
      tree.AddPrefix("System");
      tree.AddPrefix("Syslogs");
      tree.AddPrefix("Document");

      Assert.IsTrue(tree.MatchAny("DevExpress.Logs"));
      Assert.IsFalse(tree.MatchAny("DevExpress"));
      Assert.IsFalse(tree.MatchAny("Dev.Express"));
      Assert.IsTrue(tree.MatchAny("System"));
      Assert.IsTrue(tree.MatchAny("DocumentView"));
      Assert.AreEqual(4, tree.Count);

      var hs = new HashSet<string>(tree.GetAllPrefixes());
      Assert.AreEqual(4, hs.Count);

      Assert.IsTrue(hs.Contains("DevExpress."));
      Assert.IsTrue(hs.Contains("System"));
      Assert.IsTrue(hs.Contains("Syslogs"));
      Assert.IsTrue(hs.Contains("Document"));

      tree.Clear();
      Assert.AreEqual(0, tree.Count);
      Assert.AreEqual(0, tree.GetAllPrefixes().Length);
      Assert.IsFalse(tree.MatchAny("Anything"));
    }

    [TestMethod]
    public void DuplicatePrefix()
    {
      PrefixTree tree = new PrefixTree();
      tree.AddPrefix("DevExpress.");
      tree.AddPrefix("System");
      tree.AddPrefix("System");

      Assert.AreEqual(2, tree.Count);
    }

    [TestMethod]
    public void DeferredFactoryTest()
    {
      var factory = new DeferredFactory<IComponent>("System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "System.Windows.Forms.Form");
      Assert.IsTrue(factory.Create() is Component);
    }

    [TestMethod]
    public void DelegateCreation()
    {
      var line = typeof(DelegateTester).CreateDelegate<Action<string>>("SetMessage");
      line("Static call");
      Assert.AreEqual("Static call", DelegateTester.Message);

      Info inf = new Info("Info mesage", InfoLevel.Info);
      var tos = inf.CreateDelegate<Func<string>>("get_Message");
      line(tos());
      Assert.AreEqual("Info mesage", DelegateTester.Message);

      tos = inf.CreateDelegate<Func<string>>("ToString");
      line(tos());
      Assert.AreEqual(inf.ToString(), DelegateTester.Message);
    }

    private class DelegateTester
    {
      public static string Message;

      public static void SetMessage(string message)
      {
        Message = message;
      }
    }
  }
}
