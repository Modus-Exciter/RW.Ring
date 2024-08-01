using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Schicksal.Anova;
using Schicksal.Basic;
using System.Linq;

namespace ANOVATest
{
  [TestClass]
  public class InnerDispersionTest
  {
    [TestMethod]
    public void ConjugateMoment()
    {
      double[][] data = new double[11][];

      for (int i = 0; i < data.Length; i++)
        data[i] = new double[5];

      Random rnd = new Random();
      ArrayDividedSample sample = new ArrayDividedSample(data);

      for (int i = 0; i < data.Length; i++)
        for (int j = 0; j < data[i].Length; j++)
          data[i][j] = rnd.Next(40) + 5 + rnd.NextDouble();

      int n = sample.Sum(g => g.Count);
      double sum_all = sample.SelectMany(g => g).Sum();
      double c = sum_all * sum_all / n;
      double cy = sample.SelectMany(g => g).Sum(a => a * a) - c;
      int vars = sample[0].Count;


      var average = sample.SelectMany(g => g).Average();

      Assert.AreEqual(sample.SelectMany(g => g).Sum(a => (a - average) * (a - average)), cy, 1e-5);

      double cp = 0;

      for (int i = 0; i < vars; i++)
      {
        double gp = sample.Select(g => g[i]).Sum();
        cp += gp * gp;
      }

      cp /= sample.Count;
      cp -= c;

      double cv = sample.Sum(g => System.Math.Pow(g.Sum(), 2)) / vars - c;
      double cz = cy - cp - cv;

      double cp2 = 0;

      for (int i = 0; i < vars; i++)
      {
        var avg = sample.Select(s => s[i]).Average();
        cp2 += (avg - average) * (avg - average);
      }

      cp2 *= sample.Count;

      Assert.AreEqual(cp, cp2, 1e-5);
      Assert.AreEqual(cz, FisherTest.MSw(sample).SumOfSquares - cp2, 1e-5);
    }

    [TestMethod]
    public void ConjugateMoment2()
    {
      double[] a1 = new double[] { 1, 1, 1 };
      double[] a2= new double[] { 2, 2, 2 };
      double[] a3 = new double[] { 3, 3, 3 };

      ArrayDividedSample sample = new ArrayDividedSample(new double[][] { a1, a2, a3 });
      int n = sample.Sum(g => g.Count);
      double sum_all = sample.SelectMany(g => g).Sum();
      double c = sum_all * sum_all / n;
      double cy = sample.SelectMany(g => g).Sum(a => a * a) - c;
      int vars = sample[0].Count;


      var average = sample.SelectMany(g => g).Average();

      Assert.AreEqual(sample.SelectMany(g => g).Sum(a => (a - average) * (a - average)), cy, 1e-5);

      double cp = 0;

      for (int i = 0; i < vars; i++)
      {
        double gp = sample.Select(g => g[i]).Sum();
        cp += gp * gp;
      }

      cp /= sample.Count;
      cp -= c;

      double cv = sample.Sum(g => System.Math.Pow(g.Sum(), 2)) / vars - c;
      double cz = cy - cp - cv;

      double cp2 = 0;

      for (int i = 0; i < vars; i++)
      {
        var avg = sample.Select(s => s[i]).Average();
        cp2 += (avg - average) * (avg - average);
      }

      cp2 *= sample.Count;

      Assert.AreEqual(cp, cp2, 1e-5);
      Assert.AreEqual(cz, FisherTest.MSw(sample).SumOfSquares - cp2, 1e-5);
    }

