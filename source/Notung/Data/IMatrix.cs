using System;
using System.Collections;
using System.Collections.Generic;

namespace Notung.Data
{
  /// <summary>
  /// Интерфейс для работы с матрицами
  /// </summary>
  /// <typeparam name="T">Тип элемента матрицы</typeparam>
  public interface IMatrix<T>
  {
    /// <summary>
    /// Количествро строк
    /// </summary>
    int RowCount { get; }

    /// <summary>
    /// Количество колонок
    /// </summary>
    int ColumnCount { get; }

    /// <summary>
    /// Доступ к элементу матрицы на чтение и запись
    /// </summary>
    /// <param name="row">Номер строки</param>
    /// <param name="column">Номер столбца</param>
    /// <returns>Значение элемента матрицы</returns>
    T this[int row, int column] { get; set; }
  }

  /// <summary>
  /// Прямоугольная матрица логических значений с компактным размещением в памяти
  /// </summary>
  [Serializable]
  public sealed class RectangleBitMatrix : IMatrix<bool>
  {
    private BitArrayHelper m_data;
    private readonly int m_rows;
    private readonly int m_columns;

    public RectangleBitMatrix(int rowCount, int columnCount)
    {
      m_rows = rowCount;
      m_columns = columnCount;
      m_data = new BitArrayHelper(rowCount * columnCount);
    }

    public RectangleBitMatrix(int size)
    {
      m_rows = size;
      m_columns = size;
      m_data = new BitArrayHelper(size * size);
    }

    private int GetIndex(int row, int column)
    {
      if ((uint)row >= (uint)m_rows)
        throw new IndexOutOfRangeException("row");

      if ((uint)column >= (uint)m_columns)
        throw new IndexOutOfRangeException("column");

      return row * m_columns + column;
    }

    public int RowCount
    {
      get { return m_rows; }
    }

    public int ColumnCount
    {
      get { return m_columns; }
    }

    public bool this[int row, int column]
    {
      get { return m_data[GetIndex(row, column)]; }
      set { m_data[GetIndex(row, column)] = value; }
    }
  }

  /// <summary>
  /// Треугольная матрица логических значений с компактным размещением в памяти
  /// </summary>
  [Serializable]
  public sealed class TriangleBitMatrix : IMatrix<bool>
  {
    private BitArrayHelper m_data;
    private readonly int m_size;

    public TriangleBitMatrix(int size)
    {
      m_size = size;
      m_data = new BitArrayHelper(size * (size - 1) / 2 + 1);
    }

    public int RowCount
    {
      get { return m_size; }
    }

    public int ColumnCount
    {
      get { return m_size; }
    }

    private int GetIndex(int row, int column)
    {
      if ((uint)row >= (uint)m_size)
        throw new IndexOutOfRangeException("row");

      if ((uint)column >= (uint)m_size)
        throw new IndexOutOfRangeException("column");

      if (row < column)
        return row * (2 * m_size - row - 3) / 2 + column;
      else if (row > column)
        return column * (2 * m_size - column - 3) / 2 + row;
      else
        return 0;
    }

    public bool this[int row, int column]
    {
      get { return m_data[GetIndex(row, column)]; }
      set
      {
        var index = GetIndex(row, column);

        if (index > 0)
          m_data[index] = value;
      }
    }
  }

#if WEIGHTED_GRAPH
  /// <summary>
  /// Обыкновенная квадратная матрица
  /// </summary>
  /// <typeparam name="T">Тип элемента матрицы</typeparam>
  [Serializable]
  public sealed class RectangleMatrix<T> : IMatrix<T>
  {
    private readonly T[,] m_data;

    public RectangleMatrix(int rowCount, int columnCount)
    {
      m_data = new T[rowCount, columnCount];
    }

    /// <summary>
    /// Создание квадратной матрицы
    /// </summary>
    /// <param name="size">Количество строк и колонок</param>
    public RectangleMatrix(int size)
    {
      m_data = new T[size, size];
    }
    
    public int RowCount
    {
      get { return m_data.GetLength(0); }
    }

    public int ColumnCount
    {
      get { return m_data.GetLength(1); }
    }

    public T this[int row, int column]
    {
      get { return m_data[row, column]; }
      set { m_data[row, column] = value; }
    }
  }

  /// <summary>
  /// Треугольная матрица (половина квадратной)
  /// </summary>
  /// <typeparam name="T">Тип элемента матрицы</typeparam>
  [Serializable]
  public sealed class TriangleMatrix<T> : IMatrix<T>
  {
    private readonly int m_size;
    private readonly bool m_with_diagonal;
    private readonly T[] m_data;

    public TriangleMatrix(int size, bool withDiagonal)
    {
      m_size = size;
      m_with_diagonal = withDiagonal;

      if (withDiagonal)
        m_data = new T[size * (size + 1) / 2];
      else
        m_data = new T[size * (size - 1) / 2];
    }

    /// <summary>
    /// Содержит ли матрица главную диагональ
    /// </summary>
    public bool WithDiagonal
    {
      get { return m_with_diagonal; }
    }
    
    public int RowCount
    {
      get { return m_size; }
    }

    public int ColumnCount
    {
      get { return m_size; }
    }

    private int GetIndex(int row, int column)
    {
      if ((uint)row >= (uint)m_size)
        throw new IndexOutOfRangeException("row");

      if ((uint)column >= (uint)m_size)
        throw new IndexOutOfRangeException("column");

      if (!m_with_diagonal && row == column)
        throw new ArgumentOutOfRangeException("row == column");

      if (row > column)
      {
        int tmp = row;
        row = column;
        column = tmp;
      }

      if (m_with_diagonal)
        return row * (2 * m_size - row - 1) / 2 + column;
      else
        return row * (2 * m_size - row - 3) / 2 + column - 1;
    }

    public T this[int row, int column]
    {
      get { return m_data[GetIndex(row, column)]; }
      set { m_data[GetIndex(row, column)] = value; }
    }
  }
#endif
}