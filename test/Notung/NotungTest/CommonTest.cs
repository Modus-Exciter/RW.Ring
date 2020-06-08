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
    public void SingleThreadPoolTest()
    {
      using (Pool<string> pool = new Pool<string>(new string[]
      {
        "One", "Two", "Three"
      }))
      {
        using (var r1 = Rent.Create(pool))
        {
          Assert.AreEqual("One", r1.Data);

          using (var r2 = Rent.Create(pool))
          {
            Assert.AreEqual("Two", r2.Data);
            using (var r3 = Rent.Create(pool))
            {
              Assert.AreEqual("Three", r3.Data);

              Assert.AreEqual(-1, pool.Accuire(false));
            }
          }
        }
      }
    }

    [TestMethod]
    public void MultyThreadPoolTest()
    {
      using (Pool<string> pool = new Pool<string>(new string[]
      {
        "One", "Two", "Three"
      }))
      {
        Thread t1 = new Thread(() =>
          {
            using (var r1 = Rent.Create(pool))
            {
              Assert.AreEqual("One", r1.Data);
              Thread.Sleep(500);
            }
          });
        Thread t2 = new Thread(() =>
        {
          using (var r1 = Rent.Create(pool))
          {
            Assert.AreEqual("Two", r1.Data);
            Thread.Sleep(700);
          }
        });
        Thread t3 = new Thread(() =>
        {
          using (var r1 = Rent.Create(pool))
          {
            Assert.AreEqual("Three", r1.Data);
            Thread.Sleep(500);

            using (var r2 = Rent.Create(pool))
            {
              Assert.AreEqual("One", r2.Data);
              Thread.Sleep(500);
            }
          }
        });

        t1.Start();
        Thread.Sleep(100);

        t2.Start();

        Thread.Sleep(100);
        t3.Start();

        t1.Join();
        using (var r3 = Rent.Create(pool))
        {
          Assert.AreEqual("One", r3.Data);
        }
        t2.Join();
        t3.Join();
      }
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