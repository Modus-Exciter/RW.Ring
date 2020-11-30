using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastArraysTest
{
  partial class Program
  {
    /// <summary>
    /// Это умножение матриц штатными средствами, без оптимизаций
    /// </summary>
    static double[,] MatrixMultiplySafe(double[,] first, double[,] second)
    {
      if (first.GetLength(1) != second.GetLength(0))
        throw new ArgumentException("Matrix sizes does not match");

      double[,] result = new double[first.GetLength(0), second.GetLength(1)];

      for (int i = 0; i < first.GetLength(0); ++i)
      {
        for (int j = 0; j < second.GetLength(1); ++j)
        {
          double sum = 0;

          for (int k = 0; k < first.GetLength(1); ++k)
            sum += first[i, k] * second[k, j];

          result[i, j] = sum;
        }
      }
      return result;
    }

    /// <summary>
    /// Это более продвинутое умножение матриц: здесь с помощью указателей убраны
    /// проверки границ массивов, а габариты массива сохраняются в переменные.
    /// Это позволяет ускорить работу алгоритма примерно вдвое
    /// </summary>
    static unsafe double[,] MatrixMultiplyMiddle(double[,] first, double[,] second)
    {
      if (first.GetLength(1) != second.GetLength(0))
        throw new ArgumentException("Matrix sizes does not match");

      int rows = first.GetLength(0);
      int columns = second.GetLength(1);
      int middle = first.GetLength(1);

      double[,] result = new double[rows, columns];

      fixed (double* p1 = first, p2 = second, p3 = result)
      {
        for (int i = 0; i < rows; ++i)
        {
          for (int j = 0; j < columns; ++j)
          {
            double sum = 0;

            for (int k = 0; k < middle; ++k)
              sum += p1[i * middle + k] * p2[k * columns + j];

            p3[i * columns + j] = sum;
          }
        }
      }

      return result;
    }

    /// <summary>
    /// А здесь всё оптимизировано до предела: кроме использования указателей
    /// для быстрого доступа к элементам массива, вычисление индексов также
    /// оптимизировано, чтобы не было умножений. Работает втрое быстрее обычного
    /// </summary>
    static unsafe double[,] MatrixMultiply(double[,] first, double[,] second)
    {
      if (first.GetLength(1) != second.GetLength(0))
        throw new ArgumentException("Matrix sizes does not match");

      int rows = first.GetLength(0);
      int columns = second.GetLength(1);
      int middle = first.GetLength(1);

      double[,] result = new double[rows, columns];

      fixed (double* p1 = first, p2 = second, p3 = result)
      {
        double* p4 = p3;
        for (int i = 0; i < rows; ++i)
        {
          for (int j = 0; j < columns; ++j)
          {
            double sum = 0;

            int first_idx = i * middle - 1;
            int second_idx = j;

            for (int k = 0; k < middle; ++k)
            {
              sum += p1[++first_idx] * p2[second_idx];
              second_idx += columns;
            }

            *p4++ = sum;
          }
        }
      }

      return result;
    }
  }
}
