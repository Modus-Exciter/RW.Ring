using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notung;
using Notung.Data;
using Notung.Loader;
using System.Collections.Generic;

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
  }
}