    [TestMethod]
    public void ConjugateMoment3()
    {
      double[] a1 = new double[] { 1, 2, 3 };
      double[] a2 = new double[] { 1, 2, 3 };
      double[] a3 = new double[] { 1, 2, 3 };

      ArrayDividedSample sample = new ArrayDividedSample(new double[][] { a1, a2, a3 });
      int n = sample.Sum(g => g.Count);
      double sum_all = sample.SelectMany(g => g).Sum();
      double c = sum_all * sum_all / n;
      double cy = sample.SelectMany(g => g).Sum(a => a * a) - c;
      int vars = sample[0].Count;


      var average = sample.SelectMany(g => g).Average();

      Assert.AreEqual(sample.SelectMany(g => g).Sum(a => (a - average) * (a - average)), cy, 1e-5);

      double cp = 0;

      for (int i = 0; i < vars; i++)
      {
        double gp = sample.Select(g => g[i]).Sum();
        cp += gp * gp;
      }

      cp /= sample.Count;
      cp -= c;

      double cv = sample.Sum(g => System.Math.Pow(g.Sum(), 2)) / vars - c;
      double cz = cy - cp - cv;

      double cp2 = 0;

      for (int i = 0; i < vars; i++)
      {
        var avg = sample.Select(s => s[i]).Average();
        cp2 += (avg - average) * (avg - average);
      }

      cp2 *= sample.Count;

      Assert.AreEqual(cp, cp2, 1e-5);
      Assert.AreEqual(cz, FisherTest.MSw(sample).SumOfSquares - cp2, 1e-5);
    }

