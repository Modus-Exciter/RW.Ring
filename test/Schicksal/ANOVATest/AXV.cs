using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schicksal.Anova;
using Schicksal.Basic;
using System;
using System.Collections.Generic;
using System.Data;

namespace ANOVATest
{
  [TestClass]
  public class AXV
  {
    [TestMethod]
    public void OptimalDataFiltering()
    {
      var table = new DataTable();
      table.Columns.Add("F1", typeof(string));
      table.Columns.Add("F2", typeof(string));
      table.Columns.Add("Value", typeof(double));
      table.Columns.Add("Repeat", typeof(int));

      // Градации: F1 (A, B, C), F2 (X, Y, Z)
      // Комбинация C-Z отсутствует
      // Градация C 2 наблюдения (C-X, C-Y)
      // Градация Z 6 наблюдений (A-Z, B-Z)
      table.Rows.Add("A", "X", 10, 1);
      table.Rows.Add("A", "X", 11, 2);
      table.Rows.Add("A", "Y", 20, 1);
      table.Rows.Add("A", "Y", 21, 2);
      table.Rows.Add("A", "Z", 30, 1);
      table.Rows.Add("A", "Z", 31, 2);
      table.Rows.Add("A", "Z", 32, 3);

      table.Rows.Add("B", "X", 40, 1);
      table.Rows.Add("B", "X", 41, 2);
      table.Rows.Add("B", "Y", 50, 1);
      table.Rows.Add("B", "Y", 51, 2);
      table.Rows.Add("B", "Z", 60, 1);
      table.Rows.Add("B", "Z", 61, 2);
      table.Rows.Add("B", "Z", 62, 3);

      table.Rows.Add("C", "X", 70, 1); //Группа C-X (единственное наблюдение)
      table.Rows.Add("C", "Y", 80, 1); //Группа C-Y (единственное наблюдение)
                                       // C-Z отсутствует
      table.AcceptChanges();

      var sample = SampleRepack.Wrap(
          new TableDividedSample(
              new PredictedResponseParameters(table, null, FactorInfo.Parse("F1+F2"), "Value")));

      var filtered = InteractionCalculator.Filter(sample, FactorInfo.Parse("F1+F2"));

      Assert.IsTrue(InteractionCalculator.IsFull(filtered, FactorInfo.Parse("F1+F2")));

      //Ожидаем 6 групп после очистки (A-X, A-Y, A-Z, B-X, B-Y, B-Z)
      Assert.AreEqual(6, filtered.Count);

      int totalObservations = 0;
      bool hasC = false;
      bool hasZ = true;

      for (int i = 0; i < filtered.Count; i++)
      {
        var key = filtered.GetKey(i);
        totalObservations += filtered[i].Count;

        if ("C".Equals(key["F1"]))
          hasC = true;

        if ("Z".Equals(key["F2"]))  
          hasZ = true;
      }

      Assert.IsFalse(hasC, "Градация 'C' должна быть удалена");
      Assert.IsTrue(hasZ, "Градация 'Z' должна быть сохранена");
      Assert.AreEqual(14, totalObservations,
          "Удалено неверное количество наблюдений. Ожидалось 14");
    }

    [TestMethod]
    public void BaseDataFiltering()
    {
      var table = GenerateTable();
      table.Rows.Add("C", "X", 18, 1);
      table.Rows.Add("C", "X", 17, 2);
      table.Rows.Add("C", "X", 19, 3);
      table.Rows.Add("C", "X", 19, 4);
      table.AcceptChanges();

      var sample = SampleRepack.Wrap(
          new TableDividedSample(
              new PredictedResponseParameters(table, null, FactorInfo.Parse("F1+F2"), "Value")));

      var filtered = InteractionCalculator.Filter(sample, FactorInfo.Parse("F1+F2"));
      Assert.IsTrue(InteractionCalculator.IsFull(filtered, FactorInfo.Parse("F1+F2")));

      Assert.AreEqual(4, filtered.Count); //ожидаем 4 группы: A-X, A-Y, B-X, B-Y

      int totalCount = 0;
      for (int i = 0; i < filtered.Count; i++)
      {
        var key = filtered.GetKey(i);
        Assert.AreNotEqual("C", key["F1"]); //ни в одной группе не должно быть градации С
        totalCount += filtered[i].Count;
      }

      //Должно быть 16 наблюдений (изначальные данные до добавления строчек с С)
      Assert.AreEqual(16, totalCount);
    }

