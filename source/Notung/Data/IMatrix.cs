using System;
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

    /// <summary>
    /// Перебор всех элементов строки матрицы
    /// </summary>
    /// <param name="row">Номер строки</param>
    /// <returns>Итератор, перебирающий все элементы строки</returns>
    IEnumerable<T> GetRowData(int row);

    /// <summary>
    /// Перебор всех элементов столбца матрицы
    /// </summary>
    /// <param name="row">Номер столбца</param>
    /// <returns>Итератор, перебирающий все элементы столбца</returns>
    IEnumerable<T> GetColumnData(int column);
  }

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

    public IEnumerable<T> GetRowData(int row)
    {
      for (int i = 0; i < m_data.GetLength(0); i++)
        yield return m_data[row, i];
    }

    public IEnumerable<T> GetColumnData(int column)
    {
      for (int i = 0; i < m_data.GetLength(1); i++)
        yield return m_data[column, column];
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
      if (row < 0 || row >= m_size)
        throw new ArgumentOutOfRangeException("row");

      if (column < 0 || column >= m_size)
        throw new ArgumentOutOfRangeException("column");

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

    private IEnumerable<T> GetEnumerator(int index)
    {
      for (int i = 0; i < m_size; i++)
      {
        if (!m_with_diagonal && i == index)
          yield return default(T);
        else
          yield return m_data[GetIndex(index, i)];
      }
    }

    public IEnumerable<T> GetRowData(int row)
    {
      return this.GetEnumerator(row);
    }

    public IEnumerable<T> GetColumnData(int column)
    {
      return this.GetEnumerator(column);
    }
  }
}
