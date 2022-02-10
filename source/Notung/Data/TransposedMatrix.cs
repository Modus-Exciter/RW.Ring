using System;

namespace Notung.Data
{
  /// <summary>
  /// Класс для транспонирование матрицы без дублирования исходных данных
  /// </summary>
  public static class TransposedMatrix
  {
    /// <summary>
    /// Транспонирует матрицу
    /// </summary>
    /// <typeparam name="T">Тип ячейки матрицы</typeparam>
    /// <param name="source">Исходная матрица</param>
    /// <returns>Транспонированная матрица</returns>
    public static IMatrix<T> Transpose<T>(IMatrix<T> source)
    {
      if (source == null)
        throw new ArgumentNullException("source");

      var transposed = source as Wrapper<T>;

      if (transposed != null)
        return transposed.Source;
      else
        return new Wrapper<T>(source);
    }

    private class Wrapper<T> : IMatrix<T>
    {
      private readonly IMatrix<T> m_matrix;

      public Wrapper(IMatrix<T> matrix)
      {
        m_matrix = matrix;
      }

      public IMatrix<T> Source
      {
        get { return m_matrix; }
      }

      public T this[int row, int column]
      {
        get { return m_matrix[column, row]; }
        set { m_matrix[column, row] = value; }
      }

      public int RowCount
      {
        get { return m_matrix.ColumnCount; }
      }

      public int ColumnCount
      {
        get { return m_matrix.RowCount; }
      }

      public override string ToString()
      {
        return m_matrix.ToString();
      }
    }
  }
}