    [TestMethod]
    public void NoNormalizationIndependentCommon()
    {
      Utils.CheckMultipleFactors
      (
        new AnovaParameters
        (
          GenerateTable(),
          null,
          FactorInfo.Parse("F1+F2"),
          "Value",
          0.05f,
          DummyNormalizer.Instance,
          null,
          false
        ),
        new Dictionary<FactorInfo, double>
        {
          { FactorInfo.Parse("F1"), 10.9585 },
          { FactorInfo.Parse("F2"), 8.1736 },
          { FactorInfo.Parse("F2+F1"), 1.4491 }
        },
        new Dictionary<FactorInfo, double>
        {
          {FactorInfo.Parse("F1"), 0.00622022470339304 },
          { FactorInfo.Parse("F2"), 0.0143818891590552 },
          { FactorInfo.Parse("F2+F1"), 0.25188833200287 }
        }
      );
    }

    [TestMethod]
    public void NoNormalizationIndependentIndividual()
    {
      Utils.CheckMultipleFactors
      (
        new AnovaParameters
        (
          GenerateTable(),
          null,
          FactorInfo.Parse("F1+F2"),
          "Value",
          0.05f,
          DummyNormalizer.Instance,
          null,
          true
        ),
        new Dictionary<FactorInfo, double>
        {
          { FactorInfo.Parse("F1"), 10.9585 },
          { FactorInfo.Parse("F2"), 8.1736 },
          { FactorInfo.Parse("F2+F1"), 1.4491 }
        },
        new Dictionary<FactorInfo, double>
        {
          {FactorInfo.Parse("F1"), 0.00622022470339304 },
          { FactorInfo.Parse("F2"), 0.0143818891590552 },
          { FactorInfo.Parse("F2+F1"), 0.25188833200287 }
        }
      );
    }

    [TestMethod]
    public void NoNormalizationConjugationCommon()
    {
      Utils.CheckMultipleFactors
      (
        new AnovaParameters
        (
          GenerateTable(),
          null,
          FactorInfo.Parse("F1+F2"),
          "Value",
          0.05f,
          DummyNormalizer.Instance,
          "Repeat",
          false
        ),
        new Dictionary<FactorInfo, double>
        {
          { FactorInfo.Parse("F1"), 9.2878 },
          { FactorInfo.Parse("F2"), 6.9275 },
          { FactorInfo.Parse("F2+F1"), 1.2281 }
        },
        new Dictionary<FactorInfo, double>
        {
          {FactorInfo.Parse("F1"), 0.0138466340859446 },
          { FactorInfo.Parse("F2"), 0.0272683981624256 },
          { FactorInfo.Parse("F2+F1"), 0.296503864451737 }
        }
      );
    }

    [TestMethod]
    public void NoNormalizationConjugationIndividual()
    {
      Utils.CheckMultipleFactors
      (
        new AnovaParameters
        (
          GenerateTable(),
          null,
          FactorInfo.Parse("F1+F2"),
          "Value",
          0.05f,
          DummyNormalizer.Instance,
          "Repeat",
          true
        ),
        new Dictionary<FactorInfo, double>
        {
          { FactorInfo.Parse("F1"), 9.2878 },
          { FactorInfo.Parse("F2"), 6.9275 },
          { FactorInfo.Parse("F2+F1"), 1.2281 }
        },
        new Dictionary<FactorInfo, double>
        {
          {FactorInfo.Parse("F1"), 0.0138466340859446 },
          { FactorInfo.Parse("F2"), 0.0272683981624256 },
          { FactorInfo.Parse("F2+F1"), 0.296503864451737 }
        }
      );
    }

    [TestMethod]
    public void RanksIndependentCommon()
    {
      Utils.CheckMultipleFactors
      (
        new AnovaParameters
        (
          GenerateTable(),
          null,
          FactorInfo.Parse("F1+F2"),
          "Value",
          0.05f,
          new RankNormalizer(2),
          null,
          false
        ),
        new Dictionary<FactorInfo, double>
        {
          { FactorInfo.Parse("F1"), 9.9320 },
          { FactorInfo.Parse("F2"), 6.2095 },
          { FactorInfo.Parse("F2+F1"), 0.6500 }
        },
        new Dictionary<FactorInfo, double>
        {
          {FactorInfo.Parse("F1"), 0.00835161330714116 },
          { FactorInfo.Parse("F2"), 0.0283340136438335 },
          { FactorInfo.Parse("F2+F1"), 0.435814814578577 }
        }
      );
    }

