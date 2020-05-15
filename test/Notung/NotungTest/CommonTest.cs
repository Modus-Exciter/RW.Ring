using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notung;
using Notung.Data;
using Notung.Loader;

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
      Assert.IsTrue(tree.MatchAny("System"));
      Assert.IsTrue(tree.MatchAny("DocumentView"));
    }

    [TestMethod]
    public void DeferredFactoryTest()
    {
      var factory = new DeferredFactory<IComponent>("System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "System.Windows.Forms.Form");
      Assert.IsTrue(factory.Create() is Component);
    }
  }
}
