using System;
using System.ComponentModel;
using System.Globalization;
using Notung.Properties;

namespace Notung.Data
{
  /// <summary>
  /// Функции работы с матрицами
  /// </summary>
  public static class MatrixFunctions
  {
    /// <summary>
    /// Вычисление определителя матрицы методом Гаусса
    /// </summary>
    /// <typeparam name="T">Тип ячейки матрицы</typeparam>
    /// <param name="matrix">Матрица, определитель которой требуется посчитать</param>
    /// <param name="provider">Сведения о культуре для преобразования типа ячейки матрицы</param>
    /// <returns>Определитель матрицы</returns>
    public static T Determinant<T>(IMatrix<T> matrix, CultureInfo provider)
      where T : IConvertible
    {
      if (matrix == null)
        throw new ArgumentNullException("matrix");

      if (matrix.RowCount != matrix.ColumnCount)
        throw new ArgumentException(Resources.MATRIX_MUST_BE_SQUARE);

      double[,] data = CopyToDoubleArray<T>(matrix, provider);
      int n = matrix.RowCount;

      for (int shift = 0; shift < n - 1; shift++)
      {
        // Если там 0, надо взять данные из любой другой строки
        if (data[shift, shift] == 0)
        {
          for (int i = shift + 1; i < n; i++)
          {
            if (data[i, shift] != 0)
            {
              for (int j = shift; j < n; j++)
                data[shift, j] += data[i, j];

              break;
            }
          }
        }

        // Весь столбец 0 - значит, определитель будет 0
        if (data[shift, shift] == 0)
          return ResultProcessor<T>.Convert(0, provider);

        // Выполняем исключение очередной строки со столбцом
        for (int i = shift + 1; i < n; i++)
        {
          double q = data[i, shift] / data[shift, shift];

          for (int j = shift; j < n; j++)
            data[i, j] = data[i, j] - q * data[shift, j];
        }
      }

      double d = 1;

      for (int i = 0; i < n; i++)
        d *= data[i, i];

      return ResultProcessor<T>.Convert(d, provider);
    }

    /// <summary>
    /// Транспонирование матрицы
    /// </summary>
    /// <typeparam name="T">Тип ячейки матрицы</typeparam>
    /// <param name="matrix">Матрица, которую требуется транспонировать</param>
    /// <returns>Транспонированная матрица</returns>
    public static IMatrix<T> Transpose<T>(IMatrix<T> matrix)
    {
      if (matrix == null)
        throw new ArgumentNullException("matrix");

      RectangleMatrix<T> result = new RectangleMatrix<T>(matrix.ColumnCount, matrix.RowCount);
      T[,] destination = result.AsArray();

      if (matrix is RectangleMatrix<T>) // Если можно немного оптимизировать, делаем это
      {
        int rows = result.RowCount;
        int cols = result.ColumnCount;
        T[,] source = ((RectangleMatrix<T>)matrix).AsArray();

        for (int i = 0; i < rows; i++)
        {
          for (int j = 0; j < cols; j++)
            destination[i, j] = source[j, i];
        }
      }
      else
      {
        for (int i = 0; i < result.RowCount; i++)
        {
          for (int j = 0; j < result.ColumnCount; j++)
            destination[i, j] = matrix[j, i];
        }
      }

      return result;
    }

    /// <summary>
    /// Перемножение матриц
    /// </summary>
    /// <typeparam name="T">Тип ячейки матрицы</typeparam>
    /// <param name="matrix1">Первая матрица</param>
    /// <param name="matrix2">Вторая матрица</param>
    /// <param name="provider">Сведения о культуре для преобразования типа ячейки матрицы</param>
    /// <returns>Матрица с произведением матриц matrix1 и matrix2</returns>
    public static IMatrix<T> Multiply<T>(IMatrix<T> matrix1, IMatrix<T> matrix2, CultureInfo provider)
      where T : IConvertible
    {
      if (matrix1 == null)
        throw new ArgumentNullException("matrix1");

      if (matrix2 == null)
        throw new ArgumentNullException("matrix2");

      if (matrix1.ColumnCount != matrix2.RowCount)
        throw new ArgumentException(Resources.MATRIX_UNABLE_TO_MULTIPLY);

      RectangleMatrix<T> result = new RectangleMatrix<T>(matrix1.RowCount, matrix2.ColumnCount);

      if (typeof(T) == typeof(double))
      {
        if (matrix1 is RectangleMatrix<T> && matrix2 is RectangleMatrix<T>)
        {
          MultiplyOptimal(((RectangleMatrix<double>)(object)result).AsArray(),
            ((RectangleMatrix<double>)(object)matrix1).AsArray(),
            ((RectangleMatrix<double>)(object)matrix2).AsArray());
        }
        else
        {
          MultiplyOptimal(((RectangleMatrix<double>)(object)result).AsArray(),
            (IMatrix<double>)matrix1,
            (IMatrix<double>)matrix2);
        }
      }
      else
      {
        int n = result.RowCount;
        int m = result.ColumnCount;
        int l = matrix1.ColumnCount;

        for (int i = 0; i < n; i++)
        {
          for (int j = 0; j < m; j++)
          {
            double sum = 0;

            for (int k = 0; k < l; k++)
              sum += matrix1[i, k].ToDouble(provider) * matrix2[k, j].ToDouble(provider);

            result[i, j] = ResultProcessor<T>.Convert(sum, provider);
          }
        }
      }

      return result;
    }

