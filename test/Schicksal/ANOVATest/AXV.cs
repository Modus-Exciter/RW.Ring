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

      dt.Rows.Add("A", "X", 12, 1);
      dt.Rows.Add("A", "X", 13, 2);
      dt.Rows.Add("A", "X", 9, 3);
      dt.Rows.Add("A", "X", 17, 4);
      dt.Rows.Add("B", "X", 20, 1);
      dt.Rows.Add("B", "X", 15, 2);
      dt.Rows.Add("B", "X", 18, 3);
      dt.Rows.Add("A", "Y", 12, 1);
      dt.Rows.Add("A", "Y", 18, 2);
      dt.Rows.Add("A", "Y", 17, 3);
      dt.Rows.Add("A", "Y", 15, 4);
      dt.Rows.Add("B", "Y", 18, 1);
      dt.Rows.Add("B", "Y", 27, 2);
      dt.Rows.Add("B", "Y", 23, 3);
      dt.Rows.Add("B", "X", 12, 4);
      dt.Rows.Add("B", "Y", 24, 4);

      dt.EndLoadData();
      dt.AcceptChanges();

      return dt;
    }
  }
}