    //В наборе данных есть 3 фактора (A-2уровня, B-3уровня, C-2уровня), необходимо выполнить многофакторный дисперсионный анализ для одной, двух и трех факторов и проверить, одинакова ли внутренняя дисперсия в результатах анализа с одним, двумя и тремя факторами.
    [TestMethod]
    public void NdfTestInThreeGroups()
    {
      //Три фактора
      //A1
      ArrayPlainSample groupA1B1C1 = new ArrayPlainSample(new double[] { 2, 5, 4 });
      ArrayPlainSample groupA1B1C2 = new ArrayPlainSample(new double[] { 3, 6, 1 });
      ArrayPlainSample groupA1B2C1 = new ArrayPlainSample(new double[] { 2, 2, 3 });
      ArrayPlainSample groupA1B2C2 = new ArrayPlainSample(new double[] { 4, 7, 2 });
      ArrayPlainSample groupA1B3C1 = new ArrayPlainSample(new double[] { 2, 5, 6 });
      ArrayPlainSample groupA1B3C2 = new ArrayPlainSample(new double[] { 4, 8, 3 });
      ArrayDividedSample threeMultiA1 = new ArrayDividedSample(new IPlainSample[] { groupA1B1C1, groupA1B1C2, groupA1B2C1, groupA1B2C2, groupA1B3C1, groupA1B3C2 });

      //A2
      ArrayPlainSample groupA2B1C1 = new ArrayPlainSample(new double[] { 3, 5, 5 });
      ArrayPlainSample groupA2B1C2 = new ArrayPlainSample(new double[] { 6, 1, 2 });
      ArrayPlainSample groupA2B2C1 = new ArrayPlainSample(new double[] { 2, 3, 5 });
      ArrayPlainSample groupA2B2C2 = new ArrayPlainSample(new double[] { 4, 7, 5 });
      ArrayPlainSample groupA2B3C1 = new ArrayPlainSample(new double[] { 8, 1, 2 });
      ArrayPlainSample groupA2B3C2 = new ArrayPlainSample(new double[] { 4, 3, 4 });
      ArrayDividedSample threeMultiA2 = new ArrayDividedSample(new IPlainSample[] { groupA2B1C1, groupA2B1C2, groupA2B2C1, groupA2B2C2, groupA2B3C1, groupA2B3C2 });

      ArrayComplexSample threeSet = new ArrayComplexSample(new IDividedSample[] { threeMultiA1, threeMultiA2 });
      FisherMetrics f3 = FisherCriteria.CalculateMultiplyCriteria(threeSet);

      ArrayDividedSample twoMultiA1B1 = new ArrayDividedSample(new IPlainSample[] { groupA1B1C1, groupA1B1C2 });
      ArrayDividedSample twoMultiA1B2 = new ArrayDividedSample(new IPlainSample[] { groupA1B2C1, groupA1B2C2 });
      ArrayDividedSample twoMultiA1B3 = new ArrayDividedSample(new IPlainSample[] { groupA1B3C1, groupA1B3C2 });

      ArrayDividedSample twoMultiA2B1 = new ArrayDividedSample(new IPlainSample[] { groupA2B1C1, groupA2B1C2 });
      ArrayDividedSample twoMultiA2B2 = new ArrayDividedSample(new IPlainSample[] { groupA2B2C1, groupA2B2C2 });
      ArrayDividedSample twoMultiA2B3 = new ArrayDividedSample(new IPlainSample[] { groupA2B3C1, groupA2B3C2 });

      ArrayComplexSample twoSet = new ArrayComplexSample(new IDividedSample[] { twoMultiA1B1, twoMultiA1B2, twoMultiA1B3, twoMultiA2B1, twoMultiA2B2, twoMultiA2B3 });
      FisherMetrics f2 = FisherCriteria.CalculateMultiplyCriteria(twoSet);

      ArrayDividedSample oneMultiA1B1C1 = new ArrayDividedSample(new IPlainSample[] { groupA1B1C1 });
      ArrayDividedSample oneMultiA1B1C2 = new ArrayDividedSample(new IPlainSample[] { groupA1B1C2 });
      ArrayDividedSample oneMultiA1B2C1 = new ArrayDividedSample(new IPlainSample[] { groupA1B2C1 });
      ArrayDividedSample oneMultiA1B2C2 = new ArrayDividedSample(new IPlainSample[] { groupA1B2C2 });
      ArrayDividedSample oneMultiA1B3C1 = new ArrayDividedSample(new IPlainSample[] { groupA1B3C1 });
      ArrayDividedSample oneMultiA1B3C2 = new ArrayDividedSample(new IPlainSample[] { groupA1B3C2 });

      ArrayDividedSample oneMultiA2B1C1 = new ArrayDividedSample(new IPlainSample[] { groupA2B1C1 });
      ArrayDividedSample oneMultiA2B1C2 = new ArrayDividedSample(new IPlainSample[] { groupA2B1C2 });
      ArrayDividedSample oneMultiA2B2C1 = new ArrayDividedSample(new IPlainSample[] { groupA2B2C1 });
      ArrayDividedSample oneMultiA2B2C2 = new ArrayDividedSample(new IPlainSample[] { groupA2B2C2 });
      ArrayDividedSample oneMultiA2B3C1 = new ArrayDividedSample(new IPlainSample[] { groupA2B3C1 });
      ArrayDividedSample oneMultiA2B3C2 = new ArrayDividedSample(new IPlainSample[] { groupA2B3C2 });

      ArrayComplexSample oneSet = new ArrayComplexSample(new IDividedSample[] { oneMultiA1B1C1, oneMultiA1B1C2, oneMultiA1B2C1, oneMultiA1B2C2, oneMultiA1B3C1, oneMultiA1B3C2, oneMultiA2B1C1, oneMultiA2B1C2, oneMultiA2B2C1, oneMultiA2B2C2, oneMultiA2B3C1, oneMultiA2B3C2 });
      FisherMetrics f1 = FisherCriteria.CalculateMultiplyCriteria(oneSet);

      Assert.AreEqual(f3.Ndf, f2.Ndf);
      Assert.AreEqual(f3.MSw, f2.MSw);

      Assert.AreEqual(f2.Ndf, f1.Ndf);
      Assert.AreEqual(f2.MSw, f1.MSw);
    }
    [TestMethod]
    public void Dospehov228pageTest()
    {
      ArrayPlainSample groupA0B0 = new ArrayPlainSample(new double[] { 24.1, 25.8, 23, 27 });
      ArrayPlainSample groupA0B1 = new ArrayPlainSample(new double[] { 28.4, 29.7, 30.1, 27.4 });
      ArrayPlainSample groupA0B2 = new ArrayPlainSample(new double[] { 28.7, 30.4, 32, 27 });
      ArrayDividedSample multiA0 = new ArrayDividedSample(new IPlainSample[] { groupA0B0, groupA0B1, groupA0B2 });

      ArrayPlainSample groupA1B0 = new ArrayPlainSample(new double[] { 30.7, 34.4, 34, 31 });
      ArrayPlainSample groupA1B1 = new ArrayPlainSample(new double[] { 46.7, 45.5, 47.1, 46.3 });
      ArrayPlainSample groupA1B2 = new ArrayPlainSample(new double[] { 59.4, 50.7, 64.5, 60.1 });
      ArrayDividedSample multiA1 = new ArrayDividedSample(new IPlainSample[] { groupA1B0, groupA1B1, groupA1B2 });

      ArrayComplexSample set = new ArrayComplexSample(new IDividedSample[] { multiA0, multiA1 });
      FisherMetrics f = FisherCriteria.CalculateMultiplyCriteria(set);
      Assert.AreEqual(Math.Round(249.88), Math.Round(f.F));
    }

