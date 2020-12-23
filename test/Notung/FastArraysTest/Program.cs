using System;
using System.Collections;

namespace FastArraysTest
{
  partial class Program
  {
    static void Fill(double[,] matrix)
    {
      var rnd = new Random();
      for (int i = 0; i < matrix.GetLength(0); i++)
      {
        for (int j = 0; j < matrix.GetLength(1); j++)
          matrix[i, j] = rnd.NextDouble() * 10;
      }
    }

    static void Print(double[,] matrix)
    {
      for (int i = 0; i < matrix.GetLength(0); i++)
      {
        for (int j = 0; j < matrix.GetLength(1); j++)
          Console.Write("{0:0.00}\t", matrix[i, j]);

        Console.WriteLine();
      }
    }

    static double[,] One(int size)
    {
      double[,] ret = new double[size, size];

      for (int i = 0; i < size; i++)
        ret[i, i] = 1;

      return ret;
    }


    static void Main(string[] args)
    {
      TestArray();
      Console.WriteLine();
      TestBlock();
      Console.WriteLine();
      PerformanceTest();

      Console.ReadKey();
    }

    static void TestArray()
    {
      SimpleBitArray sim = new SimpleBitArray(513);

      for (int i = 0; i < sim.Length; i++)
        sim[i] = i % 3 != 0;

      for (int i = 0; i < sim.Length; i++)
        Console.Write(sim[i] ? '1':'0');

      Console.WriteLine();
    }

    private static void TestBlock()
    {
      Console.WriteLine("Optimized run");

      var date = DateTime.Now;
      using (var arr = new FastBitArray(513))
      {
        for (int j = 0; j < 200000; j++)
        {
          for (int i = 1; i < arr.Length; i++)
          {
            arr[i] = i % 3 != 0;

            if (arr[i])
              arr[i - 1] = !arr[i - 1];
          }
        }
      }

      DateTime date2 = DateTime.Now;

      Console.WriteLine(date2 - date);

      Console.WriteLine("Safe run");
      date = DateTime.Now;
      var arr1 = new BitArray(513);
      for (int j = 0; j < 200000; j++)
      {
        for (int i = 1; i < arr1.Length; i++)
        {
          arr1[i] = i % 3 != 0;

          if (arr1[i])
            arr1[i - 1] = !arr1[i - 1];
        }
      }
      date2 = DateTime.Now;
      Console.WriteLine(date2 - date);

      Console.WriteLine("Duplicate run");
      date = DateTime.Now;
      var arr2 = new SimpleBitArray(513);
      for (int j = 0; j < 200000; j++)
      {
        for (int i = 1; i < arr2.Length; i++)
        {
          arr2[i] = i % 3 != 0;

          if (arr2[i])
            arr2[i - 1] = !arr2[i - 1];
        }
      }
      date2 = DateTime.Now;
      Console.WriteLine(date2 - date);
    }

    private static void PerformanceTest()
    {
      var mt1 = new double[85, 216];
      var mt2 = new double[216, 92];

      Console.WriteLine("Preparing data...");

      Fill(mt1);
      Fill(mt2);

      Console.WriteLine("Safe run");

      var date = DateTime.Now;

      for (int i = 0; i < 100; i++)
      {
        MatrixMultiplySafe(mt1, mt2);
      }

      DateTime date2 = DateTime.Now;

      Console.WriteLine(date2 - date);

      Console.WriteLine("Optimized run");
      date = DateTime.Now;
      for (int i = 0; i < 100; i++)
        MatrixMultiply(mt1, mt2);

      date2 = DateTime.Now;
      Console.WriteLine(date2 - date);

      Console.WriteLine("Middle run");
      date = DateTime.Now;
      for (int i = 0; i < 100; i++)
        MatrixMultiplyMiddle(mt1, mt2);

      date2 = DateTime.Now;
      Console.WriteLine(date2 - date);
    }

    private static void Comparison()
    {
      var mt1 = new double[3, 6];
      var mt2 = new double[6, 3];

      Fill(mt1);
      Fill(mt2);

      Print(mt1);

      Console.WriteLine();

      Print(mt2);

      Console.WriteLine();

      var mt3 = MatrixMultiply(mt1, mt2);
      var mt4 = MatrixMultiplySafe(mt1, mt2);

      Print(mt3);

      Console.WriteLine();

      Print(mt4);
    }
  }
}
