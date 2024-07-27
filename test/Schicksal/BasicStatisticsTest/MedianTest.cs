using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schicksal.Basic;
using System;

namespace BasicStatisticsTest
{
  [TestClass]
  public class MedianTest
  {
    public static double Median2(IPlainSample group)
    {
      int min_idx = 0;

      while (double.IsInfinity(group[min_idx]))
      {
        min_idx++;

        if (min_idx == group.Count)
          return double.NaN;
      }

      double min = group[min_idx];
      double max = group[min_idx];

      for (int i = 0; i < group.Count; i++)
      {
        if (double.IsInfinity(group[i]))
          continue;

        if (min > group[i])
          min = group[i];

        if (max < group[i])
          max = group[i];
      }

      int balance = 0;
      int left = -1;
      int right = -1;

      do
      {
        double estimation = (min + max) / 2;
        double left_delta = max - min;
        double right_delta = max - min;

        balance = 0;

        for (int i = 0; i < group.Count; i++)
        {
          if (group[i] < estimation)
          {
            balance--;

            if (left_delta > estimation - group[i])
            {
              left_delta = estimation - group[i];
              left = i;
            }
          }
          else if (group[i] > estimation)
          {
            balance++;

            if (right_delta > group[i] - estimation)
            {
              right_delta = group[i] - estimation;
              right = i;
            }
          }
        }

        if (balance < -1)
          max = left >= 0 ? group[left] : estimation;
        else if (balance > 1)
          min = right>= 0 ? group[right] : estimation;
      } while (Math.Abs(balance) > 1);

      if (balance == -1)
        return group[left];
      else if (balance == 1)
        return group[right];
      else
        return (group[left] + group[right]) / 2;
    }

    [TestMethod]
    public void RandomizerEven()
    {
      Random rnd = new Random();

      double[] values = new double[32];
      var group = new ArrayPlainSample(values);

      for (int i = 0; i < 10; i++)
      {
        for (int j = 0; j < values.Length; j++)
          values[j] = Math.Log(rnd.NextDouble() * rnd.Next(50));

        Assert.AreEqual(DescriptionStatistics.Median(group), Median2(group));
      }
    }

    [TestMethod]
    public void RandomizerOdd()
    {
      Random rnd = new Random();

      double[] values = new double[31];
      var group = new ArrayPlainSample(values);

      for (int i = 0; i < 10; i++)
      {
        for (int j = 0; j < values.Length; j++)
          values[j] = Math.Exp(rnd.NextDouble() * rnd.Next(5));

        values[0] = double.PositiveInfinity;

        Assert.AreEqual(DescriptionStatistics.Median(group), Median2(group));
      }
    }
  }
}