    [TestMethod]
    public void RanksIndependentIndividual()
    {
      Utils.CheckMultipleFactors
      (
        new AnovaParameters
        (
          GenerateTable(),
          null,
          FactorInfo.Parse("F1+F2"),
          "Value",
          0.05f,
          new RankNormalizer(2),
          null,
          true
        ),
        new Dictionary<FactorInfo, double>
        {
          { FactorInfo.Parse("F1"), 9.9320 },
          { FactorInfo.Parse("F2"), 6.2095 },
          { FactorInfo.Parse("F2+F1"), 0.6500 }
        },
        new Dictionary<FactorInfo, double>
        {
          {FactorInfo.Parse("F1"), 0.00835161330714116 },
          { FactorInfo.Parse("F2"), 0.0283340136438335 },
          { FactorInfo.Parse("F2+F1"), 0.435814814578577 }
        }
      );
    }

    [TestMethod]
    public void RanksConjugationCommon()
    {
      Utils.CheckMultipleFactors
      (
        new AnovaParameters
        (
          GenerateTable(),
          null,
          FactorInfo.Parse("F1+F2"),
          "Value",
          0.05f,
          new RankNormalizer(2),
          "Repeat",
          false
        ),
        new Dictionary<FactorInfo, double>
        {
          { FactorInfo.Parse("F1"), 7.9851 },
          { FactorInfo.Parse("F2"), 4.9923 },
          { FactorInfo.Parse("F2+F1"), 0.5226 }
        },
        new Dictionary<FactorInfo, double>
        {
          {FactorInfo.Parse("F1"), 0.0198582019664885 },
          { FactorInfo.Parse("F2"), 0.0523237783421486 },
          { FactorInfo.Parse("F2+F1"), 0.488113626208968 }
        }
      );
    }

    [TestMethod]
    public void RanksConjugationIndividual()
    {
      Utils.CheckMultipleFactors
      (
        new AnovaParameters
        (
          GenerateTable(),
          null,
          FactorInfo.Parse("F1+F2"),
          "Value",
          0.05f,
          new RankNormalizer(2),
          "Repeat",
          true
        ),
        new Dictionary<FactorInfo, double>
        {
          { FactorInfo.Parse("F1"), 7.9851 },
          { FactorInfo.Parse("F2"), 4.9923 },
          { FactorInfo.Parse("F2+F1"), 0.5226 }
        },
        new Dictionary<FactorInfo, double>
        {
          {FactorInfo.Parse("F1"), 0.0198582019664885 },
          { FactorInfo.Parse("F2"), 0.0523237783421486 },
          { FactorInfo.Parse("F2+F1"), 0.488113626208968 }
        }
      );
    }

    private static DataTable GenerateTable()
    {
      var dt = new DataTable();

      dt.Columns.Add("F1", typeof(string));
      dt.Columns.Add("F2", typeof(string));
      dt.Columns.Add("Value", typeof(double));
      dt.Columns.Add("Repeat", typeof(int));

      dt.BeginLoadData();

      dt.Rows.Add("A", "X", 9, 3);
      dt.Rows.Add("B", "X", 20, 1);
      dt.Rows.Add("B", "X", 15, 2);
      dt.Rows.Add("A", "Y", 17, 3);
      dt.Rows.Add("B", "X", 18, 3);
      dt.Rows.Add("A", "X", 17, 4);
      dt.Rows.Add("B", "Y", 23, 3);
      dt.Rows.Add("B", "Y", 24, 4);
      dt.Rows.Add("A", "Y", 12, 1);
      dt.Rows.Add("A", "X", 13, 2);
      dt.Rows.Add("A", "Y", 15, 4);
      dt.Rows.Add("B", "Y", 18, 1);
      dt.Rows.Add("A", "Y", 18, 2);
      dt.Rows.Add("B", "Y", 27, 2);
      dt.Rows.Add("B", "X", 12, 4);
      dt.Rows.Add("A", "X", 12, 1);

      dt.EndLoadData();
      dt.AcceptChanges();

      return dt;
    }
  }
}