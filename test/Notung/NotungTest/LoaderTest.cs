using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notung.Loader;
using System;

namespace NotungTest
{
  [TestClass]
  public class LoaderTest
  {
    [TestMethod]
    public void SecondLoader()
    {
      bool error_was = false;
      try
      {
        ApplicationLoader<ILoader1, ILoader2> wrong = new ApplicationLoader<ILoader1, ILoader2>();
        wrong.Dependencies.ToString();
      }
      catch
      {
        error_was = true;
      }

      Assert.IsTrue(error_was);

      ApplicationLoader<ILoader1, Loader1> right1 = new ApplicationLoader<ILoader1, Loader1>();
      ApplicationLoader<ILoader1, Loader2> right2 = new ApplicationLoader<ILoader1, Loader2>();
      ApplicationLoader<ILoader2, Loader2> right3 = new ApplicationLoader<ILoader2, Loader2>();

      Assert.AreEqual(typeof(ILoader1), right1.Key);
      Assert.AreEqual(typeof(ILoader1), right2.Key);
      Assert.AreEqual(typeof(ILoader2), right3.Key);
    }
  }

  public interface ILoader1 { }

  public interface ILoader2 : ILoader1 { }

  public class Loader1 : ILoader1 { }

  public class Loader2 : ILoader2 { }
}
