using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
      PerformanceTest();

      Console.ReadKey();
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
