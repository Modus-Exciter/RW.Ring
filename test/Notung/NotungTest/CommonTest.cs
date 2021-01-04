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
    public void FillArray()
    {
      string[] texts = new string[241];
      texts.Fill("Seven");

      double[] nums = new double[342];
      nums.Fill(7.7);

      char[] atm = new char[256];
      atm.Fill('7');

      Assert.IsTrue(Array.TrueForAll(texts, t => t == "Seven"));
      Assert.IsTrue(Array.TrueForAll(nums, t => t == 7.7));
      Assert.IsTrue(Array.TrueForAll(atm, t => t == '7'));
    }

    [TestMethod]
    public void PrimeNumbers()
    {
      var type = typeof(AppManager).Assembly.GetType("Notung.Data.PrimeHelper");

      var is_prime = type.CreateDelegate<Func<int, bool>>("IsPrime");

      Assert.IsFalse(is_prime(1));
      Assert.IsTrue(is_prime(2));
      Assert.IsTrue(is_prime(3));
      Assert.IsFalse(is_prime(4));
      Assert.IsTrue(is_prime(5));
      Assert.IsFalse(is_prime(6));
      Assert.IsTrue(is_prime(7));
      Assert.IsFalse(is_prime(8));
      Assert.IsFalse(is_prime(9));
      Assert.IsFalse(is_prime(10));
      Assert.IsTrue(is_prime(11));
      Assert.IsFalse(is_prime(12));
      Assert.IsTrue(is_prime(13));
      Assert.IsFalse(is_prime(14));
      Assert.IsFalse(is_prime(15));
      Assert.IsFalse(is_prime(16));
      Assert.IsTrue(is_prime(17));
      Assert.IsTrue(is_prime(331));
      Assert.IsTrue(is_prime(int.MaxValue));

      var get_prime = type.CreateDelegate<Func<int, int>>("GetPrime");

      Assert.AreEqual(3, get_prime(1));
      Assert.AreEqual(3, get_prime(2));
      Assert.AreEqual(3, get_prime(3));
      Assert.AreEqual(5, get_prime(4));
      Assert.AreEqual(7, get_prime(6));
      Assert.AreEqual(17, get_prime(14));
      Assert.AreEqual(17, get_prime(15));
      Assert.AreEqual(37, get_prime(34));
      Assert.AreEqual(37, get_prime(35));
      Assert.AreEqual(79, get_prime(74));
      Assert.AreEqual(163, get_prime(158));
      Assert.AreEqual(331, get_prime(326));
      Assert.AreEqual(int.MaxValue - 18, get_prime(int.MaxValue - 20));
      Assert.AreEqual(int.MaxValue, get_prime(int.MaxValue - 17));
    }

    [TestMethod]
    public void FillBoolArray()
    {
      bool[] vals = new bool[2341];
      vals.Fill(true);

      Assert.IsTrue(Array.TrueForAll(vals, t => t));

      vals.Fill(false);

      Assert.IsTrue(Array.TrueForAll(vals, t => !t));
    }

    [TestMethod]
    public void WeakSetDoubleTime()
    {
      var set = new WeakSet<Item>();
      AddStrings(set);

      GC.Collect();

      set.Add("");

      Assert.IsFalse(set.Contains("A1"));
      Assert.IsFalse(set.Contains("A2"));
      Assert.IsFalse(set.Contains("A3"));
      Assert.IsTrue(set.Contains(""));
    }

    public class Item
    {
      public string Value { get; set; }

      public override bool Equals(object obj)
      {
        return Equals(this.Value, ((Item)obj).Value);
      }

      public override int GetHashCode()
      {
        return (this.Value ?? "").GetHashCode();
      }

      public static implicit operator Item(string value)
      {
        return new Item { Value = value ?? "" };
      }
    }

    private static void AddStrings(WeakSet<Item> set)
    {
      string a1 = "A1";
      string a2 = "A2";

      set.Add(a1);
      set.Add(a2);
      set.Add("B1");
      Assert.IsTrue(set.Remove("B1"));

      Assert.IsTrue(set.Contains(a1));
      Assert.IsTrue(set.Contains(a1));
      Assert.IsTrue(set.Contains("A1"));
      Assert.IsTrue(set.Contains("A2"));
      Assert.IsFalse(set.Contains("B1"));
    }

    [TestMethod]
    public void SharedLockStubWork()
    {
      var stub = new SharedLockStub();

      Assert.AreEqual(LockState.None, stub.LockState);

      using (stub.ReadLock())
        Assert.AreEqual(LockState.Read, stub.LockState);

      Assert.AreEqual(LockState.None, stub.LockState);

      using (stub.WriteLock())
        Assert.AreEqual(LockState.Write, stub.LockState);

      Assert.AreEqual(LockState.None, stub.LockState);

      using (stub.UpgradeableLock())
        Assert.AreEqual(LockState.Upgradeable, stub.LockState);

      Assert.AreEqual(LockState.None, stub.LockState);

      using (stub.ReadLock())
      {
        Assert.AreEqual(LockState.Read, stub.LockState);
        using (stub.UpgradeableLock())
        {
          Assert.AreEqual(LockState.Upgradeable, stub.LockState);
          using (stub.WriteLock())
            Assert.AreEqual(LockState.Write, stub.LockState);
          Assert.AreEqual(LockState.Upgradeable, stub.LockState);
        }
        Assert.AreEqual(LockState.Read, stub.LockState);
      }
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
        Assert.AreEqual(0, pool.Trottle);
        using (var r1 = PoolItem.Create(pool))
        {
          Assert.AreEqual(1, pool.Trottle);
          Assert.AreEqual("One", r1.Data);

          using (var r2 = PoolItem.Create(pool))
          {
            Assert.AreEqual(2, pool.Trottle);
            Assert.AreEqual("Two", r2.Data);
            using (var r3 = PoolItem.Create(pool))
            {
              Assert.AreEqual(3, pool.Trottle);
              Assert.AreEqual("Three", r3.Data);

              Assert.AreEqual(-1, pool.Accuire(false));
            }
          }
        }

        Assert.AreEqual(0, pool.Trottle);
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
            using (var r1 = PoolItem.Create(pool))
            {
              Assert.AreEqual("One", r1.Data);
              Thread.Sleep(500);
            }
          });
        Thread t2 = new Thread(() =>
        {
          using (var r1 = PoolItem.Create(pool))
          {
            Assert.AreEqual("Two", r1.Data);
            Thread.Sleep(700);
          }
        });
        Thread t3 = new Thread(() =>
        {
          using (var r1 = PoolItem.Create(pool))
          {
            Assert.AreEqual("Three", r1.Data);
            Thread.Sleep(500);

            using (var r2 = PoolItem.Create(pool))
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
        using (var r3 = PoolItem.Create(pool))
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