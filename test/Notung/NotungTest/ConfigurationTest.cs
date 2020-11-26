using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notung.Configuration;
using System.IO;
using System.ComponentModel;
using Notung;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace NotungTest
{
  /// <summary>
  /// Summary description for ConfigurationTest
  /// </summary>
  [TestClass]
  public class ConfigurationTest
  {
    public ConfigurationTest()
    {
      //
      // TODO: Add constructor logic here
      //
    }

    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }

    #region Additional test attributes
    //
    // You can use the following additional attributes as you write your tests:
    //
    // Use ClassInitialize to run code before running the first test in the class
    // [ClassInitialize()]
    // public static void MyClassInitialize(TestContext testContext) { }
    //
    // Use ClassCleanup to run code after all tests in a class have run
    // [ClassCleanup()]
    // public static void MyClassCleanup() { }
    //
    // Use TestInitialize to run code before running each test 
    // [TestInitialize()]
    // public void MyTestInitialize() { }
    //
    // Use TestCleanup to run code after each test has run
    // [TestCleanup()]
    // public void MyTestCleanup() { }
    //
    #endregion

    [TestMethod]
    public void FinderWorkingPath()
    {
      var finder = new ProductVersionConfigFileFinder(typeof(ProductVersionConfigFileFinder).Assembly);
      Assert.AreEqual(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ARI\\Notung Library\\1.0.0.0"), finder.WorkingPath);
      finder = new ProductVersionConfigFileFinder(this.GetType().Assembly);
      Assert.AreEqual(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ARI\\NotungTest\\1.0.0.0"), finder.WorkingPath);
      Assert.AreEqual(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ARI\\NotungTest\\1.0.0.0\\settings.config"), finder.OutputFilePath);
    }

    [TestMethod]
    public void FinderInputPath()
    {
      var finder = new ProductVersionConfigFileFinder(typeof(ProductVersionConfigFileFinder).Assembly);

      var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ARI\\Notung Library\\1.0.0.0");
      if (!Directory.Exists(directory))
        Directory.CreateDirectory(directory);

      if (!File.Exists(Path.Combine(directory, ProductVersionConfigFileFinder.DEFAULT_FILE)))
        File.WriteAllText(Path.Combine(directory, ProductVersionConfigFileFinder.DEFAULT_FILE), "<Configuration></Configuration>");

      Assert.AreEqual(Path.Combine(directory, ProductVersionConfigFileFinder.DEFAULT_FILE), finder.InputFilePath);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullableConfiguratorInManager()
    {
      AppManager.Configurator = null;
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void WrongChars()
    {
      var finder = new ProductVersionConfigFileFinder(@"\/|.xml");
    }

    [TestMethod]
    public void SectionDefaults()
    {
      var section = new TestSection();
      Assert.AreEqual(7, section.Number);
      Assert.AreEqual("RW", section.Text);
    }

#if APPLICATION_INFO
    [TestMethod]
    public void AppInfoTest()
    {
      var info = new ApplicationInfo(typeof(AppManager).Assembly);
      Assert.AreEqual("Base application framework for RW.Ring", info.Description);
    }
#endif

    [TestMethod]
    [ExpectedException(typeof(SerializationException))]
    public void DuplicateElementName()
    {
      AppManager.Configurator.GetSection<TestSection>();
      AppManager.Configurator.GetSection<TestSection2>();
    }
  }

  public class TestSection : ConfigurationSection
  {
    [DefaultValue(7)]
    public int Number { get; set; }

    [DefaultValue("RW")]
    public string Text { get; set; }
  }

  [XmlRoot(ElementName = "TestSection")]
  public class TestSection2 : ConfigurationSection
  {
    [DefaultValue(7)]
    public int Number { get; set; }

    [DefaultValue("RW")]
    public string Text { get; set; }

  }
}