    private struct FisherMetrics
    {
      /// <summary>
      /// Число степеней свободы для межгрупповой дисперсии
      /// </summary>
      public uint Kdf { get; internal set; }

      /// <summary>
      /// Число степеней свободы для внутригрупповой дисперсии
      /// </summary>
      public uint Ndf { get; internal set; }

      /// <summary>
      /// Межгрупповая дисперсия
      /// </summary>
      public double MSb { get; internal set; }

      /// <summary>
      /// Суммарная внутригрупповая дисперсия
      /// </summary>
      public double MSw { get; internal set; }

      /// <summary>
      /// Отношение межгрупповой дисперсии к внутригрупповой
      /// </summary>
      public double F
      {
        get { return this.MSb / this.MSw; }
      }
    }

    private class FisherCriteria
    {

      public static FisherMetrics CalculateConjugate(IComplexSample sample)
      {
        var degrees = default(FisherMetrics);

        int n = sample.SelectMany(g => g).Sum(g => g.Count);
        double sum_all = sample.SelectMany(g => g).SelectMany(g => g).Sum();
        double c = sum_all * sum_all / n;
        double cy = sample.SelectMany(g => g).SelectMany(g => g).Sum(a => a * a) - c;
        int vars = sample[0][0].Count;

        double cp = 0;

        for (int i = 0; i < vars; i++)
        {
          double gp = sample.SelectMany(g => g).Select(g => g[i]).Sum();
          cp += gp * gp;
        }
        cp /= sample.Sum(g => g.Count);
        cp -= c;
        double cv = sample.SelectMany(g => g).Sum(g => System.Math.Pow(g.Sum(), 2)) / vars - c;

        double cz = cy - cp - cv;

        double ca = 0;

        foreach (var sub_sample in sample)
        {
          var sum = sub_sample.SelectMany(g => g).Sum();
          ca += sum * sum / sub_sample.Sum(g => g.Count);
        }

        ca -= c;

        degrees.Kdf = (uint)(sample.Count - 1);
        degrees.Ndf = (uint)(n - sample.Sum(g => g.Count) - vars + 1);
        degrees.MSb = ca / degrees.Kdf;
        degrees.MSw = cz / degrees.Ndf;

        return degrees;
      }
      
      internal static FisherMetrics CalculateCriteria(IDividedSample multi)
      {
        var msw = FisherTest.MSw(multi);
        var msb = FisherTest.MSb(multi);

        return new FisherMetrics
        {
          Kdf = (uint)msb.DegreesOfFreedom,
          Ndf = (uint)msw.DegreesOfFreedom,
          MSb = msb.MeanSquare,
          MSw = msw.MeanSquare
        };
      }

      internal static FisherMetrics CalculateMultiplyCriteria(IComplexSample set)
      {
        var between = new ArrayDividedSample(set.Select(s => new JoinedSample(s)).ToArray());
        var within = new PartiallyJoinedSample(set);

        var msw = FisherTest.MSw(within);
        var msb = FisherTest.MSb(between);

        return new FisherMetrics
        {
          Kdf = (uint)msb.DegreesOfFreedom,
          Ndf = (uint)msw.DegreesOfFreedom,
          MSb = msb.MeanSquare,
          MSw = msw.MeanSquare
        };
      }
    }
  }
}