    /// <summary>
    /// Расчёт обратной матрицы
    /// </summary>
    /// <typeparam name="T">Тип ячейки матрицы</typeparam>
    /// <param name="matrix">Матрица, для которой нужно рассчитать обратную</param>
    /// <param name="provider">Сведения о культуре для преобразования типа ячейки матрицы</param>
    /// <returns>Матрица, обратная данной</returns>
    public static IMatrix<T> Invert<T>(IMatrix<T> matrix, CultureInfo provider)
      where T : IConvertible
    {
      if (matrix == null)
        throw new ArgumentNullException("matrix");

      if (matrix.RowCount != matrix.ColumnCount)
        throw new ArgumentException(Resources.MATRIX_MUST_BE_SQUARE);

      int n = matrix.ColumnCount * 2;
      double[,] data = CreateExtendedArray<T>(matrix, provider);

      // Прямой проход
      for (int shift = 0; shift < matrix.RowCount; shift++)
      {
        // Если там 0, надо взять данные из любой другой строки
        if (data[shift, shift] == 0)
        {
          for (int i = shift + 1; i < matrix.RowCount; i++)
          {
            if (data[i, shift] != 0)
            {
              for (int j = 0; j < n; j++)
                data[shift, j] += data[i, j];

              break;
            }
          }
        }

        // Весь столбец 0 - значит, определитель будет 0
        if (data[shift, shift] == 0)
          return null;

        if (data[shift, shift] != 1)
        {
          double q = 1 / data[shift, shift];

          for (int j = 0; j < n; j++)
            data[shift, j] *= q;
        }

        for (int i = shift + 1; i < matrix.RowCount; i++)
        {
          double q = data[i, shift];
          for (int j = 0; j < n; j++)
            data[i, j] -= q * data[shift, j];
        }
      }    
      
      // Обратный проход
      for (int shift = matrix.RowCount - 1; shift > 0; shift--)
      {
        for (int i = shift; i > 0; i--)
        {
          double q = data[i - 1, shift];

          for (int j = 0; j < n; j++)
            data[i - 1, j] -= q * data[shift, j];
        }
      }

      RectangleMatrix<T> result = new RectangleMatrix<T>(matrix.RowCount, matrix.ColumnCount);
      T[,] destination = result.AsArray();

      for (int i = 0; i < matrix.RowCount; i++)
      {
        for (int j = 0; j < matrix.ColumnCount; j++)
          destination[i, j] = ResultProcessor<T>.Convert(data[i, j + matrix.ColumnCount], provider);
      }

      return result;
    }

    private static void MultiplyOptimal(double[,] result, double[,] source1, double[,] source2)
    {
      int n = result.GetLength(0);
      int m = result.GetLength(1);
      int l = source1.GetLength(1);

      for (int i = 0; i < n; i++)
      {
        for (int j = 0; j < m; j++)
        {
          double sum = 0;

          for (int k = 0; k < l; k++)
            sum += source1[i, k] * source2[k, j];

          result[i, j] = sum;
        }
      }
    }

    private static void MultiplyOptimal(double[,] result, IMatrix<double> source1, IMatrix<double> source2)
    {
      int n = result.GetLength(0);
      int m = result.GetLength(1);
      int l = source1.ColumnCount;

      for (int i = 0; i < n; i++)
      {
        for (int j = 0; j < m; j++)
        {
          double sum = 0;

          for (int k = 0; k < l; k++)
            sum += source1[i, k] * source2[k, j];

          result[i, j] = sum;
        }
      }
    }

    private static double[,] CreateExtendedArray<T>(IMatrix<T> matrix, IFormatProvider provider)
      where T : IConvertible
    {
      double[,] copy = new double[matrix.RowCount, matrix.ColumnCount * 2];

      if (matrix is RectangleMatrix<T>)
      {
        var rows = matrix.RowCount;
        var cols = matrix.ColumnCount;
        var src = ((RectangleMatrix<T>)matrix).AsArray();

        for (int i = 0; i < rows; i++)
        {
          for (int j = 0; j < cols; j++)
          {
            copy[i, j] = src[i, j].ToDouble(provider);
            copy[i, j + cols] = i == j ? 1 : 0;
          }
        }
      }
      else
      {
        for (int i = 0; i < matrix.RowCount; i++)
        {
          for (int j = 0; j < matrix.ColumnCount; j++)
          {
            copy[i, j] = matrix[i, j].ToDouble(provider);
            copy[i, j + matrix.ColumnCount] = i == j ? 1 : 0;
          }
        }
      }

      return copy;
    }

    private static double[,] CopyToDoubleArray<T>(IMatrix<T> matrix, IFormatProvider provider)
      where T : IConvertible
    {
      double[,] copy = new double[matrix.RowCount, matrix.ColumnCount];

      if (matrix is RectangleMatrix<T>)
      {
        var src = ((RectangleMatrix<T>)matrix).AsArray();

        if (typeof(T) != typeof(double))
        {
          for (int i = 0; i < matrix.RowCount; i++)
          {
            for (int j = 0; j < matrix.ColumnCount; j++)
              copy[i, j] = src[i, j].ToDouble(provider);
          }
        }
        else
          Array.Copy(src, copy, src.Length);
      }
      else
      {
        for (int i = 0; i < matrix.RowCount; i++)
        {
          for (int j = 0; j < matrix.ColumnCount; j++)
            copy[i, j] = matrix[i, j].ToDouble(provider);
        }
      }

      return copy;
    }

    private class ResultProcessor<T>
    {
      private static readonly TypeConverter _converter = TypeDescriptor.GetConverter(typeof(T));

      public static T Convert(double value, CultureInfo provider)
      {
        if (typeof(double) == typeof(T))
          return (T)(object)value;

        return (T)_converter.ConvertFrom(null, provider, value);
      }
    }
  }
}