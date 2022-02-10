using System;

namespace Notung.Data
{
  public static class TransposedMatrix
  {
    public static IMatrix<T> Transpose<T>(IMatrix<T> source)
    {
      if (source == null)
        throw new ArgumentNullException("source");

      var transposed = source as TransposedMatrix<T>;

      if (transposed != null)
        return transposed.Source;
      else
        return new TransposedMatrix<T>(source);
    }
  }

  internal class TransposedMatrix<T> : IMatrix<T>
  {
    private readonly IMatrix<T> m_matrix;

    public TransposedMatrix(IMatrix<T> matrix)
    {
      if (matrix == null)
        throw new ArgumentNullException("matrix");

      m_matrix = matrix;
    }

    public IMatrix<T> Source
    {
      get { return m_matrix; }
    }

    public T this[int row, int column]
    {
      get { return m_matrix[column, row]; }
      set
      {
        m_matrix[column, row] = value;
      }
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